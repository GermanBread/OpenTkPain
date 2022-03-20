// OpenTK
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace uf.GameObject
{
    public struct ClickedEventArgs
    {
        public ClickedEventArgs(MouseButton button) {
            MouseButton = button;
        }
        public readonly MouseButton MouseButton;
    }
}