using PentaGE.Common;
using System.Numerics;

namespace PentaGE.Core.Graphics
{
    public static class MeshFactory
    {
        private static Vector2 TopLeft => new(0f, 1f);
        private static Vector2 TopRight => new(1f, 1f);
        private static Vector2 BottomLeft => new(0f, 0f);
        private static Vector2 BottomRight => new(1f, 0f);
        private static Vector2 TopCenter => new(0.5f, 1f);

        public static Mesh CreateCube(float diameter) =>
            CreateCuboid(diameter, diameter, diameter);

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
    }
}