// System

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using ManagedBass;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using uf.GameObject;
using uf.Rendering.Textures;
using uf.Utility.Audio;
using uf.Utility.Debugging;
using uf.Utility.Globals;
using uf.Utility.Input;
using uf.Utility.Logging;
using uf.Utility.Resources;
using uf.Utility.Scenes;
// Open TK

// BASS

// Unsigned Framework

namespace uf
{
    public abstract class BaseGame : GameWindow
    {
        protected BaseGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) {
            Logger.Log(new LogMessage(LogSeverity.Info, "Unsigned Framework - Made by GermanBread#9077"));

            #if DEBUG
            Logger.Log(new LogMessage(LogSeverity.Info, "This app has been configured in DEBUG mode - expect a lot of console output"));
            Title += " (DEBUG)";
            #else
            if (IsWINE())
                Logger.Log(new LogMessage(LogSeverity.Warning, "My primitive detection algo has told me that you are using WINE to run this executable! Expect problems!"));
            #endif

            EngineGlobals.Window = this;
        }
        protected override void OnLoad() {
            base.OnLoad();

            #if DEBUG
            // Prepare GL callbacks
            GlCallback.Init();
            #endif

            // We will use this buffer for basically everything we do
            arrayBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, arrayBufferHandle);

            // Dark gray because users think that black = bad
            GL.ClearColor(Color4.DarkGray);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Prepare included shaders
            ResourceManager.LoadFile("white texture", "Resources/white.png");
            ResourceManager.LoadFile("default vertex shader", "Resources/default.vert");
            ResourceManager.LoadFile("default fragment shader", "Resources/default.frag");
            ResourceManager.LoadFile("multipass vertex shader", "Resources/multipass.vert");
            ResourceManager.LoadFile("multipass fragment shader", "Resources/multipass.frag");

            // Create a blank texture (used as fallback texture)
            _ = new Texture(null);

            // Prepare Audio
            AudioManager.Init();

            // ALT + ENTER, F11 = toggle fullscreen
            KeyDown += e =>
            {
                if ((!e.Alt || e.Key != Keys.Enter) && e.Key != Keys.F11) return;
                WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;
            };
            // ESC, CONTROL + Q = quit
            KeyDown += e => {
                if ((e.Command || e.Control) && e.Key == Keys.Q)
                    Close();
                if (e.Key == Keys.Escape)
                    Close();
            };

            FocusedChanged += e =>
            {
                Logger.Log(e.IsFocused
                    ? new LogMessage(LogSeverity.Debug, "Focus changed: Focused")
                    : new LogMessage(LogSeverity.Debug, "Focus changed: Unfocused"));
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

            FrameDelta = (float)e.Time;

            #if DEBUG
            {
                var _match = Regex.Match(Title, " \\[ render .+, update .+ \\]");
                string _matchString = "";
                if (_match.Success)
                    _matchString = _match.Value;
                Title = Title.Remove(Title.IndexOf(_matchString, StringComparison.Ordinal), _match.Length) +
                        $" [ render {MathF.Round(1f / FrameDelta)}, update {MathF.Round(1f / UpdateDelta)} ]";
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
        }

        // Pretty much equal to FixedUpdate in Unity
        protected override void OnUpdateFrame(FrameEventArgs e) {
            base.OnUpdateFrame(e);

            if (SceneLoadQueue.Any()) {
                Logger.Log(new LogMessage(LogSeverity.Debug, "Executing Scene queue"));
                foreach (var _t in SceneLoadQueue)
                {
                    SceneManager.OperateOnScene(_t);
                }
                SceneLoadQueue.Clear();
            }

            UpdateDelta = (float)e.Time;
            
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

        private static void Panic(Exception ex = default) {
            StackTrace _st = new();
            var _sf = _st.GetFrame(1);
            Logger.Log(new LogMessage(LogSeverity.Critical,
                $"OVER HERE! Panic has been invoked by {_sf?.GetMethod()?.Name}.",
                ex ?? new Exception("No exception has been provided, look at the messages above")));
            Environment.Exit(1);
        }
        private static bool IsWINE()
         => OperatingSystem.IsWindows() && Process.GetProcessesByName("winlogon").Length == 0;
        public bool InvalidateObjectsCache() => invalidationQueued = true;

        protected abstract void Start();
        protected abstract void Render();
        protected abstract void Update();
        protected abstract void Stop();

        // Variables
        private int arrayBufferHandle;
        // Null = invalidated list. Null because an object or a scene got disabled, enabled, deleted or created.
        private List<BaseObject> drawableObjects;
        private bool invalidationQueued;
        /// <summary>
        /// Time it took for the last frame to draw, measured in seconds
        /// </summary>
        protected float FrameDelta { get; private set; }

        /// <summary>
        /// Time it took for FixedUpdate to complete, time in seconds
        /// </summary>
        private float UpdateDelta { get; set; }

        /// <summary>
        /// Background color used for rendering the underlying canvas
        /// </summary>
        protected Color4 FillColor { get; set; }

        protected bool PauseOnLostFocus = false;
        internal readonly List<(Scene, SceneAction)> SceneLoadQueue = new();
    }
}