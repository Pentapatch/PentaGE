namespace ConsoleIO
{
    public class MenuItemSettings
    {
        public bool Enabled { get; set; } = true;

        public bool Selected { get; set; } = false;

        public ConsoleKey? Shortcut { get; set; } = null;

        public ConsoleColor? Foreground { get; set; } = null;

        public ConsoleColor? Background { get; set; } = null;
    }
}