namespace PentaGE.Core.Events
{
    public sealed class MouseEnteredEventArgs : EngineEvent
    {
        internal override EventCategory Category => 
            EventCategory.Input | EventCategory.Mouse | EventCategory.MouseHover;

        internal override EventType Type => 
            EventType.MouseEntered;

        public MouseEnteredEventArgs(Action<EngineEvent> onEvent, Window window) : base(onEvent, window)
        {
        }
    }
}