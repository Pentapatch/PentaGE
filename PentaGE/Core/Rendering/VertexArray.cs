using static OpenGL.GL;

namespace PentaGE.Core.Rendering
{
    /// <summary>
    /// Represents an OpenGL vertex array object (VAO).
    /// </summary>
    internal class VertexArray : IDisposable
    {
        private readonly uint _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexArray"/> class.
        /// </summary>
        public VertexArray()
        {
            _id = glGenVertexArray();
        }

        /// <summary>
        /// Links a vertex attribute to the VAO.
        /// </summary>
        /// <param name="index">The index of the attribute.</param>
        /// <param name="size">The number of components per attribute.</param>
        /// <param name="type">The data type of each component.</param>
        /// <param name="stride">The byte offset between consecutive attributes.</param>
        /// <param name="pointer">The pointer to the attribute data.</param>
        public static unsafe void LinkAttribute(uint index, int size, int type, int stride, void* pointer)
        {
            glVertexAttribPointer(index, size, type, false, stride, pointer);
            glEnableVertexAttribArray(index);
        }

        /// <summary>
        /// Binds the vertex array for rendering.
        /// </summary>
        public void Bind() =>
            glBindVertexArray(_id);

        /// <summary>
        /// Unbinds the vertex array after rendering.
        /// </summary>
        public static void Unbind() =>
            glBindVertexArray(0);

        /// <inheritdoc />
        public void Dispose() =>
            glDeleteVertexArray(_id);
    }
}