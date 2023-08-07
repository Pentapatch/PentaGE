using PentaGE.Core.Events;

namespace PentaGE.Core
{
    public sealed class EmptyCameraController : CameraController
    {
        protected override void KeyDown(object? sender, KeyDownEventArgs e) { }

        protected override void KeyUp(object? sender, KeyUpEventArgs e) { }

        protected override void MouseDown(object? sender, MouseButtonEventArgs e) { }

        protected override void MouseMoved(object? sender, MouseMovedEventArgs e) { }

        protected override void MouseUp(object? sender, MouseButtonEventArgs e) { }

        protected override void Update(float deltaTime) { }
    }
}