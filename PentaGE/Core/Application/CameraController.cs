using PentaGE.Core.Rendering;

namespace PentaGE.Core
{
    /// <summary>
    /// Represents an abstract base class for camera controllers, providing methods for handling input events
    /// and updating the camera's behavior.
    /// </summary>
    public abstract class CameraController
    {
        private Camera? _activeCamera;

        /// <summary>
        /// Gets or sets the active camera that this controller is associated with.
        /// </summary>
        public Camera? ActiveCamera
        {
            get => _activeCamera;
            set
            {
                _activeCamera = value;
                CameraChanged();
            }
        }

        #region Event dispatchers

        /// <summary>
        /// Handles the mouse button down event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnMouseDown(object? sender, Events.MouseButtonEventArgs e) =>
            MouseDown(sender, e);

        /// <summary>
        /// Handles the mouse button up event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnMouseUp(object? sender, Events.MouseButtonEventArgs e) =>
            MouseUp(sender, e);

        /// <summary>
        /// Handles the mouse moved event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnMouseMoved(object? sender, Events.MouseMovedEventArgs e) =>
            MouseMoved(sender, e);

        /// <summary>
        /// Handles the key down event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnKeyDown(object? sender, Events.KeyDownEventArgs e) =>
            KeyDown(sender, e);

        /// <summary>
        /// Handles the key up event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnKeyUp(object? sender, Events.KeyUpEventArgs e) =>
            KeyUp(sender, e);

        /// <summary>
        /// Updates the camera controller's behavior.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update.</param>
        internal void OnUpdate(float deltaTime) =>
            Update(deltaTime);

        #endregion

        #region Event handlers

        /// <summary>
        /// Called when the active camera is changed.
        /// </summary>
        protected virtual void CameraChanged() { }

        /// <summary>
        /// Updates the camera controller's behavior.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update.</param>
        protected abstract void Update(float deltaTime);

        /// <summary>
        /// Handles the mouse button down event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        protected abstract void MouseDown(object? sender, Events.MouseButtonEventArgs e);

        /// <summary>
        /// Handles the mouse button up event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        protected abstract void MouseUp(object? sender, Events.MouseButtonEventArgs e);

        /// <summary>
        /// Handles the mouse moved event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        protected abstract void MouseMoved(object? sender, Events.MouseMovedEventArgs e);

        /// <summary>
        /// Handles the keyboard key down event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        protected abstract void KeyDown(object? sender, Events.KeyDownEventArgs e);

        /// <summary>
        /// Handles the keyboard key up event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        protected abstract void KeyUp(object? sender, Events.KeyUpEventArgs e);

        #endregion

        /// <summary>
        /// Gets an instance of an empty camera controller that does nothing.
        /// </summary>
        public static EmptyCameraController None { get; } = new EmptyCameraController();

    }
}