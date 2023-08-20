using System.Numerics;

namespace PentaGE.Core.Components
{
    public sealed class PositionComponent : Component
    {
        public Vector3 Position { get; set; }

        public PositionComponent()
        {
            Position = Vector3.Zero;
        }

        public PositionComponent(Vector3 position)
        {
            Position = position;
        }

        /// <inheritdoc />
        public override bool CanHaveMultiple => true;

        /// <inheritdoc />
        public override object Clone() => MemberwiseClone();

        /// <inheritdoc />
        public override void Update(float deltaTime)
        {
            // Do nothing
        }
    }
}