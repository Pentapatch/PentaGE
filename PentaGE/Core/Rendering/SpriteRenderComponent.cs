using PentaGE.Common;
using PentaGE.Core.Components;
using PentaGE.Core.Entities;
using PentaGE.Core.Graphics;
using PentaGE.Core.Rendering.Sprites;
using System.Numerics;
using static OpenGL.GL;

namespace PentaGE.Core.Rendering
{
    /// <summary>
    /// Represents a component for rendering sprites using OpenGL.
    /// </summary>
    public sealed class SpriteRenderComponent : Component, IDisposable
    {
        private readonly Sprite _sprite;
        private readonly Shader _shader;
        private VertexArray _vao;
        private VertexBuffer _vbo;
        private ElementBuffer? _ebo = null;

        /// <inheritdoc />
        public override bool CanHaveMultiple => true;

        /// <summary>
        /// Gets or sets the transform applied to the mesh.
        /// </summary>
        /// <remarks>If no transform is specified, the owning entity's transform is used.</remarks>
        public Transform? Transform { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether backface culling is enabled for rendering.
        /// </summary>
        public bool EnableCulling { get; set; } = false;

        /// <summary>
        /// Gets the transform applied to the mesh.
        /// </summary>
        /// <remarks>
        /// If no transform is specified, the owning entity's transform is used.
        /// </remarks>
        public Transform GetTransform()
        {
            if (Transform is Transform componentTransform)
                return componentTransform;
            else if (Entity is not null && Entity.Components.Get<TransformComponent>() is TransformComponent entityTransformComponent)
                return entityTransformComponent.Transform;
            else
                return new();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteRenderComponent"/> class.
        /// </summary>
        /// <param name="sprite">The sprite to render.</param>
        /// <param name="shader">The shader program used for rendering.</param>
        /// <param name="transform">The transform applied to the component (optional).</param>
        public SpriteRenderComponent(
            Sprite sprite,
            Shader shader,
            Transform? transform = null)
        {
            Transform = transform;

            _sprite = sprite;
            _shader = shader;

            InitializeBuffers();
        }

        /// <summary>
        /// Initializes the vertex and index buffers for rendering the sprite mesh.
        /// </summary>
        private unsafe void InitializeBuffers()
        {
            // Dispose of the old buffers (if applicable)
            _vao?.Dispose();
            _vbo?.Dispose();
            _ebo?.Dispose();

            // Create a VAO, VBO, and (optionally) EBO
            _vao = new();
            _vbo = new(_sprite.Mesh.Vertices);

            if (_sprite.Mesh.Indices is not null)
            {
                _ebo = new(_sprite.Mesh.Indices);
            }

            // Bind the VAO, VBO, and EBO to the current context
            _vao.Bind();
            _vbo.Bind();
            _ebo?.Bind();

            // Specify how the vertex attributes should be interpreted.
            int vertexSize = sizeof(Vertex);
            VertexArray.LinkAttribute(0, 3, GL_FLOAT, vertexSize, (void*)0);                   // Coordinates
            VertexArray.LinkAttribute(1, 3, GL_FLOAT, vertexSize, (void*)(3 * sizeof(float))); // Normal
            VertexArray.LinkAttribute(2, 2, GL_FLOAT, vertexSize, (void*)(6 * sizeof(float))); // Texture coordinates

            // Unbind the VBO, EBO, and VAO to prevent further changes to them.
            VertexBuffer.Unbind();  // Unbind the VBO
            ElementBuffer.Unbind(); // Unbind the EBO
            VertexArray.Unbind();   // Unbind the VAO
        }

        /// <summary>
        /// Renders the sprite using the specified camera and window.
        /// </summary>
        /// <param name="camera">The camera used for rendering.</param>
        /// <param name="window">The window to render onto.</param>
        /// <param name="wireframe">Whether to render the sprite in wireframe mode (optional, default is false).</param>
        internal unsafe void Render(Camera camera, Window window, bool wireframe = false, DirectionalLightEntity? directionalLight = null)
        {
            // Use the shader program
            _shader.Use();

            // Get the transform of the component (if applicable)
            // otherwise the transform component of the entity (if applicable)
            // or create a new default transform
            Transform transform = GetTransform();

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

            // Tell the shader if that the sprite has a texture
            _shader.SetUniform("hasTexture", true);

            // Pass the matrices to the shader (must be done after shader.Use())
            _shader.SetUniform("mvp", mvpMatrix);
            _shader.SetUniform("model", modelMatrix);
            _shader.SetUniform("view", viewMatrix);
            _shader.SetUniform("projection", projectionMatrix);
            _shader.SetUniform("viewportSize", new Vector2(window.Viewport.Width, window.Viewport.Height));

            // Set up the directional light
            var hasDirectionalLight = directionalLight is not null && directionalLight.Enabled;
            if (hasDirectionalLight)
            {
                _shader.SetUniform("directionalLight.direction", directionalLight!.Direction);
                _shader.SetUniform("directionalLight.color", directionalLight.Color);
                _shader.SetUniform("directionalLight.followCamera", directionalLight.FollowCamera);
            }
            _shader.SetUniform("hasDirectionalLight", hasDirectionalLight);

            _shader.SetUniform("lightColor", new Vector4(1f, 1f, 1f, 1f));
            _shader.SetUniform("cameraPosition", camera.Position);

            // Set the texture slot uniform in the shader
            _shader.SetUniform("tex0", 0);

            // Bind the texture to the current context
            _sprite.Texture.Bind();

            // Bind the Vertex Array Object (VAO) to use the configuration
            // of vertex attributes stored in it.
            _vao.Bind();

            // Draw the object using the indices of the EBO or the vertices of the VBO.
            glPolygonMode(GL_FRONT_AND_BACK, wireframe ? GL_LINE : GL_FILL);

            // Enable culling
            bool disableCulling = false;
            if (EnableCulling && !glIsEnabled(GL_CULL_FACE))
            {
                glEnable(GL_CULL_FACE);
                glCullFace(GL_BACK);
                glFrontFace(GL_CCW);
                disableCulling = true;
            }

            if (_ebo is not null && _sprite.Mesh.Indices is not null)
            {
                _ebo.Bind();
                glDrawElements((int)DrawMode.Triangles, _sprite.Mesh.Indices.Count, GL_UNSIGNED_INT, null);
            }
            else
            {
                glDrawArrays((int)DrawMode.Triangles, 0, _sprite.Mesh.Vertices.Count);
            }

            // Disable culling
            if (disableCulling) glDisable(GL_CULL_FACE);

            // Unbind the VAO, VBO & EBO to prevent accidental modification.
            VertexArray.Unbind();   // Unbind the VAO
            VertexBuffer.Unbind();  // Unbind the VBO (not necessary, but good practice)
            ElementBuffer.Unbind(); // Unbind the EBO (not necessary, but good practice)

            // Unbind the texture from the current context
            _sprite.Texture.Unbind();
        }

        /// <inheritdoc />
        public override object Clone() =>
            new SpriteRenderComponent(_sprite, _shader)
            {
                Enabled = true,
                Transform = Transform,
                EnableCulling = EnableCulling,
            };

        /// <inheritdoc />
        public void Dispose()
        {
            _vao.Dispose();
            _vbo.Dispose();
            _ebo?.Dispose();
        }
    }
}