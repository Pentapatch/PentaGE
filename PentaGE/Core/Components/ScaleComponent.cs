using System.Numerics;

namespace PentaGE.Core.Components
{
    public sealed class ScaleComponent : Component
    {
        public Vector3 Scale { get; set; }

        public ScaleComponent()
        {
            Scale = Vector3.One;
        }

        public ScaleComponent(Vector3 scale)
        {
            Scale = scale;
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