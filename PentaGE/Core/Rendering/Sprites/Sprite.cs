using PentaGE.Common;
using PentaGE.Core.Assets;
using PentaGE.Core.Graphics;
using System.Numerics;

namespace PentaGE.Core.Rendering.Sprites
{
    /// <summary>
    /// Represents a Sprite.
    /// </summary>
    public sealed class Sprite : IAsset, IDisposable, IHotReloadable
    {
        private readonly Texture _texture;
        private readonly Mesh _mesh;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class from an existing texture.
        /// </summary>
        /// <remarks>If a mesh is not provided, a default plane will be generated.</remarks>
        /// <param name="texture">The underlying texture for the sprite.</param>
        /// <param name="mesh">The mesh to render the sprite with (optional).</param>
        /// <param name="rotation">The rotation of the sprite (optional).</param>
        /// <param name="scale">The scale of the sprite (optional).</param>
        public Sprite(Texture texture, Mesh? mesh = null, Vector2? scale = null, Rotation? rotation = null)
        {
            _texture = texture;
            _mesh = mesh ?? MeshFactory.CreateRectangle(scale?.X ?? 1f, scale?.Y ?? 1f, rotation ?? new(0f, -90f, 0f));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class from an image file.
        /// </summary>
        /// <remarks>If a mesh is not provided, a default plane will be generated.</remarks>
        /// <param name="filePath">The path to the image file.</param>
        /// <param name="type">The type of the texture (e.g., <see cref="GL_TEXTURE_2D"/>, <see cref="GL_TEXTURE_CUBE_MAP"/>).</param>
        /// <param name="slot">The texture slot to bind the texture to.</param>
        /// <param name="format">The format of the texture.</param>
        /// <param name="pixelType">The type of pixels in the texture.</param>
        /// <param name="mesh">The mesh to render the sprite with (optional).</param>
        /// <param name="rotation">The rotation of the sprite (optional).</param>
        /// <param name="scale">The scale of the sprite (optional).</param>
        public Sprite(
            string filePath,
            int type,
            int slot,
            int format,
            int pixelType,
            Mesh? mesh = null,
            Vector2? scale = null,
            Rotation? rotation = null)
        {
            _texture = new(filePath, type, slot, format, pixelType);
            _mesh = mesh ?? MeshFactory.CreateRectangle(scale?.X ?? 1f, scale?.Y ?? 1f, rotation ?? Rotation.Zero);
        }

        /// <summary>
        /// Gets the underlying texture of the sprite.
        /// </summary>
        internal Texture Texture => _texture;

        /// <summary>
        /// Gets the underlying mesh of the sprite.
        /// </summary>
        internal Mesh Mesh => _mesh;

        /// <inheritdoc />
        public void Dispose() =>
            _texture.Dispose();

        /// <inheritdoc />
        public bool Load()
        {
            if (_texture.Id == 0)
            {
                return _texture.Load();
            }
            return true;
        }

        /// <inheritdoc />
        public bool Reload() =>
            _texture.Reload();
    }
}