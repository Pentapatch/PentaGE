using PentaGE.Core.Rendering;

namespace PentaGE.Core
{
    public sealed class Viewport
    {
        public int Left { get; set; }

        public int Top { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public Camera? ActiveCamera { get; set; }

        public Viewport(int left, int top, int width, int height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }
    }

}