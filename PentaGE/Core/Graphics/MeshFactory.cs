using PentaGE.Common;
using System.Numerics;

namespace PentaGE.Core.Graphics
{
    /// <summary>
    /// Provides methods for creating various types of mesh geometries.
    /// </summary>
    public static class MeshFactory
    {
        private static Vector2 TopLeft => new(0f, 1f);
        private static Vector2 TopRight => new(1f, 1f);
        private static Vector2 BottomLeft => new(0f, 0f);
        private static Vector2 BottomRight => new(1f, 0f);
        private static Vector2 TopCenter => new(0.5f, 1f);

        /// <summary>
        /// Creates a cube mesh with the specified diameter.
        /// </summary>
        /// <param name="diameter">The diameter of the cube.</param>
        /// <returns>A cube mesh with the specified diameter.</returns>
        public static Mesh CreateCube(float diameter) =>
            CreateCuboid(diameter, diameter, diameter);

        /// <summary>
        /// Creates a cuboid mesh with the specified dimensions.
        /// </summary>
        /// <param name="width">The width of the cuboid.</param>
        /// <param name="height">The height of the cuboid.</param>
        /// <param name="depth">The depth of the cuboid.</param>
        /// <returns>A cuboid mesh with the specified dimensions.</returns>
        public static Mesh CreateCuboid(float width, float height, float depth)
        {
            // Calculate half extents for each dimension
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;
            float halfDepth = depth * 0.5f;

            // Define vertices of the cuboid
            List<Vertex> vertices = new()
            {
                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.ForwardVector, BottomLeft),
                new Vertex(new Vector3(-halfWidth, halfHeight, halfDepth), World.ForwardVector, TopLeft),
                new Vertex(new Vector3(halfWidth, halfHeight, halfDepth), World.ForwardVector, TopRight),
                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.ForwardVector, BottomRight),

                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.BackwardVector, BottomLeft),
                new Vertex(new Vector3(-halfWidth, halfHeight, -halfDepth), World.BackwardVector, TopLeft),
                new Vertex(new Vector3(halfWidth, halfHeight, -halfDepth), World.BackwardVector, TopRight),
                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.BackwardVector, BottomRight),

                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.LeftVector, BottomLeft),
                new Vertex(new Vector3(-halfWidth, halfHeight, -halfDepth), World.LeftVector, TopLeft),
                new Vertex(new Vector3(-halfWidth, halfHeight, halfDepth), World.LeftVector, TopRight),
                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.LeftVector, BottomRight),

                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.RightVector, BottomLeft),
                new Vertex(new Vector3(halfWidth, halfHeight, -halfDepth), World.RightVector, TopLeft),
                new Vertex(new Vector3(halfWidth, halfHeight, halfDepth), World.RightVector, TopRight),
                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.RightVector, BottomRight),

                new Vertex(new Vector3(-halfWidth, halfHeight, -halfDepth), World.UpVector, BottomLeft),
                new Vertex(new Vector3(-halfWidth, halfHeight, halfDepth), World.UpVector, TopLeft),
                new Vertex(new Vector3(halfWidth, halfHeight, halfDepth), World.UpVector, TopRight),
                new Vertex(new Vector3(halfWidth, halfHeight, -halfDepth), World.UpVector, BottomRight),

                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.DownVector, BottomLeft),
                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.DownVector, TopLeft),
                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.DownVector, TopRight),
                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.DownVector, BottomRight),
            };

            // Define indices for the cuboid
            List<uint> indices = new()
            {
                0, 1, 2,    // Front face
                2, 3, 0,
                4, 5, 6,    // Back face
                6, 7, 4,
                8, 9, 10,   // Left face
                10, 11, 8,
                12, 13, 14, // Right face
                14, 15, 12,
                16, 17, 18, // Top face
                18, 19, 16,
                20, 21, 22, // Bottom face
                22, 23, 20
            };

            return new Mesh(vertices, indices);
        }

        /// <summary>
        /// Creates a pyramid mesh with the specified dimensions.
        /// </summary>
        /// <param name="width">The width of the pyramid base.</param>
        /// <param name="height">The height of the pyramid.</param>
        /// <param name="depth">The depth of the pyramid base.</param>
        /// <returns>A pyramid mesh with the specified dimensions.</returns>
        public static Mesh CreatePyramid(float width, float height, float depth)
        {
            // Calculate half extents for each dimension
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;
            float halfDepth = depth * 0.5f;

            Vector3 upVector = new(0f, 1f, 0f);

            // Define vertices of the pyramid
            List<Vertex> vertices = new()
            {
                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.ForwardVector, BottomLeft),
                new Vertex(upVector, World.ForwardVector, TopCenter),
                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.ForwardVector, BottomRight),

                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.BackwardVector, BottomLeft),
                new Vertex(upVector, World.BackwardVector, TopCenter),
                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.BackwardVector, BottomRight),

                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.LeftVector, BottomLeft),
                new Vertex(upVector, World.LeftVector, TopCenter),
                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.LeftVector, BottomRight),

                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.RightVector, BottomLeft),
                new Vertex(upVector, World.RightVector, TopCenter),
                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.RightVector, BottomRight),

                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.DownVector, BottomLeft),
                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.DownVector, TopLeft),
                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.DownVector, TopRight),
                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.DownVector, BottomRight),
            };

            // Recalculate the normals for the front, back, left and right faces
            for (int i = 0; i < 12; i += 3)
            {
                Vector3 a = vertices[i].Coordinates;
                Vector3 b = vertices[i + 1].Coordinates;
                Vector3 c = vertices[i + 2].Coordinates;
                Vector3 normal = Vector3.Cross(c - a, b - a).Normalize();
                vertices[i] = new Vertex(vertices[i].Coordinates, normal, vertices[i].TextureCoordinates);
                vertices[i + 1] = new Vertex(vertices[i + 1].Coordinates, normal, vertices[i + 1].TextureCoordinates);
                vertices[i + 2] = new Vertex(vertices[i + 2].Coordinates, normal, vertices[i + 2].TextureCoordinates);
            }

            // Define indices for the pyramid
            List<uint> indices = new()
            {
                0, 1, 2,    // Front face
                3, 4, 5,    // Back face
                6, 7, 8,    // Left face
                9, 10, 11,  // Right face
                12, 13, 14, // Bottom face
                14, 15, 12, // Bottom face
            };

            return new Mesh(vertices, indices);
        }

        /// <summary>
        /// Creates a plane mesh with a square shape and the specified size.
        /// </summary>
        /// <param name="size">The size of the plane (both width and height).</param>
        /// <param name="rotation">Optional rotation applied to the plane.</param>
        /// <returns>A plane mesh with the specified size.</returns>
        public static Mesh CreatePlane(float size, Rotation? rotation = null) => 
            CreatePlane(size, size, rotation);

        /// <summary>
        /// Creates a plane mesh with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the plane.</param>
        /// <param name="height">The height of the plane.</param>
        /// <param name="rotation">Optional rotation applied to the plane.</param>
        /// <returns>A plane mesh with the specified width and height.</returns>
        public static Mesh CreatePlane(float width, float height, Rotation? rotation = null)
        {
            // Calculate half extents for each dimension
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;

            //       2          COUNTER CLOCKWISE       3-------2 
            //      / \         WINDING ORDER _^        | \     |
            //     / D \        Triangle A: 0, 3, 5     |  \    |  PLANE
            //  5 /-----\ 4     Triangle B: 3, 4, 5     | A \ B |  Triangle A: 0, 1, 3
            //   / \ B / \      Triangle C: 3, 1, 4     |    \  |  Triangle B: 1, 2, 3
            //  / A \ / C \     Triangle D: 5, 4, 2     |     \ |  
            // 0 ----3-----1    -------------------     0-------1

            // Define vertices of the plane
            List<Vertex> vertices = new()
            {
                new Vertex(new Vector3(-halfWidth, 0f, -halfHeight), World.UpVector, BottomLeft),
                new Vertex(new Vector3(halfWidth, 0f, -halfHeight), World.UpVector, BottomRight),
                new Vertex(new Vector3(halfWidth, 0f, halfHeight), World.UpVector, TopRight),
                new Vertex(new Vector3(-halfWidth, 0f, halfHeight), World.UpVector, TopLeft),
            };

            // Define indices for the plane
            List<uint> indices = new()
            {
                0, 1, 3,
                1, 2, 3
            };

            Mesh mesh = new(vertices, indices);
            if (rotation is not null)
            {
                mesh.Rotate(rotation.Value);
            }

            return mesh;
        }

        /// <summary>
        /// Creates a cylinder mesh with the specified radius and height.
        /// </summary>
        /// <param name="radius">The radius of the cylinder.</param>
        /// <param name="height">The height of the cylinder.</param>
        /// <param name="segments">The number of segments used to approximate the cylinder's sides.</param>
        /// <param name="textureAspectRatio">The aspect ratio to adjust the texture mapping.</param>
        /// <returns>A cylinder mesh.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when radius, height, segments, or textureAspectRatio is out of valid range.</exception>
        public static Mesh CreateCylinder(float radius, float height, int segments = 16, float textureAspectRatio = 1f)
        {
            if (radius <= 0f)
                throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be greater than zero.");

            if (height <= 0f)
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");

            if (segments < 3)
                throw new ArgumentOutOfRangeException(nameof(segments), "Segments must be greater than or equal to three.");

            if (textureAspectRatio <= 0f)
                throw new ArgumentOutOfRangeException(nameof(textureAspectRatio), "Texture aspect ratio must be greater than zero.");

            // Calculate half height
            float halfHeight = height * 0.5f;

            // Define vertices of the cylinder
            List<Vertex> vertices = new();
            Vector3 topCenter = new(0f, halfHeight, 0f);
            Vector3 bottomCenter = new(0f, -halfHeight, 0f);
            for (int i = 0; i <= segments; i++)
            {
                float angle = 2 * MathF.PI * i / segments;
                float x = radius * MathF.Cos(angle);
                float z = radius * MathF.Sin(angle);

                // Calculate texture coordinates based on vertex position
                Vector2 yTexCoord = new(x / (radius * 2) + 0.5f, z / (radius * 2) + 0.5f);

                // Adjust the x texture coordinate for the sides
                float u = x / radius * 0.5f + 0.5f;

                // Adjust for texture aspect ratio
                u *= textureAspectRatio;

                // Bottom side face vertex
                Vector3 bottomSidePosition = new(x, -halfHeight, z);
                Vector3 bottomSideNormal = new Vector3(x, 0f, z).Normalize();
                Vector2 bottomSideTexCoord = new(u, 0f);
                vertices.Add(new Vertex(bottomSidePosition, bottomSideNormal, bottomSideTexCoord));

                // Top side face vertex
                Vector3 topSidePosition = new(x, halfHeight, z);
                Vector3 topSideNormal = new Vector3(x, 0f, z).Normalize();
                Vector2 topSideTexCoord = new(u, 1f);
                vertices.Add(new Vertex(topSidePosition, topSideNormal, topSideTexCoord));

                // Top face vertices
                vertices.Add(new Vertex(new Vector3(x, halfHeight, z), World.UpVector, yTexCoord));
                vertices.Add(new Vertex(topCenter, World.UpVector, new Vector2(0.5f, 0.5f)));

                // Bottom face vertices
                vertices.Add(new Vertex(new Vector3(x, -halfHeight, z), World.DownVector, yTexCoord));
                vertices.Add(new Vertex(bottomCenter, World.DownVector, new Vector2(0.5f, 0.5f)));
            }

            // Define indices for the cylinder
            List<uint> indices = new();

            int stride = 6;
            for (int i = 0; i < segments * stride; i += stride)
            {
                // Bottom side face
                indices.Add((uint)i);                   // BS1  |    x
                indices.Add((uint)(i + 1));             // TS1  |   xx
                indices.Add((uint)(i + stride));        // BS2  |  xxx

                // Top side face
                indices.Add((uint)(i + stride));        // BS2  |  xxx
                indices.Add((uint)(i + 1));             // TS1  |  xx
                indices.Add((uint)(i + stride + 1));    // TS2  |  x

                // Top face
                indices.Add((uint)(i + 2));             // TF1  |  xxxxx
                indices.Add((uint)(i + 3));             // TC   |   xxx
                indices.Add((uint)(i + stride + 2));    // TF2  |    x  

                // Bottom face
                indices.Add((uint)(i + 4));             // BF1  |  xxxxx
                indices.Add((uint)(i + 5));             // BC   |   xxx
                indices.Add((uint)(i + stride + 4));    // BF2  |    x
            }

            return new Mesh(vertices, indices);
        }

        /// <summary>
        /// Creates a sphere mesh with the specified diameter using spherical coordinates mapping.
        /// </summary>
        /// <param name="diameter">The diameter of the sphere.</param>
        /// <param name="latitudeSegments">The number of segments used to approximate the sphere's latitude.</param>
        /// <param name="longitudeSegments">The number of segments used to approximate the sphere's longitude.</param>
        /// <returns>A sphere mesh with the specified diameter.</returns>
        public static Mesh CreateSphere(float diameter, int latitudeSegments = 16, int longitudeSegments = 32)
        {
            // Calculate radius
            float radius = diameter * 0.5f;

            // Define vertices of the sphere
            List<Vertex> vertices = new();

            for (int lat = 0; lat <= latitudeSegments; lat++)
            {
                // Calculate the angle of latitude
                float theta = lat * MathF.PI / latitudeSegments;
                float sinTheta = MathF.Sin(theta);
                float cosTheta = MathF.Cos(theta);

                for (int lon = 0; lon <= longitudeSegments; lon++)
                {
                    // Calculate the angle of longitude
                    float phi = lon * 2 * MathF.PI / longitudeSegments;
                    float sinPhi = MathF.Sin(phi);
                    float cosPhi = MathF.Cos(phi);

                    // Calculate vertex position
                    float x = cosPhi * sinTheta;
                    float y = cosTheta;
                    float z = sinPhi * sinTheta;

                    Vector3 position = new(x * radius, y * radius, z * radius);
                    Vector3 normal = new(x, y, z);
                    Vector2 texCoord = new((float)lon / longitudeSegments, (float)lat / latitudeSegments);

                    vertices.Add(new Vertex(position, normal, -texCoord));
                }
            }

            // Define indices for the sphere
            List<uint> indices = new();

            for (int lat = 0; lat < latitudeSegments; lat++)
            {
                for (int lon = 0; lon < longitudeSegments; lon++)
                {
                    int first = lat * (longitudeSegments + 1) + lon;
                    int second = first + longitudeSegments + 1;

                    indices.Add((uint)first);
                    indices.Add((uint)second);
                    indices.Add((uint)(first + 1));

                    indices.Add((uint)second);
                    indices.Add((uint)(second + 1));
                    indices.Add((uint)(first + 1));
                }
            }

            return new Mesh(vertices, indices);
        }
    }
}