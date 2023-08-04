﻿using static OpenGL.GL;

namespace PentaGE.Core.Rendering
{
    internal class ElementBuffer : IDisposable
    {
        private readonly uint _id;

        public unsafe ElementBuffer(uint[] indices)
        {
            _id = glGenBuffer(); // Generate a buffer object and store its ID

            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, _id);
            fixed (uint* i = &indices[0])
            {
                // Calculate the size of the buffer based on the number of vertices
                int size = sizeof(uint) * indices.Length;

                glBufferData(GL_ELEMENT_ARRAY_BUFFER, size, i, GL_STATIC_DRAW);
            }
        }

        public ElementBuffer(List<uint> indices) : this(indices.ToArray())
        {

        }

        public void Bind()
        {
            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, _id);
        }

        public static void Unbind()
        {
            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
        }

        public void Dispose()
        {
            glDeleteBuffer(_id);
        }
    }
}