namespace PentaGE.Core.Application
{
    public sealed class CameraManager
    {
        private readonly PentaGameEngine _engine;

        public CameraController ActiveController { get; set; }

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

        private void Events_MouseDown(object? sender, Events.MouseButtonEventArgs e) =>
            ActiveController.OnMouseDown(sender, e);

        private void Events_MouseUp(object? sender, Events.MouseButtonEventArgs e) =>
            ActiveController.OnMouseUp(sender, e);

        private void Events_MouseMoved(object? sender, Events.MouseMovedEventArgs e) =>
            ActiveController.OnMouseMoved(sender, e);

        private void Events_KeyDown(object? sender, Events.KeyDownEventArgs e) =>
            ActiveController.OnKeyDown(sender, e);

        private void Events_KeyUp(object? sender, Events.KeyUpEventArgs e) =>
            ActiveController.OnKeyUp(sender, e);
    }
}