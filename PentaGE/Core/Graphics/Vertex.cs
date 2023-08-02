using System.Numerics;

namespace PentaGE.Core.Graphics
{
    internal struct Vertex
    {
        public Vector3 Coordinates { get; set; }

        public Vector3 Normal { get; set; }

        public Vector4 Color { get; set; }

        public Vector2 TextureCoordinates { get; set; }

        public Vertex()
        {
            Coordinates = Vector3.Zero;
            Normal = Vector3.Zero;
            TextureCoordinates = Vector2.Zero;
            Color = Vector4.Zero;
        }

        public Vertex(Vector3 coordinates)
        {
            Coordinates = coordinates;
        }

        public Vertex(Vector3 coordinates, Vector3 normal) : this(coordinates)
        {
            Normal = normal;
        }

        public Vertex(Vector3 coordinates, Vector3 normal, Vector4 color) : this(coordinates, normal)
        {
            Color = color;
        }

        public Vertex(Vector3 coordinates, Vector3 normal, Vector4 color, Vector2 textureCoordinates) : this(coordinates, normal, color)
        {
            Coordinates = coordinates;
            Normal = normal;
            Color = color;
            TextureCoordinates = textureCoordinates;
        }
    }
}