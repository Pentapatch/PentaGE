using PentaGE.Core.Rendering;

namespace PentaGE.Core.Entities
{
    /// <summary>
    /// Represents a specialized entity that holds components to render a grid object.
    /// </summary>
    internal sealed class RenderableGridEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderableGridEntity"/> class with the specified grid and shader.
        /// </summary>
        /// <param name="grid">The grid to be rendered.</param>
        /// <param name="shader">The shader used for rendering the grid.</param>
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