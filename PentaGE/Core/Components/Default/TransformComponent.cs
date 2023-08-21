using PentaGE.Common;

namespace PentaGE.Core.Components
{
    /// <summary>
    /// Represents a component responsible for managing the transformation of an entity within the game world.
    /// </summary>
    public sealed class TransformComponent : Component
    {
        /// <summary>
        /// Gets or sets the transformation data for the entity.
        /// </summary>
        public Transform Transform { get; set; }

        /// <inheritdoc />
        public override bool CanHaveMultiple => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformComponent"/> class.
        /// </summary>
        /// <param name="transform">The transform to initialize the component with. If not provided, a default transform is used.</param>
        public TransformComponent(Transform? transform = null)
        {
            Transform = transform ?? Transform.Identity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformComponent"/> class with a default transform.
        /// </summary>
        public TransformComponent() : this(null) { }

        /// <inheritdoc />
        public override object Clone() => MemberwiseClone();
    }
}