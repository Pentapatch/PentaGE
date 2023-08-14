namespace PentaGE.Core.Assets
{
    public interface IAsset : IDisposable
    {
        /// <summary>
        /// Loads the entity and its components.
        /// </summary>
        /// <remarks>Return <c>true</c> if loading was successful, otherwise <c>false</c>.</remarks>
        /// <returns></returns>
        bool Load();
    }
}