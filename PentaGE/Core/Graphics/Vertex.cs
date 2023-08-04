using System.Numerics;

namespace PentaGE.Core.Graphics
{
    internal struct Vertex
    {
        public Vector3 Coordinates { get; set; }

        public Vector3 Normal { get; set; }

        public Vector2 TextureCoordinates { get; set; }

        public Vertex()
        {
            Coordinates = Vector3.Zero;
            Normal = Vector3.Zero;
            TextureCoordinates = Vector2.Zero;
        }

        public Vertex(Vector3 coordinates)
        {
            Coordinates = coordinates;
        }

        public Vertex(Vector3 coordinates, Vector3 normal) : this(coordinates)
        {
            Normal = normal;
        }

        public Vertex(Vector3 coordinates, Vector3 normal, Vector2 textureCoordinates) : this(coordinates, normal)
        {
            Coordinates = coordinates;
            Normal = normal;
            TextureCoordinates = textureCoordinates;
        }
    }
}