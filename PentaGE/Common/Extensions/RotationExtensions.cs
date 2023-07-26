using PentaGE.Maths;
using System.Numerics;

namespace PentaGE.Common
{
    /// <summary>
    /// Provides extension methods for the <see cref="Rotation"/> struct.
    /// </summary>
    public static class RotationExtensions
    {
        /// <summary>
        /// Converts a <see cref="Rotation"/> to a <see cref="Matrix4x4"/> representing the rotation.
        /// </summary>
        /// <param name="source">The <see cref="Rotation"/> to convert.</param>
        /// <returns>A <see cref="Matrix4x4"/> representing the rotation.</returns>
        public static Matrix4x4 ToMatrix4x4(this Rotation source)
        {
            Quaternion q = Quaternion.CreateFromYawPitchRoll(
                MathHelper.DegreesToRadians(source.Yaw),
                MathHelper.DegreesToRadians(source.Pitch),
                MathHelper.DegreesToRadians(source.Roll));
            return Matrix4x4.CreateFromQuaternion(q);
        }
    }
}