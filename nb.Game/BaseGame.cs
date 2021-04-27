// System
using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

// Open TK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

// BASS
using ManagedBass;

// New Beginnings
using nb.Game.GameObject;
using nb.Game.Utility.Input;
using nb.Game.Utility.Audio;
using nb.Game.Utility.Scenes;
using nb.Game.Utility.Globals;
using nb.Game.Utility.Logging;
using nb.Game.Utility.Resources;
using nb.Game.Utility.Debugging;
using nb.Game.Utility.Attributes;

namespace nb.Game
{
    public abstract class BaseGame : GameWindow
    {
        public BaseGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) {
            #if !DISABLE_SPLASH
            Logger.Log(new LogMessage(LogSeverity.Info, "Unsigned Framework - Made by GermanBread#9077"));
            #endif

            #if DEBUG
            Title += " (DEBUG)";
            #endif

            // Create a stacktrace and obtain the child class
            childObject = new StackTrace().GetFrame(1).GetMethod().DeclaringType;
        }
        protected override void OnLoad() {
            base.OnLoad();

            #if DEBUG
            // Prepare GL callbacks
            GL_Callback.Init();
            #endif

            // Create a buffer where we feed out vertices into
            arrayBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, arrayBufferHandle);

            GL.ClearColor(Color4.DarkGray);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Prepare default shaders
            ResourceManager.LoadResource("default vertex shader", "default.vert");
            ResourceManager.LoadResource("default fragment shader", "default.frag");
            ResourceManager.LoadResource("multipass vertex shader", "multipass.vert");
            ResourceManager.LoadResource("multipass fragment shader", "multipass.frag");

            // Prepare BASS
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
                if (e.Key == Keys.Escape)
                    Close();
                if ((e.Command || e.Control) && e.Key == Keys.Q)
                    Close();
            };

            FocusedChanged += (FocusedChangedEventArgs e) => {
                if (e.IsFocused)
                    Logger.Log(new LogMessage(LogSeverity.Debug, "Focus changed: Focused"));
                else
                    Logger.Log(new LogMessage(LogSeverity.Debug, "Focus changed: Unfocused"));
            };

            Invoke("Init");
            Invoke("Update");
            
            //EngineGlobals.Scenes.ForEach(x => x.GameObjects.ForEach(y => y.Init()));

            // Initialize our objects / scenes
            SceneManager.LoadScene("default");
            SceneManager.LoadScene("overlay");
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            base.OnRenderFrame(e);

            frameDelta = e.Time;
            int FPS = -1;
            if (frameDelta > double.Epsilon)
                FPS = (int)Math.Ceiling(1 / frameDelta);
            // Loop audio when applicable
            if (IsFocused || !PauseOnLostFocus) {
                Logger.Log(new LogMessage(LogSeverity.Debug, $"Frame delta: {frameDelta}  | FPS: {(FPS > 0 ? FPS : "Not Applicable")}"));
                // FIXME: Does not loop audio for some reason
                foreach (var clip in AudioManager.AudioClips.Where(x => (x.Loop && x.IsPlaying && x.ClipStatus == PlaybackState.Stopped)))
                    clip.Play();
            }
            if (PauseOnLostFocus && IsFocused) {
                Bass.Start();
            }
            else if (PauseOnLostFocus)
                Bass.Pause();

            // We want "update" to not mess with the timing
            if (IsFocused || !PauseOnLostFocus)
                Invoke("Update");
            
            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            InputManager.PerformMultipassRender();

            // Create out two lists
            var _scenes = EngineGlobals.Scenes.Where(x => x.IsLoaded);
            List<BaseObject> _objects = new();
            
            // Insert the game objects into the _objects list
            foreach (var scene in _scenes) {
                foreach (var go in scene.GameObjects) {
                    _objects.Add(go);
                }
            }

            // Sort
            _objects.Sort((o1, o2) => o1.Layer.CompareTo(o2.Layer));

            GL.ClearColor(FillColor);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            // Now draw
            foreach (var go in _objects)
                go.Draw();

            Context.SwapBuffers();

            return;
        }

        protected override void OnUnload() {
            base.OnUnload();

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
        
        /// <summary>
        /// Invoke a child method
        /// </summary>
        private void Invoke(string MethodName) {
            try {
                MethodInfo _method;
                
                // First we check if our cache already contains the method we want to invoke (reflection is slow!)
                bool _presentInCache = reflectionCache.ContainsKey(MethodName);
                if (_presentInCache)
                    _method = reflectionCache[MethodName];
                else
                    _method = childObject.GetMethod(MethodName);

                bool _canTimeOut = _method.CustomAttributes.FirstOrDefault(x
                 => x.AttributeType.Equals(typeof(NoTimeout))) == null;
                
                // Invoke the child method and run it as a Task
                Logger.Log(new LogMessage(LogSeverity.Debug, $"Invoking {MethodName}"));
                Task _loadInvoke;
                _loadInvoke = new TaskFactory().StartNew(()
                 => _method.Invoke(this, null));
                
                // Add the method to our cache
                if (!_presentInCache)
                    reflectionCache.Add(MethodName, _method);

                // Wait 1 second
                if (!_loadInvoke.Wait(1000) && _canTimeOut) {
                    // If it timed out, display a warning message
                    Logger.Log(new LogMessage(LogSeverity.Warning, "Invoke exceeded time limit"));
                }
                _loadInvoke.Wait();
                
                _loadInvoke.Dispose();
            } catch (Exception e) {
                Logger.Log(new LogMessage(LogSeverity.Error, "Invoke failed", e.GetType() == typeof(AggregateException) ? e.InnerException : e));
            }
        }

        // Variables
        private double frameDelta;
        private int arrayBufferHandle;
        private Type childObject;
        private Dictionary<string, MethodInfo> reflectionCache = new();
        /// <summary>
        /// Time it took for the last frame to draw, measured in milliseconds
        /// </summary>
        public float FrameDelta { get => (float)frameDelta; }
        /// <summary>
        /// Background color used for rendering the underlying canvas
        /// </summary>
        public Color4 FillColor { get; set; }
        public bool PauseOnLostFocus = false;
    }
}