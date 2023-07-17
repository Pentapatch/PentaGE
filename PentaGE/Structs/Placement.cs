using System.Numerics;

namespace PentaGE.Structs
{
    public struct Placement : IPositionable, IOrientable
    {
        public Vector3 Position { get; set; }
        public EulerAngles Orientation { get; set; }

        public Placement(Vector3 position, EulerAngles orientation)
        {
            Position = position;
            Orientation = orientation;
        }
    }
}