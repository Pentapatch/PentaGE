namespace PentaGE.Core.Assets
{
    public interface IHotReloadable
    {
        /// <summary>
        /// Gets called when the asset should be reloaded by the hot-reload mechanism.
        /// </summary>
        /// <remarks>Return <c>true</c> if the reload was successfull, otherwise <c>false</c>.</remarks>
        bool Reload();
    }
}