namespace PentaGE.Core
{
    internal record Frame
    {
        private readonly int _gameSpeed = 1000;

        public int Number { get; private set; }

        public double Delta { get; private set; }

        public int Elapsed { get; private set; }

        public int TotalElapsed { get; private set; }

        public double GetProgress() => TotalElapsed / (double)_gameSpeed;

        public Frame(int number, int elapsed, int totalElapsed, int gameSpeed)
        {
            Number = number;
            Elapsed = elapsed;
            TotalElapsed = totalElapsed;
            Delta = elapsed / (double)gameSpeed;
            _gameSpeed = gameSpeed;
        }

        public Frame() { }

        //public static bool operator ==(Frame frame, int number) =>
        //    frame?.Number == number;

        //public static bool operator !=(Frame frame, int number) =>
        //    frame?.Number != number;

        public static implicit operator int(Frame frame) =>
            frame.Number;

        public override string ToString() =>
            $"Frame: {Number}\n" +
            $"Delta: {Math.Round(Delta, 4)}\n" +
            $"Elapsed: {Elapsed}ms\n" +
            $"Total Elapsed: {TotalElapsed}ms\n" +
            $"Progress {Math.Round(GetProgress() * 100)}%";
    }
}