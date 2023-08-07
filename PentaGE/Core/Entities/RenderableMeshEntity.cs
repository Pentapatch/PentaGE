﻿using PentaGE.Core.Graphics;
using PentaGE.Core.Rendering;
using PentaGE.Core.Rendering.Materials;

namespace PentaGE.Core.Entities
{
    /// <summary>
    /// Represents an entity that holds components to render a mesh object.
    /// </summary>
    public sealed class RenderableMeshEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderableMeshEntity"/> class with the specified mesh, shader, texture, and material.
        /// </summary>
        /// <param name="mesh">The mesh to be rendered.</param>
        /// <param name="shader">The shader used for rendering the mesh.</param>
        /// <param name="texture">The texture applied to the mesh (optional).</param>
        /// <param name="material">The material properties of the mesh (optional).</param>
        public RenderableMeshEntity(Mesh mesh, Shader shader, Texture? texture = null, PBRMaterial? material = null)
        {
            var meshRenderer = new MeshRenderComponent(mesh, shader, texture, material);

            AddComponent(meshRenderer);
        }
    }
}