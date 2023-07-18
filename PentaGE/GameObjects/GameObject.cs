using PentaGE.Structs;
using System.Drawing;

namespace PentaGE.GameObjects
{
    public class TestGameObject : ITransformable
    {
        public Transform Transform { get; set; }

        public Size Size { get; set; }

        public Color Color { get; set; }

        public TestGameObject()
        {
            Transform = new()
            {
                Position = new(0, -200, 0),
                Orientation = new(0, 0, 0),
            };

            Size = new(10, 10);
            Color = Color.Yellow;
        }
    }
}