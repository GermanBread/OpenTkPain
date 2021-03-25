// System
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

// Open TK
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;

// New Beginnings
using nb.Game.GameObject;
using nb.Game.Utility.Scenes;
using nb.Game.Utility.Globals;
using nb.Game.Utility.Logging;

namespace nb.Game
{
    public abstract class BaseGame : GameWindow
    {
        private int bufferHandle;
        public BaseGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) {
            Logger.Log(new LogMessage(LogSeverity.Info, "BaseGame", "Made by GermanBread#9077"));
        }
        protected override void OnLoad() {
            GL.ClearColor(0f, 0f, 0f, 1);

            #if DEBUG
            Title += " (DEBUG)";
            #endif

            // Fullscreen test
            /*KeyDown += (KeyboardKeyEventArgs e) => {
                if (e.Alt & e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.Enter) {
                    if (base.WindowState == WindowState.Fullscreen)
                        base.WindowState = WindowState.Normal;
                    else
                        base.WindowState = WindowState.Fullscreen;
                }
            };*/

            // Create a buffer where we feed out vertices into
            bufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferHandle);
            
            // Initialize our objects for loading
            SceneManager.LoadScene("default");
            foreach (var scene in EngineGlobals.Scenes.Where(x => x.isLoaded))
            {
                scene.gameObjects.Sort((val1, val2) => val1.layer.CompareTo(val2.layer));
                scene.gameObjects.ForEach(x => {
                    x.Init();
                });
            }
            
            Invoke("load");
            
            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            // We want OnUpdateFrame to not mess with the timing
            Invoke("update");
            
            GL.Clear(ClearBufferMask.ColorBufferBit);

            foreach (var scene in EngineGlobals.Scenes.Where(x => x.isLoaded))
            {
                scene.gameObjects.ForEach(x => {
                    x.Draw();
                });
            }

            Context.SwapBuffers();
            
            base.OnRenderFrame(e);
        }

        protected override void OnUnload() {
            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }
        
        /// <summary>
        /// Invoke a child method
        /// </summary>
        /// <param name="method"></param>
        private void Invoke(string method) {
            try {
                // Invoke the child method and run it as a Task
                Logger.Log(new LogMessage(LogSeverity.Verbose, "BaseGame", $"Invoking {method}"));
                Task loadInvoke = new TaskFactory().StartNew(()
                 => typeof(Game).GetMethod(method).Invoke(this, null));
                
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
    }
}