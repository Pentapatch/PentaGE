using GLFW;
using PentaGE.Common;
using PentaGE.Core.Components;
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

            // Add engine events for moving the camera
            // TODO: Only for debugging - remove later
            _engine.Events.KeyDown += Events_KeyDown;

            return true;
        }

        /// <summary>
        /// Handles graphics rendering.
        /// </summary>
        internal unsafe void Render()
        {
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

        private void Events_KeyDown(object? sender, Events.KeyDownEventArgs e)
        {
            // TODO: This code does not belong here, but is here for testing purposes
            //       This code is actually specific to a concrete implementation of an editor
            if (e.Key == Key.Left)
            {
                if (e.ModifierKeys == ModifierKey.None)
                {
                    objectDisplayTransform.Rotation += new Rotation(-1, 0, 0) * 5;
                    Log.Information($"Object yaw left: {objectDisplayTransform.Rotation}");
                }
                else if (e.ModifierKeys == ModifierKey.Control)
                {
                    objectDisplayTransform.Rotation = Rotation.GetLookAt(World.LeftVector * 5, objectDisplayTransform.Position);
                }
            }
            else if (e.Key == Key.Right)
            {
                if (e.ModifierKeys == ModifierKey.None)
                {
                    objectDisplayTransform.Rotation += new Rotation(1, 0, 0) * 5;
                    Log.Information($"Object yaw right: {objectDisplayTransform.Rotation}");
                }
                else if (e.ModifierKeys == ModifierKey.Control)
                {
                    objectDisplayTransform.Rotation = Rotation.GetLookAt(World.RightVector * 5, objectDisplayTransform.Position);
                }
            }
            else if (e.Key == Key.Up)
            {
                if (e.ModifierKeys == ModifierKey.None)
                {
                    objectDisplayTransform.Rotation += new Rotation(0, 1, 0) * 5;
                    Log.Information($"Object pitch up: {objectDisplayTransform.Rotation}");
                }
                else if (e.ModifierKeys == ModifierKey.Control)
                {
                    objectDisplayTransform.Rotation = Rotation.GetLookAt(World.UpVector * 5, objectDisplayTransform.Position);
                }
                else if (e.ModifierKeys == (ModifierKey.Control | ModifierKey.Shift))
                {
                    objectDisplayTransform.Rotation = Rotation.GetLookAt(World.ForwardVector * 5, objectDisplayTransform.Position);
                }
            }
            else if (e.Key == Key.Down)
            {
                if (e.ModifierKeys == ModifierKey.None)
                {
                    objectDisplayTransform.Rotation += new Rotation(0, -1, 0) * 5;
                    Log.Information($"Object pitch down: {objectDisplayTransform.Rotation}");
                }
                else if (e.ModifierKeys == ModifierKey.Control)
                {
                    objectDisplayTransform.Rotation = Rotation.GetLookAt(World.DownVector * 5, objectDisplayTransform.Position);
                }
                else if (e.ModifierKeys == (ModifierKey.Control | ModifierKey.Shift))
                {
                    objectDisplayTransform.Rotation = Rotation.GetLookAt(World.BackwardVector * 5, objectDisplayTransform.Position);
                }
            }
            else if (e.Key == Key.F1)
            {
                wireframe = !wireframe;
            }
            else if (e.Key == Key.F2)
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
            else if (e.Key == Key.F3)
            {
                materialTest = !materialTest;
            }
            else if (e.Key == Key.F5)
            {
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Shader = _engine.Assets.Get<Shader>("Default")!;
            }
            else if (e.Key == Key.F6)
            {
                if (e.ModifierKeys == ModifierKey.Shift)
                    _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Shader = _engine.Assets.Get<Shader>("Face2")!;
                else
                    _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Shader = _engine.Assets.Get<Shader>("Face")!;
            }
            else if (e.Key == Key.F7)
            {
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Shader = _engine.Assets.Get<Shader>("Normal")!;
            }
            else if (e.Key == Key.F10)
            {
                var mesh = _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Mesh;
                mesh.Subdivide();
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Mesh = mesh;
            }
            else if (e.Key == Key.F11)
            {
                var mesh = _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Mesh;
                mesh.TileTexture(3, 3);
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Mesh = mesh;
            }
            else if (e.Key == Key.Backspace && e.ModifierKeys == ModifierKey.Control)
            {
                if (_engine.Windows[0].Viewport.CameraManager.ActiveController is EditorCameraController cameraController)
                {
                    cameraController.SetOrbitTarget(_engine.Scene[1], 1.5f); // Orbit around the light
                }
            }
            else if (e.Key == Key.Alpha1)
            {
                var mesh = MeshFactory.CreateCube(1f);
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Mesh = mesh;
            }
            else if (e.Key == Key.Alpha2)
            {
                var mesh = MeshFactory.CreateSphere(1f);
                //testMesh1.TileTexture(4, 4);
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Mesh = mesh;
            }
            else if (e.Key == Key.Alpha3)
            {
                var mesh = MeshFactory.CreateCylinder(0.5f, 1f);
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Mesh = mesh;
            }
            else if (e.Key == Key.Alpha4)
            {
                var mesh = MeshFactory.CreatePyramid(1f, 1f, 1f);
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Mesh = mesh;
            }
            else if (e.Key == Key.Alpha5)
            {
                var mesh = MeshFactory.CreateCone(0.5f, 1f);
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Mesh = mesh;
            }
            else if (e.Key == Key.Alpha6)
            {
                var mesh = MeshFactory.CreatePlane(1f, 1f, new Rotation(0, -90, 0));
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Mesh = mesh;
            }
            else if (e.Key == Key.R && e.ModifierKeys == ModifierKey.Control)
            {
                rotate = !rotate;
            }
            else if (e.Key == Key.I)
            {
                blackTexture = !blackTexture;
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Texture = blackTexture ?
                    _engine.Assets.Get<Texture>("BlackPentaTexture") :
                    _engine.Assets.Get<Texture>("WhitePentaTexture");
            }
            else if (e.Key == Key.F12)
            {
                if (e.ModifierKeys == ModifierKey.Control)
                {
                    var mesh = _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Mesh;
                    mesh.Explode(0.15f);
                    _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Mesh = mesh;
                }
                else
                {
                    using (Log.Logger.BeginPerfLogger("Roughen"))
                    {
                        var mesh = _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Mesh;
                        mesh.Roughen(0.1f);
                        _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Mesh = mesh;
                    }
                }
            }
            else if (e.Key == Key.L)
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
            else if (e.Key == Key.T)
            {
                // Perform tests here
            }
        }
    }
}