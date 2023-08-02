namespace PentaGE.Core.Graphics
{
    public sealed class Mesh
    {
        internal List<Vertex> Vertices { get; set; }

        internal List<uint>? Indices { get; set; }

        internal Mesh(List<Vertex> vertices, List<uint>? indices = null)
        {
            Vertices = vertices;
            Indices = indices ?? new List<uint>();
        }

    }
}