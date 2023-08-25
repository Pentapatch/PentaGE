namespace ConsoleIO;

public sealed class ConsoleMenuSettings
{
    public ConsoleColor Background { get; set; } = ConsoleColor.Black;

    public ConsoleColor Foreground { get; set; } = ConsoleColor.Gray;

    public string Title { get; set; } = string.Empty;
}