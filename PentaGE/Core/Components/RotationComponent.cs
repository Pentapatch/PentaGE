using PentaGE.Common;

namespace PentaGE.Core.Components
{
    public sealed class RotationComponent : Component
    {
        public Rotation Rotation { get; set; }

        public RotationComponent()
        {
            Rotation = Rotation.Zero;
        }

        public RotationComponent(Rotation rotation)
        {
            Rotation = rotation;
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