using PentaGE.Core.Assets;
using PentaGE.Core.Components;
using System.Collections;

namespace PentaGE.Core.Entities
{
    /// <summary>
    /// Represents an entity in the game world that can hold and manage components.
    /// </summary>
    public abstract class Entity : IAsset, IEnumerable<Component>
    {
        private readonly List<Component> _components;

        /// <summary>
        /// Gets the unique identifier of the entity.
        /// </summary>
        public Guid ID { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        public Entity()
        {
            ID = Guid.NewGuid();
            _components = new List<Component>();
        }

        /// <inheritdoc />
        public virtual bool Load() => true;

        /// <inheritdoc />
        public virtual void Dispose() { }

        /// <summary>
        /// Adds a component to the entity.
        /// </summary>
        /// <param name="component">The component to be added.</param>
        /// <returns><see langword="true"/> if the component was successfully added; otherwise, <see langword="false"/>.</returns>
        public virtual bool AddComponent(Component component)
        {
            component.Entity = this;

            _components.Add(component);
            return true;
        }

        /// <summary>
        /// Removes a component from the entity.
        /// </summary>
        /// <param name="component">The component to be removed.</param>
        /// <returns><see langword="true"/> if the component was successfully removed; otherwise, <see langword="false"/>.</returns>
        public virtual bool RemoveComponent(Component component)
        {
            if (!ReferenceEquals(component.Entity, this)) return false;
            if (!_components.Contains(component)) return false;

            component.Entity = null;

            return _components.Remove(component);
        }

        /// <summary>
        /// Removes a component of a specific type from the entity.
        /// </summary>
        /// <typeparam name="T">The type of component to remove.</typeparam>
        /// <returns><see langword="true"/> if the component was removed or was not present; otherwise, <see langword="false"/>.</returns>
        public bool RemoveComponent<T>() where T : Component
        {
            var component = GetComponent<T>();
            if (component is null) return true;

            return RemoveComponent(component);
        }

        /// <summary>
        /// Gets a component of a specific type attached to the entity.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve.</typeparam>
        /// <returns>The component of the specified type, if found; otherwise, <see langword="null"/>.</returns>
        public T? GetComponent<T>() where T : Component =>
            _components.FirstOrDefault(c => c is T) as T;

        /// <inheritdoc />
        public IEnumerator<Component> GetEnumerator() =>
            ((IEnumerable<Component>)_components).GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable)_components).GetEnumerator();
    }
}