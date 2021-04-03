// OpenTK
using OpenTK.Windowing.Desktop;

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
                window.Run();
                window.Dispose();
            }

            // TODO
            // Camera
            // Working textures
            // Input helper
            // Parenting
            // Scene position offset, rotation, scale
            // Text
        }
    }
}
