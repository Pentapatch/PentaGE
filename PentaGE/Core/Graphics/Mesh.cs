using PentaGE.Common;
using PentaGE.Maths;
using System.Numerics;

namespace PentaGE.Core.Graphics
{
    /// <summary>
    /// Represents a 3D mesh consisting of vertices and optional indices.
    /// </summary>
    public sealed class Mesh : ICloneable
    {
        private readonly Random _random = new();

        /// <summary>
        /// Gets or sets the list of vertices composing the mesh.
        /// </summary>
        internal List<Vertex> Vertices { get; set; }

        /// <summary>
        /// Gets or sets the list of indices defining the vertex connectivity (optional).
        /// </summary>
        internal List<uint>? Indices { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mesh"/> class with the specified vertices and optional indices.
        /// </summary>
        /// <param name="vertices">The list of vertices composing the mesh.</param>
        /// <param name="indices">The list of indices defining the vertex connectivity (optional).</param>
        internal Mesh(List<Vertex> vertices, List<uint>? indices = null)
        {
            Vertices = vertices;
            Indices = indices;
        }

        /// <inheritdoc/>
        public object Clone()
        {
            var vertices = new List<Vertex>(Vertices);
            var indices = new List<uint>(Indices ?? new List<uint>());

            return new Mesh(vertices, indices);
        }

        /// <summary>
        /// Offsets all vertex positions of the mesh by specified x, y, and z values.
        /// </summary>
        /// <param name="x">The offset along the x-axis.</param>
        /// <param name="y">The offset along the y-axis.</param>
        /// <param name="z">The offset along the z-axis.</param>
        public void Offset(float x, float y, float z) =>
            Offset(new Vector3(x, y, z));

        /// <summary>
        /// Offsets all vertex positions of the mesh by a specified vector.
        /// </summary>
        /// <param name="offset">The vector by which to offset the vertices.</param>
        public void Offset(Vector3 offset)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new(
                    Vertices[i].Coordinates + offset,
                    Vertices[i].Normal,
                    Vertices[i].TextureCoordinates);
            }
        }

        /// <summary>
        /// Scales the mesh uniformly by a specified factor.
        /// </summary>
        /// <param name="scale">The scale factor.</param>
        public void Scale(float scale) =>
            Scale(new Vector3(scale));

        /// <summary>
        /// Scales the mesh non-uniformly along the x, y, and z axes.
        /// </summary>
        /// <param name="x">The scale factor along the x-axis.</param>
        /// <param name="y">The scale factor along the y-axis.</param>
        /// <param name="z">The scale factor along the z-axis.</param>
        public void Scale(float x, float y, float z) =>
            Scale(new Vector3(x, y, z));

        /// <summary>
        /// Scales the mesh uniformly by a specified factor.
        /// </summary>
        /// <param name="scale">The scale factor.</param>
        public void Scale(Vector3 scale)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new(
                    Vertices[i].Coordinates * scale,
                    Vertices[i].Normal,
                    Vertices[i].TextureCoordinates);
            }
        }

        /// <summary>
        /// Rotates the mesh by a specified rotation.
        /// </summary>
        /// <param name="rotation">The rotation to apply.</param>
        public void Rotate(Rotation rotation)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new(
                    Vector3.Transform(
                        Vertices[i].Coordinates,
                        Matrix4x4.CreateFromYawPitchRoll(
                            MathHelper.DegreesToRadians(rotation.Yaw % 360),
                            MathHelper.DegreesToRadians(-rotation.Pitch % 360),
                            MathHelper.DegreesToRadians(rotation.Roll % 360))),
                    Vector3.Transform(
                        Vertices[i].Normal,
                        Matrix4x4.CreateFromYawPitchRoll(
                            MathHelper.DegreesToRadians(rotation.Yaw % 360),
                            MathHelper.DegreesToRadians(-rotation.Pitch % 360),
                            MathHelper.DegreesToRadians(rotation.Roll % 360))),
                    Vertices[i].TextureCoordinates);
            }
        }

        /// <summary>
        /// Rotates the mesh by the specified yaw, pitch, and roll angles.
        /// </summary>
        /// <param name="yaw">The yaw angle in degrees.</param>
        /// <param name="pitch">The pitch angle in degrees.</param>
        /// <param name="roll">The roll angle in degrees.</param>
        public void Rotate(float yaw, float pitch, float roll) =>
            Rotate(new Rotation(yaw, pitch, roll));

        /// <summary>
        /// Rotates the mesh by a specified angle around a specified axis.
        /// </summary>
        /// <param name="angle">The angle of rotation in radians.</param>
        /// <param name="axis">The axis of rotation.</param>
        public void Rotate(float angle, Vector3 axis)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new(
                    Vector3.Transform(
                        Vertices[i].Coordinates,
                        Matrix4x4.CreateFromAxisAngle(axis, angle)),
                    Vector3.Transform(
                        Vertices[i].Normal,
                        Matrix4x4.CreateFromAxisAngle(axis, angle)),
                    Vertices[i].TextureCoordinates);
            }
        }

        /// <summary>
        /// Rotates the mesh by a specified angle around a specified axis.
        /// </summary>
        /// <param name="angle">The angle of rotation in radians.</param>
        /// <param name="x">The x-coordinate of the rotation axis.</param>
        /// <param name="y">The y-coordinate of the rotation axis.</param>
        /// <param name="z">The z-coordinate of the rotation axis.</param>
        public void Rotate(float angle, float x, float y, float z) =>
            Rotate(angle, new Vector3(x, y, z));


        /// <summary>
        /// Tiles the texture coordinates of the mesh vertices by the given factors along the X and Y axes.
        /// </summary>
        /// <param name="x">The tiling factor along the X axis.</param>
        /// <param name="y">The tiling factor along the Y axis.</param>
        /// <remarks>
        /// This method adjusts the texture coordinates of each vertex in the mesh by multiplying them with the provided factors.
        /// The result is a tiled texture appearance on the mesh.
        /// </remarks>
        public void TileTexture(float x, float y)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new(
                    Vertices[i].Coordinates,
                    Vertices[i].Normal,
                    new Vector2(Vertices[i].TextureCoordinates.X * x, Vertices[i].TextureCoordinates.Y * y));
            }
        }

        public void Subdivide(int subdivisionLevels = 1)
        {
            if (subdivisionLevels < 1)
                throw new ArgumentOutOfRangeException(nameof(subdivisionLevels), "The number of subdivisions must be greater than zero.");

            if (Indices is null) throw new InvalidOperationException("Cannot subdivide a mesh without indices.");

            for (int level = 0; level < subdivisionLevels; level++)
            {
                if (Indices is not null)
                {
                    // Mesh has indices, so we can use them to subdivide
                    int count = Indices.Count;
                    for (int i = 0; i < count; i += 3)
                    {
                        // Get the original vertices
                        Vertex vertex0 = Vertices[(int)Indices[i]];
                        Vertex vertex1 = Vertices[(int)Indices[i + 1]];
                        Vertex vertex2 = Vertices[(int)Indices[i + 2]];

                        // Calculate midpoints for each edge
                        Vertex vertex3 = CalculateMidpoint(vertex0, vertex1);
                        Vertex vertex4 = CalculateMidpoint(vertex1, vertex2);
                        Vertex vertex5 = CalculateMidpoint(vertex2, vertex0);

                        // Add the new vertices
                        int vertexCount = Vertices.Count;
                        Vertices.Add(vertex3);
                        Vertices.Add(vertex4);
                        Vertices.Add(vertex5);

                        // Store the indices of the vertices for optimization
                        uint index0 = Indices[i];
                        uint index1 = Indices[i + 1];
                        uint index2 = Indices[i + 2];
                        uint index3 = (uint)vertexCount;
                        uint index4 = (uint)vertexCount + 1;
                        uint index5 = (uint)vertexCount + 2;

                        // Triangle A
                        Indices[i] = index0;
                        Indices[i + 1] = index3;
                        Indices[i + 2] = index5;

                        // Triangle B
                        Indices.Add(index3);
                        Indices.Add(index4);
                        Indices.Add(index5);

                        // Triangle C
                        Indices.Add(index3);
                        Indices.Add(index1);
                        Indices.Add(index4);

                        // Triangle D
                        Indices.Add(index5);
                        Indices.Add(index4);
                        Indices.Add(index2);
                    }
                }
            }
        }

        public void Roughen(float scale)
        {
            // Group vertices by their position
            Dictionary<Vector3, List<int>> vertexGroupIndices = new();
            for (int i = 0; i < Vertices.Count; i++)
            {
                if (vertexGroupIndices.ContainsKey(Vertices[i].Coordinates))
                {
                    vertexGroupIndices[Vertices[i].Coordinates].Add(i);
                }
                else
                {
                    vertexGroupIndices.Add(Vertices[i].Coordinates, new List<int> { i });
                }
            }

            // Loop through all groups of vertices
            foreach (var indices in vertexGroupIndices.Values)
            {
                // Calculate distance-based strength
                float randomValue = (float)_random.NextDouble();
                float strength = scale * randomValue;

                // Calculate averaged normal for the direction
                Vector3 averagedNormal = Vector3.Zero;
                foreach (var index in indices)
                {
                    averagedNormal += Vertices[index].Normal;
                }
                averagedNormal /= indices.Count;

                // Calculate the final offset
                Vector3 offset = averagedNormal * strength;

                // Offset all vertices in the group
                for (int i = 0; i < indices.Count; i++)
                {
                    var vertex = Vertices[indices[i]];

                    vertex.Coordinates += offset;

                    Vertices[indices[i]] = vertex;
                }
            }

            // Recalculate normals
            RecalculateNormals();
        }

        private void RecalculateNormals()
        {
            if (Indices is null) return;

            for (int i = 0; i < Indices.Count; i += 3)
            {
                Vertex vertexA = Vertices[(int)Indices[i]];
                Vertex vertexB = Vertices[(int)Indices[i + 1]];
                Vertex vertexC = Vertices[(int)Indices[i + 2]];

                // Calculate the new normal
                Vector3 normal = Vector3.Cross(
                    vertexB.Coordinates - vertexA.Coordinates,
                    vertexC.Coordinates - vertexA.Coordinates)
                    .Normalize();

                vertexA.Normal = normal;
                vertexB.Normal = normal;
                vertexC.Normal = normal;

                Vertices[(int)Indices[i]] = vertexA;
                Vertices[(int)Indices[i + 1]] = vertexB;
                Vertices[(int)Indices[i + 2]] = vertexC;
            }
        }

        public void Explode(float scale)
        {
            // TODO: Does this work as expected on spherical meshes?
            //       i.e. it seems to keep the original shape, but just scaled up
            for (int i = 0; i < Vertices.Count; i++)
            {
                var vertex = Vertices[i];
                vertex.Coordinates += vertex.Normal * scale;
                Vertices[i] = vertex;
            }
        }

        private static Vertex CalculateMidpoint(Vertex vertexA, Vertex vertexB)
        {
            Vector3 midpointPosition = (vertexA.Coordinates + vertexB.Coordinates) * 0.5f;
            Vector3 midpointNormal = (vertexA.Normal + vertexB.Normal) * 0.5f;
            Vector2 midpointTexCoord = (vertexA.TextureCoordinates + vertexB.TextureCoordinates) * 0.5f;

            return new Vertex(midpointPosition, midpointNormal, midpointTexCoord);
        }
    }
}