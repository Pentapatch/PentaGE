using System.Numerics;

namespace PentaGE.WorldGrid
{
    public static class World
    {
        public static readonly Vector3 UpVector = new(0, 1, 0);

        public static readonly Vector3 DownVector = new(0, -1, 0);

        public static readonly Vector3 ForwardVector = new(0, 0, 1);

        public static readonly Vector3 BackwardVector = new(0, 0, -1);

        public static readonly Vector3 RightVector = new(1, 0, 0);

        public static readonly Vector3 LeftVector = new(-1, 0, 0);

        public const float PixelsPerMeter = 32.0f;
    }
}