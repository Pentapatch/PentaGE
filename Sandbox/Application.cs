using PentaGE.Common;
using PentaGE.Core;
using PentaGE.Core.Components;
using PentaGE.Core.Entities;
using PentaGE.Core.Events;
using PentaGE.Core.Graphics;
using PentaGE.Core.Rendering;
using Sandbox.Components;
using System.Numerics;
using static OpenGL.GL;

namespace Sandbox
{
    internal class Application : PentaGameEngine
    {
        protected override bool Initialize()
        {
            // Subscribe to custom timing events
            Timing.CustomTimings[1].Tick += Application_Tick;

            Events.KeyBindings[StopScene].Bind(Key.P, ModifierKey.Control);
            Events.KeyBindings[PauseScene].Bind(Key.P, ModifierKey.Shift);
            Events.KeyBindings[PlayScene].Bind(Key.P);
            Events.KeyBindings[Test1].Bind(Key.Enter);
            Events.KeyBindings[Test2].Bind(Key.Enter, ModifierKey.Control);
            Events.KeyBindings[ToggleRotation].Bind(Key.R, ModifierKey.Control);

            AutoPlay = false;

            return true;
        }

        public void ToggleRotation()
        {
            var rotator = Scenes.Scene[0].Components.Get<DisplayRotator>();
            if (rotator is not null) rotator.Enabled = !rotator.Enabled;
        }

        public void Test1()
        {
            var component = Scenes.Scene[1].Components.Get<TransformComponent>()!.Transform;
            component.Position += new Vector3(1, 1, 0);
            Scenes.Scene[1].Components.Get<TransformComponent>()!.Transform = component;
        }

        public void Test2()
        {
            var entity = new RenderableMeshEntity(MeshFactory.CreateSphere(1f), Assets.Get<Shader>("Default")!);
            Scenes.Scene.Add(entity);
        }

        private void PlayScene() => Scenes.Run();

        private void PauseScene() => Scenes.Pause();

        private void StopScene() => Scenes.Stop();

        protected override bool LoadResources()
        {
            // Set up shaders
            Assets.EnableHotReload(seconds: 5);

            string shaderPath = @"C:\Users\newsi\source\repos\PentaGE\Sandbox\SourceFiles\Shaders\";
            if (!Assets.AddShader("Default", $"{shaderPath}Default.shader")) return false;
            if (!Assets.AddShader("Face", $"{shaderPath}Face.shader")) return false;
            if (!Assets.AddShader("Face2", $"{shaderPath}Face2.shader")) return false;
            if (!Assets.AddShader("Normal", $"{shaderPath}Normal.shader")) return false;
            if (!Assets.AddShader("Light", $"{shaderPath}Light.shader")) return false;
            if (!Assets.AddShader("Grid", $"{shaderPath}Grid.shader")) return false;
            if (!Assets.AddShader("Axes", $"{shaderPath}Axes.shader")) return false;

            // Initialize test texture
            var texturePath = @"C:\Users\newsi\source\repos\PentaGE\Sandbox\SourceFiles\Textures\";
            if (!Assets.AddTexture("BlackPentaTexture",
                    $"{texturePath}Pentapatch_Texture_2k_A.jpg",
                    GL_TEXTURE_2D,
                    GL_TEXTURE0,
                    GL_RGBA,
                    GL_UNSIGNED_BYTE))
                return false;

            if (!Assets.AddTexture("WhitePentaTexture",
                    $"{texturePath}Pentapatch_Texture_2k_B.jpg",
                    GL_TEXTURE_2D,
                    GL_TEXTURE0,
                    GL_RGBA,
                    GL_UNSIGNED_BYTE))
                return false;

            // Set up subject mesh
            var subjectMesh = MeshFactory.CreateCube(1f);
            var transform = new Transform(new(0, 0, 0), new(0, 0, 0), new(1f, 1f, 1f));
            var renderableMesh = new RenderableMeshEntity(
                subjectMesh,
                Assets.Get<Shader>("Default")!,
                Assets.Get<Texture>("BlackPentaTexture"));

            renderableMesh.Components.Add(new TransformComponent(transform));
            renderableMesh.Components.Get<MeshRenderComponent>()!.Material.Albedo = new(1f, 0f, 1f);
            renderableMesh.Components.Get<MeshRenderComponent>()!.Material.SpecularStrength = 1f;
            renderableMesh.Components.Add<DisplayRotator>();
            Assets.Add("Subject", renderableMesh);

            // Set up test light
            var lightMesh = MeshFactory.CreateSphere(0.2f);
            var transform2 = new Transform(new(0.75f, 0.75f, 0.75f), new(0, 0, 0), new(1f, 1f, 1f));
            var renderableLight = new RenderableMeshEntity(lightMesh, Assets.Get<Shader>("Light")!);

            renderableLight.Components.Add(new TransformComponent(transform2));
            Assets.Add("LightEntity", renderableLight);

            // Initialize grid
            Grid gridA = new(10, 10, new(1, 1, 1), 0.2f);
            Grid gridB = new(10, 20, new(0, 0, 0), 0.15f);
            var gridShader = Assets.Get<Shader>("Grid")!;
            var renderableGridMajor = new RenderableGridEntity(gridA, gridShader);
            var renderableGridMinor = new RenderableGridEntity(gridB, gridShader);
            Assets.Add("GridMajor", renderableGridMajor);
            Assets.Add("GridMinor", renderableGridMinor);

            // Initialize axes gizmo
            var axesGizmoMesh = MeshFactory.CreateAxesGizmo(0.1f);
            var renderableAxesGizmo = new RenderableMeshEntity(axesGizmoMesh, Assets.Get<Shader>("Axes")!);
            renderableAxesGizmo.Components.Get<MeshRenderComponent>()!.DrawMode = DrawMode.Lines;
            Assets.Add("AxesGizmo", renderableAxesGizmo);

            // Add entities to the scene
            var scene = Scenes.Add("Main");
            scene.Add((Entity)Assets["Subject"]!);
            scene.Add((Entity)Assets["LightEntity"]!);
            scene.Add((Entity)Assets["GridMajor"]!);
            scene.Add((Entity)Assets["GridMinor"]!);
            scene.Add((Entity)Assets["AxesGizmo"]!);

            // Test: Set up scene
            Scenes.SetActiveScene(scene);

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