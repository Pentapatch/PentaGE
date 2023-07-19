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

        /// <summary>
        /// Transforms the interpolation factor using the specified curve type and power.
        /// </summary>
        /// <param name="originalFactor">The original interpolation factor to be transformed.</param>
        /// <param name="curvePower">The power or strength of the interpolation curve.</param>
        /// <param name="curveType">The type of interpolation curve to apply.</param>
        /// <returns>
        /// The transformed interpolation factor. The value will be different depending on the specified
        /// interpolation curve and curve power. For <see cref="InterpolationCurve.Linear"/>, the original factor
        /// is returned unchanged. For other curve types, the original factor is transformed based on the curve
        /// and power values.
        /// </returns>
        public static float TransformInterpolationFactor(
            float originalFactor,
            float curvePower,
            InterpolationCurve curveType = InterpolationCurve.Linear) =>
            curveType switch
            {
                InterpolationCurve.EaseIn => CalculateEaseInFactor(originalFactor, curvePower),
                InterpolationCurve.EaseOut => CalculateEaseOutFactor(originalFactor, curvePower),
                InterpolationCurve.EaseInOut => CalculateEaseInOutFactor(originalFactor, curvePower),
                InterpolationCurve.EaseOutIn => CalculateEaseOutInFactor(originalFactor, curvePower),
                _ => originalFactor, // Linear or unsupported curves remain unchanged
            };

        /// <summary>
        /// Clamps the interpolation factor to ensure it is within the valid range (0 - 1).
        /// </summary>
        /// <param name="t">The original interpolation factor.</param>
        /// <returns>The clamped interpolation factor within the range (0 - 1).</returns>
        private static float ClampFactor(float t) =>
            Math.Clamp(t, 0f, 1f);

        /// <summary>
        /// Calculates the ease-in interpolation factor using the specified original factor and power.
        /// </summary>
        /// <param name="t">The original interpolation factor in the range (0 - 1).</param>
        /// <param name="power">The power value that affects the ease-in curve.</param>
        /// <returns>The transformed interpolation factor with the ease-in curve applied.</returns>
        private static float CalculateEaseInFactor(float t, float power) =>
            MathF.Pow(ClampFactor(t), power);

        /// <summary>
        /// Calculates the ease-out interpolation factor using the specified original factor and power.
        /// </summary>
        /// <param name="t">The original interpolation factor in the range (0 - 1).</param>
        /// <param name="power">The power value that affects the ease-out curve.</param>
        /// <returns>The transformed interpolation factor with the ease-out curve applied.</returns>
        private static float CalculateEaseOutFactor(float t, float power) =>
            1f - MathF.Pow(1f - ClampFactor(t), power);

        /// <summary>
        /// Calculates the ease-in-out interpolation factor using the specified original factor and power.
        /// </summary>
        /// <param name="t">The original interpolation factor in the range (0 - 1).</param>
        /// <param name="power">The power value that affects the ease-in-out curve.</param>
        /// <returns>The transformed interpolation factor with the ease-in-out curve applied.</returns>
        private static float CalculateEaseInOutFactor(float t, float power)
        {
            t = ClampFactor(t);
            return t < 0.5f
                ? 0.5f * MathF.Pow(2f * t, power)
                : 1f - 0.5f * MathF.Pow(2f * (1f - t), power);
        }

        /// <summary>
        /// Calculates the ease-out-in interpolation factor using the specified original factor and power.
        /// </summary>
        /// <param name="t">The original interpolation factor in the range (0 - 1).</param>
        /// <param name="power">The power value that affects the ease-out-in curve.</param>
        /// <returns>The transformed interpolation factor with the ease-out-in curve applied.</returns>
        private static float CalculateEaseOutInFactor(float t, float power)
        {
            t = ClampFactor(t);
            return t < 0.5f
                ? 0.5f * (1f - MathF.Pow(1f - 2f * t, power))
                : 0.5f + 0.5f * MathF.Pow(2f * t - 1f, power);
        }

    }
}