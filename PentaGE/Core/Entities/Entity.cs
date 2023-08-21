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
        private readonly Dictionary<Guid, Entity> entityReferences = new();
        private readonly Dictionary<Guid, Component> componentReferences = new();

        /// <summary>
        /// Sets a reference to the specified entity, allowing efficient access and management of cross-entity relationships.
        /// </summary>
        /// <param name="entity">The entity to establish a reference to. If null, the method returns <see cref="Guid.Empty"/>.</param>
        /// <returns>The unique identifier of the entity reference.</returns>
        protected Guid SetReference(Entity? entity)
        {
            if (entity is null) return Guid.Empty;

            if (entityReferences.ContainsKey(entity.ID))
            {
                entityReferences[entity.ID] = entity;
            }
            else
            {
                entityReferences.Add(entity.ID, entity);
            }

            return entity.ID;
        }

        /// <summary>
        /// Sets a reference to the specified component, allowing efficient access and management of cross-component relationships.
        /// </summary>
        /// <param name="component">The component to establish a reference to. If null, the method returns <see cref="Guid.Empty"/>.</param>
        /// <returns>The unique identifier of the component reference.</returns>
        protected Guid SetReference(Component? component)
        {
            if (component is null) return Guid.Empty;

            if (componentReferences.ContainsKey(component.ID))
            {
                componentReferences[component.ID] = component;
            }
            else
            {
                componentReferences.Add(component.ID, component);
            }

            return component.ID;
        }

        /// <summary>
        /// Retrieves a reference to the specified entity or component using its unique identifier.
        /// </summary>
        /// <typeparam name="T">The type of the reference (entity or component).</typeparam>
        /// <param name="id">The unique identifier of the entity or component.</param>
        /// <returns>The reference to the entity or component, or null if not found.</returns>
        protected T? GetReference<T>(Guid id) where T : class
        {
            if (typeof(T).IsSubclassOf(typeof(Entity)))
            {
                return entityReferences[id] as T;
            }
            else if (typeof(T).IsSubclassOf(typeof(Component)))
            {
                return componentReferences[id] as T;
            }

            return null;
        }

        /// <summary>
        /// Updates references to entities and components in the provided scene, ensuring accurate cross-entity relationships.
        /// </summary>
        /// <param name="scene">The scene in which to update references.</param>
        internal void UpdateReferences(Scene scene)
        {
            foreach (var entity in entityReferences)
            {
                entityReferences[entity.Key] = scene.Get(entity.Key)!;
            }
            foreach (var component in componentReferences)
            {
                componentReferences[component.Key] = scene.Get(ID)!.Components.Get(component.Key)!;
            }
        }

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
        /// Gets the display mode for the entity. By default, the entity is always displayed.
        /// </summary>
        public virtual DisplayMode DisplayMode => DisplayMode.Always;

        /// <summary>
        /// Gets or sets the update mode for the entity.
        /// </summary>
        public virtual UpdateMode UpdateMode => UpdateMode.WhenPlaying;

        /// <summary>
        /// Gets or sets if the entity is enabled and should recieve update events.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets if the entity is visible and should be rendered.
        /// </summary>
        public bool Visible { get; set; } = true;

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

        /// <summary>
        /// This method is called once when the playable scene is stopped.
        /// Implement this method in derived entities to reestablish references and perform
        /// any necessary initialization for interactions within the new scene.
        /// </summary>
        /// <param name="scene">The scene in which the entity is a part of.</param>
        public virtual void SceneEnd(Scene scene) { }

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