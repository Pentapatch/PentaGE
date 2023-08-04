using PentaGE.Common;
using PentaGE.Core.Components;
using PentaGE.Core.Graphics;
using PentaGE.Core.Rendering.Materials;
using System.Numerics;
using static OpenGL.GL;

namespace PentaGE.Core.Rendering
{
    public sealed class MeshRenderComponent : Component, IDisposable
    {
        private readonly VertexArray vao;
        private readonly VertexBuffer vbo;
        private readonly ElementBuffer? ebo = null;

        // TODO: Changing the mesh should update the VBO and EBO.
        public Mesh Mesh { get; set; }

        public Shader Shader { get; set; }

        public Texture? Texture { get; }

        public PBRMaterial Material { get; set; }

        public unsafe MeshRenderComponent(Mesh mesh, Shader shader, Texture? texture = null, PBRMaterial? material = null)
        {
            Mesh = mesh;
            Shader = shader;
            Texture = texture;
            Material = material ?? new();

            // Create a VAO, VBO, and (optionally) EBO
            vao = new();
            vbo = new(Mesh.Vertices);

            if (Mesh.Indices is not null)
            {
                ebo = new(Mesh.Indices);
            }

            // Bind the VAO, VBO, and EBO to the current context
            vao.Bind();
            vbo.Bind();
            ebo?.Bind();

            // Specify how the vertex attributes should be interpreted.
            int vertexSize = sizeof(Vertex);
            VertexArray.LinkAttribute(0, 3, GL_FLOAT, vertexSize, (void*)0);                          // Coordinates
            VertexArray.LinkAttribute(1, 3, GL_FLOAT, vertexSize, (void*)(3 * sizeof(float)));        // Normal
            VertexArray.LinkAttribute(2, 2, GL_FLOAT, vertexSize, (void*)(6 * sizeof(float)));       // Texture coordinates

            // Unbind the VBO, EBO, and VAO to prevent further changes to them.
            VertexBuffer.Unbind();  // Unbind the VBO
            ElementBuffer.Unbind(); // Unbind the EBO
            VertexArray.Unbind();   // Unbind the VAO
        }

        internal unsafe void Render(Camera camera, Window window, bool wireframe = false)
        {
            // Use the shader program
            Shader.Use();

            // Get the transform of the object (if applicable)
            // or create a new default transform
            Transform transform = Entity?.GetComponent<TransformComponent>()?.Transform ?? new Transform();

            // Calculate the view and projection matrices from the camera
            // ViewMatrix means "camera space" (or "eye space") and is used for moving the camera.
            // ProjectionMatrix means "clip space" (or "normalized device coordinates")
            //  and is used for clipping and perspective.
            // ModelMatrix means "object space" (or "model space") - the default space for an object.
            var viewMatrix = camera.GetViewMatrix();
            var projectionMatrix = camera.GetProjectionMatrix(window.Size.Width, window.Size.Height);
            var modelMatrix = Matrix4x4.CreateScale(transform.Scale)
                * transform.Rotation.ToMatrix4x4()
                * Matrix4x4.CreateTranslation(transform.Position);

            // Combine the model, view, and projection matrices to get the final MVP matrix
            var mvpMatrix = modelMatrix * viewMatrix * projectionMatrix;

            // Tell the shader if the object has a texture or not
            Shader.SetUniform("hasTexture", Texture is not null);

            // Pass the matrices to the shader (must be done after shader.Use())
            Shader.SetUniform("mvp", mvpMatrix);
            Shader.SetUniform("model", modelMatrix);

            // Set the light color uniforms
            Shader.SetUniform("lightColor", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
            Shader.SetUniform("lightPosition", new Vector3(0.5f, 0.5f, 0.5f));
            Shader.SetUniform("cameraPosition", camera.Position);

            // Set the material properties as uniforms in the shader
            Shader.SetUniform("material.albedo", Material.Albedo);
            Shader.SetUniform("material.roughness", Material.Roughness);
            Shader.SetUniform("material.metalness", Material.Metalness);
            Shader.SetUniform("material.ambientOcclusion", Material.AmbientOcclusion);
            Shader.SetUniform("material.specularStrength", Material.SpecularStrength);

            // Bind the texture to the current context
            Texture?.Bind();

            // Bind the Vertex Array Object (VAO) to use the configuration
            // of vertex attributes stored in it.
            vao.Bind();

            // Draw the object using the indices of the EBO or the vertices of the VBO.
            glPolygonMode(GL_FRONT_AND_BACK, wireframe ? GL_LINE : GL_FILL);

            if (ebo is not null && Mesh.Indices is not null)
            {
                ebo.Bind();
                glDrawElements(GL_TRIANGLES, Mesh.Indices.Count, GL_UNSIGNED_INT, null);
            }
            else
            {
                glDrawArrays(GL_TRIANGLES, 0, Mesh.Vertices.Count);
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

        public override void Update(float deltaTime)
        {
            // Do nothing
        }
    }
}