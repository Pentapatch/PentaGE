using PentaGE.Core.Logging;
using PentaGE.Core.Rendering;
using Serilog;

namespace PentaGE.Core.Assets
{
    public sealed class AssetManager : IDisposable
    {
        private readonly PentaGameEngine _engine;

        private readonly Dictionary<string, IAsset> _assets = new();
        private readonly Dictionary<string, HotReloadItem> _hotReloadItems = new();

        private int _hotReloadIntervalInSeconds;
        private bool _hotReloadEnabled;

        public bool HotReloadEnabled { get => _hotReloadEnabled; }

        internal AssetManager(PentaGameEngine engine)
        {
            _engine = engine;
            _hotReloadEnabled = false;
        }

        public IAsset? this[string name]
        {
            get => Get<IAsset>(name);
        }

        public IQueryable<Shader> Shaders => 
            _assets.Values.Where(a => a is Shader).Cast<Shader>().AsQueryable();

        public bool AddShader(string name, string filePath)
        {
            try
            {
                Shader shader = new(filePath);
                return Add(name, shader, filePath);
            }
            catch (FileNotFoundException ex)
            {
                Log.Error($"Failed to add shader '{name}': {ex}");
                return false;
            }
        }

        public bool AddShader(string name, string vertexSourceCode, string fragmentSourceCode, string? geometrySourceCode = null) =>
            Add(name, new Shader(vertexSourceCode, fragmentSourceCode, geometrySourceCode));

        public bool Add(string name, IAsset asset, string? filePath = null)
        {
            string typeName = asset.GetType().Name;
            using var logger = Log.Logger.BeginPerfLogger($"Adding {typeName} asset '{name}'");

            // Load the asset
            if (!asset.Load())
            {
                Log.Warning($"Failed to load {typeName} asset '{name}'");
                return false; // TODO: Throw exception or change return type?
            }

            // Add the asset to the dictionary
            try
            {
                _assets.Add(name, asset);
            }
            catch (Exception ex)
            {
                Log.Warning($"Failed to add {typeName} asset '{name}': {ex}");
                return false;
            }

            // Register the asset for hot reload
            if (asset is IHotReloadable hotReloadable)
            {
                if (filePath is not null)
                    RegisterPath(name, filePath, hotReloadable);
                else
                    Log.Warning($"Failed to register {typeName} asset '{name}' for hot reload: No file path provided");
            }

            return true;
        }

        public void Remove(string name)
        {
            if (_assets.TryGetValue(name, out IAsset? asset))
            {
                asset?.Dispose();

                // Unregister the asset from hot reload
                if (asset is IHotReloadable hotReloadable)
                    UnregisterPath(name);
            }

            _assets.Remove(name);
        }

        public void Dispose()
        {

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

        public T? Get<T>(string name) where T : IAsset => 
            _assets.TryGetValue(name, out IAsset? asset) ? (T?)asset : default;

        private void RegisterPath(string name, string path, IHotReloadable item) => 
            _hotReloadItems.Add(name, new(path, item));

        private void UnregisterPath(string name) => 
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
                    ReloadAsset(name, item.Instance);

                    // Update the last modification time in the dictionary
                    item.ModificationTime = currentModified;
                }
            }
        }

        private void ReloadAsset(string name, IHotReloadable asset)
        {
            string typeName = asset.GetType().Name;
            Log.Information($"Hot-Reloading {typeName} asset '{name}'");

            if (!asset.Reload())
                Log.Warning($"Failed to hot-reload {typeName} asset '{name}'");
        }
    }
}