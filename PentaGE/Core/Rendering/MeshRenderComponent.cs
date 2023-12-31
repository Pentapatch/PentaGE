﻿using PentaGE.Common;
using PentaGE.Core.Components;
using PentaGE.Core.Entities;
using PentaGE.Core.Graphics;
using System.Numerics;
using static OpenGL.GL;

namespace PentaGE.Core.Rendering
{
    /// <summary>
    /// Represents a component for rendering a mesh using OpenGL.
    /// </summary>
    public sealed class MeshRenderComponent : Component, IDisposable
    {
        private Mesh _mesh;
        private Texture? _texture;
        private VertexArray _vao;
        private VertexBuffer _vbo;
        private ElementBuffer? _ebo = null;

        // TODO: Changing the mesh should update the VBO and EBO.
        /// <summary>
        /// Gets or sets the mesh to be rendered.
        /// </summary>
        public Mesh Mesh
        {
            get => _mesh;
            set
            {
                _mesh = value;
                InitializeBuffers();
            }
        }

        /// <summary>
        /// Gets or sets the transform applied to the mesh.
        /// </summary>
        /// <remarks>If no transform is specified, the owning entity's transform is used.</remarks>
        public Transform? Transform { get; set; }

        /// <summary>
        /// Gets or sets the shader used for rendering the mesh.
        /// </summary>
        public Shader Shader { get; set; }

        /// <summary>
        /// Gets or sets the texture applied to the mesh (optional).
        /// </summary>
        public Texture? Texture
        {
            get => _texture; set
            {
                _texture = value;
            }
        }

        /// <summary>
        /// Gets or sets the PBR material applied to the mesh.
        /// </summary>
        public PBRMaterial Material { get; set; }

        /// <summary>
        /// Gets or sets the draw mode used for rendering the mesh.
        /// </summary>
        public DrawMode DrawMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether backface culling is enabled for rendering.
        /// </summary>
        public bool EnableCulling { get; set; } = false;

        /// <inheritdoc />
        public override bool CanHaveMultiple => true;

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
        /// Initializes a new instance of the <see cref="MeshRenderComponent"/> class.
        /// </summary>
        /// <param name="mesh">The mesh to be rendered.</param>
        /// <param name="shader">The shader used for rendering the mesh.</param>
        /// <param name="texture">The texture applied to the mesh (optional).</param>
        /// <param name="material">The PBR material applied to the mesh (optional).</param>
        /// <param name="transform">The transform applied to the mesh (optional).</param>
        public unsafe MeshRenderComponent(Mesh mesh, Shader shader, Texture? texture = null, PBRMaterial? material = null, Transform? transform = null)
        {
            _mesh = mesh;
            Shader = shader;
            _texture = texture;
            Material = material ?? new();
            DrawMode = DrawMode.Triangles;
            Transform = transform;

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
            _vbo = new(Mesh.Vertices);

            if (Mesh.Indices is not null)
            {
                _ebo = new(Mesh.Indices);
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
        /// Renders the mesh using the specified camera and window.
        /// </summary>
        /// <param name="camera">The camera used for rendering.</param>
        /// <param name="window">The window to render onto.</param>
        /// <param name="wireframe">Whether to render the mesh in wireframe mode (optional, default is false).</param>
        internal unsafe void Render(Camera camera, Window window, bool wireframe = false, DirectionalLightEntity? directionalLight = null)
        {
            // Use the shader program
            Shader.Use();

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

            // Tell the shader if the object has a texture or not
            Shader.SetUniform("hasTexture", Texture is not null);

            // Pass the matrices to the shader (must be done after shader.Use())
            Shader.SetUniform("mvp", mvpMatrix);
            Shader.SetUniform("model", modelMatrix);
            Shader.SetUniform("view", viewMatrix);
            Shader.SetUniform("projection", projectionMatrix);
            Shader.SetUniform("viewportSize", new Vector2(window.Viewport.Width, window.Viewport.Height));

            // Set up the directional light
            var hasDirectionalLight = directionalLight is not null && directionalLight.Enabled;
            if (hasDirectionalLight)
            {
                Shader.SetUniform("directionalLight.direction", directionalLight!.Direction);
                Shader.SetUniform("directionalLight.color", directionalLight.Color);
                Shader.SetUniform("directionalLight.followCamera", directionalLight.FollowCamera);
            }
            Shader.SetUniform("hasDirectionalLight", hasDirectionalLight);

            Shader.SetUniform("lightColor", new Vector4(1f, 1f, 1f, 1f));
            Shader.SetUniform("cameraPosition", camera.Position);

            // Set the material properties as uniforms in the shader
            Shader.SetUniform("material.albedo", Material.Albedo);
            Shader.SetUniform("material.roughness", Material.Roughness);
            Shader.SetUniform("material.metalness", Material.Metallic);
            Shader.SetUniform("material.ambientOcclusion", Material.AmbientOcclusion);
            Shader.SetUniform("material.specularStrength", Material.SpecularStrength);
            Shader.SetUniform("material.opacity", Material.Opacity);

            // Set the texture slot uniform in the shader
            // TODO: Make this more flexible and support multiple textures
            if (Texture is not null)
            {
                Shader.SetUniform("tex0", 0);
            }

            // Bind the texture to the current context
            Texture?.Bind();

            // Bind the Vertex Array Object (VAO) to use the configuration
            // of vertex attributes stored in it.
            _vao.Bind();

            // Enable culling
            bool disableCulling = false;
            if (EnableCulling && !glIsEnabled(GL_CULL_FACE))
            {
                glEnable(GL_CULL_FACE);
                glCullFace(GL_BACK);
                glFrontFace(GL_CCW);
                disableCulling = true;
            }

            // Draw the object using the indices of the EBO or the vertices of the VBO.
            glPolygonMode(GL_FRONT_AND_BACK, wireframe ? GL_LINE : GL_FILL);

            if (_ebo is not null && Mesh.Indices is not null)
            {
                _ebo.Bind();
                glDrawElements((int)DrawMode, Mesh.Indices.Count, GL_UNSIGNED_INT, null);
            }
            else
            {
                glDrawArrays((int)DrawMode, 0, Mesh.Vertices.Count);
            }

            // Disable culling
            if (disableCulling) glDisable(GL_CULL_FACE);

            // Unbind the VAO, VBO & EBO to prevent accidental modification.
            VertexArray.Unbind();   // Unbind the VAO
            VertexBuffer.Unbind();  // Unbind the VBO (not necessary, but good practice)
            ElementBuffer.Unbind(); // Unbind the EBO (not necessary, but good practice)
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _vao.Dispose();
            _vbo.Dispose();
            _ebo?.Dispose();
        }

        /// <inheritdoc />
        public override object Clone() =>
            new MeshRenderComponent(Mesh, Shader, Texture, Material.Clone() as PBRMaterial)
            {
                DrawMode = DrawMode, // Copy the draw mode directly
                Enabled = true,
                Transform = Transform
            };
    }
}