namespace ConsoleIO
{
    public sealed class ConsoleMenuSettings
    {
        public ConsoleColor Background { get; set; } = ConsoleColor.Black;

        public ConsoleColor Foreground { get; set; } = ConsoleColor.Gray;

        public char? SelectorChar { get; set; } = '>';

        public ConsoleColor? SelectedForecolor { get; set; } = null;

        public ConsoleColor? SelectedBackcolor { get; set; } = null;

        public string Title { get; set; } = string.Empty;
    }
}