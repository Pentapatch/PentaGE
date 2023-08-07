using PentaGE.Core.Entities;

namespace PentaGE.Core.Components
{
    /// <summary>
    /// Base class for all components that can be attached to entities within the game world.
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// Gets or sets the entity to which this component is attached.
        /// </summary>
        /// <remarks>
        /// This property allows components to reference the entity to which they are attached.
        /// </remarks>
        public Entity? Entity { get; internal set; }

        /// <summary>
        /// Called to update the component's state and behavior over time.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update in seconds.</param>
        public abstract void Update(float deltaTime);
    }
}