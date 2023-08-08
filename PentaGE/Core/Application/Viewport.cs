using PentaGE.Core.Application;

namespace PentaGE.Core
{
    /// <summary>
    /// Represents a viewport that defines a rendering area within the game window.
    /// </summary>
    public sealed class Viewport
    {
        /// <summary>
        /// Gets or sets the left position of the viewport within the game window.
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Gets or sets the top position of the viewport within the game window.
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        /// Gets or sets the width of the viewport.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the viewport.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets the camera manager associated with this viewport, responsible for managing camera controllers and input events.
        /// </summary>
        public CameraManager CameraManager { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Viewport"/> class with the specified parameters.
        /// </summary>
        /// <param name="engine">The Penta Game Engine instance associated with the viewport.</param>
        /// <param name="left">The left position of the viewport within the game window.</param>
        /// <param name="top">The top position of the viewport within the game window.</param>
        /// <param name="width">The width of the viewport.</param>
        /// <param name="height">The height of the viewport.</param>
        internal Viewport(PentaGameEngine engine, int left, int top, int width, int height)
        {
            CameraManager = new(engine);
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }
    }

}