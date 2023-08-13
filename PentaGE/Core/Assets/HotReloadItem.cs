namespace PentaGE.Core.Assets
{
    internal sealed class HotReloadItem
    {
        public string FilePath { get; init; }

        public DateTime? ModificationTime { get; set; }

        public IHotReloadable Instance { get; init; }

        internal HotReloadItem(string filePath, IHotReloadable item)
        {
            FilePath = filePath;
            Instance = item;
            ModificationTime = null;
        }
    }
}