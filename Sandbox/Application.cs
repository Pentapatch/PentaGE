using PentaGE.Common;
using PentaGE.Core;
using PentaGE.Core.Components;
using PentaGE.Core.Entities;
using PentaGE.Core.Events;
using PentaGE.Core.Graphics;
using PentaGE.Core.Logging;
using PentaGE.Core.Rendering;
using Serilog;
using static OpenGL.GL;

namespace Sandbox
{
    internal class Application : PentaGameEngine
    {
        protected override bool Initialize()
        {
            // Do initialization work
            Timing.CustomTimings[1].Tick += Application_Tick;

            return true;
        }

        protected override bool LoadResources()
        {
            // Set up shaders
            Shaders.EnableHotReload(seconds: 5);

            string path = @"C:\Users\newsi\source\repos\PentaGE\PentaGE\Core\Rendering\Shaders\SourceCode\";
            if (!Shaders.Add("Default", $"{path}Default.shader")) return false;
            if (!Shaders.Add("Face", $"{path}Face.shader")) return false;
            if (!Shaders.Add("Face2", $"{path}Face2.shader")) return false;
            if (!Shaders.Add("Normal", $"{path}Normal.shader")) return false;
            if (!Shaders.Add("Light", $"{path}Light.shader")) return false;
            if (!Shaders.Add("Grid", $"{path}Grid.shader")) return false;
            if (!Shaders.Add("Axes", $"{path}Axes.shader")) return false;

            // Initialize test texture
            // TODO: Move this to a texture manager and create a resource manager
            //       Resources > Shaders / Textures / Meshes
            Texture texture = new(
            @"C:\Users\newsi\source\repos\PentaGE\PentaGE\Core\Rendering\Textures\SourceFiles\TestTexture.jpg",
            GL_TEXTURE_2D,
            GL_TEXTURE0,
            GL_RGBA,
            GL_UNSIGNED_BYTE);
            Texture.SetTextureSlot(Shaders.Get("Default")!, "tex0", 0); // Set the texture slot to 0

            // Set up subject mesh
            var subjectMesh = MeshFactory.CreatePyramid(1f, 1.4f, 1f);
            subjectMesh.TileTexture(5, 6);
            var transform = new Transform(new(0, 0, 0), new(0, 0, 0), new(1f, 1f, 1f));
            var renderableMesh = new RenderableMeshEntity(subjectMesh, Shaders.Get("Default")!, texture);

            renderableMesh.AddComponent(new TransformComponent(transform));
            renderableMesh.GetComponent<MeshRenderComponent>()!.Material.Albedo = new(1f, 0f, 1f);
            renderableMesh.GetComponent<MeshRenderComponent>()!.Material.SpecularStrength = 1f;

            // Set up test light
            var lightMesh = MeshFactory.CreateSphere(0.2f);
            var transform2 = new Transform(new(0.75f, 0.75f, 0.75f), new(0, 0, 0), new(1f, 1f, 1f));
            var renderableLight = new RenderableMeshEntity(lightMesh, Shaders.Get("Light")!);

            renderableLight.AddComponent(new TransformComponent(transform2));

            // Initialize grid
            Grid gridA = new(10, 10, new(1, 1, 1), 0.2f);
            Grid gridB = new(10, 20, new(0, 0, 0), 0.15f);
            var renderableGridMajor = new RenderableGridEntity(gridA, Shaders.Get("Grid")!);
            var renderableGridMinor = new RenderableGridEntity(gridB, Shaders.Get("Grid")!);

            // Initialize axes gizmo
            var axesGizmoMesh = MeshFactory.CreateAxesGizmo(0.1f);
            var renderableAxesGizmo = new RenderableMeshEntity(axesGizmoMesh, Shaders.Get("Axes")!);
            renderableAxesGizmo.GetComponent<MeshRenderComponent>()!.DrawMode = DrawMode.Lines;

            Scene.AddEntity(renderableMesh);
            Scene.AddEntity(renderableLight);
            Scene.AddEntity(renderableGridMajor);
            Scene.AddEntity(renderableGridMinor);
            Scene.AddEntity(renderableAxesGizmo);

            return true;
        }

        private void Application_Tick(object? sender, CustomTimingTickEventArgs e)
        {
            Windows[0].Title = $"{Timing.CurrentFps}FPS : {e.ElapsedTime}s : {Timing.RunTime:h\\:mm\\:ss}";
        }

        protected override void Shutdown()
        {
            // Unload resources
        }

        protected override void Update()
        {
            // Update game state
        }

    }
}