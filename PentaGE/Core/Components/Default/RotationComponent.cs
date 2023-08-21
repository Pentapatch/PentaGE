using PentaGE.Common;

namespace PentaGE.Core.Components
{
    /// <summary>
    /// Represents a component that holds the rotation of an entity in 3D space.
    /// </summary>
    public sealed class RotationComponent : Component
    {
        /// <summary>
        /// Gets or sets the rotation of the entity in 3D space.
        /// </summary>
        public Rotation Rotation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RotationComponent"/> class with an optional rotation.
        /// </summary>
        /// <param name="rotation">The optional initial rotation of the entity. If not provided, the rotation will be set to zero.</param>
        public RotationComponent(Rotation? rotation = null)
        {
            Rotation = rotation ?? Rotation.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RotationComponent"/> class without specifying a rotation.
        /// </summary>
        public RotationComponent() : this(null) { }

        /// <inheritdoc />
        public override bool CanHaveMultiple => true;

        /// <inheritdoc />
        public override object Clone() => MemberwiseClone();
    }
}