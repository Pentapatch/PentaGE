using System.Numerics;

namespace PentaGE.WorldGrid
{
    public static class World
    {
        public static readonly Vector3 UpVector = new(0, 0, 1); //      Z

        public static readonly Vector3 ForwardVector = new(0, 1, 0); // Y

        public static readonly Vector3 RightVector = new(1, 0, 0); //   X

        public static readonly Vector3 DownVector = -UpVector;

        public static readonly Vector3 BackwardVector = -ForwardVector;

        public static readonly Vector3 LeftVector = -RightVector;

        public const float PixelsPerMeter = 32.0f;
    }
}