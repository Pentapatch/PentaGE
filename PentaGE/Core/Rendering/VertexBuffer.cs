using PentaGE.Core.Graphics;
using static OpenGL.GL;

namespace PentaGE.Core.Rendering
{
    internal class VertexBuffer : IDisposable
    {
        private readonly uint _id;

        public unsafe VertexBuffer(Vertex[] vertices)
        {
            _id = glGenBuffer(); // Generate a buffer object and store its ID

            glBindBuffer(GL_ARRAY_BUFFER, _id);
            fixed (Vertex* v = &vertices[0])
            {
                // Calculate the size of the buffer based on the number of vertices
                int size = sizeof(Vertex) * vertices.Length;

                // Create a pointer to the data
                IntPtr data = new(v);

                // Pass the data to glBufferData
                glBufferData(GL_ARRAY_BUFFER, size, data, GL_STATIC_DRAW); // TODO: Check if Use only v instead of data?
            }
        }

        public VertexBuffer(List<Vertex> vertices) : this(vertices.ToArray())
        {

        }

        public void Bind()
        {
            glBindBuffer(GL_ARRAY_BUFFER, _id);
        }

        public static void Unbind()
        {
            glBindBuffer(GL_ARRAY_BUFFER, 0);
        }

        public void Dispose()
        {
            glDeleteBuffer(_id);
        }
    }
}