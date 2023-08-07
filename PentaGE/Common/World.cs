using System.Numerics;

namespace PentaGE.Common
{
    /// <summary>
    /// Represents a static class that defines standard vectors and constants for a world coordinate system.
    /// </summary>
    public static class World
    {
        /// <summary>
        /// Gets the forward vector of the world. This points along the positive Z-axis.
        /// </summary>
        public static readonly Vector3 ForwardVector = new(0, 0, 1);

        /// <summary>
        /// Gets the up vector of the world. This points along the positive Y-axis.
        /// </summary>
        public static readonly Vector3 UpVector = new(0, 1, 0);

        /// <summary>
        /// Gets the right vector of the world. This points along the positive X-axis.
        /// </summary>
        public static readonly Vector3 RightVector = new(1, 0, 0);

        /// <summary>
        /// Gets the backward vector of the world. This points along the negative Z-axis.
        /// </summary>
        public static readonly Vector3 BackwardVector = -ForwardVector;

        /// <summary>
        /// Gets the down vector of the world. This points along the negative Y-axis.
        /// </summary>
        public static readonly Vector3 DownVector = -UpVector;

        /// <summary>
        /// Gets the left vector of the world. This points along the negative X-axis.
        /// </summary>
        public static readonly Vector3 LeftVector = -RightVector;

        /// <summary>
        /// Defines the number of pixels per meter in the world coordinate system.
        /// </summary>
        public const float PixelsPerMeter = 200.0f;
    }
}