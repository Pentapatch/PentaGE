using System.Numerics;

namespace PentaGE.Structs
{
    public struct Transform : IPositionable, IOrientable, IScalable
    {
        public Vector3 Position { get; set; } = new();

        public EulerAngles Orientation { get; set; } = new();

        public Vector3 Scale { get; set; } = Vector3.One;

        public Transform(Vector3 position, EulerAngles orientation)
        {
            Position = position;
            Orientation = orientation;
        }
    }
}