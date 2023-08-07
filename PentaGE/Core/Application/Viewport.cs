using PentaGE.Core.Application;

namespace PentaGE.Core
{
    public sealed class Viewport
    {
        public int Left { get; set; }

        public int Top { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public CameraManager CameraManager { get; private set; }

        internal Viewport(PentaGameEngine engine, int left, int top, int width, int height)
        {
            CameraManager = new(engine);
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }
    }

}