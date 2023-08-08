using System.Numerics;

namespace PentaGE.Common
{
    /// <summary>
    /// Provides extension methods for working with <see cref="Vector3"/> objects.
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        /// Converts a <see cref="Vector3"/> to a user-friendly string representation.<br />
        /// Small components (close to zero) are clamped to zero to improve readability.<br />
        /// <em>Note: Negative zero components will be displayed as positive zero.</em>
        /// </summary>
        /// <param name="source">The <see cref="Vector3"/> to convert.</param>
        /// <returns>A user-friendly string representation of the <see cref="Vector3"/>.</returns>
        public static string ToUserFriendlyString(this Vector3 source)
        {
            // Define the threshold for clamping small components to zero
            float threshold = 1e-6f;

            // Clamp small components to zero for improved readability
            float x = MathF.Abs(source.X) < threshold ? 0 : source.X;
            float y = MathF.Abs(source.Y) < threshold ? 0 : source.Y;
            float z = MathF.Abs(source.Z) < threshold ? 0 : source.Z;

            // Round the components to a maximum number of decimal places
            const int decimalPlaces = 4;
            x = MathF.Round(x, decimalPlaces);
            y = MathF.Round(y, decimalPlaces);
            z = MathF.Round(z, decimalPlaces);

            return $"X{x} Y{y} Z{z}";
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector3"/> is approximately equal to another <see cref="Vector3"/>.<br />
        /// <em>Note: This method uses a threshold of 0.001f.</em>
        /// </summary>
        /// <param name="source">The <see cref="Vector3"/> to compare.</param>
        /// <param name="target">The <see cref="Vector3"/> to compare to.</param>
        /// <param name="threshold">The threshold for determining equality. Default is 0.001f.</param>
        /// <returns><c>true</c> if the specified <see cref="Vector3"/> is approximately equal to the other <see cref="Vector3"/>; otherwise, <c>false</c>.</returns>
        public static bool IsApproximately(this Vector3 source, Vector3 target, float threshold = 0.001f) =>
            MathF.Abs(source.X - target.X) < threshold
            && MathF.Abs(source.Y - target.Y) < threshold
            && MathF.Abs(source.Z - target.Z) < threshold;
    }
}