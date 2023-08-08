
using Sandbox;

Application engine = new();
while (true)
{
    Console.Clear();
    engine.Start();
    Console.WriteLine("Press F5 to restart");
    if (Console.ReadKey(true).Key != ConsoleKey.F5)
    {
        break;
    }
}