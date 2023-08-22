using PentaGE.Core.Rendering;
using PentaGE.Core.Rendering.Sprites;

namespace PentaGE.Core.Entities
{
    public class SpriteEntity : Entity
    {
        public SpriteEntity(Sprite sprite, Shader shader)
        {
            Components.Add(new SpriteRenderComponent(sprite, shader));
        }
    }
}