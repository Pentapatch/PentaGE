using PentaGE.Core.Entities;
using Serilog;

namespace PentaGE.Core.Scenes
{
    /// <summary>
    /// Manages scenes within the game engine, allowing users to create, manipulate, and control scenes and their entities.
    /// </summary>
    public sealed class SceneManager
    {
        private readonly Dictionary<string, Scene> _scenes;
        private Scene _activeScene;
        private Scene? _playableScene;

        /// <summary>
        /// Gets the current state of the scene manager.
        /// </summary>
        public SceneState State { get; private set; }

        /// <summary>
        /// Gets the <see cref="ActiveScene"/> if the current state is <see cref="SceneState.Idle"/>, otherwise a deep-cloned copy of the active scene.
        /// </summary>
        public Scene CurrentScene =>
            _playableScene ?? _activeScene;

        /// <summary>
        /// Gets the scene that is set as the active scene.
        /// </summary>
        public Scene ActiveScene => _activeScene;

        /// <summary>
        /// Initializes a new instance of the SceneManager class.
        /// </summary>
        internal SceneManager()
        {
            _activeScene = new("Default", this);
            _scenes = new();
            State = SceneState.Idle;
        }

        /// <summary>
        /// Gets the <see cref="Scene"/> with the specified name from the collection of scenes.
        /// </summary>
        /// <param name="name">The name of the scene to retrieve.</param>
        /// <returns>The scene with the specified name.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the scene with the specified name does not exist.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the name is null.</exception>"
        public Scene this[string name] => _scenes[name];

        /// <summary>
        /// Gets the <see cref="Scene"/> at the specified index in the collection of scenes.
        /// </summary>
        /// <param name="index">The index of the scene to retrieve.</param>
        /// <returns>The scene at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
        /// <exception cref="ArgumentNullException">Thrown when the index is null.</exception>
        public Scene this[int index] => _scenes.Values.ElementAt(index);

        /// <summary>
        /// Sets the scene with the specified name as the active scene, stopping any ongoing activity in the current scene.
        /// </summary>
        /// <param name="name">The name of the scene to set as the active scene.</param>
        /// <returns><c>true</c> if the scene with the specified name was found and set as the active scene; otherwise, <c>false</c>.</returns>
        public bool SetActiveScene(string name)
        {
            if (_scenes.TryGetValue(name, out var scene))
            {
                SetActiveScene(scene);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the provided scene as the active scene, stopping any ongoing activity in the current scene.
        /// </summary>
        /// <param name="scene">The scene to set as the active scene.</param>
        public void SetActiveScene(Scene scene)
        {
            _activeScene = scene;
            Stop();
            Log.Information("Current scene is set to {SceneName}.", scene.Name);
        }

        /// <summary>
        /// Adds a new scene with the specified name to the collection of scenes.
        /// </summary>
        /// <param name="name">The name of the scene to add.</param>
        /// <returns>The newly added scene.</returns>
        public Scene Add(string name)
        {
            var scene = new Scene(name, this);
            _scenes.Add(name, scene);

            return scene;
        }

        /// <summary>
        /// Checks if a scene with the specified name exists in the collection of scenes.
        /// </summary>
        /// <param name="name">The name of the scene to check.</param>
        /// <returns><c>true</c> if a scene with the specified name exists; otherwise, <c>false</c>.</returns>
        public bool Exists(string name) =>
            _scenes.ContainsKey(name);

        /// <summary>
        /// Removes the scene with the specified name from the collection of scenes.
        /// </summary>
        /// <param name="name">The name of the scene to remove.</param>
        /// <remarks>
        /// This method will fail if attempting to remove the active scene.
        /// </remarks>
        /// <returns><c>true</c> if the scene was successfully removed; otherwise, <c>false</c>.</returns>
        public bool Remove(string name)
        {
            if (_activeScene.Name == name) return false;
            return _scenes.Remove(name);
        }

        /// <summary>
        /// Removes the specified scene from the collection of scenes.
        /// </summary>
        /// <param name="scene">The scene to remove.</param>
        /// <remarks>
        /// This method will fail if attempting to remove the active scene.
        /// </remarks>
        /// <returns><c>true</c> if the scene was successfully removed; otherwise, <c>false</c>.</returns>
        public bool Remove(Scene scene) =>
            _scenes.Remove(scene.Name);

        #region State management

        /// <summary>
        /// Starts running the scene. If the current state is <see cref="SceneState.Idle"/>,
        /// a playable scene is created before setting the state to <see cref="SceneState.Running"/>.
        /// </summary>
        /// <returns><c>true</c> if the scene is started or resumed; otherwise, <c>false</c>.</returns>
        public bool Run()
        {
            if (State == SceneState.Running)
                return false;
            else if (State == SceneState.Idle)
                CreatePlayableScene();

            State = SceneState.Running;
            Log.Information("Scene is running.");

            return true;
        }

        /// <summary>
        /// Stops the scene, resetting the state to <see cref="SceneState.Idle"/>.
        /// </summary>
        /// <returns><c>true</c> if the scene is stopped; otherwise, <c>false</c>.</returns>
        public bool Stop()
        {
            if (State == SceneState.Idle) return false;

            _playableScene = null;

            State = SceneState.Idle;
            Log.Information("Scene is stopped.");

            return true;
        }

        /// <summary>
        /// Restarts the scene. If the current state is <see cref="SceneState.Running"/> or <see cref="SceneState.Paused"/>,
        /// the scene is stopped and then started again.
        /// </summary>
        /// <returns><c>true</c> if the scene is restarted; otherwise, <c>false</c>.</returns>
        public bool Restart()
        {
            if (State != SceneState.Running &&
                State != SceneState.Paused) return false;

            Stop();
            Run();

            return true;
        }

        /// <summary>
        /// Pauses the running scene, changing its state to <see cref="SceneState.Paused"/>.
        /// </summary>
        /// <returns><c>true</c> if the scene is paused; otherwise, <c>false</c>.</returns>
        public bool Pause()
        {
            if (State != SceneState.Running) return false;

            State = SceneState.Paused;
            Log.Information("Scene is paused.");

            return true;
        }

        #endregion

        /// <summary>
        /// Updates the playable scene if the current state of the <see cref="SceneManager"/> is <see cref="SceneState.Running"/>.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame.</param>
        internal void Update(float deltaTime)
        {
            if (State != SceneState.Running) return;
            if (_playableScene is null) return;

            _playableScene.Update(deltaTime);
        }

        /// <summary>
        /// Creates a playable deep copy of the active scene.
        /// </summary>
        private void CreatePlayableScene()
        {
            var playableScene = new Scene($"{_activeScene.Name}_play", this);

            foreach (var entity in _activeScene)
            {
                var clonedEntity = (Entity)entity.Clone(); // Deep copy the entity
                playableScene.Add(clonedEntity);
            }

            _playableScene = playableScene;
        }
    }
}