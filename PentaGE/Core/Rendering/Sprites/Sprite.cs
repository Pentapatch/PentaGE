using PentaGE.Core.Assets;

namespace PentaGE.Core.Rendering.Sprites
{
    /// <summary>
    /// Represents a Sprite.
    /// </summary>
    public sealed class Sprite : IAsset, IDisposable, IHotReloadable
    {
        private readonly Texture _texture;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class from an existing texture.
        /// </summary>
        /// <param name="texture">The underlying texture for the sprite.</param>
        public Sprite(Texture texture)
        {
            _texture = texture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class from an image file.
        /// </summary>
        /// <param name="filePath">The path to the image file.</param>
        /// <param name="type">The type of the texture (e.g., <see cref="GL_TEXTURE_2D"/>, <see cref="GL_TEXTURE_CUBE_MAP"/>).</param>
        /// <param name="slot">The texture slot to bind the texture to.</param>
        /// <param name="format">The format of the texture.</param>
        /// <param name="pixelType">The type of pixels in the texture.</param>
        public Sprite(string filePath, int type, int slot, int format, int pixelType)
        {
            _texture = new(filePath, type, slot, format, pixelType);
        }

        /// <summary>
        /// Gets the underlying <see cref="Texture"/> of the sprite.
        /// </summary>
        internal Texture Texture => _texture;

        /// <inheritdoc />
        public void Dispose() => 
            _texture.Dispose();

        /// <inheritdoc />
        public bool Load() => 
            _texture.Load();

        /// <inheritdoc />
        public bool Reload() =>
            _texture.Reload();
    }
}