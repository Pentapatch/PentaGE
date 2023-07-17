using System.Numerics;

namespace PentaGE.Structs
{
    public struct Transform : IPositionable, IOrientable
    {
        public Vector3 Position { get; set; } = new();

        public EulerAngles Orientation { get; set; } = new();

        public Transform(Vector3 position, EulerAngles orientation)
        {
            Position = position;
            Orientation = orientation;
        }
    }
}