﻿using PentaGE.Core.Graphics;
using static OpenGL.GL;

namespace PentaGE.Core.Rendering
{
    /// <summary>
    /// Represents an OpenGL vertex buffer object (VBO) for storing vertex data.
    /// </summary>
    internal class VertexBuffer : IDisposable
    {
        private readonly uint _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexBuffer"/> class.
        /// </summary>
        /// <param name="vertices">An array of vertices to store in the buffer.</param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexBuffer"/> class.
        /// </summary>
        /// <param name="vertices">A list of vertices to store in the buffer.</param>
        public VertexBuffer(List<Vertex> vertices) : this(vertices.ToArray())
        {

        }

        /// <summary>
        /// Binds the vertex buffer for rendering.
        /// </summary>
        public void Bind() =>
            glBindBuffer(GL_ARRAY_BUFFER, _id);

        /// <summary>
        /// Unbinds the vertex buffer after rendering.
        /// </summary>
        public static void Unbind() =>
            glBindBuffer(GL_ARRAY_BUFFER, 0);

        /// <inheritdoc />
        public void Dispose() =>
            glDeleteBuffer(_id);
    }
}