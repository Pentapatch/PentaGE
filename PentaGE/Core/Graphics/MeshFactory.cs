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
        private static Vector2 TopLeftUV => new(1f, 1f);
        private static Vector2 TopRightUV => new(0f, 1f);
        private static Vector2 BottomLeftUV => new(1f, 0f);
        private static Vector2 BottomRightUV => new(0f, 0f);
        private static Vector2 TopCenterUV => new(0.5f, 1f);
        private static Vector2 CenterUV => new(0.5f, 0.5f);

        #region 3D Primitives

        /// <summary>
        /// Creates an axes widget mesh with a specified scale for each axis.
        /// </summary>
        /// <param name="scale">The scale to be applied to the axes.</param>
        /// <returns>The axes widget mesh.</returns>
        // TODO: Should probably be moved to the Sandbox project?
        public static Mesh CreateAxesWidget(float scale)
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

        /// <summary>
        /// Creates a cone mesh with the specified radius and height.
        /// </summary>
        /// <param name="radius">The radius at the base of the cylinder.</param>
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
                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.ForwardVector, BottomLeftUV),       // 0
                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.ForwardVector, BottomRightUV),       // 1
                new Vertex(new Vector3(halfWidth, halfHeight, halfDepth), World.ForwardVector, TopRightUV),           // 2
                new Vertex(new Vector3(-halfWidth, halfHeight, halfDepth), World.ForwardVector, TopLeftUV),           // 3

                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.BackwardVector, BottomRightUV),    // 4
                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.BackwardVector, BottomLeftUV),      // 5
                new Vertex(new Vector3(halfWidth, halfHeight, -halfDepth), World.BackwardVector, TopLeftUV),          // 6
                new Vertex(new Vector3(-halfWidth, halfHeight, -halfDepth), World.BackwardVector, TopRightUV),        // 7

                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.LeftVector, BottomLeftUV),         // 8
                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.LeftVector, BottomRightUV),         // 9
                new Vertex(new Vector3(-halfWidth, halfHeight, halfDepth), World.LeftVector, TopRightUV),             // 10
                new Vertex(new Vector3(-halfWidth, halfHeight, -halfDepth), World.LeftVector, TopLeftUV),             // 11

                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.RightVector, BottomRightUV),        // 12
                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.RightVector, BottomLeftUV),          // 13
                new Vertex(new Vector3(halfWidth, halfHeight, halfDepth), World.RightVector, TopLeftUV),              // 14
                new Vertex(new Vector3(halfWidth, halfHeight, -halfDepth), World.RightVector, TopRightUV),            // 15

                new Vertex(new Vector3(-halfWidth, halfHeight, halfDepth), World.UpVector, BottomLeftUV),             // 16
                new Vertex(new Vector3(halfWidth, halfHeight, halfDepth), World.UpVector, BottomRightUV),             // 17
                new Vertex(new Vector3(halfWidth, halfHeight, -halfDepth), World.UpVector, TopRightUV),               // 18
                new Vertex(new Vector3(-halfWidth, halfHeight, -halfDepth), World.UpVector, TopLeftUV),               // 19

                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.DownVector, TopLeftUV),             // 20
                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.DownVector, TopRightUV),             // 21
                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.DownVector, BottomRightUV),         // 22
                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.DownVector, BottomLeftUV),         // 23
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
                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.ForwardVector, BottomLeftUV),
                new Vertex(upVector, World.ForwardVector, TopCenterUV),
                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.ForwardVector, BottomRightUV),

                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.BackwardVector, BottomLeftUV),
                new Vertex(upVector, World.BackwardVector, TopCenterUV),
                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.BackwardVector, BottomRightUV),

                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.LeftVector, BottomLeftUV),
                new Vertex(upVector, World.LeftVector, TopCenterUV),
                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.LeftVector, BottomRightUV),

                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.RightVector, BottomLeftUV),
                new Vertex(upVector, World.RightVector, TopCenterUV),
                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.RightVector, BottomRightUV),

                new Vertex(new Vector3(-halfWidth, -halfHeight, halfDepth), World.DownVector, TopLeftUV),
                new Vertex(new Vector3(halfWidth, -halfHeight, halfDepth), World.DownVector, TopRightUV),
                new Vertex(new Vector3(halfWidth, -halfHeight, -halfDepth), World.DownVector, BottomRightUV),
                new Vertex(new Vector3(-halfWidth, -halfHeight, -halfDepth), World.DownVector, BottomLeftUV),
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
        /// Creates a sphere mesh with the specified diameter using spherical coordinates mapping.
        /// </summary>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="latitudeSegments">The number of segments used to approximate the sphere's latitude.</param>
        /// <param name="longitudeSegments">The number of segments used to approximate the sphere's longitude.</param>
        /// <returns>A sphere mesh with the specified diameter.</returns>
        public static Mesh CreateSphere(float radius, int latitudeSegments = 16, int longitudeSegments = 32)
        {
            if (radius <= 0f)
                throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be greater than zero.");

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

        #endregion

        #region 2D Primitives

        /// <summary>
        /// Creates a circular 2D mesh with the specified diameter and number of segments.
        /// </summary>
        /// <param name="radius">The diameter of the circle.</param>
        /// <param name="segments">The number of segments used to approximate the circle.</param>
        /// <param name="rotation">Optional rotation applied to the circle.</param>
        /// <returns>A circular mesh with the specified diameter and number of segments.</returns>
        public static Mesh CreateCircle(float radius, int segments = 64, Rotation? rotation = null) =>
            CreateEllipse(radius, radius, segments, rotation);

        /// <summary>
        /// Creates an elliptical 2D mesh with the specified radii and number of segments.
        /// </summary>
        /// <param name="radiusX">The X-axis radius of the ellipse.</param>
        /// <param name="radiusY">The Y-axis radius of the ellipse.</param>
        /// <param name="segments">The number of segments used to approximate the ellipse.</param>
        /// <param name="rotation">Optional rotation applied to the ellipse.</param>
        /// <returns>An elliptical mesh with the specified radii and number of segments.</returns>
        public static Mesh CreateEllipse(float radiusX, float radiusY, int segments = 64, Rotation? rotation = null)
        {
            if (radiusX <= 0f)
                throw new ArgumentOutOfRangeException(nameof(radiusX), "Radius X must be greater than zero.");
            if (radiusY <= 0f)
                throw new ArgumentOutOfRangeException(nameof(radiusY), "Radius Y must be greater than zero.");
            if (segments < 5)
                throw new ArgumentOutOfRangeException(nameof(segments), "Segments must be greater than or equal to five.");

            List<Vertex> vertices = new();
            List<uint> indices = new();

            // Calculate center position
            Vector3 centerPosition = Vector3.Zero;

            // Add center vertex
            vertices.Add(new Vertex(centerPosition, World.UpVector, CenterUV));

            // Calculate the aspect ratio of the ellipse
            float aspectRatio = radiusY / radiusX;

            // Generate vertices and indices
            for (int i = 0; i < segments + 1; i++)
            {
                // Calculate the position of the point on the ellipse
                float angle = i * (MathF.PI * 2f / segments);
                Vector3 vertexPosition = new(MathF.Cos(angle) * radiusX, 0f, MathF.Sin(angle) * radiusY);

                // Calculate texture coordinates based on vertex position
                float u = 1f - (vertexPosition.X / (radiusX * 2 * aspectRatio) + 0.5f); // Adjust for aspect ratio
                float v = 1f - (vertexPosition.Z / (radiusY * 2) + 0.5f);               // Map [-1, 1] to [0, 1]
                Vector2 vertexUV = new(u, v);

                // Define vertices of the ellipse
                vertices.Add(new Vertex(vertexPosition, World.UpVector, vertexUV));

                // Define indices of the ellipse
                indices.Add(0);                                 // Center vertex
                indices.Add((uint)(i == segments ? 1 : i + 2)); // Next vertex (loop around)
                indices.Add((uint)(i + 1));                     // Current vertex
            }

            var mesh = new Mesh(vertices, indices);
            if (rotation is not null)
            {
                mesh.Rotate(rotation.Value);
            }

            return mesh;
        }

        /// <summary>
        /// Creates a rectangle 2D plane mesh with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="rotation">Optional rotation applied to the rectangle.</param>
        /// <returns>A plane mesh representing a rectangle with the specified width and height.</returns>
        public static Mesh CreateRectangle(float width, float height, Rotation? rotation = null)
        {
            // Calculate half extents for each dimension
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;

            // Define vertices of the plane
            List<Vertex> vertices = new()
            {
                new Vertex(new Vector3(-halfWidth, 0f, halfHeight), World.UpVector, BottomLeftUV),
                new Vertex(new Vector3(halfWidth, 0f, halfHeight), World.UpVector, BottomRightUV),
                new Vertex(new Vector3(halfWidth, 0f, -halfHeight), World.UpVector, TopRightUV),
                new Vertex(new Vector3(-halfWidth, 0f, -halfHeight), World.UpVector, TopLeftUV),
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
        /// Creates a square 2D plane mesh with the specified size.
        /// </summary>
        /// <param name="size">The size of the square (both width and height).</param>
        /// <param name="rotation">Optional rotation applied to the square.</param>
        /// <returns>A plane mesh representing a square with the specified size.</returns>
        public static Mesh CreateSquare(float size, Rotation? rotation = null) =>
            CreateRectangle(size, size, rotation);

        #endregion
    }
}