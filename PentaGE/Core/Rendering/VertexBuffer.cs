using static OpenGL.GL;

namespace PentaGE.Core.Rendering
{
    internal class VertexBuffer : IDisposable
    {
        private readonly uint _id;

        public unsafe VertexBuffer(ref float[] vertices, int size)
        {
            _id = glGenBuffer(); // Generate a buffer object and store its ID

            glBindBuffer(GL_ARRAY_BUFFER, _id);
            fixed (float* v = &vertices[0])
            {
                glBufferData(GL_ARRAY_BUFFER, size, v, GL_STATIC_DRAW);
            }
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