namespace PentaGE.MathExtensions
{
    public static class MathHelper
    {
        public static float DegreesToRadians(float degrees) =>
            degrees * (MathF.PI / 180f);

        public static float RadiansToDegrees(float radians) =>
        radians * (180f / MathF.PI);
    }
}