using System.Numerics;

namespace PentaGE.Core.Components
{
    /// <summary>
    /// Represents a component that holds color information for an entity.
    /// </summary>
    public sealed class ColorComponent : Component
    {
        /// <summary>
        /// Gets or sets the color value represented as a <see cref="Vector4"/>.
        /// </summary>
        public Vector4 Color { get; set; }

        /// <summary>
        /// Gets or sets the color value represented as a <see cref="Vector3"/>.
        /// </summary>
        public Vector3 Color3
        {
            get => new(Color.X, Color.Y, Color.Z);
            set => Color = new(value.X, value.Y, value.Z, Color.W);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorComponent"/> class without specifying a color.
        /// </summary>
        public ColorComponent() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorComponent"/> class with the specified color.
        /// </summary>
        /// <param name="color">The color value. If not provided, a default color is used.</param>
        public ColorComponent(Vector4? color = null)
        {
            Color = color ?? Vector4.One;
        }

        /// <inheritdoc />
        public override bool CanHaveMultiple => true;

        /// <inheritdoc />
        public override object Clone() => MemberwiseClone();

    }
}