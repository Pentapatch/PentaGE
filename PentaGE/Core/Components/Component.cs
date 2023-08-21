using PentaGE.Core.Entities;

namespace PentaGE.Core.Components
{
    /// <summary>
    /// Base class for all components that can be attached to entities within the game world.
    /// </summary>
    public abstract class Component : ICloneable
    {
        /// <summary>
        /// Gets the unique identifier of the component.
        /// </summary>
        public Guid ID { get; private set; }

        /// <summary>
        /// Specifies if the component can be attached to an entity multiple times.
        /// </summary>
        public abstract bool CanHaveMultiple { get; }

        /// <summary>
        /// Gets or sets the entity to which this component is attached.
        /// </summary>
        /// <remarks>
        /// This property allows components to reference the entity to which they are attached.
        /// </remarks>
        public Entity? Entity { get; internal set; }

        /// <summary>
        /// Gets or sets wether the component is enabled and should recieve update events.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Component"/> class with a new unique identifier.
        /// </summary>
        public Component()
        {
            ID = Guid.NewGuid();
        }

        /// <summary>
        /// Sets the component's unique identifier to the specified value.
        /// </summary>
        /// <remarks>This is only to be used when cloning components for the playable scene.</remarks>
        /// <param name="id"></param>
        internal void ReuseId(Guid id)
        {
            ID = id;
        }

        /// <inheritdoc />
        public abstract object Clone();

        /// <summary>
        /// Called to update the component's state and behavior over time.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update in seconds.</param>
        public virtual void Update(float deltaTime) { }
    }
}