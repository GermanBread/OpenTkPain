// OpenTK
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace uf.GameObject
{
    public struct ClickedEventArgs
    {
        public ClickedEventArgs(MouseButton Button) {
            MouseButton = Button;
        }
        public MouseButton MouseButton;
    }
}