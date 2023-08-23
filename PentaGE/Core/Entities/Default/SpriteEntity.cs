using PentaGE.Common;
using PentaGE.Core.Rendering;
using PentaGE.Core.Rendering.Sprites;
using System.Numerics;

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
        /// <param name="position">The position of the <see cref="SpriteRenderComponent"/> (optional).</param>
        /// <param name="scale">The scale of the <see cref="SpriteRenderComponent"/> (optional).</param>
        public SpriteEntity(Sprite sprite, Shader shader, Vector3? position = null, Vector2? scale = null)
        {
            Transform? transform = position is Vector3 pos
                ? new Transform(pos, Rotation.Zero, scale is Vector2 v2scale ? new(v2scale.X, v2scale.Y, 0f) : Vector3.One) : null;

            Components.Add(new SpriteRenderComponent(sprite, shader, transform));
        }
    }
}