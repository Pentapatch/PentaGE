using GLFW;
using PentaGE.Common;
using PentaGE.Core.Components;
using PentaGE.Core.Entities;
using PentaGE.Core.Graphics;
using PentaGE.Core.Logging;
using Serilog;
using System.Numerics;
using static OpenGL.GL;

namespace PentaGE.Core.Rendering
{
    /// <summary>
    /// The renderer responsible for handling graphics rendering in the PentaGameEngine.
    /// </summary>
    internal class Renderer
    {
        private readonly PentaGameEngine _engine;
        private bool cullingEnabled = false;
        private bool wireframe = false;
        private bool blackTexture = true;
        private int activeSubjectIndex = 0;

        /// <summary>
        /// Creates a new instance of the Renderer class.
        /// </summary>
        /// <param name="engine">The PentaGameEngine instance associated with this Renderer.</param>
        internal Renderer(PentaGameEngine engine)
        {
            _engine = engine;
        }

        /// <summary>
        /// Initializes GLFW and sets up the necessary context hints for rendering.
        /// </summary>
        /// <returns><c>true</c> if GLFW is successfully initialized; otherwise, <c>false</c>.</returns>
        internal unsafe bool InitializeGLFW()
        {
            if (!Glfw.Init())
            {
                Log.Fatal("Failed to initialize GLFW");
                return false;
            }

            _engine.Events.GlfwError += Events_GlfwError;

            // Set up GLFW versioning
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);

            // Initialize and create all windows
            if (!_engine.Windows.Initialize())
            {
                Log.Fatal("Failed to create all GLFW windows.");
                return false;
            }

            // Enable depth testing
            glEnable(GL_DEPTH_TEST);

            // Enable blending
            glEnable(GL_BLEND);
            glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

            #region Subscribe to HotKey events

            // Subscribe to events
            // TODO: This belongs to the concrete implementation of the engine (Editor)
            //       and should be moved there
            _engine.Events.HotKeys[Key.F1].Event += ToggleWireframe_HotKey;
            _engine.Events.HotKeys[Key.F2].Event += ToggleCulling_HotKey;
            _engine.Events.HotKeys[Key.F5].Event += SetShaderToDefault_HotKey;
            _engine.Events.HotKeys[Key.F6].Event += SetShaderToFaceA_HotKey;
            _engine.Events.HotKeys[Key.F6, ModifierKey.Shift].Event += SetShaderToFaceB_HotKey;
            _engine.Events.HotKeys[Key.F7].Event += SetShaderToNormal_HotKey;
            _engine.Events.HotKeys[Key.F10].Event += Subdivide_HotKey;
            _engine.Events.HotKeys[Key.F11].Event += TileTexture_HotKey;
            _engine.Events.HotKeys[Key.F12].Event += Roughen_HotKey;
            _engine.Events.HotKeys[Key.F12, ModifierKey.Control].Event += Explode_HotKey;
            _engine.Events.HotKeys[Key.I, ModifierKey.Control].Event += ToggleTexture_HotKey;
            _engine.Events.HotKeys[Key.Alpha1].Event += CreateCube_HotKey;
            _engine.Events.HotKeys[Key.Alpha2].Event += CreateCylinder_HotKey;
            _engine.Events.HotKeys[Key.Alpha3].Event += CreateCone_HotKey;
            _engine.Events.HotKeys[Key.Alpha4].Event += CreatePyramid_HotKey;
            _engine.Events.HotKeys[Key.Alpha5].Event += CreateSphere_HotKey;
            _engine.Events.HotKeys[Key.Alpha6].Event += CreatePlane_HotKey;
            _engine.Events.HotKeys[Key.Backspace, ModifierKey.Control].Event += OrbitLight_HotKey;
            _engine.Events.HotKeys[Key.L, ModifierKey.Control].Event += GenerateLandscape_HotKey;
            _engine.Events.HotKeys[Key.T, ModifierKey.Control].Event += PerformTest_HotKey;
            _engine.Events.HotKeys[Key.Left].Event += YawSubjectLeft_HotKey;
            _engine.Events.HotKeys[Key.Right].Event += YawSubjectRight_HotKey;
            _engine.Events.HotKeys[Key.Up].Event += PitchSubjectUp_HotKey;
            _engine.Events.HotKeys[Key.Down].Event += PitchSubjectDown_HotKey;
            _engine.Events.HotKeys[Key.Left, ModifierKey.Control].Event += LookAtSubjectLeftSide_HotKey;
            _engine.Events.HotKeys[Key.Right, ModifierKey.Control].Event += LookAtSubjectRightSide_HotKey;
            _engine.Events.HotKeys[Key.Up, ModifierKey.Control].Event += LookAtSubjectTopSide_HotKey;
            _engine.Events.HotKeys[Key.Down, ModifierKey.Control].Event += LookAtSubjectBottomSide_HotKey;
            _engine.Events.HotKeys[Key.Up, ModifierKey.Control | ModifierKey.Shift].Event += LookAtSubjectFrontSide_HotKey;
            _engine.Events.HotKeys[Key.Down, ModifierKey.Control | ModifierKey.Shift].Event += LookAtSubjectBackSide_HotKey;
            _engine.Events.KeyBindings[SetActiveSubjectToOne].Bind(Key.Alpha1, ModifierKey.Control);
            _engine.Events.KeyBindings[SetActiveSubjectToTwo].Bind(Key.Alpha2, ModifierKey.Control);
            _engine.Events.KeyBindings[SetActiveSubjectToThree].Bind(Key.Alpha3, ModifierKey.Control);
            _engine.Events.KeyBindings[SetActiveSubjectToFour].Bind(Key.Alpha4, ModifierKey.Control);
            _engine.Events.KeyBindings[SetActiveSubjectToFive].Bind(Key.Alpha5, ModifierKey.Control);
            _engine.Events.KeyBindings[SetActiveSubjectToSix].Bind(Key.Alpha6, ModifierKey.Control);
            _engine.Events.KeyBindings[SetActiveSubjectToSeven].Bind(Key.Alpha7, ModifierKey.Control);
            _engine.Events.KeyBindings[SetActiveSubjectToEight].Bind(Key.Alpha8, ModifierKey.Control);
            _engine.Events.KeyBindings[SetActiveSubjectToNine].Bind(Key.Alpha9, ModifierKey.Control);
            _engine.Events.KeyBindings[Delete_Entity].Bind(Key.Delete);
            _engine.Events.KeyBindings[ToggleVisible].Bind(Key.V, ModifierKey.Control);

            #endregion

            return true;
        }

        /// <summary>
        /// Handles graphics rendering.
        /// </summary>
        internal unsafe void Render()
        {
            foreach (var window in _engine.Windows)
            {
                // Update the current window's rendering context
                window.RenderingContext.Use();

                // Clear the screen to a dark gray color and clear the depth buffer
                glClearColor(0.2f, 0.2f, 0.2f, 1);
                glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

                // Render the scene
                if (window.Viewport.CameraManager.ActiveController.ActiveCamera is not null)
                {
                    _engine.Scenes.CurrentScene.Render(window.Viewport.CameraManager.ActiveController.ActiveCamera, window, wireframe);
                }
            }
        }

        private void PerformTest_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            using (Log.Logger.BeginPerfLogger("Perfoming test"))
            {
                // Perform tests here
            }
        }

        internal void Terminate()
        {
            Log.Information("Terminating GLFW.");
            Glfw.Terminate();
        }

        private void Events_GlfwError(object? sender, Events.GlfwErrorEventArgs e) =>
            Log.Error($"GLFW error detected: {e.ErrorCode} - {e.Message}");

        #region Debugging hotkeys

        // TODO: Remove these hotkeys when they are no longer needed
        //       or move them to a concrete implementation of the engine

        private void ToggleVisible() =>
            _engine.Scenes.CurrentScene[activeSubjectIndex].Visible = !_engine.Scenes.CurrentScene[activeSubjectIndex].Visible;

        private void Delete_Entity()
        {
            try
            {
                _engine.Scenes.CurrentScene.Remove(_engine.Scenes.CurrentScene[activeSubjectIndex]);
            }
            catch (System.Exception) { }
        }

        private void SetActiveSubjectToOne() => activeSubjectIndex = 0;
        private void SetActiveSubjectToTwo() => activeSubjectIndex = 1;
        private void SetActiveSubjectToThree() => activeSubjectIndex = 2;
        private void SetActiveSubjectToFour() => activeSubjectIndex = 3;
        private void SetActiveSubjectToFive() => activeSubjectIndex = 4;
        private void SetActiveSubjectToSix() => activeSubjectIndex = 5;
        private void SetActiveSubjectToSeven() => activeSubjectIndex = 6;
        private void SetActiveSubjectToEight() => activeSubjectIndex = 7;
        private void SetActiveSubjectToNine() => activeSubjectIndex = 8;

        private void ToggleWireframe_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            wireframe = !wireframe;

        private void ToggleCulling_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            if (!cullingEnabled)
            {
                glEnable(GL_CULL_FACE);
                glCullFace(GL_BACK);
                glFrontFace(GL_CCW);
            }
            else
            {
                glDisable(GL_CULL_FACE);
            }
            cullingEnabled = !cullingEnabled;
        }

        private void SetShaderToDefault_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Shader = _engine.Assets.Get<Shader>("Default")!;

        private void SetShaderToFaceA_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Shader = _engine.Assets.Get<Shader>("Face")!;

        private void SetShaderToFaceB_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Shader = _engine.Assets.Get<Shader>("Face2")!;

        private void SetShaderToNormal_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Shader = _engine.Assets.Get<Shader>("Normal")!;

        private void Subdivide_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Mesh;
            mesh.Subdivide();
            _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void TileTexture_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Mesh;
            mesh.TileTexture(3, 3);
            _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void Roughen_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            using (Log.Logger.BeginPerfLogger("Roughen"))
            {
                var mesh = _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Mesh;
                mesh.Roughen(0.1f);
                _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Mesh = mesh;
            }
        }

        private void Explode_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Mesh;
            mesh.Explode(0.15f);
            _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void ToggleTexture_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            blackTexture = !blackTexture;
            _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Texture = blackTexture ?
                _engine.Assets.Get<Texture>("BlackPentaTexture") :
                _engine.Assets.Get<Texture>("WhitePentaTexture");
        }

        private void CreateCube_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = MeshFactory.CreateCube(1f);
            _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void CreateSphere_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = MeshFactory.CreateSphere(1f);
            _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void CreateCylinder_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = MeshFactory.CreateCylinder(0.5f, 1f);
            _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void CreatePyramid_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = MeshFactory.CreatePyramid(1f, 1f, 1f);
            _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void CreateCone_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = MeshFactory.CreateCone(0.5f, 1f);
            _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void CreatePlane_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = MeshFactory.CreatePlane(1f, 1f, new Rotation(0, -90, 0));
            _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void OrbitLight_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            if (_engine.Windows[0].Viewport.CameraManager.ActiveController is EditorCameraController cameraController)
            {
                cameraController.SetOrbitTarget(_engine.Scenes.CurrentScene[activeSubjectIndex], 1.5f); // Orbit around the light
            }
        }

        private void GenerateLandscape_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            using (Log.Logger.BeginPerfLogger("Generating landscape"))
            {
                if (_engine.Windows[0].Viewport.CameraManager.ActiveController is EditorCameraController controller)
                {
                    controller.SetPosition(new Vector3(0f, 5f, 3f));
                    controller.SetOrbitTarget(new Vector3(0f, 0f, 0f));
                }
                var mesh = MeshFactory.CreatePlane(40f, 40f);
                mesh.Offset(0f, -2f, 0f);
                mesh.Subdivide();
                mesh.Roughen(2f);
                mesh.Subdivide();
                mesh.Roughen(4f);
                mesh.Subdivide(2);
                mesh.Roughen(0.3f);
                mesh.Subdivide(2);
                mesh.Roughen(0.1f);

                if (_engine.Assets["Landscape"] is RenderableMeshEntity landscape)
                {
                    landscape.Components.Get<MeshRenderComponent>()!.Mesh = mesh;
                }
                else
                {
                    _engine.Assets.Add("Landscape", new RenderableMeshEntity(
                        mesh,
                        _engine.Assets.Get<Shader>("Default")!,
                        _engine.Assets.Get<Texture>("BlackPentaTexture")));
                    _engine.Scenes.CurrentScene.Add((RenderableMeshEntity)_engine.Assets["Landscape"]!);
                }
            }
        }

        private void UpdateRotation(float yaw, float pitch, float roll)
        {
            var angle = 15f;
            var component = _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<TransformComponent>()!;
            var transform = component.Transform;
            transform.Rotation += new Rotation(yaw, pitch, roll) * angle;
            component.Transform = transform;
            Log.Information($"Object rotated: {transform.Rotation}");
        }

        private void UpdateLookAt(Vector3 vector)
        {
            var component = _engine.Scenes.CurrentScene[activeSubjectIndex].Components.Get<TransformComponent>()!;
            var transform = component.Transform;
            transform.Rotation = Rotation.GetLookAt(vector * 5, transform.Position);
            component.Transform = transform;
            Log.Information($"Object rotated to look at side: {transform.Rotation}");
        }

        private void YawSubjectLeft_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            UpdateRotation(-1, 0, 0);

        private void YawSubjectRight_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            UpdateRotation(1, 0, 0);

        private void PitchSubjectUp_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            UpdateRotation(0, 1, 0);

        private void PitchSubjectDown_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            UpdateRotation(0, -1, 0);

        private void LookAtSubjectLeftSide_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            UpdateLookAt(World.LeftVector);

        private void LookAtSubjectRightSide_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            UpdateLookAt(World.RightVector);

        private void LookAtSubjectTopSide_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            UpdateLookAt(World.UpVector);

        private void LookAtSubjectBottomSide_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            UpdateLookAt(World.DownVector);

        private void LookAtSubjectFrontSide_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            UpdateLookAt(World.BackwardVector);

        private void LookAtSubjectBackSide_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            UpdateLookAt(World.ForwardVector);

        #endregion
    }
}