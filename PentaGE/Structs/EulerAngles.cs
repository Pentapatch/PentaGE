using PentaGE.MathExtensions;
using System.Numerics;

namespace PentaGE.Structs
{
    public struct EulerAngles
    {
        public float Yaw;
        public float Pitch;
        public float Roll;

        public EulerAngles(float yaw, float pitch, float roll)
        {
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
        }

        public Vector3 GetForwardVector()
        {
            // Convert the yaw and pitch angles to radians
            float yawRad = MathHelper.DegreesToRadians(Yaw);
            float pitchRad = MathHelper.DegreesToRadians(Pitch);

            // Calculate the forward vector using spherical coordinate conversions
            float x = MathF.Cos(yawRad) * MathF.Cos(pitchRad);
            float y = MathF.Sin(pitchRad);
            float z = MathF.Sin(yawRad) * MathF.Cos(pitchRad);

            return new Vector3(x, y, z);
        }

        public Vector3 GetUpVector()
        {
            // Convert the yaw, pitch and roll angles to radians
            float yawRad = MathHelper.DegreesToRadians(Yaw);
            float pitchRad = MathHelper.DegreesToRadians(Pitch);
            float rollRad = MathHelper.DegreesToRadians(Roll);

            float x = -MathF.Sin(rollRad) * MathF.Sin(yawRad) + MathF.Cos(rollRad) * MathF.Sin(pitchRad) * MathF.Cos(yawRad);
            float y = MathF.Cos(rollRad) * MathF.Sin(pitchRad);
            float z = MathF.Sin(rollRad) * MathF.Cos(yawRad) + MathF.Cos(rollRad) * MathF.Sin(pitchRad) * MathF.Sin(yawRad);

            return new Vector3(x, y, z);
        }

        public Vector3 GetRightVector()
        {
            Vector3 forward = GetForwardVector();
            Vector3 up = GetUpVector();

            return Vector3.Cross(up, forward);
        }
    }
}