using static OpenGL.GL;

namespace PentaGE.Core.Rendering
{
    /// <summary>
    /// Represents an OpenGL element buffer (EBO).
    /// </summary>
    internal class ElementBuffer : IDisposable
    {
        private readonly uint _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementBuffer"/> class.
        /// </summary>
        /// <param name="indices">An array of indices representing the elements.</param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementBuffer"/> class.
        /// </summary>
        /// <param name="indices">A list of indices representing the elements.</param>
        public ElementBuffer(List<uint> indices) : this(indices.ToArray())
        {

        }

        /// <summary>
        /// Binds the element buffer for rendering.
        /// </summary>
        public void Bind() =>
            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, _id);

        /// <summary>
        /// Unbinds the element buffer after rendering.
        /// </summary>
        public static void Unbind() =>
            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);

        /// <inheritdoc />
        public void Dispose() =>
            glDeleteBuffer(_id);
    }
}