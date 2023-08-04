using PentaGE.Common;
using PentaGE.Core.Graphics;
using PentaGE.Core.Rendering.Materials;
using System.Numerics;

namespace PentaGE.Core.Rendering
{
    public sealed class Grid
    {
        private Mesh _mesh;
        private PBRMaterial _material;
        private int _majorUnits;
        private int _minorUnits;
        private Vector3 _color;
        private float _opacity;

        public Mesh Mesh => _mesh;

        public PBRMaterial Material => _material;

        public int MajorUnits
        {
            get => _majorUnits;
            set { _majorUnits = value; RegenerateMesh(); }
        }

        public int MinorUnits
        {
            get => _minorUnits;
            set { _minorUnits = value; RegenerateMesh(); }
        }

        public Vector3 Color
        {
            get => _color;
            set { _color = value; RegenerateMaterial(); }
        }

        public float Opacity
        {
            get => _opacity;
            set { _opacity = value; RegenerateMaterial(); }
        }

        public Grid()
        {
            _majorUnits = 10;
            _minorUnits = 10;
            _mesh = new Mesh(new List<Vertex>());
            _material = new();
            Color = new(0, 0, 0);
            Opacity = 1f;
            RegenerateMesh();
            RegenerateMaterial();
        }

        public Grid(int majorUnits, int minorUnits, Vector3? color = null, float? opacity = 1)
        {
            MajorUnits = majorUnits;
            MinorUnits = minorUnits;
            Color = color ?? Vector3.Zero;
            Opacity = opacity ?? 1f;
            _mesh = new Mesh(new List<Vertex>());
            _material = new();
            RegenerateMesh();
            RegenerateMaterial();
        }

        private void RegenerateMaterial()
        {
            _material = new()
            {
                Albedo = Color,
                Opacity = Opacity,
            };
        }

        private void RegenerateMesh()
        {
            List<Vertex> vertices = new();
            List<uint> indices = new();

            // Calculate the size of the grid based on the major and minor units
            float gridSize = _majorUnits;
            float minorGridSize = gridSize / _minorUnits;

            int vertexIndex = 0;

            // Generate vertices and indices for the grid along the X axis
            for (int i = -_minorUnits / 2; i <= _minorUnits / 2; i++)
            {
                // Vertical lines along the X axis
                vertices.Add(new Vertex { Coordinates = new Vector3(i * minorGridSize, 0f, -gridSize / 2f), Normal = World.UpVector });
                vertices.Add(new Vertex { Coordinates = new Vector3(i * minorGridSize, 0f, gridSize / 2f), Normal = World.UpVector });
                indices.Add((uint)vertexIndex++);
                indices.Add((uint)vertexIndex++);
            }

            // Generate vertices and indices for the grid along the Z axis
            for (int i = -_minorUnits / 2; i <= _minorUnits / 2; i++)
            {
                // Horizontal lines along the Z axis
                vertices.Add(new Vertex { Coordinates = new Vector3(-gridSize / 2f, 0f, i * minorGridSize), Normal = World.UpVector });
                vertices.Add(new Vertex { Coordinates = new Vector3(gridSize / 2f, 0f, i * minorGridSize), Normal = World.UpVector });
                indices.Add((uint)vertexIndex++);
                indices.Add((uint)vertexIndex++);
            }

            _mesh = new Mesh(vertices, indices);
        }
    }
}