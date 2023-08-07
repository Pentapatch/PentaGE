using PentaGE.Core.Rendering;

namespace PentaGE.Core
{
    public abstract class CameraController
    {
        private Camera? _activeCamera;

        public Camera? ActiveCamera
        {
            get => _activeCamera; 
            set
            {
                _activeCamera = value;
                CameraChanged();
            }
        }

        internal void OnMouseDown(object? sender, Events.MouseButtonEventArgs e) =>
            MouseDown(sender, e);

        internal void OnMouseUp(object? sender, Events.MouseButtonEventArgs e) =>
            MouseUp(sender, e);

        internal void OnMouseMoved(object? sender, Events.MouseMovedEventArgs e) =>
            MouseMoved(sender, e);

        internal void OnKeyDown(object? sender, Events.KeyDownEventArgs e) =>
            KeyDown(sender, e);

        internal void OnKeyUp(object? sender, Events.KeyUpEventArgs e) =>
            KeyUp(sender, e);

        internal void OnUpdate(float deltaTime) =>
            Update(deltaTime);

        protected virtual void CameraChanged() { }

        protected abstract void Update(float deltaTime);

        protected abstract void MouseDown(object? sender, Events.MouseButtonEventArgs e);

        protected abstract void MouseUp(object? sender, Events.MouseButtonEventArgs e);

        protected abstract void MouseMoved(object? sender, Events.MouseMovedEventArgs e);

        protected abstract void KeyDown(object? sender, Events.KeyDownEventArgs e);

        protected abstract void KeyUp(object? sender, Events.KeyUpEventArgs e);

        public static EmptyCameraController None { get; } = new EmptyCameraController();

    }
}