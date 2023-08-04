using PentaGE.Core.Rendering;

namespace PentaGE.Core.Entities
{
    internal sealed class RenderableGridEntity : Entity
    {
        internal RenderableGridEntity(Grid grid, Shader shader)
        {
            var meshRenderer = new MeshRenderComponent(grid.Mesh, shader, null, grid.Material)
            {
                DrawMode = DrawMode.Lines,
            };

            AddComponent(meshRenderer);
        }
    }
}