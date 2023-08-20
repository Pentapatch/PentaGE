using PentaGE.Core.Assets;
using PentaGE.Core.Components;

namespace PentaGE.Core.Entities
{
    /// <summary>
    /// Represents an entity in the game world that can hold and manage components.
    /// </summary>
    public abstract class Entity : IAsset, ICloneable
    {
        private ComponentManager _components;

        /// <summary>
        /// Gets the unique identifier of the entity.
        /// </summary>
        public Guid ID { get; private set; }

        /// <summary>
        /// Gets the collection of components attached to the entity.
        /// </summary>
        public ComponentManager Components => _components;

        /// <summary>
        /// Specifies if there can be more than one of this entity in the scene.
        /// </summary>
        public virtual bool CanHaveMultipleInstances => true;

        /// <summary>
        /// Gets or sets if the entity is enabled and should recieve update events.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        public Entity()
        {
            ID = Guid.NewGuid();
            _components = new(this);
        }

        /// <inheritdoc />
        public virtual bool Load() => true;

        /// <inheritdoc />
        public virtual void Dispose() { }

        /// <inheritdoc />
        public virtual object Clone()
        {
            var clonedEntity = (Entity)MemberwiseClone();

            clonedEntity.ID = Guid.NewGuid(); // Generate a new ID for the cloned entity

            // Deep copy components
            clonedEntity._components = new(clonedEntity); // Make sure that reference is not shared

            foreach (var component in _components)
            {
                var clonedComponent = (Component)component.Clone();

                clonedComponent.Entity = clonedEntity;
                clonedEntity._components.Add(clonedComponent);
            }

            return clonedEntity;
        }
    }
}