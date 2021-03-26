// System
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

// Open TK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

// New Beginnings
using nb.Game.GameObject;
using nb.Game.Utility.Scenes;
using nb.Game.Utility.Globals;
using nb.Game.Utility.Logging;

namespace nb.Game
{
    public abstract class BaseGame : GameWindow
    {
        public BaseGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) {
            Logger.Log(new LogMessage(LogSeverity.Info, "BaseGame", "Made by GermanBread#9077"));
        }
        protected override void OnLoad() {
            #if DEBUG
            Title += " (DEBUG)";
            #endif

            EngineGlobals.CurrentResolution = Size;
            
            // Create a buffer where we feed out vertices into
            Context.MakeCurrent();
            arrayBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, arrayBufferHandle);
            GL.ClearColor(fillColor);

            // Fullscreen
            KeyDown += (KeyboardKeyEventArgs e) => {
                if (e.Alt && e.Key == Keys.Enter) {
                    if (base.WindowState == WindowState.Fullscreen)
                        base.WindowState = WindowState.Normal;
                    else
                        base.WindowState = WindowState.Fullscreen;
                }
            };
            // ESC, CONTROL + Q = quit
            KeyDown += (KeyboardKeyEventArgs e) => {
                if (e.Key == Keys.Escape)
                    Close();
                if ((e.Command || e.Control) && e.Key == Keys.Q)
                    Close();
            };

            Invoke("init");
            
            // Initialize our objects for loading
            SceneManager.LoadScene("default");
            foreach (var scene in EngineGlobals.Scenes.Where(x => x.isLoaded))
            {
                scene.gameObjects.Sort((val1, val2) => val1.Layer.CompareTo(val2.Layer));
                scene.gameObjects.ForEach(x => {
                    x.Init();
                });
            }
            
            Invoke("load");
            
            Context.MakeNoneCurrent();
            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            frameDelta = e.Time;
            int FPS = -1;
            if (frameDelta > double.Epsilon)
                FPS = (int)Math.Ceiling(1 / frameDelta);
            Logger.Log(new LogMessage(LogSeverity.Verbose, "BaseGame", $"Frame delta: {frameDelta}  | FPS: {(FPS > 0 ? FPS : "Not Applicable")}"));

            // We want "update" to not mess with the timing
            Invoke("update");
            
            Context.MakeCurrent();
            GL.ClearColor(fillColor);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            foreach (var scene in EngineGlobals.Scenes.Where(x => x.isLoaded))
            {
                scene.gameObjects.ForEach(x => {
                    x.Draw();
                });
            }

            Context.SwapBuffers();
            Context.MakeNoneCurrent();
            
            base.OnRenderFrame(e);
        }

        protected override void OnUnload() {
            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Height, e.Width);
            EngineGlobals.CurrentResolution = e.Size;
            Logger.Log(new LogMessage(LogSeverity.Verbose, "BaseGame", $"Resized window to: {e.Size}"));
            base.OnResize(e);
        }
        
        /// <summary>
        /// Invoke a child method
        /// </summary>
        private void Invoke(string MethodName, bool CaseInsensitive = true) {
            try {
                // Invoke the child method and run it as a Task
                Logger.Log(new LogMessage(LogSeverity.Verbose, "BaseGame", $"Invoking {MethodName}"));
                Task loadInvoke;
                if (CaseInsensitive)
                    loadInvoke = new TaskFactory().StartNew(()
                     => typeof(Game).GetMethods().First(x => x.Name.ToLower() == MethodName.ToLower()).Invoke(this, null));
                else
                    loadInvoke = new TaskFactory().StartNew(()
                     => typeof(Game).GetMethod(MethodName).Invoke(this, null));
                
                // Wait 5 seconds
                if (!loadInvoke.Wait(1000)) {
                    // If it timed out, display a warning message
                    Logger.Log(new LogMessage(LogSeverity.Warning, "BaseGame", "Invoke exceeded time limit"));
                    loadInvoke.Wait();
                }
                
                // If the task completed, dispose
                //Logger.Log(new LogMessage("BaseGame", LogSeverity.Info, "Invoke done"));
                loadInvoke.Dispose();
            } catch (Exception e) {
                // Log any errors happening
                Logger.Log(new LogMessage(LogSeverity.Error, "BaseGame", "Invoke failed", e));
            }
        }

        // Variables
        private double frameDelta;
        private int arrayBufferHandle;
        private Color4 fillColor = Color4.Black;
        /// <summary>
        /// Gets the time it took for the last frame to draw
        /// </summary>
        public double FrameDelta { get => frameDelta; }
        public Color4 FillColor { get => fillColor; set => fillColor = value; }
    }
}