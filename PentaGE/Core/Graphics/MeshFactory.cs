using PentaGE.Common;
using System.Numerics;

//   __________    +
//  /_________/|   ^
// |         | |   | Y
// |         | |   v
// |         | |   -
// |_________|/   /
//               / Z
// - <-- X --> +

//       2          COUNTER CLOCKWISE       3 ----- 2                          -1:1 ------- 1:1
//      / \         WINDING ORDER _^        | \     |                            |     |     |   OpenGL
//     / D \        Triangle A: 0, 3, 5     |  \    |  PLANE                     |     |     |   Right
//  5 /-----\ 4     Triangle B: 3, 4, 5     | A \ B |  Triangle A: 0, 1, 3       |----0:0----|   Handed
//   / \ B / \      Triangle C: 3, 1, 4     |    \  |  Triangle B: 1, 2, 3       |     |     |   Coordinate
//  / A \ / C \     Triangle D: 5, 4, 2     |     \ |                            |     |     |   System
// 0 ----3---- 1    -------------------     0 ----- 1                          -1:-1 ------ 1:-1

//       2          CLOCKWISE               3 ----- 2                          -1:1 ------- 1:1
//      / \         WINDING ORDER _v        | \     |                            |     |     |   OpenGL
//     / A \        Triangle A: 0, 5, 3     |  \    |  PLANE                     |     |     |   Right
//  5 /-----\ 4     Triangle B: 3, 5, 4     | A \ B |  Triangle A: 0, 3, 1       |----0:0----|   Handed
//   / \ C / \      Triangle C: 3, 4, 1     |    \  |  Triangle B: 1, 3, 2       |     |     |   Coordinate
//  / D \ / B \     Triangle D: 5, 2, 4     |     \ |                            |     |     |   System
// 0 ----3-----1    -------------------     0 ----- 1                          -1:-1 ------ 1:-1

// Front, Left, Top    = Counter Clockwise Winding
// Back, Right, Bottom = Clockwise Winding

namespace PentaGE.Core.Graphics
{
    /// <summary>
    /// Provides methods for creating various types of mesh geometries.
    /// </summary>
    public static class MeshFactory
    {
        private static Vector2 TopLeft => new(1f, 1f);
        private static Vector2 TopRight => new(0f, 1f);
        private static Vector2 BottomLeft => new(1f, 0f);
        private static Vector2 BottomRight => new(0f, 0f);
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
                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.ForwardVector, BottomLeft),       // 0
                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.ForwardVector, BottomRight),       // 1
                new Vertex(new Vector3(halfWidth, halfHeight, halfDepth), World.ForwardVector, TopRight),           // 2
                new Vertex(new Vector3(-halfWidth, halfHeight, halfDepth), World.ForwardVector, TopLeft),           // 3

                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.BackwardVector, BottomRight),    // 4
                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.BackwardVector, BottomLeft),      // 5
                new Vertex(new Vector3(halfWidth, halfHeight, -halfDepth), World.BackwardVector, TopLeft),          // 6
                new Vertex(new Vector3(-halfWidth, halfHeight, -halfDepth), World.BackwardVector, TopRight),        // 7

                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.LeftVector, BottomLeft),         // 8
                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.LeftVector, BottomRight),         // 9
                new Vertex(new Vector3(-halfWidth, halfHeight, halfDepth), World.LeftVector, TopRight),             // 10
                new Vertex(new Vector3(-halfWidth, halfHeight, -halfDepth), World.LeftVector, TopLeft),             // 11

                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.RightVector, BottomRight),        // 12
                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.RightVector, BottomLeft),          // 13
                new Vertex(new Vector3(halfWidth, halfHeight, halfDepth), World.RightVector, TopLeft),              // 14
                new Vertex(new Vector3(halfWidth, halfHeight, -halfDepth), World.RightVector, TopRight),            // 15

                new Vertex(new Vector3(-halfWidth, halfHeight, halfDepth), World.UpVector, BottomLeft),             // 16
                new Vertex(new Vector3(halfWidth, halfHeight, halfDepth), World.UpVector, BottomRight),             // 17
                new Vertex(new Vector3(halfWidth, halfHeight, -halfDepth), World.UpVector, TopRight),               // 18
                new Vertex(new Vector3(-halfWidth, halfHeight, -halfDepth), World.UpVector, TopLeft),               // 19

                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.DownVector, TopLeft),             // 20
                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.DownVector, TopRight),             // 21
                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.DownVector, BottomRight),         // 22
                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.DownVector, BottomLeft),         // 23
            };

            // Define indices for the cuboid
            List<uint> indices = new()
            {
                0, 1, 3,    // Front face
                1, 2, 3,
                4, 7, 5,    // Back face
                5, 7, 6,
                8, 9, 11,   // Left face
                9, 10, 11,
                12, 15, 13, // Right face
                13, 15, 14,
                16, 17, 19, // Top face
                17, 18, 19,
                20, 23, 21, // Bottom face
                21, 23, 22
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

            Vector3 upVector = new(0f, halfHeight, 0f);

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

                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.DownVector, TopLeft),
                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.DownVector, TopRight),
                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.DownVector, BottomRight),
                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.DownVector, BottomLeft),
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
                2, 1, 0,    // Front face
                3, 5, 4,    // Back face
                8, 7, 6,    // Left face
                11, 10, 9,  // Right face
                12, 15, 13, // Bottom face
                13, 15, 14, // Bottom face
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

            // Define vertices of the plane
            List<Vertex> vertices = new()
            {
                new Vertex(new Vector3(-halfWidth, 0f, halfHeight), World.UpVector, BottomLeft),
                new Vertex(new Vector3(halfWidth, 0f, halfHeight), World.UpVector, BottomRight),
                new Vertex(new Vector3(halfWidth, 0f, -halfHeight), World.UpVector, TopRight),
                new Vertex(new Vector3(-halfWidth, 0f, -halfHeight), World.UpVector, TopLeft),
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

            if (segments < 5)
                throw new ArgumentOutOfRangeException(nameof(segments), "Segments must be greater than or equal to five.");

            if (textureAspectRatio <= 0f)
                throw new ArgumentOutOfRangeException(nameof(textureAspectRatio), "Texture aspect ratio must be greater than zero.");

            // Calculate half height
            float halfHeight = height * 0.5f;

            // Define vertices of the cylinder
            List<Vertex> vertices = new();
            Vector3 topCenter = new(0f, halfHeight, 0f);
            Vector3 bottomCenter = new(0f, -halfHeight, 0f);
            int segmentsPerFace = segments / 4;
            float textureMapPerFace = 1f / segmentsPerFace;
            for (int i = 0; i <= segments; i++)
            {
                float angle = 2 * MathF.PI * i / segments;
                float x = radius * MathF.Cos(angle);
                float z = radius * MathF.Sin(angle);

                // Calculate texture coordinates based on vertex position
                Vector2 topTexCoord = new(x / (radius * -2) + 0.5f, z / (radius * -2) + 0.5f);
                Vector2 bottomTexCoord = new(x / (radius * -2) + 0.5f, z / (radius * 2) + 0.5f);

                // Adjust the x texture coordinate for the sides
                float u = (i + (1 % segmentsPerFace)) * textureMapPerFace;

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
                vertices.Add(new Vertex(new Vector3(x, halfHeight, z), World.UpVector, topTexCoord));
                vertices.Add(new Vertex(topCenter, World.UpVector, new Vector2(0.5f, 0.5f)));

                // Bottom face vertices
                vertices.Add(new Vertex(new Vector3(x, -halfHeight, z), World.DownVector, bottomTexCoord));
                vertices.Add(new Vertex(bottomCenter, World.DownVector, new Vector2(0.5f, 0.5f)));
            }

            // Define indices for the cylinder
            List<uint> indices = new();

            int stride = 6;
            for (int i = 0; i < segments * stride; i += stride)
            {
                // Bottom side face
                indices.Add((uint)(i));                 // X
                indices.Add((uint)(i + stride + 1));    // XX
                indices.Add((uint)(i + stride));        // XXX

                // Top side face
                indices.Add((uint)(i));                 // XXX
                indices.Add((uint)(i + 1));             //  XX
                indices.Add((uint)(i + stride + 1));    //   X

                // Top face
                indices.Add((uint)(i + 2));             //   X
                indices.Add((uint)(i + 3));             //  XXX
                indices.Add((uint)(i + stride + 2));    // XXXXX

                // Bottom face
                indices.Add((uint)(i + stride + 4));    // XXXXX
                indices.Add((uint)(i + 5));             //  XXX
                indices.Add((uint)(i + 4));             //   X
            }

            return new Mesh(vertices, indices);
        }

        /// <summary>
        /// Creates a cone mesh with the specified radius and height.
        /// </summary>
        /// <param name="radius">The radius of the cylinder.</param>
        /// <param name="height">The height of the cylinder.</param>
        /// <param name="segments">The number of segments used to approximate the cylinder's sides.</param>
        /// <param name="textureAspectRatio">The aspect ratio to adjust the texture mapping.</param>
        /// <returns>A cone mesh.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when radius, height, segments, or textureAspectRatio is out of valid range.</exception>
        public static Mesh CreateCone(float radius, float height, int segments = 64, float textureAspectRatio = 1f)
        {
            // TODO: Known issues:
            // - Not sure that the normals are correct for the cone sides
            //   Behaves wierd when subdivide is called on the mesh
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
                float nextAngle = 2 * MathF.PI * (i + 1) / segments; // Calculate the angle for the next vertex
                float x = radius * MathF.Cos(angle);
                float z = radius * MathF.Sin(angle);

                // Calculate texture coordinates based on vertex position
                Vector2 yTexCoord = new(x / (radius * -2) + 0.5f, z / (radius * 2) + 0.5f);

                // Calculate the azimuthal angle (φ) for the longitude
                float phi = i * 2 * MathF.PI / segments;

                // Calculate azimuthal angle (φ) for texture mapping
                float phiForTexture = phi + MathF.PI / 2; // Adjust by 90 degrees
                float u = phiForTexture / (2 * MathF.PI);

                // Adjust for texture aspect ratio
                u *= textureAspectRatio;

                // Side face vertex
                Vector3 sidePosition = new(x, -halfHeight, z);

                // Calculate the next position along the side
                float nextX = radius * MathF.Cos(nextAngle);
                float nextZ = radius * MathF.Sin(nextAngle);
                Vector3 nextSidePosition = new(nextX, -halfHeight, nextZ);

                Vector3 topPosition = new(0f, halfHeight, 0f);

                // Calculate two vectors for the cross product
                Vector3 sideVector = nextSidePosition - sidePosition;
                Vector3 slantVector = topPosition - sidePosition;

                // Calculate the side normal using the cross product
                Vector3 sideNormal = -Vector3.Cross(sideVector, slantVector).Normalize();

                Vector2 sideTexCoord = new(u, 0f);
                vertices.Add(new Vertex(sidePosition, sideNormal, sideTexCoord));

                // Top face vertex
                vertices.Add(new Vertex(topCenter, World.UpVector, new Vector2(u, 1f)));

                // Bottom face vertices
                vertices.Add(new Vertex(new Vector3(x, -halfHeight, z), World.DownVector, yTexCoord));
                vertices.Add(new Vertex(bottomCenter, World.DownVector, new Vector2(0.5f, 0.5f)));
            }

            // Define indices for the cylinder
            List<uint> indices = new();

            int stride = 4;
            for (int i = 0; i < segments * stride; i += stride)
            {
                // Side face
                indices.Add((uint)(i));                 //   X
                indices.Add((uint)(i + 1));             //  XXX
                indices.Add((uint)(i + stride));        // XXXXX

                // Bottom face
                indices.Add((uint)(i + 2));             //   X
                indices.Add((uint)(i + stride + 2));    //  XXX
                indices.Add((uint)(i + 3));             // XXXXX
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
                // Calculate the inclination angle (θ) for the latitude
                float theta = lat * MathF.PI / latitudeSegments;
                float sinTheta = MathF.Sin(theta);
                float cosTheta = MathF.Cos(theta);

                for (int lon = 0; lon <= longitudeSegments; lon++)
                {
                    // Calculate the azimuthal angle (φ) for the longitude
                    float phi = lon * 2 * MathF.PI / longitudeSegments;
                    float sinPhi = MathF.Sin(phi);
                    float cosPhi = MathF.Cos(phi);

                    // Calculate vertex position
                    float x = cosPhi * sinTheta;
                    float y = cosTheta;
                    float z = sinPhi * sinTheta;

                    Vector3 position = new(x * radius, y * radius, z * radius);
                    Vector3 normal = new(x, y, z);

                    // Calculate azimuthal angle (φ) for texture mapping
                    float phiForTexture = phi + MathF.PI / 2; // Adjust by 90 degrees

                    // Calculate texture coordinates using spherical coordinates
                    float u = phiForTexture / (2 * MathF.PI);
                    float v = theta / MathF.PI;

                    Vector2 texCoord = new(u, -v);

                    vertices.Add(new Vertex(position, normal, texCoord));
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

                    indices.Add((uint)second);
                    indices.Add((uint)first + 1);
                    indices.Add((uint)(second + 1));

                    indices.Add((uint)second);
                    indices.Add((uint)(first));
                    indices.Add((uint)first + 1);
                }
            }

            return new Mesh(vertices, indices);
        }

        /// <summary>
        /// Creates an axes gizmo mesh with a specified scale for each axis.
        /// </summary>
        /// <param name="scale">The scale to be applied to the axes.</param>
        /// <returns>The axes gizmo mesh.</returns>
        // TODO: Should probably be moved to the Sandbox project?
        public static Mesh CreateAxesGizmo(float scale)
        {
            // Define vertices of the axes gizmo
            List<Vertex> vertices = new()
            {
                // X axis
                new Vertex(new Vector3(0, 0, 0),     World.RightVector),
                new Vertex(new Vector3(scale, 0, 0), World.RightVector),
                // Y axis
                new Vertex(new Vector3(0, 0, 0),     World.UpVector),
                new Vertex(new Vector3(0, scale, 0), World.UpVector),
                // Z axis
                new Vertex(new Vector3(0, 0, 0),     World.ForwardVector),
                new Vertex(new Vector3(0, 0, scale), World.ForwardVector),
            };

            // Define indices for the axes gizmo
            List<uint> indices = new()
            {
                0, 1,
                2, 3,
                4, 5,
            };

            return new Mesh(vertices, indices);
        }
    }
}