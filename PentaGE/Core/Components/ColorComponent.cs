using System.Numerics;

namespace PentaGE.Core.Components
{
    public sealed class ColorComponent : Component
    {
        public Vector4 Color { get; set; }

        public Vector3 Color3
        {
            get => new(Color.X, Color.Y, Color.Z);
            set => Color = new(value.X, value.Y, value.Z, Color.W);
        }

        public ColorComponent()
        {
            Color = Vector4.One;
        }

        public ColorComponent(Vector4 color)
        {
            Color = color;
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