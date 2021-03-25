using OpenTK.Windowing.Desktop;

namespace nb.Game
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game window = new Game(new GameWindowSettings { 
                IsMultiThreaded = true, 
                RenderFrequency = 60, 
                UpdateFrequency = 120 
            }, new NativeWindowSettings {
                Title = "OpenTK again..."
            })) {
                window.Run();
            }
        }
    }
}
