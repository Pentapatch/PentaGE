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

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformComponent"/> class with a default transform.
        /// </summary>
        public TransformComponent()
        {
            Transform = new Transform();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformComponent"/> class with a specified transform.
        /// </summary>
        /// <param name="transform">The initial transformation data for the component.</param>
        public TransformComponent(Transform transform)
        {
            Transform = transform;
        }

        /// <summary>
        /// Called to update the component's state and behavior over time.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update in seconds.</param>
        public override void Update(float deltaTime)
        {
            // Do nothing
        }
    }
}