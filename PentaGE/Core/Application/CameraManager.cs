namespace PentaGE.Core.Application
{
    /// <summary>
    /// Represents a class responsible for managing camera controllers and dispatching input events to them.
    /// </summary>
    public sealed class CameraManager
    {
        private readonly PentaGameEngine _engine;

        /// <summary>
        /// Gets or sets the currently active camera controller.
        /// </summary>
        public CameraController ActiveController { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraManager"/> class with the specified Penta Game Engine instance.
        /// </summary>
        /// <param name="engine">The game engine instance to associate with the camera manager.</param>
        internal CameraManager(PentaGameEngine engine)
        {
            _engine = engine;
            ActiveController = CameraController.None;

            _engine.Events.KeyDown += Events_KeyDown; ;
            _engine.Events.KeyUp += Events_KeyUp; ;
            _engine.Events.KeyRepeat += Events_KeyDown;
            _engine.Events.MouseDown += Events_MouseDown; ;
            _engine.Events.MouseMoved += Events_MouseMoved; ;
            _engine.Events.MouseUp += Events_MouseUp; ;
        }

        #region Event dispatchers

        /// <summary>
        /// Dispatches the mouse button down event to the active camera controller.
        /// </summary>
        private void Events_MouseDown(object? sender, Events.MouseButtonEventArgs e) =>
            ActiveController.OnMouseDown(sender, e);

        /// <summary>
        /// Dispatches the mouse button up event to the active camera controller.
        /// </summary>
        private void Events_MouseUp(object? sender, Events.MouseButtonEventArgs e) =>
            ActiveController.OnMouseUp(sender, e);

        /// <summary>
        /// Dispatches the mouse moved event to the active camera controller.
        /// </summary>
        private void Events_MouseMoved(object? sender, Events.MouseMovedEventArgs e) =>
            ActiveController.OnMouseMoved(sender, e);

        /// <summary>
        /// Dispatches the key down event to the active camera controller.
        /// </summary>
        private void Events_KeyDown(object? sender, Events.KeyDownEventArgs e) =>
            ActiveController.OnKeyDown(sender, e);

        /// <summary>
        /// Dispatches the key up event to the active camera controller.
        /// </summary>
        private void Events_KeyUp(object? sender, Events.KeyUpEventArgs e) =>
            ActiveController.OnKeyUp(sender, e);

        #endregion
    }
}