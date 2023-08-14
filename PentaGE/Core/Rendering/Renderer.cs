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
        private bool rotate = true;
        private bool materialTest = false;
        private bool wireframe = false;
        private bool blackTexture = true;
        private RenderableMeshEntity activeSubject;

        /// <summary>
        /// Creates a new instance of the Renderer class.
        /// </summary>
        /// <param name="engine">The PentaGameEngine instance associated with this Renderer.</param>
        internal Renderer(PentaGameEngine engine)
        {
            _engine = engine;
        }

        // Set up the rendered test objects transform
        private Transform objectTransform = new()
        {
            Position = new(0, 0, 0),    // in units
            Rotation = new(0, 0, 0),    // in degrees
            Scale = new(1, 1, 1),       // multipliers
        };

        private Transform objectDisplayTransform = new()
        {
            Position = new(0, 0, 0),    // in units
            Rotation = new(0, 0, 0),    // in degrees
            Scale = new(1, 1, 1),       // multipliers
        };

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
            _engine.Events.HotKeys[Key.F3].Event += ToggleMaterialTest_HotKey;
            _engine.Events.HotKeys[Key.F5].Event += SetShaderToDefault_HotKey;
            _engine.Events.HotKeys[Key.F6].Event += SetShaderToFaceA_HotKey;
            _engine.Events.HotKeys[Key.F6, ModifierKey.Shift].Event += SetShaderToFaceB_HotKey;
            _engine.Events.HotKeys[Key.F7].Event += SetShaderToNormal_HotKey;
            _engine.Events.HotKeys[Key.F10].Event += Subdivide_HotKey;
            _engine.Events.HotKeys[Key.F11].Event += TileTexture_HotKey;
            _engine.Events.HotKeys[Key.F12].Event += Roughen_HotKey;
            _engine.Events.HotKeys[Key.F12, ModifierKey.Control].Event += Explode_HotKey;
            _engine.Events.HotKeys[Key.R, ModifierKey.Control].Event += ToggleRotation_HotKey;
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

            #endregion

            return true;
        }

        /// <summary>
        /// Handles graphics rendering.
        /// </summary>
        internal unsafe void Render()
        {
            activeSubject ??= _engine.Assets.Get<RenderableMeshEntity>("Subject")!;

            foreach (var window in _engine.Windows)
            {
                #region Test rotation

                // Rotate the object based on the current time
                objectTransform.Rotation = new(
                    MathF.Sin((float)_engine.Timing.TotalElapsedTime) * 180,
                    0,
                    0);

                // Animate the color and specular strength of the object
                _engine.Scene[0].GetComponent<TransformComponent>()!.Transform = rotate ? objectTransform : objectDisplayTransform;

                float hue = MathF.Sin((float)_engine.Timing.TotalElapsedTime) * 0.5f + 0.5f; // 0 - 1
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Material.Albedo =
                    materialTest ?
                        ColorFromHSL(hue, 0.8f, 0.85f) :
                        new(1, 1, 1);
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Material.SpecularStrength =
                    materialTest ?
                        MathF.Sin(((float)_engine.Timing.TotalElapsedTime) + 1) / 2 :
                        1;

                #endregion

                // Update the current window's rendering context
                window.RenderingContext.Use();

                // Clear the screen to a dark gray color and clear the depth buffer
                glClearColor(0.2f, 0.2f, 0.2f, 1);
                glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

                // Draw the test mesh
                if (window.Viewport.CameraManager.ActiveController.ActiveCamera is not null)
                    _engine.Scene.Render(window.Viewport.CameraManager.ActiveController.ActiveCamera, window, wireframe);
            }
        }

        private void PerformTest_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            using (Log.Logger.BeginPerfLogger("Perfoming test"))
            {
                // Perform tests here
                activeSubject = _engine.Assets.Get<RenderableMeshEntity>("LightEntity")!;
            }
        }

        internal void Terminate()
        {
            Log.Information("Terminating GLFW.");
            Glfw.Terminate();
        }

        private void Events_GlfwError(object? sender, Events.GlfwErrorEventArgs e) =>
            Log.Error($"GLFW error detected: {e.ErrorCode} - {e.Message}");

        private static Vector3 ColorFromHSL(float hue, float saturation, float lightness)
        {
            // TODO: This code does not belong here, but is here for testing purposes
            // Convert HSL to RGB
            // Only for visuallisational purposes
            // If i desire a method like this i should create a proper color management system
            if (saturation == 0f)
            {
                return new Vector3(lightness, lightness, lightness);
            }
            else
            {
                float q = lightness < 0.5f ? lightness * (1 + saturation) : lightness + saturation - lightness * saturation;
                float p = 2 * lightness - q;
                float[] rgb = new float[3];
                rgb[0] = hue + 1f / 3f;
                rgb[1] = hue;
                rgb[2] = hue - 1f / 3f;
                for (int i = 0; i < 3; i++)
                {
                    if (rgb[i] < 0f) rgb[i]++;
                    if (rgb[i] > 1f) rgb[i]--;
                    if (6f * rgb[i] < 1f)
                    {
                        rgb[i] = p + ((q - p) * 6f * rgb[i]);
                    }
                    else if (2f * rgb[i] < 1f)
                    {
                        rgb[i] = q;
                    }
                    else if (3f * rgb[i] < 2f)
                    {
                        rgb[i] = p + ((q - p) * 6f * ((2f / 3f) - rgb[i]));
                    }
                    else
                    {
                        rgb[i] = p;
                    }
                }
                return new Vector3(rgb[0], rgb[1], rgb[2]);
            }
        }

        #region Debugging hotkeys

        // TODO: Remove these hotkeys when they are no longer needed
        //       or move them to a concrete implementation of the engine

        private void ToggleWireframe_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            wireframe = !wireframe;

        private void ToggleMaterialTest_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            materialTest = !materialTest;

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
            activeSubject.GetComponent<MeshRenderComponent>()!.Shader = _engine.Assets.Get<Shader>("Default")!;

        private void SetShaderToFaceA_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            activeSubject.GetComponent<MeshRenderComponent>()!.Shader = _engine.Assets.Get<Shader>("Face")!;

        private void SetShaderToFaceB_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            activeSubject.GetComponent<MeshRenderComponent>()!.Shader = _engine.Assets.Get<Shader>("Face2")!;

        private void SetShaderToNormal_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            activeSubject.GetComponent<MeshRenderComponent>()!.Shader = _engine.Assets.Get<Shader>("Normal")!;

        private void Subdivide_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = activeSubject.GetComponent<MeshRenderComponent>()!.Mesh;
            mesh.Subdivide();
            activeSubject.GetComponent<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void TileTexture_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = activeSubject.GetComponent<MeshRenderComponent>()!.Mesh;
            mesh.TileTexture(3, 3);
            activeSubject.GetComponent<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void Roughen_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            using (Log.Logger.BeginPerfLogger("Roughen"))
            {
                var mesh = activeSubject.GetComponent<MeshRenderComponent>()!.Mesh;
                mesh.Roughen(0.1f);
                activeSubject.GetComponent<MeshRenderComponent>()!.Mesh = mesh;
            }
        }

        private void Explode_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = activeSubject.GetComponent<MeshRenderComponent>()!.Mesh;
            mesh.Explode(0.15f);
            activeSubject.GetComponent<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void ToggleRotation_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            rotate = !rotate;

        private void ToggleTexture_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            blackTexture = !blackTexture;
            activeSubject.GetComponent<MeshRenderComponent>()!.Texture = blackTexture ?
                _engine.Assets.Get<Texture>("BlackPentaTexture") :
                _engine.Assets.Get<Texture>("WhitePentaTexture");
        }

        private void CreateCube_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = MeshFactory.CreateCube(1f);
            activeSubject.GetComponent<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void CreateSphere_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = MeshFactory.CreateSphere(1f);
            activeSubject.GetComponent<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void CreateCylinder_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = MeshFactory.CreateCylinder(0.5f, 1f);
            activeSubject.GetComponent<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void CreatePyramid_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = MeshFactory.CreatePyramid(1f, 1f, 1f);
            activeSubject.GetComponent<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void CreateCone_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = MeshFactory.CreateCone(0.5f, 1f);
            activeSubject.GetComponent<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void CreatePlane_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            var mesh = MeshFactory.CreatePlane(1f, 1f, new Rotation(0, -90, 0));
            activeSubject.GetComponent<MeshRenderComponent>()!.Mesh = mesh;
        }

        private void OrbitLight_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            if (_engine.Windows[0].Viewport.CameraManager.ActiveController is EditorCameraController cameraController)
            {
                cameraController.SetOrbitTarget(_engine.Scene[1], 1.5f); // Orbit around the light
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
                var mesh = MeshFactory.CreatePlane(20f, 20f);
                mesh.Offset(0f, -2f, 0f);
                mesh.Subdivide();
                mesh.Roughen(4f);
                mesh.Subdivide(2);
                mesh.Roughen(0.3f);
                mesh.Subdivide(2);
                mesh.Roughen(0.1f);
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Mesh = mesh;
                rotate = false;
            }
        }

        private void YawSubjectLeft_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            objectDisplayTransform.Rotation += new Rotation(-1, 0, 0) * 5;
            Log.Information($"Object yaw left: {objectDisplayTransform.Rotation}");
        }

        private void YawSubjectRight_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            objectDisplayTransform.Rotation += new Rotation(1, 0, 0) * 5;
            Log.Information($"Object yaw right: {objectDisplayTransform.Rotation}");
        }

        private void PitchSubjectUp_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            objectDisplayTransform.Rotation += new Rotation(0, 1, 0) * 5;
            Log.Information($"Object pitch up: {objectDisplayTransform.Rotation}");
        }

        private void PitchSubjectDown_HotKey(object? sender, Events.HotKeyEventArgs e)
        {
            objectDisplayTransform.Rotation += new Rotation(0, -1, 0) * 5;
            Log.Information($"Object pitch down: {objectDisplayTransform.Rotation}");
        }

        private void LookAtSubjectLeftSide_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            objectDisplayTransform.Rotation = Rotation.GetLookAt(World.LeftVector * 5, objectDisplayTransform.Position);

        private void LookAtSubjectRightSide_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            objectDisplayTransform.Rotation = Rotation.GetLookAt(World.RightVector * 5, objectDisplayTransform.Position);

        private void LookAtSubjectTopSide_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            objectDisplayTransform.Rotation = Rotation.GetLookAt(World.UpVector * 5, objectDisplayTransform.Position);

        private void LookAtSubjectBottomSide_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            objectDisplayTransform.Rotation = Rotation.GetLookAt(World.DownVector * 5, objectDisplayTransform.Position);

        private void LookAtSubjectFrontSide_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            objectDisplayTransform.Rotation = Rotation.GetLookAt(World.BackwardVector * 5, objectDisplayTransform.Position);

        private void LookAtSubjectBackSide_HotKey(object? sender, Events.HotKeyEventArgs e) =>
            objectDisplayTransform.Rotation = Rotation.GetLookAt(World.ForwardVector * 5, objectDisplayTransform.Position);

#endregion
    }
}