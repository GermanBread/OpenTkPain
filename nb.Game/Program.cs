// System
using System;

// OpenTK
using OpenTK.Windowing.Desktop;

using nb.Game.Utility.Globals;
using nb.Game.Utility.Logging;

namespace nb.Game
{
    class Entry
    {
        static void Main(string[] args)
        {
            EngineGlobals.CLArgs = args;
            using (Game window = new Game(new GameWindowSettings { 
                RenderFrequency = 60, 
                UpdateFrequency = 120
            }, new NativeWindowSettings {
                Title = "Unsigned Framework Dev."
            })) {
                EngineGlobals.Window = window;
                try {
                    window.Run();
                }
                catch (Exception ex) {
                    Logger.Log(new LogMessage(LogSeverity.Critical, "Something is wrong in the process!", ex));
                }
                window.Dispose();
            }

            // TODO
            // [DIFFICULTY] lower = easier; scale 1-10
            
            // Working on it:
            // [8] Fix some issues SPECIFIC TO WINDOWS (Accessviolation in Init() method. Most likely caused during texture load)
            //!    2021-04-09: This is caused by a bug in Shader.cs (probably)
            
            // To be done
            // [5] Parenting (might need recursion)
            // [2] Working textures (i got the atlas working, does this count? ...no?)
            // [4] Text (pass an array to the shader pointing to the characters, no no don't do that)
            // [3] Animation system

            // Done
            // [3] Backport to older versions of OpenGL
            // [3] Scene position offset, rotation, scale
            // [2] Input helper (must also be able to determine the object clicked)
            // [1] Camera (must be able to convert between camera-coordinates, world-coordinates and cursor-coordinates)
            // [4] Texture atlas (applies to text too!) [DONE!]
        }
    }
}
