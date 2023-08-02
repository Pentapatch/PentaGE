using PentaGE.Core.Entities;
using PentaGE.Core.Rendering;
using System.Collections;

namespace PentaGE.Core.Scenes
{
    public sealed class Scene : IEnumerable<Entity>
    {
        private readonly List<Entity> _entities;

        internal Scene()
        {
            _entities = new();
        }

        public Entity this[int index]
        {
            get => _entities[index];
            set => _entities[index] = value;
        }

        public void AddEntity(Entity entity) =>
            _entities.Add(entity);

        public bool RemoveEntity(Entity entity) =>
            _entities.Remove(entity);

        public void Update(float deltaTime)
        {
            // Loop through entities and update their components.
            foreach (var entity in _entities)
            {
                foreach (var component in entity)
                {
                    component.Update(deltaTime);
                }
            }
        }

        public void Render(Camera camera, Window window)
        {
            // Loop through entities and render entities with a MeshRendererComponent.
            foreach (var entity in _entities)
            {
                var meshRenderer = entity.GetComponent<MeshRenderComponent>();
                meshRenderer?.Render(camera, window);
            }
        }

        public IEnumerator<Entity> GetEnumerator() =>
            ((IEnumerable<Entity>)_entities).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable)_entities).GetEnumerator();
    }
}