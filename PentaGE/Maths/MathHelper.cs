namespace PentaGE.Maths
{
    /// <summary>
    /// A utility class containing various math-related helper methods.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">The angle in degrees to convert to radians.</param>
        /// <returns>The angle in radians.</returns>
        public static float DegreesToRadians(float degrees) =>
            degrees * (MathF.PI / 180f);

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">The angle in radians to convert to degrees.</param>
        /// <returns>The angle in degrees.</returns>
        public static float RadiansToDegrees(float radians) =>
            radians * (180f / MathF.PI);

        /// <summary>
        /// Performs linear interpolation between two floating-point values.
        /// </summary>
        /// <param name="value1">The start value of the interpolation.</param>
        /// <param name="value2">The end value of the interpolation.</param>
        /// <param name="t">The interpolation factor. Should be a value between 0 and 1.</param>
        /// <returns>The interpolated value between <paramref name="value1"/> and <paramref name="value2"/>.</returns>
        public static float LerpF(float value1, float value2, float t) =>
            value1 + ((value2 - value1) * t);

        /// <summary>
        /// Performs linear interpolation between two floating-point values.
        /// </summary>
        /// <param name="source">The start value of the interpolation.</param>
        /// <param name="value">The end value of the interpolation.</param>
        /// <param name="t">The interpolation factor. Should be a value between 0 and 1.</param>
        /// <returns>The interpolated value between <paramref name="source"/> and <paramref name="value"/>.</returns>
        public static float Lerp(this float source, float value, float t) =>
            LerpF(source, value, t);
    }
}