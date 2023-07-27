using System.Numerics;

namespace PentaGE.Common
{
    public static class World
    {
        public static readonly Vector3 ForwardVector = new(0, 0, 1); // Positive Z

        public static readonly Vector3 UpVector = new(0, 1, 0);      // Positive Y

        public static readonly Vector3 RightVector = new(1, 0, 0);   // Positive X

        public static readonly Vector3 DownVector = -UpVector;

        public static readonly Vector3 BackwardVector = -ForwardVector;

        public static readonly Vector3 LeftVector = -RightVector;

        public const float PixelsPerMeter = 200.0f;
    }
}