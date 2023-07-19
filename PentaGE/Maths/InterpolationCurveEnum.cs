namespace PentaGE.Maths
{
    /// <summary>
    /// Specifies the type of interpolation curve to use when transforming an interpolation factor.
    /// </summary>
    public enum InterpolationCurve
    {
        /// <summary>
        /// Represents a linear interpolation curve. The interpolation factor remains unchanged.
        /// </summary>
        Linear = 1,

        /// <summary>
        /// Represents an ease-in interpolation curve. The interpolation factor starts slowly and accelerates.
        /// </summary>
        EaseIn,

        /// <summary>
        /// Represents an ease-out interpolation curve. The interpolation factor starts quickly and decelerates.
        /// </summary>
        EaseOut,

        /// <summary>
        /// Represents an ease-in-out interpolation curve. The interpolation factor starts and ends slowly,
        /// with faster interpolation in the middle.
        /// </summary>
        EaseInOut,

        /// <summary>
        /// Represents an ease-out-in interpolation curve. The interpolation factor starts and ends quickly,
        /// with slower interpolation in the middle.
        /// </summary>
        EaseOutIn,
    }
}