using static OpenGL.GL;
using PentaGE.Core.Rendering;

namespace PentaGE.Core.Graphics
{
    public class Mesh : IDisposable
    {
        private readonly VertexArray vao;
        private readonly VertexBuffer vbo;
        private readonly ElementBuffer? ebo = null;

        internal List<Vertex> Vertices { get; set; }

        internal List<uint>? Indices { get; set; }

        internal unsafe Mesh(List<Vertex> vertices, List<uint>? indices = null)
        {
            Vertices = vertices;
            Indices = indices ?? new List<uint>();

            // Create a VAO, VBO, and (optionally) EBO
            vao = new();
            vbo = new(vertices);
            
            if (indices is not null)
            {
                ebo = new(indices);
            }

            // Bind the VAO, VBO, and EBO to the current context
            vao.Bind();
            vbo.Bind();
            ebo?.Bind();

            // Specify how the vertex attributes should be interpreted.
            int vertexSize = sizeof(Vertex);
            VertexArray.LinkAttribute(0, 3, GL_FLOAT, vertexSize, (void*)0);                          // Coordinates
            VertexArray.LinkAttribute(1, 3, GL_FLOAT, vertexSize, (void*)(3 * sizeof(float)));        // Normal
            VertexArray.LinkAttribute(2, 4, GL_FLOAT, vertexSize, (void*)(6 * sizeof(float)));        // Color
            VertexArray.LinkAttribute(3, 2, GL_FLOAT, vertexSize, (void*)(10 * sizeof(float)));       // Texture coordinates

            // Unbind the VBO, EBO, and VAO to prevent further changes to them.
            VertexBuffer.Unbind();  // Unbind the VBO
            ElementBuffer.Unbind(); // Unbind the EBO
            VertexArray.Unbind();   // Unbind the VAO
        }

        internal unsafe void Draw(bool wireframe = false)
        {
            // Bind the Vertex Array Object (VAO) to use the configuration
            // of vertex attributes stored in it.
            vao.Bind();

            // Draw the object using the indices of the EBO
            glPolygonMode(GL_FRONT_AND_BACK, wireframe ? GL_LINE : GL_FILL);

            if (ebo is not null && Indices is not null)
            {
                ebo.Bind();
                glDrawElements(GL_TRIANGLES, Indices.Count, GL_UNSIGNED_INT, null);
            }
            else
            {
                glDrawArrays(GL_TRIANGLES, 0, Vertices.Count);
            }

            // Unbind the VAO, VBO & EBO to prevent accidental modification.
            VertexArray.Unbind();   // Unbind the VAO
            VertexBuffer.Unbind();  // Unbind the VBO (not necessary, but good practice)
            ElementBuffer.Unbind(); // Unbind the EBO (not necessary, but good practice)
        }

        public void Dispose()
        {
            vao.Dispose();
            vbo.Dispose();
            ebo?.Dispose();
        }
    }
}