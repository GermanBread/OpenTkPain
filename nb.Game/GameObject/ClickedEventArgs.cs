// OpenTK
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace nb.Game.GameObject
{
    public struct ClickedEventArgs
    {
        public ClickedEventArgs(MouseButton Button) {
            MouseButton = Button;
        }
        public MouseButton MouseButton;
    }
}