using PentaGE.Core.Entities;
using System.Collections;

namespace PentaGE.Core.Components
{
    /// <summary>
    /// Represents a manager for components attached to an entity.
    /// </summary>
    public sealed class ComponentManager : IEnumerable<Component>
    {
        private readonly List<Component> _components;
        private readonly Entity _entity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentManager"/> class.
        /// </summary>
        /// <param name="entity">The entity to which this manager is associated.</param>
        internal ComponentManager(Entity entity)
        {
            _components = new List<Component>();
            _entity = entity;
        }

        /// <summary>
        /// Gets the component at the specified index within the manager.
        /// </summary>
        /// <param name="index">The index of the component to get.</param>
        /// <returns>The component at the specified index, or null if index is out of range.</returns>
        public Component? this[int index] =>
            index >= 0 && index < _components.Count ? _components[index] : null;

        /// <summary>
        /// Gets the first component of the specified type within the manager.
        /// </summary>
        /// <param name="type">The type of component to get.</param>
        /// <returns>The first component of the specified type, or null if not found.</returns>
        public Component? this[Type type] =>
            _components.FirstOrDefault(c => c.GetType() == type);

        /// <summary>
        /// Gets the component with the specified unique identifier within the manager.
        /// </summary>
        /// <param name="id">The unique identifier of the component to get.</param>
        /// <returns>The component with the specified ID, or null if not found.</returns>
        public Component? this[Guid id] =>
            _components.FirstOrDefault(c => c.ID == id);

        /// <summary>
        /// Gets the first component with the specified type name within the manager.
        /// </summary>
        /// <param name="typeName">The case insensitive name of the component type to get.</param>
        /// <returns>The first component with the specified type name, or null if not found.</returns>
        public Component? this[string typeName] =>
            _components.FirstOrDefault(c => c.GetType().Name.ToLower() == typeName.ToLower());

        /// <summary>
        /// Adds a component to the entity.
        /// </summary>
        /// <param name="component">The component to be added.</param>
        /// <returns>The unique identifier of the added component if successful; otherwise, <see langword="null"/>.</returns>
        public Guid? Add(Component component)
        {
            // Check if the component is already attached to an entity.
            if (!component.CanHaveMultiple && this.Has(component.GetType()))
                return null;

            component.Entity = _entity;

            _components.Add(component);
            return component.ID;
        }

        /// <summary>
        /// Adds a new empty component of the specified type to the entity.
        /// </summary>
        /// <typeparam name="T">The type of component to add.</typeparam>
        /// <returns>The unique identifier of the added component if successful; otherwise, <see langword="null"/>.</returns>
        public Guid? Add<T>() where T : Component, new() =>
            Add(new T());

        /// <summary>
        /// Removes a component from the entity.
        /// </summary>
        /// <param name="component">The component to be removed.</param>
        /// <returns><see langword="true"/> if the component was successfully removed; otherwise, <see langword="false"/>.</returns>
        public bool Remove(Component component)
        {
            if (!ReferenceEquals(component.Entity, _entity)) return false;
            if (!_components.Contains(component)) return false;

            component.Entity = null;

            return _components.Remove(component);
        }

        /// <summary>
        /// Removes the first component of a specific type from the entity.
        /// </summary>
        /// <typeparam name="T">The type of component to remove.</typeparam>
        /// <returns><see langword="true"/> if a component was found and removed; otherwise, <see langword="false"/>.</returns>
        public bool Remove<T>() where T : Component
        {
            var component = this.Get<T>();
            if (component is null) return true;

            return Remove(component);
        }

        /// <summary>
        /// Removes a component attached to the entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the component to remove.</param>
        /// <returns><see langword="true"/> if the component with the given ID was found and removed; otherwise, <see langword="false"/>.</returns>
        public bool Remove(Guid id)
        {
            if (id == Guid.Empty) return false;

            if (this.Get(id) is Component component)
                return Remove(component);

            return false;
        }

        /// <summary>
        /// Removes all components of a specific type from the entity.
        /// </summary>
        /// <typeparam name="T">The type of components to remove.</typeparam>
        public void RemoveAll<T>() where T : Component
        {
            var components = this.GetAll<T>();
            if (components is null) return;

            foreach (var component in components)
                Remove(component);
        }

        /// <inheritdoc />
        public IEnumerator<Component> GetEnumerator() =>
            ((IEnumerable<Component>)_components).GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable<Component>)_components).GetEnumerator();
    }
}