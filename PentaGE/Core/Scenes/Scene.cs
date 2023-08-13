using PentaGE.Core.Entities;
using PentaGE.Core.Rendering;
using System.Collections;

namespace PentaGE.Core.Scenes
{
    /// <summary>
    /// Represents a scene containing a collection of entities and manages their updates and rendering.
    /// </summary>
    public sealed class Scene : IEnumerable<Entity>
    {
        private readonly List<Entity> _entities;

        /// <summary>
        /// Initializes a new instance of the Scene class.
        /// </summary>
        internal Scene()
        {
            _entities = new();
        }

        /// <summary>
        /// Gets or sets an entity at the specified index within the scene.
        /// </summary>
        /// <param name="index">The index of the entity to get or set.</param>
        /// <returns>The entity at the specified index.</returns>
        public Entity this[int index]
        {
            get => _entities[index];
            set => _entities[index] = value;
        }

        /// <summary>
        /// Adds an entity to the scene.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void AddEntity(Entity entity) =>
            _entities.Add(entity);

        /// <summary>
        /// Removes an entity from the scene.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>True if the entity was removed successfully; otherwise, false.</returns>
        public bool RemoveEntity(Entity entity) =>
            _entities.Remove(entity);

        /// <summary>
        /// Updates all entities and their components in the scene.
        /// </summary>
        /// <param name="deltaTime">The time passed since the last frame.</param>
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

        /// <summary>
        /// Renders all entities with mesh render components in the scene.
        /// </summary>
        /// <param name="camera">The camera used for rendering.</param>
        /// <param name="window">The window used for rendering.</param>
        public void Render(Camera camera, Window window, bool wireframe = false)
        {
            // Loop through entities and render entities with a MeshRendererComponent.
            foreach (var entity in _entities)
            {
                var meshRenderer = entity.GetComponent<MeshRenderComponent>();
                meshRenderer?.Render(camera, window, wireframe);
            }
        }

        /// <inheritdoc />
        public IEnumerator<Entity> GetEnumerator() =>
            ((IEnumerable<Entity>)_entities).GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable)_entities).GetEnumerator();
    }
}