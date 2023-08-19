using System.Numerics;

namespace PentaGE.Common
{
    /// <summary>
    /// Provides utility methods for color manipulation and conversion.
    /// </summary>
    public static class Colors
    {
        /// <summary>
        /// Converts HSL (Hue, Saturation, Lightness) values to an RGB color vector.
        /// </summary>
        /// <param name="hue">The hue value.</param>
        /// <param name="saturation">The saturation value.</param>
        /// <param name="lightness">The lightness value.</param>
        /// <returns>An RGB color vector corresponding to the given HSL values.</returns>
        public static Vector3 Vector3FromHSL(float hue, float saturation, float lightness)
        {
            // If saturation is zero, the color is a shade of gray, and the RGB values are equal to the lightness.
            if (saturation == 0f) return new Vector3(lightness, lightness, lightness);

            // Calculate q and p values based on the lightness and saturation.
            float q = lightness < 0.5f ? lightness * (1 + saturation) : lightness + saturation - lightness * saturation;
            float p = 2 * lightness - q;

            float[] rgb = new float[3];
            rgb[0] = hue + 1f / 3f;
            rgb[1] = hue;
            rgb[2] = hue - 1f / 3f;

            for (int i = 0; i < 3; i++)
            {
                // Normalize the current RGB value if it's outside the [0, 1] range.
                if (rgb[i] < 0f) rgb[i]++;
                if (rgb[i] > 1f) rgb[i]--;

                if (6f * rgb[i] < 1f)
                {
                    // If the current normalized RGB component is in the first range (0 - 1/6),
                    // use linear interpolation to calculate the adjusted value.
                    rgb[i] = p + ((q - p) * 6f * rgb[i]);
                }
                else if (2f * rgb[i] < 1f)
                {
                    // If the current normalized RGB component is in the second range (1/6 - 1/2),
                    // keep it constant at the calculated 'q' value.
                    rgb[i] = q;
                }
                else if (3f * rgb[i] < 2f)
                {
                    // If the current normalized RGB component is in the third range (1/2 - 2/3),
                    // use linear interpolation to calculate the adjusted value based on the reverse range.
                    rgb[i] = p + ((q - p) * 6f * ((2f / 3f) - rgb[i]));
                }
                else
                {
                    // If the current normalized RGB component is in the fourth range (2/3 - 1),
                    // keep it constant at the calculated 'p' value.
                    rgb[i] = p;
                }
            }

            return new Vector3(rgb[0], rgb[1], rgb[2]);
        }
    }
}