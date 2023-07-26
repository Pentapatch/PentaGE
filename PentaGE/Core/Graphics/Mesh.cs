namespace PentaGE.Core.Graphics
{
    public class Mesh
    {
        private readonly int _vertexStride;
       
        public float[] Vertices { get; set; }
        public int VertexCount { get; private set; }
        public float[]? TextureCoordinates { get; set; } // For texture mapping
        public float[]? Normals { get; set; } // For lighting calculations


        public Mesh(float[] vertices, float[]? textureCoordinates, float[]? normals)
        {
            Vertices = vertices;
            TextureCoordinates = textureCoordinates;
            Normals = normals;
            _vertexStride = CalculateVertexStride();
            VertexCount = vertices.Length / (_vertexStride / sizeof(float));
        }

        public static Mesh CreateRectangle2d(float width, float height)
        {
            float[] vertices = new float[]
            {
                // Position                 // Texture coordinates
                -width / 2, -height / 2,    0.0f, 0.0f, // Bottom-left
                width / 2, -height / 2,     1.0f, 0.0f, // Bottom-right
                width / 2, height / 2,      1.0f, 1.0f, // Top-right
                -width / 2, height / 2,     0.0f, 1.0f // Top-left
            };

            return new Mesh(vertices, null, null); // Texture coordinates and normals are null for 2D rectangle
        }

        private int CalculateVertexStride()
        {
            int stride = 0;

            // Position (x, y)
            stride += 2;

            // Texture coordinates (u, v)
            if (TextureCoordinates is not null)
            {
                stride += 2;
            }

            // Normals (x, y, z)
            if (Normals is not null)
            {
                stride += 3;
            }

            // Add padding to make the stride a multiple of float size (optional but recommended for performance)
            stride += stride % 4 == 0 ? 0 : 4 - (stride % 4);

            return stride * sizeof(float);
        }

    }
}