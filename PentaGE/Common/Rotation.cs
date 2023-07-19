using PentaGE.Maths;
using System.Numerics;

namespace PentaGE.Common
{
    public struct Rotation
    {
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }

        public Rotation(float yaw, float pitch, float roll)
        {
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
        }

        #region Vectors

        public Vector3 GetForwardVector()
        {
            float yawRad = MathHelper.DegreesToRadians(Yaw);
            float pitchRad = MathHelper.DegreesToRadians(Pitch);

            float x = MathF.Cos(pitchRad) * MathF.Sin(yawRad);
            float y = MathF.Sin(pitchRad);
            float z = MathF.Cos(pitchRad) * MathF.Cos(yawRad);

            return Vector3.Normalize(new Vector3(x, y, z));
        }

        public Vector3 GetUpVector()
        {
            Vector3 forward = GetForwardVector();
            Vector3 right = GetRightVector();

            return Vector3.Cross(forward, right);
        }

        public Vector3 GetRightVector()
        {
            float yawRad = MathHelper.DegreesToRadians(Yaw);

            float x = MathF.Cos(yawRad);
            float y = 0;
            float z = -MathF.Sin(yawRad);

            return Vector3.Normalize(new Vector3(x, y, z));
        }

        // For convenience:
        public Vector3 GetLeftVector() => -GetRightVector();

        public Vector3 GetBottomVector() => -GetUpVector();

        public Vector3 GetBackwardVector() => -GetForwardVector();

        #endregion

        #region Math

        public Rotation Lerp(Rotation rotation, float t) =>
            Lerp(this, rotation, t);

        public static Rotation Lerp(Rotation rotationA, Rotation rotationB, float t)
        {
            float yawRadA = MathHelper.DegreesToRadians(rotationA.Yaw);
            float pitchRadA = MathHelper.DegreesToRadians(rotationA.Pitch);
            float rollRadA = MathHelper.DegreesToRadians(rotationA.Roll);

            float yawRadB = MathHelper.DegreesToRadians(rotationB.Yaw);
            float pitchRadB = MathHelper.DegreesToRadians(rotationB.Pitch);
            float rollRadB = MathHelper.DegreesToRadians(rotationB.Roll);

            float yawRadInterpolated = MathHelper.LerpF(yawRadA, yawRadB, t);
            float pitchRadInterpolated = MathHelper.LerpF(pitchRadA, pitchRadB, t);
            float rollRadInterpolated = MathHelper.LerpF(rollRadA, rollRadB, t);

            float yawInterpolated = MathHelper.RadiansToDegrees(yawRadInterpolated);
            float pitchInterpolated = MathHelper.RadiansToDegrees(pitchRadInterpolated);
            float rollInterpolated = MathHelper.RadiansToDegrees(rollRadInterpolated);

            return new Rotation(yawInterpolated, pitchInterpolated, rollInterpolated);
        }

        public Rotation Normalize() =>
            Normalize(this);

        public static Rotation Normalize(Rotation rotation)
        {
            float normalizedYaw = NormalizeAngle(rotation.Yaw);
            float normalizedPitch = NormalizeAngle(rotation.Pitch);
            float normalizedRoll = NormalizeAngle(rotation.Roll);

            return new Rotation(normalizedYaw, normalizedPitch, normalizedRoll);
        }

        #endregion

        #region Factory methods

        public static Rotation FromVector3(Vector3 vector, float roll = 0f)
        {
            float pitch = MathHelper.RadiansToDegrees(MathF.Asin(vector.Y));
            float yaw = MathHelper.RadiansToDegrees(MathF.Atan2(vector.X, vector.Z));

            return new Rotation(yaw, pitch, roll);
        }

        #endregion

        #region Operator Overloads

        public static Rotation operator +(Rotation a, Rotation b) =>
            new(a.Yaw + b.Yaw, a.Pitch + b.Pitch, a.Roll + b.Roll);

        public static Rotation operator -(Rotation a, Rotation b) =>
            new(a.Yaw - b.Yaw, a.Pitch - b.Pitch, a.Roll - b.Roll);

        public static Rotation operator *(Rotation rotation, float scalar) =>
            new(rotation.Yaw * scalar, rotation.Pitch * scalar, rotation.Roll * scalar);

        public static Rotation operator *(float scalar, Rotation rotation) =>
            rotation * scalar;

        public static Rotation operator /(Rotation rotation, float scalar) =>
            new(rotation.Yaw / scalar, rotation.Pitch / scalar, rotation.Roll / scalar);

        public static Rotation operator /(float scalar, Rotation rotation) =>
            new(scalar / rotation.Yaw, scalar / rotation.Pitch, scalar / rotation.Roll);

        #endregion

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