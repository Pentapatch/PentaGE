using PentaGE.Core.Logging;
using Serilog;

namespace PentaGE.Core.Rendering
{
    public sealed class ShaderManager : IDisposable
    {
        private readonly Dictionary<string, Shader> _shaders = new();
        private readonly Dictionary<string, string> _shaderPaths = new();
        private readonly Dictionary<string, DateTime> _shaderModificationTimes = new();
        private readonly PentaGameEngine _engine;
        private int _hotReloadIntervalInSeconds;
        private bool _hotReloadEnabled;

        internal ShaderManager(PentaGameEngine engine)
        {
            _engine = engine;
        }

        public void EnableHotReload(int seconds = 3)
        {
            if (_hotReloadEnabled)
                throw new InvalidOperationException("Hot reload is already enabled.");

            _hotReloadIntervalInSeconds = seconds;
            HandleHotReloadSubscription(true);
        }

        public void DisableHotReload()
        {
            HandleHotReloadSubscription(false);
        }

        private void HandleHotReloadSubscription(bool enabled)
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

        public bool HotReloadEnabled { get => _hotReloadEnabled; }

        private void HotReload_Tick(object? sender, Events.CustomTimingTickEventArgs e) =>
            CheckForShaderChanges();

        private void CheckForShaderChanges()
        {
            // Loop through all shaders and reload them if they have changed
            foreach ((string name, string filePath) in _shaderPaths)
            {
                if (!_shaderModificationTimes.TryGetValue(filePath, out DateTime lastModified))
                {
                    // If the shader is not in the dictionary, add it with the current modification time
                    _shaderModificationTimes[filePath] = File.GetLastWriteTime(filePath);
                    continue;
                }

                DateTime currentModified = File.GetLastWriteTime(filePath);

                if (currentModified > lastModified)
                {
                    // Shader file has been modified since the last check, reload it
                    ReloadShader(name);

                    // Update the last modification time in the dictionary
                    _shaderModificationTimes[filePath] = currentModified;
                }
            }
        }

        private void ReloadShader(string name)
        {
            if (Get(name) is Shader shader)
            {
                Log.Information($"Reloading shader '{name}'");
                shader.Reload();
            }
        }

        public bool Add(string name, string vertexSourceCode, string fragmentSourceCode, string? geometrySourceCode = null)
        {
            using var logger = Log.Logger.BeginPerfLogger($"Loading shader '{name}'");

            // Create the shader
            Shader shader = new(vertexSourceCode, fragmentSourceCode, geometrySourceCode);

            // Load the shader
            if (!shader.Load())
            {
                Log.Warning($"Failed to load shader '{name}'");
                return false; // TODO: Throw exception or change return type?
            }

            // Add the shader to the dictionary
            try
            {
                _shaders.Add(name, shader);
            }
            catch (Exception ex)
            {
                Log.Warning($"Failed to add shader '{name}': {ex}");
                return false;
            }

            return true;
        }

        public bool Add(string name, string filePath)
        {
            using var logger = Log.Logger.BeginPerfLogger($"Loading shader '{name}'");
            
            try
            {
                // Create the shader
                Shader shader = new(filePath);

                // Load the shader
                if (!shader.Load())
                {
                    Log.Warning($"Failed to load shader '{name}' from file '{filePath}'");
                    return false; // TODO: Throw exception or change return type?
                }

                // Add the shader to the dictionary
                try
                {
                    _shaders.Add(name, shader);
                    _shaderPaths.Add(name, filePath);
                }
                catch (Exception ex)
                {
                    Log.Warning($"Failed to add shader '{name}': {ex}");
                }

                return true;
            }
            catch (FileNotFoundException ex)
            {
                Log.Warning($"Failed to load shader '{name}' from file '{filePath}': {ex.Message}");
                return false; // TODO: Throw exception or change return type?
            }
        }

        public bool Remove(string name)
        {
            if (_shaders.TryGetValue(name, out Shader? shader))
            {
                shader?.Dispose();
            }
            return _shaders.Remove(name);
        }

        public Shader? Get(string name) =>
            _shaders.TryGetValue(name, out Shader? shader) ? shader : null;

        public void Dispose()
        {
            foreach (Shader shader in _shaders.Values)
            {
                shader.Dispose();
            }
        }
    }
}