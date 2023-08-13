using PentaGE.Core.Assets;
using PentaGE.Core.Logging;
using Serilog;

namespace PentaGE.Core.Rendering
{
    public sealed class ShaderManager : IDisposable, IHotReloadable
    {
        private readonly Dictionary<string, Shader> _shaders = new();
        
        private readonly AssetManager _assetManager;

        internal ShaderManager(AssetManager assetManager)
        {
            _assetManager = assetManager;
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

                    // Register the shader for hot reload
                    if (this is IHotReloadable hotReloadable)
                        hotReloadable.RegisterPath(name, filePath, this);
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

                // Unregister the shader for hot reload
                if (this is IHotReloadable hotReloadable)
                    hotReloadable.UnregisterPath(name);
            }
            return _shaders.Remove(name);
        }

        public Shader? Get(string name) =>
            _shaders.TryGetValue(name, out Shader? shader) ? shader : null;

        public void Dispose()
        {
            foreach (Shader shader in _shaders.Values)
                shader.Dispose();
        }

        void IHotReloadable.Reload(string name, string filePath)
        {
            if (Get(name) is Shader shader)
            {
                Log.Information($"Reloading shader '{name}'");
                shader.Reload();
            }
        }

        void IHotReloadable.RegisterPath(string name, string path, IHotReloadable item) => 
            _assetManager.RegisterPath(name, path, this);

        void IHotReloadable.UnregisterPath(string name) => 
            _assetManager.UnregisterPath(name);
    }
}