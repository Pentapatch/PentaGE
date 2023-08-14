using PentaGE.Common;
using PentaGE.Core.Graphics;
using System.Numerics;

namespace PentaGE.Core.Rendering
{
    /// <summary>
    /// Represents a grid used for visual reference and alignment.
    /// </summary>
    public sealed class Grid
    {
        private Mesh _mesh;
        private PBRMaterial _material;
        private int _majorUnits;
        private int _minorUnits;
        private Vector3 _color;
        private float _opacity;

        /// <summary>
        /// Gets the mesh representing the visual grid.
        /// </summary>
        public Mesh Mesh => _mesh;

        /// <summary>
        /// Gets the material of the grid.
        /// </summary>
        public PBRMaterial Material => _material;

        /// <summary>
        /// Gets or sets the number of major grid units.
        /// </summary>
        public int MajorUnits
        {
            get => _majorUnits;
            set { _majorUnits = value; RegenerateMesh(); }
        }

        /// <summary>
        /// Gets or sets the number of minor grid units.
        /// </summary>
        public int MinorUnits
        {
            get => _minorUnits;
            set { _minorUnits = value; RegenerateMesh(); }
        }

        /// <summary>
        /// Gets or sets the color of the grid lines.
        /// </summary>
        public Vector3 Color
        {
            get => _color;
            set { _color = value; RegenerateMaterial(); }
        }

        /// <summary>
        /// Gets or sets the opacity of the grid lines.
        /// </summary>
        public float Opacity
        {
            get => _opacity;
            set { _opacity = value; RegenerateMaterial(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid"/> class with default values.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid"/> class with specified parameters.
        /// </summary>
        /// <param name="majorUnits">The number of major grid units.</param>
        /// <param name="minorUnits">The number of minor grid units.</param>
        /// <param name="color">The color of the grid lines.</param>
        /// <param name="opacity">The opacity of the grid lines.</param>
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

        /// <summary>
        /// Regenerates the material of the grid based on its color and opacity settings.
        /// </summary>
        private void RegenerateMaterial()
        {
            _material = new()
            {
                Albedo = Color,
                Opacity = Opacity,
            };
        }

        /// <summary>
        /// Regenerates the mesh of the grid with updated vertices and indices.
        /// </summary>
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