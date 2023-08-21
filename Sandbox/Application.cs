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
            Events.KeyBindings[RestartScene].Bind(Key.P, ModifierKey.Control | ModifierKey.Shift);
            Events.KeyBindings[Test1].Bind(Key.Enter);
            Events.KeyBindings[Test2].Bind(Key.Enter, ModifierKey.Control);
            Events.KeyBindings[ToggleRotation].Bind(Key.R, ModifierKey.Control);
            Events.KeyBindings[ToggleMaterialModulator].Bind(Key.M, ModifierKey.Control);
            Events.KeyBindings[ToggleFollowLight].Bind(Key.F, ModifierKey.Control);
            Events.KeyBindings[ToggleDirectionalLight].Bind(Key.F1, ModifierKey.Control);

            AutoPlay = false;

            return true;
        }

        public void ToggleDirectionalLight() =>
            Scenes.CurrentScene.DirectionalLight!.Enabled = !Scenes.CurrentScene.DirectionalLight.Enabled;

        public void ToggleFollowLight() => 
            Scenes.CurrentScene.DirectionalLight!.FollowCamera = !Scenes.CurrentScene.DirectionalLight.FollowCamera;

        public void ToggleMaterialModulator()
        {
            bool enabled = false;
            foreach (var modulator in Scenes.CurrentScene[0].Components.GetAll<MaterialModulator>())
            {
                modulator.Enabled = !modulator.Enabled;
                if (modulator.Enabled) enabled = true;
            }

            var component = Scenes.CurrentScene[0].Components.Get<MeshRenderComponent>()!;
            if (!enabled)
            {
                component.Material = new PBRMaterial();
                component.Texture = Assets.Get<Texture>("BlackPentaTexture")!;
            }
            else
            {
                component.Texture = Assets.Get<Texture>("WhitePentaTexture")!;
            }
        }

        public void ToggleRotation()
        {
            foreach (var rotator in Scenes.CurrentScene[0].Components.GetAll<ConstantRotator>())
            {
                rotator.Enabled = !rotator.Enabled;
            }
        }

        public void Test1()
        {
            for (int i = 0; i < 10; i++)
            {
                var mesh = MeshFactory.CreateSphere((float)Random.Shared.NextDouble() * 0.15f + 0.05f);
                var shader = Assets.Get<Shader>("Default")!;
                var texture = Assets.Get<Texture>("WhitePentaTexture")!;
                var entity = new RenderableMeshEntity(mesh, shader, texture);
                var transform = new Transform
                {
                    Position = new Vector3(
                        (float)Random.Shared.NextDouble() * 4 - 2,
                        (float)Random.Shared.NextDouble() * 4 - 2,
                        (float)Random.Shared.NextDouble() * 4 - 2)
                };
                var transformComponent = new TransformComponent(transform);
                entity.Components.Add(transformComponent);
                entity.Components.Add(new ConstantRotator());
                entity.Components.Add(new MaterialModulator());
                Scenes.CurrentScene.SpawnEntity(entity);
            }
        }

        public void Test2()
        {
            var scene = Scenes.Exists("Default") ? Scenes["Default"] : Scenes.Add("Default");
            var mesh = Random.Shared.NextDouble() switch
            {
                < 0.25 => MeshFactory.CreateCube(1f),
                < 0.5 => MeshFactory.CreateSphere(1f),
                < 0.75 => MeshFactory.CreateCylinder(1f, 1f),
                _ => MeshFactory.CreateCone(1f, 1f)
            };
            var shader = Assets.Get<Shader>("Default")!;
            var texture = Assets.Get<Texture>("BlackPentaTexture")!;
            var entity = new RenderableMeshEntity(mesh, shader, texture);
            entity.Components.Add<TransformComponent>();
            entity.Components.Add<ConstantRotator>();
            scene.Clear();
            scene.Add(entity);
            scene.Load();
            Scenes.Run();
        }

        private void PlayScene() => Scenes.Run();

        private void PauseScene()
        {
            if (Scenes.State == PentaGE.Core.Scenes.SceneState.Paused)
                Scenes.Run();
            else
                Scenes.Pause();
        }

        private void StopScene() => Scenes.Stop();

        private void RestartScene() => Scenes.Restart();

        protected override bool LoadResources()
        {
            // Set up shaders
            Assets.EnableHotReload(intervalInSeconds: 5);

            string shaderPath = @"C:\Users\newsi\source\repos\PentaGE\Sandbox\SourceFiles\Shaders\";
            if (!Assets.AddShader("Default", $"{shaderPath}Default.shader")) return false;
            if (!Assets.AddShader("Face", $"{shaderPath}Face.shader")) return false;
            if (!Assets.AddShader("Face2", $"{shaderPath}Face2.shader")) return false;
            if (!Assets.AddShader("Normal", $"{shaderPath}Normal.shader")) return false;
            if (!Assets.AddShader("Light", $"{shaderPath}Light.shader")) return false;
            if (!Assets.AddShader("Grid", $"{shaderPath}Grid.shader")) return false;
            if (!Assets.AddShader("AxesShader", $"{shaderPath}Axes.shader")) return false;

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
            renderableMesh.Components.Add<ConstantRotator>();
            renderableMesh.Components.Add(new ConstantRotator(false, true, false) { Speed = 0.25f });
            renderableMesh.Components.Add(new MaterialModulator() { AlbedoEnabled = true, AlbedoModulatorFactors = new(1f, 0.75f, 0.25f), Enabled = false });
            Assets.Add("Subject", renderableMesh);

            // Set up test directional light
            var dirLightMesh = MeshFactory.CreateSphere(0.2f);
            var widgetTransform = new Transform(new Vector3(0f, 1f, 0f), Rotation.Zero, Vector3.One);
            var rotation = new Rotation(45f, -45f, 0f);
            var color = new Vector4(1f, 1f, 1f, 1f);
            var directionalLight = new DirectionalLightEntity(dirLightMesh, Assets.Get<Shader>("Default")!, rotation, widgetTransform, color);
            Assets.Add("DirectionalLightEntity", directionalLight);

            // Set up test sun
            var sunMesh = MeshFactory.CreateSphere(20f);
            var sun = new SunEntity(sunMesh, Assets.Get<Shader>("Light")!, Windows[0].Viewport.CameraManager.ActiveController, directionalLight);
            Assets.Add("SunEntity", sun);
            
            // Initialize grid
            Grid gridA = new(10, 10, new(1, 1, 1), 0.2f);
            Grid gridB = new(10, 20, new(0, 0, 0), 0.15f);
            var gridShader = Assets.Get<Shader>("Grid")!;
            var renderableGridMajor = new RenderableGridEntity(gridA, gridShader);
            var renderableGridMinor = new RenderableGridEntity(gridB, gridShader);
            Assets.Add("GridMajor", renderableGridMajor);
            Assets.Add("GridMinor", renderableGridMinor);

            // Initialize axes gizmo
            var cameraOrientationWidgetMesh = MeshFactory.CreateAxesGizmo(0.1f);
            var cameraOrientationWidgetEntity = new CameraOrientationWidgetEntity(cameraOrientationWidgetMesh, Assets.Get<Shader>("AxesShader")!);
            Assets.Add("CameraOrientationWidget", cameraOrientationWidgetEntity);

            // Add entities to the scene
            var scene = Scenes.Add("Main");
            scene.Add((Entity)Assets["Subject"]!);
            scene.Add((Entity)Assets["GridMajor"]!);
            scene.Add((Entity)Assets["GridMinor"]!);
            scene.Add((Entity)Assets["CameraOrientationWidget"]!);
            scene.Add((Entity)Assets["DirectionalLightEntity"]!);
            scene.Add((Entity)Assets["SunEntity"]!);
            scene.Load();

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