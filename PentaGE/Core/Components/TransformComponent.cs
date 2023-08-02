using PentaGE.Common;

namespace PentaGE.Core.Components
{
    public sealed class TransformComponent : Component
    {
        public Transform Transform { get; set; }

        public TransformComponent()
        {
            Transform = new Transform();
        }

        public TransformComponent(Transform transform)
        {
            Transform = transform;
        }

        public override void Update(float deltaTime)
        {
            // Do nothing
        }
    }
}