using PentaGE.Common;
using PentaGE.Core.Graphics;
using PentaGE.Core.Rendering;
using PentaGE.Core.Rendering.Sprites;

namespace PentaGE.Core.Entities
{
    /// <summary>
    /// Represents an entity with a sprite component.
    /// </summary>
    public sealed class SpriteEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteEntity"/> class with the specified sprite and shader.
        /// </summary>
        /// <param name="sprite">The sprite to be rendered by the entity.</param>
        /// <param name="shader">The shader used for rendering.</param>
        /// <param name="mesh">The mesh representing the sprite's geometry (optional).</param>
        /// <param name="transform">The transform of the entity (optional).</param>
        /// <param name="meshTransform">The transform of the mesh (optional).</param>
        public SpriteEntity(Sprite sprite, Shader shader, Mesh? mesh = null, Transform? transform = null, Transform? meshTransform = null)
        {
            Components.Add(new SpriteRenderComponent(sprite, shader, mesh, transform, meshTransform));
        }
    }
}