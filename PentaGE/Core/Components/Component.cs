using PentaGE.Core.Entities;

namespace PentaGE.Core.Components
{
    public abstract class Component
    {
        public Entity? Entity { get; internal set; }

        public abstract void Update(float deltaTime);
    }
}