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
                /*RenderFrequency = 60, 
                UpdateFrequency = 120*/
            }, new NativeWindowSettings {
                Title = "Unsigned Framework Dev."
            })) {
                window.VSync = OpenTK.Windowing.Common.VSyncMode.Off;
                try {
                    window.Run();
                }
                catch (Exception ex) {
                    Logger.Log(new LogMessage(LogSeverity.Critical, "Something is wrong in the process!", ex));
                }
            }

            // Troubleshooting steps when something doesn't work:
            // Make sure everything runs in one thread
            
            // TODO
            // [DIFFICULTY] lower = easier; scale 1-10
            
            // Working on it:
            
            
            // To be done
            // [5] Parenting (might need recursion)
            // [5] Animation system
            // [6] Rework Audio: Split Music / Sound streams. FFT Sample should request blocks in size of 2^x
            //!    Volume can be set with Bass.SampleSetInfo(); !
            // [2] Move drawable objects list from BaseGame.cs to SceneManager.cs
            // [3] Drawing & Multipass: Instead of invalidating the list when am element changes, remove/add that object/scene from the list.
            // [8] Text
            // [?] Integrate ImGUI.Net

            // Done
            // [4] Textures (atlas)
            // [3] Rewrite the entire Shader class
            // [2] Multipass aka cursor detection
            // [8] Fix some issues SPECIFIC TO WINDOWS (Accessviolation in Init() method. Most likely caused during texture load)
            //!    2021-04-09: This is caused by a bug in Shader.cs (probably)
            //!    2021-04-28: New issue: Context.MakeCurrent() fails because the resource is in use (._.)
            // [3] Backport to older versions of OpenGL
            // [3] Scene position offset, rotation, scale
            // [2] Input helper (must also be able to determine the object clicked)
            // [1] Camera (must be able to convert between camera-coordinates, world-coordinates and cursor-coordinates)
            // [4] Texture atlas (applies to text too!) [DONE!]
        }
    }
}
