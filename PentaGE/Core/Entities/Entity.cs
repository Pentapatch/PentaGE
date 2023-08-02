using PentaGE.Core.Components;
using System.Collections;

namespace PentaGE.Core.Entities
{
    public abstract class Entity : IEnumerable<Component>
    {
        public Guid ID { get; private set; }
        private readonly List<Component> _components;

        public Entity()
        {
            ID = Guid.NewGuid();
            _components = new List<Component>();
        }

        public virtual bool AddComponent(Component component)
        {
            component.Entity = this;

            _components.Add(component);
            return true;
        }

        public virtual bool RemoveComponent(Component component)
        {
            if (!ReferenceEquals(component.Entity, this)) return false;
            if (!_components.Contains(component)) return false;

            component.Entity = null;

            return _components.Remove(component);
        }

        public bool RemoveComponent<T>() where T : Component
        {
            var component = GetComponent<T>();
            if (component is null) return true;

            return RemoveComponent(component);
        }

        public T? GetComponent<T>() where T : Component =>
            _components.FirstOrDefault(c => c is T) as T;

        public IEnumerator<Component> GetEnumerator() =>
            ((IEnumerable<Component>)_components).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            ((IEnumerable)_components).GetEnumerator();
    }
}