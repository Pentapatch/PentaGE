using PentaGE.Core.Graphics;
using PentaGE.Core.Rendering;

namespace PentaGE.Core.Entities
{
    public class CameraOrientationWidgetEntity : Entity
    {
        public override bool CanHaveMultipleInstances => false;

        public override DisplayMode DisplayMode => DisplayMode.WhenEditing;

        public CameraOrientationWidgetEntity(Mesh mesh, Shader shader, DisplayMode displayMode = DisplayMode.Always)
        {
            var meshRenderer = new MeshRenderComponent(mesh, shader, texture: null, material: null)
            {
                DrawMode = DrawMode.Lines,
            };

            Components.Add(meshRenderer);
        }
    }
}