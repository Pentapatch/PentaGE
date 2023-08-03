using PentaGE.Maths;
using System.Numerics;

namespace PentaGE.Common
{
    /// <summary>
    /// Represents a rotation in 3D space using yaw, pitch, and roll angles.
    /// </summary>
    public struct Rotation
    {
        /// <summary>
        /// Gets or sets the yaw angle (rotation around the vertical axis, positive Y).
        /// </summary>
        public float Yaw { get; set; }

        /// <summary>
        /// Gets or sets the pitch angle (rotation around the lateral axis, positive X).
        /// </summary>
        public float Pitch { get; set; }

        /// <summary>
        /// Gets or sets the roll angle (rotation around the longitudinal axis, positive Z).
        /// </summary>
        public float Roll { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rotation"/> struct with the specified yaw, pitch, and roll angles.
        /// </summary>
        /// <param name="yaw">The yaw angle in degrees.</param>
        /// <param name="pitch">The pitch angle in degrees.</param>
        /// <param name="roll">The roll angle in degrees.</param>
        public Rotation(float yaw, float pitch, float roll)
        {
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
        }

        #region Vectors

        /// <summary>
        /// Calculates the forward vector of the rotation.
        /// </summary>
        /// <returns>The normalized forward vector as a <see cref="Vector3"/> object.</returns>
        public Vector3 GetForwardVector()
        {
            // Create the rotation matrix from the yaw, pitch, and roll angles
            Matrix4x4 rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(
                MathHelper.DegreesToRadians(Yaw),
                MathHelper.DegreesToRadians(Pitch),
                MathHelper.DegreesToRadians(Roll)
            );

            // Extract the right, up, and forward vectors from the rotation matrix
            Vector3 forward = new(rotationMatrix.M31, rotationMatrix.M32, rotationMatrix.M33);

            // Normalize the vectors to ensure unit length
            return Vector3.Normalize(forward);
        }

        /// <summary>
        /// Calculates the up vector of the rotation based on its forward and right vectors.
        /// </summary>
        /// <returns>The normalized up vector as a <see cref="Vector3"/> object.</returns>
        public Vector3 GetUpVector()
        {
            // Create the rotation matrix from the yaw, pitch, and roll angles
            Matrix4x4 rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(
                MathHelper.DegreesToRadians(Yaw),
                MathHelper.DegreesToRadians(Pitch),
                MathHelper.DegreesToRadians(Roll)
            );

            // Extract the right, up, and forward vectors from the rotation matrix
            Vector3 up = new(rotationMatrix.M21, rotationMatrix.M22, rotationMatrix.M23);

            // Normalize the vectors to ensure unit length
            return Vector3.Normalize(up);
        }

        /// <summary>
        /// Calculates the right vector of the rotation.
        /// </summary>
        /// <returns>The normalized right vector as a <see cref="Vector3"/> object.</returns>
        public Vector3 GetRightVector()
        {
            Matrix4x4 rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(
                MathHelper.DegreesToRadians(Yaw),
                MathHelper.DegreesToRadians(Pitch),
                MathHelper.DegreesToRadians(Roll)
            );

            // Extract the right, up, and forward vectors from the rotation matrix
            Vector3 rightVector = new(rotationMatrix.M11, rotationMatrix.M12, rotationMatrix.M13);

            // Normalize the vectors to ensure unit length
            return Vector3.Normalize(rightVector);
        }

        /// <summary>
        /// Calculates the left vector of the rotation by negating its right vector.
        /// </summary>
        /// <returns>The normalized left vector as a <see cref="Vector3"/> object.</returns>
        public Vector3 GetLeftVector() => -GetRightVector();

        /// <summary>
        /// Calculates the down vector of the rotation by negating its up vector.
        /// </summary>
        /// <returns>The normalized down vector as a <see cref="Vector3"/> object.</returns>
        public Vector3 GetDownVector() => -GetUpVector();

        /// <summary>
        /// Calculates the backward vector of the rotation by negating its forward vector.
        /// </summary>
        /// <returns>The normalized backward vector as a <see cref="Vector3"/> object.</returns>
        public Vector3 GetBackwardVector() => -GetForwardVector();

        #endregion

        #region Math

        /// <summary>
        /// Linearly interpolates between this rotation and another rotation using the specified interpolation factor.
        /// </summary>
        /// <param name="rotation">The target rotation to interpolate towards.</param>
        /// <param name="t">The interpolation factor in the range (0 - 1).</param>
        /// <returns>A new <see cref="Rotation"/> instance representing the interpolated rotation.</returns>
        public Rotation Lerp(Rotation rotation, float t) =>
            Lerp(this, rotation, t);

        /// <summary>
        /// Linearly interpolates between two rotations using the specified interpolation factor.
        /// </summary>
        /// <param name="rotationA">The starting rotation.</param>
        /// <param name="rotationB">The target rotation to interpolate towards.</param>
        /// <param name="t">The interpolation factor in the range (0 - 1).</param>
        /// <returns>A new <see cref="Rotation"/> instance representing the interpolated rotation.</returns>
        public static Rotation Lerp(Rotation rotationA, Rotation rotationB, float t)
        {
            // Convert angles to radians
            float yawRadA = MathHelper.DegreesToRadians(rotationA.Yaw);
            float pitchRadA = MathHelper.DegreesToRadians(rotationA.Pitch);
            float rollRadA = MathHelper.DegreesToRadians(rotationA.Roll);

            float yawRadB = MathHelper.DegreesToRadians(rotationB.Yaw);
            float pitchRadB = MathHelper.DegreesToRadians(rotationB.Pitch);
            float rollRadB = MathHelper.DegreesToRadians(rotationB.Roll);

            // Interpolate each component separately
            float yawRadInterpolated = MathHelper.LerpF(yawRadA, yawRadB, t);
            float pitchRadInterpolated = MathHelper.LerpF(pitchRadA, pitchRadB, t);
            float rollRadInterpolated = MathHelper.LerpF(rollRadA, rollRadB, t);

            // Convert back to degrees
            float yawInterpolated = MathHelper.RadiansToDegrees(yawRadInterpolated);
            float pitchInterpolated = MathHelper.RadiansToDegrees(pitchRadInterpolated);
            float rollInterpolated = MathHelper.RadiansToDegrees(rollRadInterpolated);

            return new Rotation(yawInterpolated, pitchInterpolated, rollInterpolated);
        }

        /// <summary>
        /// Interpolates between two rotations using the specified interpolation factor and curve type.
        /// </summary>
        /// <param name="rotationA">The starting rotation.</param>
        /// <param name="rotationB">The target rotation to interpolate towards.</param>
        /// <param name="t">The interpolation factor in the range (0 - 1).</param>
        /// <param name="curvePower">The power/exponent to apply to the interpolation factor (for custom curves).</param>
        /// <param name="curveType">The type of interpolation curve to use.</param>
        /// <returns>A new <see cref="Rotation"/> instance representing the interpolated rotation.</returns>
        public static Rotation Interpolate(
            Rotation rotationA,
            Rotation rotationB,
            float t,
            float curvePower,
            InterpolationCurve curveType) =>
            Lerp(rotationA, rotationB, MathHelper.TransformInterpolationFactor(t, curvePower, curveType));

        /// <summary>
        /// Interpolates between this rotation and a target rotation using the specified interpolation factor and curve type.
        /// </summary>
        /// <param name="rotation">The target rotation to interpolate towards.</param>
        /// <param name="t">The interpolation factor in the range (0 - 1).</param>
        /// <param name="curvePower">The power/exponent to apply to the interpolation factor (for custom curves).</param>
        /// <param name="curveType">The type of interpolation curve to use.</param>
        /// <returns>A new <see cref="Rotation"/> instance representing the interpolated rotation.</returns>
        public Rotation Interpolate(
            Rotation rotation,
            float t,
            float curvePower,
            InterpolationCurve curveType) =>
            Interpolate(this, rotation, t, curvePower, curveType);

        /// <summary>
        /// Normalizes the rotation angles to be within the range (0 - 360) degrees.
        /// </summary>
        /// <returns>A new <see cref="Rotation"/> instance with normalized angles.</returns>
        public Rotation Normalize() =>
            Normalize(this);

        /// <summary>
        /// Normalizes the rotation angles to be within the range (0 - 360) degrees.
        /// </summary>
        /// <param name="rotation">The rotation to normalize.</param>
        /// <returns>A new <see cref="Rotation"/> instance with normalized angles.</returns>
        public static Rotation Normalize(Rotation rotation)
        {
            float yawNormalized = NormalizeAngle(rotation.Yaw);
            float pitchNormalized = NormalizeAngle(rotation.Pitch);
            float rollNormalized = NormalizeAngle(rotation.Roll);

            return new Rotation(yawNormalized, pitchNormalized, rollNormalized);
        }

        /// <summary>
        /// Sets the rotation to face a target position.
        /// </summary>
        /// <param name="targetPosition">The position of the target to look at.</param>
        /// <remarks>
        /// The rotation will be adjusted to face the <paramref name="targetPosition"/> in the X-Z plane,
        /// and the pitch angle will be adjusted to look at the <paramref name="targetPosition"/> in the Y direction.
        /// </remarks>
        public void LookAt(Vector3 targetPosition, Vector3 sourcePosition)
        {
            Vector3 direction = Vector3.Normalize(sourcePosition - targetPosition);

            // Calculate and set the rotation angles
            Yaw = MathHelper.RadiansToDegrees(MathF.Atan2(direction.X, direction.Z));
            Pitch = MathHelper.RadiansToDegrees(MathF.Asin(-direction.Y));
        }

        /// <summary>
        /// Calculates the rotation needed to face a target position.
        /// </summary>
        /// <param name="targetPosition">The position of the target to look at.</param>
        /// <returns>A new <see cref="Rotation"/> instance representing the rotation needed to face the target position.</returns>
        /// <remarks>
        /// The rotation will be adjusted to face the <paramref name="targetPosition"/> in the X-Z plane,
        /// and the pitch angle will be adjusted to look at the <paramref name="targetPosition"/> in the Y direction.
        /// </remarks>
        public static Rotation GetLookAt(Vector3 targetPosition, Vector3 sourcePosition)
        {
            Vector3 direction = Vector3.Normalize(sourcePosition - targetPosition);

            // Calculate the rotation angles
            float yaw = MathHelper.RadiansToDegrees(MathF.Atan2(direction.X, direction.Z));
            float pitch = MathHelper.RadiansToDegrees(MathF.Asin(-direction.Y));

            return new Rotation(yaw, pitch, 0f);
        }

        /// <summary>
        /// Inverts the rotation angles in a rotation.
        /// </summary>
        /// <param name="rotation">The rotation to invert.</param>
        /// <returns>A new <see cref="Rotation"/> instance representing the inverted rotation.</returns>
        public static Rotation Invert(Rotation rotation) =>
            new(-rotation.Yaw, -rotation.Pitch, -rotation.Roll);

        /// <summary>
        /// Inverts the rotation angles in this rotation.
        /// </summary>
        /// <returns>A new <see cref="Rotation"/> instance representing the inverted rotation.</returns>
        public Rotation Invert() => Invert(this);

        #endregion

        #region Factory methods

        /// <summary>
        /// Creates a new <see cref="Rotation"/> instance from a 3D vector representing the orientation.
        /// </summary>
        /// <param name="vector">The 3D vector representing the orientation.</param>
        /// <param name="roll">The roll angle in degrees (default is 0).</param>
        /// <returns>A new <see cref="Rotation"/> instance with the specified yaw, pitch, and roll angles.</returns>
        public static Rotation FromVector3(Vector3 vector, float roll = 0f)
        {
            // Convert the vector components to yaw and pitch angles
            float pitch = MathHelper.RadiansToDegrees(MathF.Asin(vector.Y));
            float yaw = MathHelper.RadiansToDegrees(MathF.Atan2(vector.X, vector.Z));

            return new Rotation(yaw, pitch, roll);
        }

        /// <summary>
        /// Gets a <see cref="Rotation"/> instance representing zero rotation (0 degrees in all axes).
        /// </summary>
        public static Rotation Zero => new(0, 0, 0);

        #endregion

        #region Operator Overloads

        /// <summary>
        /// Adds two <see cref="Rotation"/> instances.
        /// </summary>
        /// <param name="a">The first <see cref="Rotation"/> to add.</param>
        /// <param name="b">The second <see cref="Rotation"/> to add.</param>
        /// <returns>A new <see cref="Rotation"/> representing the sum of the two input rotations.</returns>
        public static Rotation operator +(Rotation a, Rotation b) =>
            new(a.Yaw + b.Yaw, a.Pitch + b.Pitch, a.Roll + b.Roll);

        /// <summary>
        /// Subtracts one <see cref="Rotation"/> from another.
        /// </summary>
        /// <param name="a">The <see cref="Rotation"/> to subtract from (minuend).</param>
        /// <param name="b">The <see cref="Rotation"/> to subtract (subtrahend).</param>
        /// <returns>A new <see cref="Rotation"/> representing the difference between the two input rotations.</returns>
        public static Rotation operator -(Rotation a, Rotation b) =>
            new(a.Yaw - b.Yaw, a.Pitch - b.Pitch, a.Roll - b.Roll);

        /// <summary>
        /// Negates the rotation by inverting the yaw, pitch, and roll angles.
        /// </summary>
        /// <param name="rotation">The rotation to negate.</param>
        /// <returns>A new <see cref="Rotation"/> instance representing the negated rotation.</returns>
        public static Rotation operator -(Rotation rotation) => rotation.Invert();

        /// <summary>
        /// Multiplies a <see cref="Rotation"/> by a scalar value.
        /// </summary>
        /// <param name="rotation">The <see cref="Rotation"/> to multiply.</param>
        /// <param name="scalar">The scalar value to multiply the rotation by.</param>
        /// <returns>A new <see cref="Rotation"/> representing the result of the multiplication.</returns>
        public static Rotation operator *(Rotation rotation, float scalar) =>
            new(rotation.Yaw * scalar, rotation.Pitch * scalar, rotation.Roll * scalar);

        /// <summary>
        /// Multiplies a scalar value by a <see cref="Rotation"/>.
        /// </summary>
        /// <param name="scalar">The scalar value to multiply.</param>
        /// <param name="rotation">The <see cref="Rotation"/> to multiply.</param>
        /// <returns>A new <see cref="Rotation"/> representing the result of the multiplication.</returns>
        public static Rotation operator *(float scalar, Rotation rotation) =>
            rotation * scalar;

        /// <summary>
        /// Divides a <see cref="Rotation"/> by a scalar value.
        /// </summary>
        /// <param name="rotation">The <see cref="Rotation"/> to divide.</param>
        /// <param name="scalar">The scalar value to divide the rotation by.</param>
        /// <returns>A new <see cref="Rotation"/> representing the result of the division.</returns>
        public static Rotation operator /(Rotation rotation, float scalar) =>
            new(rotation.Yaw / scalar, rotation.Pitch / scalar, rotation.Roll / scalar);


        #endregion

        #region Conversations

        /// <summary>
        /// Converts this <see cref="Rotation"/> to a <see cref="Matrix4x4"/> representing the rotation.
        /// </summary>
        /// <returns>A <see cref="Matrix4x4"/> representing this rotation.</returns>
        public Matrix4x4 ToMatrix4x4()
        {
            Quaternion q = Quaternion.CreateFromYawPitchRoll(
                MathHelper.DegreesToRadians(Yaw),
                MathHelper.DegreesToRadians(Pitch),
                MathHelper.DegreesToRadians(Roll));
            return Matrix4x4.CreateFromQuaternion(q);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Rotation"/> in the format "Y{Yaw} P{Pitch} R{Roll}".
        /// </summary>
        /// <returns>A string representing the <see cref="Rotation"/>.</returns>
        public override readonly string ToString() => $"Y{Yaw} P{Pitch} R{Roll}";

        #endregion

        private static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle < 0f)
            {
                angle += 360f;
            }
            return angle;
        }

    }
}