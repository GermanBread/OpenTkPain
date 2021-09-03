// System
using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Open TK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

// BASS
using ManagedBass;

// Unsigned Framework
using uf.GameObject;
using uf.Utility.Input;
using uf.Utility.Audio;
using uf.Utility.Scenes;
using uf.Utility.Globals;
using uf.Utility.Logging;
using uf.Utility.Resources;
using uf.Utility.Debugging;
using uf.Rendering.Textures;

namespace uf
{
    public abstract class BaseGame : GameWindow
    {
        public BaseGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) {
            Logger.Log(new LogMessage(LogSeverity.Info, "Unsigned Framework - Made by GermanBread#9077"));

            #if DEBUG
            Logger.Log(new LogMessage(LogSeverity.Info, "This app has been configured in DEBUG mode - expect a lot of console output"));
            Title += " (DEBUG)";
            #endif

            EngineGlobals.Window = this;
        }
        protected override void OnLoad() {
            base.OnLoad();

            #if DEBUG
            // Prepare GL callbacks
            GLCallback.Init();
            #endif

            // We will use this buffer for basically everything we do
            arrayBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, arrayBufferHandle);

            // Dark gray because users think that black = bad
            GL.ClearColor(Color4.DarkGray);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Prepare included shaders
            ResourceManager.LoadFile("default vertex shader", "Resources/default.vert");
            ResourceManager.LoadFile("default fragment shader", "Resources/default.frag");
            ResourceManager.LoadFile("multipass vertex shader", "Resources/multipass.vert");
            ResourceManager.LoadFile("multipass fragment shader", "Resources/multipass.frag");

            // Create a blank texture (used as fallback texture)
            _ = new Texture(null);

            // Prepare Audio
            AudioManager.Init();

            // ALT + ENTER, F11 = toggle fullscreen
            KeyDown += (KeyboardKeyEventArgs e) => {
                if ((e.Alt && e.Key == Keys.Enter) || e.Key == Keys.F11) {
                    if (WindowState == WindowState.Fullscreen)
                        WindowState = WindowState.Normal;
                    else
                        WindowState = WindowState.Fullscreen;
                }
            };
            // ESC, CONTROL + Q = quit
            KeyDown += (KeyboardKeyEventArgs e) => {
                if ((e.Command || e.Control) && e.Key == Keys.Q)
                    Close();
                if (e.Key == Keys.Escape)
                    Close();
            };

            FocusedChanged += (FocusedChangedEventArgs e) => {
                if (e.IsFocused)
                    Logger.Log(new LogMessage(LogSeverity.Debug, "Focus changed: Focused"));
                else
                    Logger.Log(new LogMessage(LogSeverity.Debug, "Focus changed: Unfocused"));
            };

            try {
                Start();
            }
            catch (Exception ex) {
                Panic(ex);
            }
            Update();
            
            // This is old code that won't be run anymore. To be removed...
            //EngineGlobals.Scenes.ForEach(x => x.GameObjects.ForEach(y => y.Init()));

            // This will be the proper way of initializing objects
            SceneManager.LoadScene("default");
            SceneManager.LoadScene("overlay");
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);

            frameDelta = (float)e.Time;

            #if DEBUG
            {
                var _match = Regex.Match(Title, " \\[ render .+, update .+ \\]");
                string _matchString = "";
                if (_match.Success)
                    _matchString = _match.Value;
                Title = Title.Remove(Title.IndexOf(_matchString), _match.Length) + $" [ render {MathF.Round(1f / frameDelta)}, update {MathF.Round(1f / updateDelta)} ]";
            }
            #endif

            if (IsFocused || !PauseOnLostFocus)
                Render();
            
            // Only recreate when list has been invalidated. See comment in declaration
            if (invalidationQueued) {
                drawableObjects = null;
                invalidationQueued = false;
            }

            if (drawableObjects == null) {
                drawableObjects = new();
                
                var _scenes = EngineGlobals.Scenes.Where(x => x.IsLoaded);
            
                // Insert the game objects into the _objects list
                foreach (var scene in _scenes) {
                    foreach (var go in scene.GameObjects) {
                        drawableObjects.Add(go);
                    }
                }

                // Sort
                drawableObjects.Sort((o1, o2) => o1.Layer.CompareTo(o2.Layer));
            }

            // Black because that's how the multipass system works.
            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            InputManager.PerformMultipassRender(drawableObjects);

            // Can be used for troubleshooting
            //Context.SwapBuffers();

            GL.ClearColor(FillColor);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            // Now draw
            foreach (var go in drawableObjects)
                go.Draw();

            Context.SwapBuffers();

            return;
        }

        // Pretty much equal to FixedUpdate in Unity
        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            if (SceneLoadQueue.Any()) {
                Logger.Log(new LogMessage(LogSeverity.Debug, "Executing Scene queue"));
                for (int i = 0; i < SceneLoadQueue.Count; i++) {
                    SceneManager.OperateOnScene(SceneLoadQueue[i]);
                }
                SceneLoadQueue.Clear();
            }

            updateDelta = (float)e.Time;
            
            // Loop audio when applicable
            if (IsFocused || !PauseOnLostFocus) {
                if (AudioManager.Ready)
                    foreach (var clip in AudioManager.AudioClips.Where(x => (x.Loop && x.IsPlaying && x.ClipStatus == PlaybackState.Stopped)))
                        clip.Play();
            }
            if (AudioManager.Ready) {
                if (PauseOnLostFocus && IsFocused)
                    Bass.Start();
                else if (PauseOnLostFocus)
                    Bass.Pause();
            }

            // Run update on each object
            foreach (var go in drawableObjects ?? new())
                go.Update();
            
            if (IsFocused || !PauseOnLostFocus)
                Update();
        }

        protected override void OnUnload() {
            base.OnUnload();

            Stop();

            AudioManager.Dispose();
        }
        protected override void OnClosed() {
            Logger.Log(new LogMessage(LogSeverity.Debug, "Exit"));
            base.OnClosed();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e); //Is not needed, since it's originally just a stub. Commenting this out also makes resizes faster
            GL.Viewport(0, 0, e.Width, e.Height);
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Resized window to: {e.Size}"));
        }
        
        protected static void Panic(Exception ex = default) {
            StackTrace _st = new();
            StackFrame _sf = _st.GetFrame(1);
            Logger.Log(new LogMessage(LogSeverity.Critical, $"OVER HERE! Panic has been invoked by {_sf.GetMethod().Name}.", ex ?? new Exception("No exception has been provided, look at the messages above")));
            Environment.Exit(1);
        }
        public bool InvalidateObjectsCache() => invalidationQueued = true;

        public abstract void Start();
        public abstract void Render();
        public abstract void Update();
        public abstract void Stop();

        // Variables
        private float frameDelta;
        private float updateDelta;
        private int arrayBufferHandle;
        // Null = invalidated list. Null because an object or a scene got disabled, enabled, deleted or created.
        private List<BaseObject> drawableObjects = null;
        private bool invalidationQueued = false;
        /// <summary>
        /// Time it took for the last frame to draw, measured in seconds
        /// </summary>
        public float FrameDelta { get => frameDelta; }
        /// <summary>
        /// Time it took for FixedUpdate to complete, time in seconds
        /// </summary>
        public float UpdateDelta { get => updateDelta; }
        /// <summary>
        /// Background color used for rendering the underlying canvas
        /// </summary>
        public Color4 FillColor { get; set; }
        public bool PauseOnLostFocus = false;
        readonly internal List<(Scene, SceneAction)> SceneLoadQueue = new();
    }
}