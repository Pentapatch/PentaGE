﻿using PentaGE.Maths;
using System.Numerics;

namespace PentaGE.Common
{
    /// <summary>
    /// Represents a rotation in 3D space using yaw, pitch, and roll angles.
    /// </summary>
    public struct Rotation
    {
        /// <summary>
        /// Gets or sets the yaw angle (rotation around the vertical axis, positive Z).
        /// </summary>
        public float Yaw { get; set; }

        /// <summary>
        /// Gets or sets the pitch angle (rotation around the lateral axis, positive Y).
        /// </summary>
        public float Pitch { get; set; }

        /// <summary>
        /// Gets or sets the roll angle (rotation around the longitudinal axis, positive X).
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
            // Convert the yaw and pitch angles to radians
            float yawRad = MathHelper.DegreesToRadians(Yaw);
            float pitchRad = MathHelper.DegreesToRadians(Pitch);

            // Calculate the forward vector using spherical coordinate conversions
            float x = MathF.Cos(pitchRad) * MathF.Sin(yawRad);
            float y = MathF.Sin(pitchRad);
            float z = MathF.Cos(pitchRad) * MathF.Cos(yawRad);

            // Normalize the vector to ensure unit length
            return Vector3.Normalize(new Vector3(x, y, z));
        }

        /// <summary>
        /// Calculates the up vector of the rotation based on its forward and right vectors.
        /// </summary>
        /// <returns>The normalized up vector as a <see cref="Vector3"/> object.</returns>
        public Vector3 GetUpVector()
        {
            // Calculate the forward and right vectors
            Vector3 forward = GetForwardVector();
            Vector3 right = GetRightVector();

            // Calculate the up vector by taking the cross product of forward and right
            return Vector3.Cross(forward, right);
        }

        /// <summary>
        /// Calculates the right vector of the rotation.
        /// </summary>
        /// <returns>The normalized right vector as a <see cref="Vector3"/> object.</returns>
        public Vector3 GetRightVector()
        {
            // Convert the yaw angle to radians
            float yawRad = MathHelper.DegreesToRadians(Yaw);

            // Calculate the right vector using spherical coordinate conversions
            float x = MathF.Cos(yawRad);
            float y = 0;
            float z = -MathF.Sin(yawRad);

            // Normalize the vector to ensure unit length
            return Vector3.Normalize(new Vector3(x, y, z));
        }

        /// <summary>
        /// Calculates the left vector of the rotation by negating its right vector.
        /// </summary>
        /// <returns>The normalized left vector as a <see cref="Vector3"/> object.</returns>
        public Vector3 GetLeftVector() => -GetRightVector();

        /// <summary>
        /// Calculates the bottom vector of the rotation by negating its up vector.
        /// </summary>
        /// <returns>The normalized bottom vector as a <see cref="Vector3"/> object.</returns>
        public Vector3 GetBottomVector() => -GetUpVector();

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
            float normalizedYaw = NormalizeAngle(rotation.Yaw);
            float normalizedPitch = NormalizeAngle(rotation.Pitch);
            float normalizedRoll = NormalizeAngle(rotation.Roll);

            return new Rotation(normalizedYaw, normalizedPitch, normalizedRoll);
        }

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

        /// <summary>
        /// Divides a scalar value by a <see cref="Rotation"/>.
        /// </summary>
        /// <param name="scalar">The scalar value to divide.</param>
        /// <param name="rotation">The <see cref="Rotation"/> to divide into.</param>
        /// <returns>A new <see cref="Rotation"/> representing the result of the division.</returns>
        public static Rotation operator /(float scalar, Rotation rotation) =>
            new(scalar / rotation.Yaw, scalar / rotation.Pitch, scalar / rotation.Roll);

        #endregion

        /// <summary>
        /// Returns a string representation of the <see cref="Rotation"/> in the format "Y{Yaw} P{Pitch} R{Roll}".
        /// </summary>
        /// <returns>A string representing the <see cref="Rotation"/>.</returns>
        public override readonly string ToString() => $"Y{Yaw} P{Pitch} R{Roll}";

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