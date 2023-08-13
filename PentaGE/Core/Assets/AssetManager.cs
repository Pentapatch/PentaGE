using PentaGE.Core.Rendering;

namespace PentaGE.Core.Assets
{
    public sealed class AssetManager : IDisposable
    {
        private readonly PentaGameEngine _engine;
        private readonly ShaderManager _shaderManager;

        private readonly Dictionary<string, HotReloadItem> _hotReloadItems = new();

        private int _hotReloadIntervalInSeconds;
        private bool _hotReloadEnabled;

        public ShaderManager Shaders => _shaderManager;

        public bool HotReloadEnabled { get => _hotReloadEnabled; }

        internal AssetManager(PentaGameEngine engine)
        {
            _engine = engine;
            _shaderManager = new(this);
        }

        public void Dispose()
        {
            _shaderManager.Dispose();
        }

        public void EnableHotReload(int seconds = 3)
        {
            if (_hotReloadEnabled)
                throw new InvalidOperationException("Hot reload is already enabled.");

            _hotReloadIntervalInSeconds = seconds;
            HandleHotReloadEventSubscription(true);
        }

        public void DisableHotReload()
        {
            if (!_hotReloadEnabled)
                throw new InvalidOperationException("Hot reload is already disabled.");

            HandleHotReloadEventSubscription(false);
        }

        internal void RegisterPath(string name, string path, IHotReloadable item) => 
            _hotReloadItems.Add(name, new(path, item));

        internal void UnregisterPath(string name) => 
            _hotReloadItems.Remove(name);

        private void HandleHotReloadEventSubscription(bool enabled)
        {
            if (enabled)
            {
                // Register the event handler
                _engine.Timing.CustomTimings[_hotReloadIntervalInSeconds].Tick += HotReload_Tick;
            }
            else
            {
                // Unregister the event handler
                _engine.Timing.CustomTimings[_hotReloadIntervalInSeconds].Tick -= HotReload_Tick;
            }

            // Set the hot reload enabled flag
            _hotReloadEnabled = enabled;
        }

        private void HotReload_Tick(object? sender, Events.CustomTimingTickEventArgs e) =>
            CheckForFileChanges();

        private void CheckForFileChanges()
        {
            // Loop through all shaders and reload them if they have changed
            foreach ((string name, HotReloadItem item) in _hotReloadItems)
            {
                if (item.ModificationTime is null)
                {
                    // If the file has no modification time stamp, add it
                    item.ModificationTime = File.GetLastWriteTime(item.FilePath);
                    continue;
                }

                DateTime currentModified = File.GetLastWriteTime(item.FilePath);

                if (currentModified > item.ModificationTime)
                {
                    // File has been modified since the last check, reload it
                    item.Instance.Reload(name, item.FilePath);

                    // Update the last modification time in the dictionary
                    item.ModificationTime = currentModified;
                }
            }
        }
    }
}