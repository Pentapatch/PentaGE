using PentaGE.Core.Entities;
using Serilog;

namespace PentaGE.Core.Scenes
{
    public sealed class SceneManager
    {
        private readonly Dictionary<string, Scene> _scenes;
        private Scene _activeScene;
        private Scene? _currentScene;

        public SceneState State { get; private set; }

        public Scene Scene => 
            _currentScene ?? _activeScene;

        internal SceneManager()
        {
            _activeScene = new();
            _scenes = new();
            State = SceneState.Idle;
        }

        public bool SetActiveScene(string name)
        {
            if (_scenes.TryGetValue(name, out var scene))
            {
                SetActiveScene(scene);
                return true;
            }

            return false;
        }

        public void SetActiveScene(Scene scene)
        {
            _activeScene = scene;

            Stop();
        }

        public Scene Add(string name)
        {
            var scene = new Scene();
            _scenes.Add(name, scene);

            return scene;
        }

        #region State management

        public bool Run()
        {
            if (State == SceneState.Running) 
                return false;
            else if (State == SceneState.Idle) 
                CloneActiveScene();

            State = SceneState.Running;
            Log.Information("Scene is running.");

            return true;
        }

        public bool Stop()
        {
            if (State == SceneState.Idle) return false;

            _currentScene = null;

            State = SceneState.Idle;
            Log.Information("Scene is stopped.");

            return true;
        }

        public bool Restart()
        {
            if (State != SceneState.Running) return false;

            Stop();
            Run();

            return true;
        }

        public bool Pause()
        {
            if (State != SceneState.Running) return false;

            State = SceneState.Paused;
            Log.Information("Scene is paused.");

            return true;
        }

        #endregion

        internal void Update(float deltaTime)
        {
            if (State != SceneState.Running) return;
            if (_currentScene is null) return;

            // Update the scene
            _currentScene.Update(deltaTime);
        }

        private void CloneActiveScene()
        {
            var playableScene = new Scene();

            foreach (var entity in _activeScene)
            {
                var clonedEntity = (Entity)entity.Clone(); // Deep copy the entity
                playableScene.Add(clonedEntity); 
            }

            _currentScene = playableScene;
        }
    }
}