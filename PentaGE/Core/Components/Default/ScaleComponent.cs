using System.Numerics;

namespace PentaGE.Core.Components
{
    /// <summary>
    /// Represents a component that holds the scale of an entity in 3D space.
    /// </summary>
    public sealed class ScaleComponent : Component
    {
        /// <summary>
        /// Gets or sets the scale of the entity in 3D space.
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleComponent"/> class with an optional scale.
        /// </summary>
        /// <param name="scale">The optional initial scale of the entity. If not provided, the scale will be set to (1, 1, 1).</param>
        public ScaleComponent(Vector3? scale = null)
        {
            Scale = scale ?? Vector3.One;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleComponent"/> class without specifying a scale.
        /// </summary>
        public ScaleComponent() : this(null) { }

        /// <inheritdoc />
        public override bool CanHaveMultiple => true;

        /// <inheritdoc />
        public override object Clone() => MemberwiseClone();
    }
}