namespace PentaGE.Maths
{
    public static class MathHelper
    {

        public static float DegreesToRadians(float degrees) =>
            degrees * (MathF.PI / 180f);

        public static float RadiansToDegrees(float radians) =>
            radians * (180f / MathF.PI);

        public static float LerpF(float value1, float value2, float t) =>
            value1 + ((value2 - value1) * t);

        public static float Lerp(this float source, float value, float t) =>
            LerpF(source, value, t);
    }
}