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
        /// <returns><see langword="true"/> if the component was successfully added; otherwise, <see langword="false"/>.</returns>
        public bool Add(Component component)
        {
            // Check if the component is already attached to an entity.
            if (!component.CanHaveMultiple && Has(component.GetType()))
                return false;

            component.Entity = _entity;

            _components.Add(component);
            return true;
        }

        /// <summary>
        /// Adds a new empty component of the specified type to the entity.
        /// </summary>
        /// <typeparam name="T">The type of component to add.</typeparam>
        /// <returns><see langword="true"/> if the component was successfully added; otherwise, <see langword="false"/>.</returns>
        public bool Add<T>() where T : Component, new() =>
            Add(new T());

        /// <summary>
        /// Tests if the entity has a component of a specific type.
        /// </summary>
        /// <typeparam name="T">The type of component to test.</typeparam>
        /// <returns></returns>
        public bool Has<T>() where T : Component =>
            Get<T>() is not null;

        /// <summary>
        /// Tests if the entity has a component of a specific type.
        /// </summary>
        /// <param name="type">The type of component to test.</param>
        /// <returns></returns>
        public bool Has(Type type) =>
            _components.Any(c => c.GetType() == type);

        /// <summary>
        /// Removes a component from the entity.
        /// </summary>
        /// <param name="component">The component to be removed.</param>
        /// <returns><see langword="true"/> if the component was successfully removed; otherwise, <see langword="false"/>.</returns>
        public bool Remove(Component component)
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
        public bool Remove<T>() where T : Component
        {
            var component = Get<T>();
            if (component is null) return true;

            return Remove(component);
        }

        /// <summary>
        /// Gets a component of a specific type attached to the entity.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve.</typeparam>
        /// <returns>The component of the specified type, if found; otherwise, <see langword="null"/>.</returns>
        public T? Get<T>() where T : Component =>
            _components.FirstOrDefault(c => c is T) as T;

        /// <summary>
        /// Gets all components of a specific type attached to the entity.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve.</typeparam>
        /// <returns>A list of components of the specified type.</returns>
        public List<T> GetAll<T>() where T : Component =>
            _components.Where(c => c is T).Cast<T>().ToList();

        /// <inheritdoc />
        public IEnumerator<Component> GetEnumerator() =>
            ((IEnumerable<Component>)_components).GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable<Component>)_components).GetEnumerator();
    }
}