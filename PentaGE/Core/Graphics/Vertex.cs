using System.Numerics;

namespace PentaGE.Core.Graphics
{
    /// <summary>
    /// Represents a vertex with position, normal, and texture coordinate information.
    /// </summary>
    internal struct Vertex
    {
        /// <summary>
        /// Gets or sets the coordinates of the vertex in 3D space.
        /// </summary>
        public Vector3 Coordinates { get; set; } = Vector3.Zero;

        /// <summary>
        /// Gets or sets the normal vector of the vertex.
        /// </summary>
        public Vector3 Normal { get; set; } = Vector3.Zero;

        /// <summary>
        /// Gets or sets the texture coordinates of the vertex.
        /// </summary>
        public Vector2 TextureCoordinates { get; set; } = Vector2.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> struct with default values.
        /// </summary>
        public Vertex()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> struct with the specified coordinates.
        /// </summary>
        /// <param name="coordinates">The coordinates of the vertex.</param>
        public Vertex(Vector3 coordinates)
        {
            Coordinates = coordinates;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> struct with the specified coordinates and normal.
        /// </summary>
        /// <param name="coordinates">The coordinates of the vertex.</param>
        /// <param name="normal">The normal vector of the vertex.</param>
        public Vertex(Vector3 coordinates, Vector3 normal) : this(coordinates)
        {
            Normal = normal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> struct with the specified coordinates, normal, and texture coordinates.
        /// </summary>
        /// <param name="coordinates">The coordinates of the vertex.</param>
        /// <param name="normal">The normal vector of the vertex.</param>
        /// <param name="textureCoordinates">The texture coordinates of the vertex.</param>
        public Vertex(Vector3 coordinates, Vector3 normal, Vector2 textureCoordinates) : this(coordinates, normal)
        {
            Coordinates = coordinates;
            Normal = normal;
            TextureCoordinates = textureCoordinates;
        }

        public override string ToString() =>
            $"Vertex: {Coordinates.X} {Coordinates.Y} {Coordinates.Z}";
    }
}