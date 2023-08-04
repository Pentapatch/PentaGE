using PentaGE.Core.Graphics;
using PentaGE.Core.Rendering;
using PentaGE.Core.Rendering.Materials;

namespace PentaGE.Core.Entities
{
    public sealed class RenderableMeshEntity : Entity
    {
        public RenderableMeshEntity(Mesh mesh, Shader shader, Texture? texture = null, PBRMaterial? material = null)
        {
            var meshRenderer = new MeshRenderComponent(mesh, shader, texture, material);

            AddComponent(meshRenderer);
        }
    }
}