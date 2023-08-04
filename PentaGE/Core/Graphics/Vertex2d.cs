using System.Numerics;

namespace PentaGE.Core.Graphics
{
    public struct Vertex2d
    {
        public Vector2 Coordinates { get; set; }

        public Vertex2d()
        {
            Coordinates = Vector2.Zero;
        }

        public Vertex2d(Vector2 coordinates)
        {
            Coordinates = coordinates;
        }
    }
}