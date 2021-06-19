// System
using System;

// OpenTK
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using uf.Utility.Globals;
using uf.Utility.Logging;

namespace uf
{
    class Entry
    {
        static void Main(string[] args)
        {
            EngineGlobals.CLArgs = args;
            
            using Game window = new(new GameWindowSettings {
                RenderFrequency = 60,
                UpdateFrequency = 120,
            }, new NativeWindowSettings {
                Title = "Unsigned Framework Dev.",
                Profile = ContextProfile.Any
            });
            window.VSync = VSyncMode.Off;
            try {
                window.Run();
            }
            catch (Exception ex) {
                Logger.Log(new LogMessage(LogSeverity.Critical, "Something is wrong in the process!", ex));
            }

            // Troubleshooting steps when something doesn't work:
            // Make sure everything runs in one thread
            
            // TODO
            // [DIFFICULTY] lower = easier; scale 1-10

            // Working on it:
            // [5] Animation system
            // [8] Text
            // [2] Convert to library
            
            // To be done
            // [3] Add support for multiple atlases
            // [6] Rework Audio: Split Music / Sound streams. FFT Sample should request blocks in size of 2^x
            //!    Volume can be set with Bass.SampleSetInfo();
            // [8] Add a class that does instanced rendering... Perhaps called "InstancedObject"?
            //!    It should also check if all objects are of the same type... or it should draw the different types in a instanced render
            // [5] Add ability to save / load texture atlases
            // [3] Implement a file format for resources (perhaps .ufr?)
            //!    These resource files should support containing more than one file, and even other resource files
            // [5] Rewrite Transform.cs to feature less spaghetti
            // [6] Make multithreading easy
            //!    Provide a class that can be used to launch code in a seperate thread
            //!    Race conditions should be avoided!
            // [2] Move drawable objects list from BaseGame.cs to SceneManager.cs
            // [3] Drawing & Multipass: Instead of invalidating the list when am element changes, remove/add that object/scene from the list.
            // [?] Integrate ImGUI.Net
            // [?] Migrate to OpenAL

            // Done
            // [3] Change LoadScene() to load the scenes indirectly. Instead it should be done by the main thread!
            // [5] Parenting (might need recursion)
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
