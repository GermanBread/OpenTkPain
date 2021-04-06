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
            // [PRIORITY, DIFFICULTY] lower = more urgent, easier; scale 1-10
            // [1, 1] Camera (must be able to convert between camera-coordinates, world-coordinates and cursor-coordinates)
            // [1, 5] Parenting
            // [1, 3] Scene position offset, rotation, scale
            // [2, 2] Working textures
            // [2, 6] Input helper (must also be able to determine the object clicked)
            // [2, 4] Texture atlas (applies to text too!)
            // [3, 4] Text (pass an array to the shader pointing to the characters)
            // [3, 3] Animation system
            // [4, 8] Fix some issues SPECIFIC TO WINDOWS (Accessviolation in Init() method. Most likely caused during texture load. I'm starting to hate Windows more and more each second...)
        }
    }
}
