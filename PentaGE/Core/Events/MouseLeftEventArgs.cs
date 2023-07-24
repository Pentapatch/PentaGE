namespace PentaGE.Core.Events
{
    public sealed class MouseLeftEventArgs : EngineEvent
    {
        internal override EventCategory Category =>
            EventCategory.Input | EventCategory.Mouse | EventCategory.MouseHover;

        internal override EventType Type =>
            EventType.MouseLeft;

        public MouseLeftEventArgs(Action<EngineEvent> onEvent, Window window) : base(onEvent, window)
        {
        }
    }
}