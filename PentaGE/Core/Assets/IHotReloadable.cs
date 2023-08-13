namespace PentaGE.Core.Assets
{
    internal interface IHotReloadable
    {
        void Reload(string name, string filePath);

        internal void RegisterPath(string name, string path, IHotReloadable item);

        internal void UnregisterPath(string name);
    }
}