using PentaGE.Core.Components;
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
        private readonly SceneManager _manager;

        /// <summary>
        /// Gets the name of the scene.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the <see cref="DirectionalLightEntity"/> used for lighting the scene if there is one.
        /// </summary>
        public DirectionalLightEntity? DirectionalLight { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Scene class.
        /// </summary>
        internal Scene(string name, SceneManager manager, DirectionalLightEntity? directionalLight = null)
        {
            _entities = new();
            Name = name;
            _manager = manager;
            DirectionalLight = directionalLight;
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
        /// Gets an enumerable collection of entities in the scene.
        /// </summary>
        public IEnumerable<Entity> Entities => _entities;

        /// <summary>
        /// Adds an entity to the scene. 
        /// If the entity is marked as single instance, ensures only one instance of that type exists in the scene.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns><see langword="true"/> if the entity was successfully added, <see langword="false"/> if it was 
        /// not added due to already existing or other reasons.</returns>
        public bool Add(Entity entity)
        {
            // Make sure that single instance entities are not added more than once.
            if (!entity.CanHaveMultipleInstances && _entities.Of(entity).Any()) return false;

            _entities.Add(entity);

            return true;
        }

        /// <summary>
        /// Removes an entity from the scene.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>True if the entity was removed successfully; otherwise, false.</returns>
        public bool Remove(Entity entity) =>
            _entities.Remove(entity);

        /// <summary>
        /// Spawns an entity in the scene during runtime. If the scene is in a valid state (running or paused),
        /// the entity is added to the scene's collection of entities.
        /// </summary>
        /// <param name="entity">The entity to be spawned.</param>
        /// <returns><c>true</c> if the entity is successfully spawned; otherwise, <c>false</c>.</returns>
        public bool SpawnEntity(Entity entity)
        {
            if (_manager.State != SceneState.Running &&
                _manager.State != SceneState.Paused) return false;

            Add(entity);

            return true;
        }

        /// <summary>
        /// Spawns a collection of entities in the scene during runtime. If the scene is in a valid state (running or paused),
        /// the entities are added to the scene's collection of entities.
        /// </summary>
        /// <param name="entities">The collection of entities to be spawned.</param>
        /// <returns><c>true</c> if all entities are successfully spawned; otherwise, <c>false</c>.</returns>
        public bool SpawnEntities(IEnumerable<Entity> entities)
        {
            if (_manager.State != SceneState.Running &&
                _manager.State != SceneState.Paused) return false;

            foreach (var entity in entities)
            {
                Add(entity);
            }

            return true;
        }

        /// <summary>
        /// Destroys the specified entity in the scene during runtime. If the scene is in a valid state (running or paused),
        /// the entity is removed from the scene's collection of entities.
        /// </summary>
        /// <param name="entity">The entity to be destroyed.</param>
        /// <returns><c>true</c> if the entity is successfully destroyed; otherwise, <c>false</c>.</returns>
        public bool DestroyEntity(Entity entity)
        {
            if (_manager.State != SceneState.Running &&
                _manager.State != SceneState.Paused) return false;

            return Remove(entity);
        }

        /// <summary>
        /// Destroys a collection of entities in the scene during runtime. If the scene is in a valid state (running or paused),
        /// the entities are removed from the scene's collection of entities.
        /// </summary>
        /// <param name="entities">The collection of entities to be destroyed.</param>
        /// <returns><c>true</c> if all entities are successfully destroyed; otherwise, <c>false</c>.</returns>
        public bool DestroyEntities(IEnumerable<Entity> entities)
        {
            if (_manager.State != SceneState.Running &&
                _manager.State != SceneState.Paused) return false;

            foreach (var entity in entities.ToList())
            {
                Remove(entity);
            }

            return true;
        }

        /// <summary>
        /// Destroys all entities of the specified type in the scene during runtime. If the scene is in a valid state (running or paused),
        /// the entities of the specified type are removed from the scene's collection of entities.
        /// </summary>
        /// <typeparam name="T">The type of entities to be destroyed.</typeparam>
        /// <returns><c>true</c> if all entities of the specified type are successfully destroyed; otherwise, <c>false</c>.</returns>
        public bool DestroyEntitiesOf<T>() where T : Entity
        {
            if (_manager.State != SceneState.Running &&
                _manager.State != SceneState.Paused) return false;

            var entitiesToRemove = _entities.Of<T>().ToList();

            foreach (var entity in entitiesToRemove)
            {
                Remove(entity);
            }

            return true;
        }

        /// <summary>
        /// Destroys all entities containing the specified component type in the scene during runtime.
        /// If the scene is in a valid state (running or paused), the entities containing the component type
        /// are removed from the scene's collection of entities.
        /// </summary>
        /// <typeparam name="T">The type of component for which entities should be destroyed.</typeparam>
        /// <returns><c>true</c> if all entities containing the specified component type are successfully destroyed; otherwise, <c>false</c>.</returns>
        public bool DestroyEntitiesWith<T>() where T : Component
        {
            if (_manager.State != SceneState.Running &&
                _manager.State != SceneState.Paused) return false;

            var entitiesToRemove = _entities.With<T>().ToList();

            foreach (var entity in entitiesToRemove)
            {
                Remove(entity);
            }

            return true;
        }

        /// <summary>
        /// Removes all entities from the scene.
        /// </summary>
        public void Clear() =>
            _entities.Clear();

        /// <summary>
        /// Loads the scene by setting it as the active scene in the scene manager.
        /// </summary>
        public void Load() =>
            _manager.SetActiveScene(this);

        /// <summary>
        /// Updates all entities and their components in the scene.
        /// </summary>
        /// <param name="deltaTime">The time passed since the last frame.</param>
        internal void Update(float deltaTime)
        {
            // Loop through entities and update their components.
            foreach (var entity in _entities)
            {
                if (!entity.Enabled) continue;

                if (entity.UpdateMode == UpdateMode.WhenPlaying && _manager.State != SceneState.Running) continue;
                if (entity.UpdateMode == UpdateMode.WhenEditing && _manager.State != SceneState.Idle) continue;

                entity.Update(deltaTime);

                foreach (var component in entity.Components)
                {
                    if (component.Enabled) component.Update(deltaTime);
                }
            }
        }

        /// <summary>
        /// Renders all entities with mesh render components in the scene.
        /// </summary>
        /// <param name="camera">The camera used for rendering.</param>
        /// <param name="window">The window used for rendering.</param>
        internal void Render(Camera camera, Window window, bool wireframe = false)
        {
            // Loop through entities and render entities with a MeshRendererComponent.
            foreach (var entity in _entities)
            {
                if (!entity.Visible) continue;

                if (entity.DisplayMode == DisplayMode.WhenPlaying &&
                    _manager.State != SceneState.Running &&
                    _manager.State != SceneState.Paused)
                    continue;

                if (entity.DisplayMode == DisplayMode.WhenEditing &&
                    _manager.State != SceneState.Idle)
                    continue;

                foreach (var meshRenderer in entity.Components.GetAll<MeshRenderComponent>())
                {
                    meshRenderer?.Render(camera, window, wireframe, DirectionalLight);
                }

                foreach (var spriteRenderer in entity.Components.GetAll<SpriteRenderComponent>())
                {
                    spriteRenderer?.Render(camera, window, wireframe, DirectionalLight);
                }
            }
        }

        internal void SetDirectionalLight(DirectionalLightEntity directionalLight) =>
            DirectionalLight = directionalLight;

        /// <inheritdoc />
        public IEnumerator<Entity> GetEnumerator() =>
            ((IEnumerable<Entity>)_entities).GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable)_entities).GetEnumerator();
    }
}