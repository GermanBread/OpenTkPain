// OpenTK
using OpenTK.Windowing.Desktop;

using nb.Game.Utility.Globals;

namespace nb.Game
{
    class Entry
    {
        static void Main(string[] args)
        {
            using (Game window = new Game(new GameWindowSettings { 
                RenderFrequency = 60, 
                UpdateFrequency = 120
            }, new NativeWindowSettings {
                Title = "Unsigned Framework Dev."
            })) {
                EngineGlobals.Window = window;
                window.Run();
                window.Dispose();
            }

            // TODO
            // [DIFFICULTY] lower = easier; scale 1-10
            
            // Working on it:            
            // [2] Working textures (i got the atlas working, does this count? ...no?)
            // [1] Camera (must be able to convert between camera-coordinates, world-coordinates and cursor-coordinates)
            
            // To be done
            // [5] Parenting
            // [3] Scene position offset, rotation, scale
            // [6] Input helper (must also be able to determine the object clicked)
            // [4] Texture atlas (applies to text too!) [DONE!]
            // [4] Text (pass an array to the shader pointing to the characters)
            // [3] Animation system
            // [8] Fix some issues SPECIFIC TO WINDOWS (Accessviolation in Init() method. Most likely caused during texture load. I'm starting to hate Windows more and more each second...)
        }
    }
}
