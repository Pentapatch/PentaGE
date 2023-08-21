using PentaGE.Core.Assets;
using PentaGE.Core.Components;
using PentaGE.Core.Scenes;

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

        /// <summary>
        /// Allows the entity to update its state and behavior over time.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update in seconds.</param>
        /// <param name="scene">The scene to which the entity belongs.</param>
        public virtual void Update(float deltaTime) { }

        /// <summary>
        /// This method is called once at the beginning of a scene or game loop iteration.
        /// Implement this method in derived entities to establish references and perform
        /// any necessary initialization for interactions within the scene.
        /// </summary>
        /// <param name="scene">The scene in which the entity is a part of.</param>
        public virtual void SceneBegin(Scene scene) { }

        /// <inheritdoc />
        public virtual bool Load() => true;

        /// <inheritdoc />
        public virtual void Dispose() { }

        /// <inheritdoc />
        public virtual object Clone()
        {
            var clonedEntity = (Entity)MemberwiseClone();

            // TODO: Investigate if the cloned entity should have the same ID as the original
            //clonedEntity.ID = Guid.NewGuid(); // Generate a new ID for the cloned entity

            // Deep copy components
            clonedEntity._components = new(clonedEntity); // Make sure that reference is not shared

            foreach (var component in _components)
            {
                var clonedComponent = (Component)component.Clone();

                clonedComponent.Entity = clonedEntity;
                clonedComponent.ReuseId(component.ID);
                clonedEntity._components.Add(clonedComponent);
            }

            return clonedEntity;
        }
    }
}