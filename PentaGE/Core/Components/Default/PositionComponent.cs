using System.Numerics;

namespace PentaGE.Core.Components
{
    /// <summary>
    /// Represents a component that holds the position of an entity in 3D space.
    /// </summary>
    public sealed class PositionComponent : Component
    {
        /// <summary>
        /// Gets or sets the position of the entity in 3D space.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionComponent"/> class with an optional position.
        /// </summary>
        /// <param name="position">The optional initial position of the entity. If not provided, the position will be set to the origin.</param>
        public PositionComponent(Vector3? position = null)
        {
            Position = position ?? Vector3.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionComponent"/> class without specifying a position.
        /// </summary>
        public PositionComponent() : this(null) { }

        /// <inheritdoc />
        public override bool CanHaveMultiple => true;

        /// <inheritdoc />
        public override object Clone() => MemberwiseClone();
    }
}