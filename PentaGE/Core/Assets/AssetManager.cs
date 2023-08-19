using PentaGE.Core.Entities;
using PentaGE.Core.Logging;
using PentaGE.Core.Rendering;
using Serilog;
using StbImageSharp;

namespace PentaGE.Core.Assets
{
    /// <summary>
    /// Manages loading, tracking, and hot-reloading assets in the PentaGameEngine.
    /// </summary>
    public sealed class AssetManager : IDisposable
    {
        private readonly PentaGameEngine _engine;

        private readonly Dictionary<string, IAsset> _assets = new();
        private readonly Dictionary<string, HotReloadItem> _hotReloadItems = new();

        private int _hotReloadIntervalInSeconds;
        private bool _hotReloadEnabled;

        /// <summary>
        /// Gets a value indicating whether hot reload is enabled for assets.
        /// </summary>
        public bool HotReloadEnabled { get => _hotReloadEnabled; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetManager"/> class.
        /// </summary>
        /// <param name="engine">The PentaGameEngine instance associated with this asset manager.</param>
        internal AssetManager(PentaGameEngine engine)
        {
            _engine = engine;
            _hotReloadEnabled = false;
        }

        /// <summary>
        /// Gets the asset with the specified name.
        /// </summary>
        /// <param name="name">The name of the asset.</param>
        /// <returns>The asset with the specified name, if found; otherwise, <c>null</c>.</returns>
        public IAsset? this[string name] => Get<IAsset>(name);

        /// <summary>
        /// Gets all shaders managed by the asset manager.
        /// </summary>
        public IEnumerable<Shader> Shaders =>
            GetAll<Shader>();

        /// <summary>
        /// Gets all textures managed by the asset manager.
        /// </summary>
        public IEnumerable<Texture> Textures =>
            GetAll<Texture>();

        /// <summary>
        /// Gets all entities managed by the asset manager.
        /// </summary>
        public IEnumerable<Entity> Entities =>
            GetAll<Entity>();

        /// <summary>
        /// Adds a shader asset to the asset manager.
        /// </summary>
        /// <param name="name">The name to assign to the shader.</param>
        /// <param name="filePath">The path to the shader file.</param>
        /// <returns><c>true</c> if the shader was added successfully; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Adds a shader asset to the asset manager using provided source code.
        /// </summary>
        /// <param name="name">The name to assign to the shader.</param>
        /// <param name="vertexSourceCode">The source code of the vertex shader.</param>
        /// <param name="fragmentSourceCode">The source code of the fragment shader.</param>
        /// <param name="geometrySourceCode">The source code of the geometry shader (optional).</param>
        /// <returns><c>true</c> if the shader was added successfully; otherwise, <c>false</c>.</returns>
        public bool AddShader(string name, string vertexSourceCode, string fragmentSourceCode, string? geometrySourceCode = null) =>
            Add(name, new Shader(vertexSourceCode, fragmentSourceCode, geometrySourceCode));

        /// <summary>
        /// Adds a texture asset to the asset manager using a file path.
        /// </summary>
        /// <param name="name">The name to assign to the texture.</param>
        /// <param name="filePath">The path to the texture file.</param>
        /// <param name="type">The texture type.</param>
        /// <param name="slot">The texture slot.</param>
        /// <param name="format">The texture format.</param>
        /// <param name="pixelType">The pixel data type.</param>
        /// <returns><c>true</c> if the texture was added successfully; otherwise, <c>false</c>.</returns>
        public bool AddTexture(string name, string filePath, int type, int slot, int format, int pixelType)
        {
            try
            {
                Texture texture = new(filePath, type, slot, format, pixelType);
                return Add(name, texture, filePath);
            }
            catch (FileNotFoundException ex)
            {
                Log.Error($"Failed to add texture '{name}': {ex}");
                return false;
            }
        }

        /// <summary>
        /// Adds a texture asset to the asset manager using an <see cref="ImageResult"/>.
        /// </summary>
        /// <param name="name">The name to assign to the texture.</param>
        /// <param name="image">The image data for the texture.</param>
        /// <param name="type">The texture type.</param>
        /// <param name="slot">The texture slot.</param>
        /// <param name="format">The texture format.</param>
        /// <param name="pixelType">The pixel data type.</param>
        /// <returns><c>true</c> if the texture was added successfully; otherwise, <c>false</c>.</returns>
        public bool AddTexture(string name, ImageResult image, int type, int slot, int format, int pixelType) =>
            Add(name, new Texture(image, type, slot, format, pixelType));

        /// <summary>
        /// Adds an <see cref="Entity"/> asset to the asset manager.
        /// </summary>
        /// <param name="name">The name to assign to the entity.</param>
        /// <param name="entity">The entity to be added as an asset.</param>
        /// <returns><c>true</c> if the entity was added successfully; otherwise, <c>false</c>.</returns>
        public bool Add(string name, Entity entity) =>
            Add(name, (IAsset)entity);

        /// <summary>
        /// Adds a <see cref="Shader"/> asset to the asset manager.
        /// </summary>
        /// <param name="name">The name to assign to the shader.</param>
        /// <param name="shader">The shader to be added as an asset.</param>
        /// <returns><c>true</c> if the shader was added successfully; otherwise, <c>false</c>.</returns>
        public bool Add(string name, Shader shader) =>
            Add(name, (IAsset)shader);

        /// <summary>
        /// Adds a <see cref="Texture"/> asset to the asset manager.
        /// </summary>
        /// <param name="name">The name to assign to the texture.</param>
        /// <param name="texture">The texture to be added as an asset.</param>
        /// <param name="filePath">The path to the texture file, if applicable.</param>
        /// <returns><c>true</c> if the texture was added successfully; otherwise, <c>false</c>.</returns>
        public bool Add(string name, Texture texture, string? filePath = null) =>
            Add(name, (IAsset)texture, filePath);

        /// <summary>
        /// Adds an asset to the asset manager.
        /// </summary>
        /// <param name="name">The name of the asset.</param>
        /// <param name="asset">The asset to add.</param>
        /// <param name="filePath">The optional file path of the asset.</param>
        /// <returns><c>true</c> if the asset was successfully added; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Removes an asset from the asset manager.
        /// </summary>
        /// <param name="name">The name of the asset to remove.</param>
        public void Remove(string name)
        {
            if (_assets.TryGetValue(name, out IAsset? asset))
            {
                asset?.Dispose();

                // Unregister the asset from hot reload
                if (asset is IHotReloadable)
                    UnregisterPath(name);
            }

            _assets.Remove(name);
        }

        /// <summary>
        /// Disposes of all assets managed by the asset manager.
        /// </summary>
        public void Dispose()
        {
            foreach (var asset in _assets.Values)
            {
                asset.Dispose();
            }
        }

        /// <summary>
        /// Enables hot reload for assets with the specified interval.
        /// </summary>
        /// <param name="intervalInSeconds">The interval in seconds for checking file changes.</param>
        /// <exception cref="InvalidOperationException">Thrown if hot reload is already enabled.</exception>"
        public void EnableHotReload(int intervalInSeconds = 3)
        {
            if (_hotReloadEnabled)
                throw new InvalidOperationException("Hot reload is already enabled.");

            _hotReloadIntervalInSeconds = intervalInSeconds;
            HandleHotReloadEventSubscription(true);
        }

        /// <summary>
        /// Disables hot reload for assets.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if hot reload is already disabled.</exception>""
        public void DisableHotReload()
        {
            if (!_hotReloadEnabled)
                throw new InvalidOperationException("Hot reload is already disabled.");

            HandleHotReloadEventSubscription(false);
        }

        /// <summary>
        /// Retrieves an asset of type <typeparamref name="T"/> with the specified name.
        /// </summary>
        /// <typeparam name="T">The type of the asset.</typeparam>
        /// <param name="name">The name of the asset.</param>
        /// <returns>The asset if found; otherwise, <c>null</c>.</returns>
        public T? Get<T>(string name) where T : IAsset =>
            _assets.TryGetValue(name, out IAsset? asset) ? (T?)asset : default;

        /// <summary>
        /// Retrieves an asset with the specified name.
        /// </summary>
        /// <param name="name">The name of the asset.</param>
        /// <returns>The asset if found; otherwise, <c>null</c>.</returns>
        public IAsset? Get(string name) =>
            _assets.TryGetValue(name, out IAsset? asset) ? asset : default;

        /// <summary>
        /// Retrieves all assets of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of assets to retrieve.</typeparam>
        /// <returns>An enumerable collection of assets of type <typeparamref name="T"/>.</returns>
        public IEnumerable<T> GetAll<T>() where T : IAsset =>
            _assets.Values.Where(a => a is T).Cast<T>().AsEnumerable();

        /// <summary>
        /// Registers the file path for an asset to enable hot reload.
        /// </summary>
        /// <param name="name">The name of the asset.</param>
        /// <param name="path">The file path of the asset.</param>
        /// <param name="item">The hot-reloadable item associated with the asset.</param>
        private void RegisterPath(string name, string path, IHotReloadable item) =>
            _hotReloadItems.Add(name, new(path, item));

        /// <summary>
        /// Unregisters the file path of an asset to disable hot reload.
        /// </summary>
        /// <param name="name">The name of the asset.</param>
        private void UnregisterPath(string name) =>
            _hotReloadItems.Remove(name);

        /// <summary>
        /// Handles the subscription or unsubscription of the hot reload event.
        /// </summary>
        /// <param name="enabled"><c>true</c> to enable hot reload, <c>false</c> to disable.</param>
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

        /// <summary>
        /// Event handler for the hot reload tick event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HotReload_Tick(object? sender, Events.CustomTimingTickEventArgs e) =>
            CheckForFileChanges();

        /// <summary>
        /// Checks for file changes in hot-reloadable assets and reloads them if necessary.
        /// </summary>
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

        /// <summary>
        /// Reloads a hot-reloadable asset.
        /// </summary>
        /// <param name="name">The name of the asset.</param>
        /// <param name="asset">The asset to reload.</param>
        private static void ReloadAsset(string name, IHotReloadable asset)
        {
            string typeName = asset.GetType().Name;
            Log.Information($"Hot-Reloading {typeName} asset '{name}'");

            if (!asset.Reload())
                Log.Warning($"Failed to hot-reload {typeName} asset '{name}'");
        }
    }
}