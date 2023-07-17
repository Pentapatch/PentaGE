using PentaGE.Cameras;
using System.Drawing;

namespace PentaGE.Viewports
{
    public class Viewport
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Size Size => new(Width, Height);
        public int Left { get; set; }
        public int Top { get; set; }
        public Point Location => new(Left, Top);

        public Camera? ActiveCamera { get; set; }

        public Viewport(int left, int top, Rectangle size, Camera? camera = null)
        {
            Left = left;
            Top = top;
            Width = size.Width;
            Height = size.Height;
            ActiveCamera = camera;
        }
    }
}