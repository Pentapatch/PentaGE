namespace ConsoleIO
{
    public sealed class ConsoleMenuSettings
    {
        public ConsoleColor Background { get; set; } = ConsoleColor.Black;

        public ConsoleColor Foreground { get; set; } = ConsoleColor.Gray;

        public char? SelectorChar { get; set; } = '>';

        public ConsoleColor? SelectedForeground { get; set; } = null;

        public ConsoleColor? SelectedBackground { get; set; } = null;

        public ConsoleColor? DisabledForeground { get; set; } = ConsoleColor.DarkGray;

        public ConsoleColor? DisabledBackground { get; set; } = null;

        public ConsoleColor? MessageForeground { get; set; } = null;

        public ConsoleColor? MessageBackground { get; set; } = null;

        public string Title { get; set; } = string.Empty;

        public bool WrapArround { get; set; } = true;
    }
}