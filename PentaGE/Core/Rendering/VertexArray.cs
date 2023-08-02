using System.Runtime.CompilerServices;
using static OpenGL.GL;

namespace PentaGE.Core.Rendering
{
    internal class VertexArray : IDisposable
    {
        private readonly uint _id;

        public VertexArray()
        {
            _id = glGenVertexArray();
        }

        public static unsafe void LinkAttribute(uint index, int size, int type, int stride, void* pointer)
        {
            glVertexAttribPointer(index, size, type, false, stride, pointer);
            glEnableVertexAttribArray(index);
        }

        public void Bind()
        {
            glBindVertexArray(_id);
        }

        public static void Unbind()
        {
            glBindVertexArray(0);
        }

        public void Dispose()
        {
            glDeleteVertexArray(_id);
        }
    }
}