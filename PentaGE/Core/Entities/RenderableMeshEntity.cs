using PentaGE.Core.Graphics;
using PentaGE.Core.Rendering;

namespace PentaGE.Core.Entities
{
    public sealed class RenderableMeshEntity : Entity
    {
        public RenderableMeshEntity(Mesh mesh, Shader shader, Texture? texture = null)
        {
            var meshRenderer = new MeshRenderComponent(mesh, shader, texture);

            AddComponent(meshRenderer);
        }
    }
}