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
        private Shader shader;
        private Shader faceShader;
        private Shader lightShader;
        private Shader gridShader;
        private Texture texture;
        private Mesh testMesh1;
        private Mesh lightMesh1;
        private bool rotate = true;
        private bool materialTest = true;
        private bool wireframe = false;
        private readonly Grid gridA = new(10, 10, new(1, 1, 1), 0.2f);
        private readonly Grid gridB = new(10, 20, new(0, 0, 0), 0.15f);

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

            #region Set up a test object to render
            // TODO: None of these should be here, it's just for testing

            // Initializing test shaders
            using (var logger = Log.Logger.BeginPerfLogger("Loading default shader"))
            {
                try
                {
                    shader = new(@"C:\Users\newsi\source\repos\PentaGE\PentaGE\Core\Rendering\Shaders\SourceCode\Default.shader");
                    shader.Load();
                }
                catch (System.Exception ex)
                {
                    Log.Error($"Error loading shader: {ex}");
                }
            }

            using (var logger = Log.Logger.BeginPerfLogger("Loading face shader"))
            {
                try
                {
                    faceShader = new(@"C:\Users\newsi\source\repos\PentaGE\PentaGE\Core\Rendering\Shaders\SourceCode\Face.shader");
                    faceShader.Load();
                }
                catch (System.Exception ex)
                {
                    Log.Error($"Error loading shader: {ex}");
                }
            }

            using (var logger = Log.Logger.BeginPerfLogger("Loading light shader"))
            {
                try
                {
                    lightShader = new(@"C:\Users\newsi\source\repos\PentaGE\PentaGE\Core\Rendering\Shaders\SourceCode\Light.shader");
                    lightShader.Load();
                }
                catch (System.Exception ex)
                {
                    Log.Error($"Error loading shader: {ex}");
                }
            }

            using (var logger = Log.Logger.BeginPerfLogger("Loading grid shader"))
            {
                try
                {
                    gridShader = new(@"C:\Users\newsi\source\repos\PentaGE\PentaGE\Core\Rendering\Shaders\SourceCode\Grid.shader");
                    gridShader.Load();
                }
                catch (System.Exception ex)
                {
                    Log.Error($"Error loading shader: {ex}");
                }
            }

            // Initialize test texture
            using (var logger = Log.Logger.BeginPerfLogger("Loading test texture"))
            {
                try
                {
                    texture = new(
                    @"C:\Users\newsi\source\repos\PentaGE\PentaGE\Core\Rendering\Textures\SourceFiles\TestTexture.jpg",
                    GL_TEXTURE_2D,
                    GL_TEXTURE0,
                    GL_RGBA,
                    GL_UNSIGNED_BYTE);
                    Texture.SetTextureSlot(shader, "tex0", 0); // Set the texture slot to 0
                }
                catch { /* Gets logged in the constructor */ }
            }

            // Initialize test mesh
            testMesh1 = MeshFactory.CreatePyramid(1f, 0.6f, 1f);
            //testMesh1 = MeshFactory.CreatePlane(10f, new(0, -90f, 0));
            testMesh1.TileTexture(5, 6);
            testMesh1.Subdivide(1);
            //testMesh1.Offset(0, 0.25f, 0);
            //testMesh1.Rotate(45, 0, 0);
            var transform = new Transform(new(0, 0, 0), new(0, 0, 0), new(1f, 1f, 1f));
            var renderableMesh = new RenderableMeshEntity(testMesh1, shader, texture);

            renderableMesh.AddComponent(new TransformComponent(transform));
            renderableMesh.GetComponent<MeshRenderComponent>()!.Material.Albedo = new(1f, 0f, 1f);
            renderableMesh.GetComponent<MeshRenderComponent>()!.Material.SpecularStrength = 1f;

            // Initialize test light
            lightMesh1 = MeshFactory.CreateSphere(0.2f);
            var transform2 = new Transform(new(0.75f, 0.75f, 0.75f), new(0, 0, 0), new(1f, 1f, 1f));
            var renderableLight = new RenderableMeshEntity(lightMesh1, lightShader);

            renderableLight.AddComponent(new TransformComponent(transform2));

            // Initialize grid
            //gridB.Mesh.Offset(0, -0.25f, 0);
            var renderableGridMajor = new RenderableGridEntity(gridA, gridShader);
            var renderableGridMinor = new RenderableGridEntity(gridB, gridShader);

            _engine.Scene.AddEntity(renderableMesh);
            _engine.Scene.AddEntity(renderableLight);
            _engine.Scene.AddEntity(renderableGridMajor);
            _engine.Scene.AddEntity(renderableGridMinor);

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
                #region Test rotation

                // Rotate the object based on the current time
                //objectTransform.Rotation = rotate ? new(
                //    MathF.Sin((float)_engine.Timing.TotalElapsedTime) * 90,
                //    MathF.Cos((float)_engine.Timing.TotalElapsedTime) * 90,
                //    0) :
                //    new(0, 0, 0);
                objectTransform.Rotation = rotate ? new(
                    MathF.Sin((float)_engine.Timing.TotalElapsedTime) * 180,
                    0,
                    0) :
                    new(0, 0, 0); // objectTransform.Rotation;

                // Animate the color and specular strength of the object
                _engine.Scene[0].GetComponent<TransformComponent>()!.Transform = objectTransform;
                float hue = MathF.Sin((float)_engine.Timing.TotalElapsedTime) * 0.5f + 0.5f; // Adjust the range to [0, 1]
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Material.Albedo = 
                    materialTest ? 
                    ColorFromHSL(hue, 1.0f, 0.5f) :
                    new(1, 1, 1);
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Material.SpecularStrength = 
                    materialTest ? 
                    (MathF.Sin((float)_engine.Timing.TotalElapsedTime) + 1) / 2 * 2 :
                    2;

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

        }

        private void Events_GlfwError(object? sender, Events.GlfwErrorEventArgs e)
        {
            Log.Error($"GLFW Error: {e.ErrorCode} - {e.Message}");
        }

        // This code does not belong here, but is here for testing purposes
        private static Vector3 ColorFromHSL(float hue, float saturation, float lightness)
        {
            // Convert HSL to RGB
            // Only for visuallisational purposes
            // TODO: Remove this and use the actual color values
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

        // TODO: This code does not belong here, but is here for testing purposes
        private void Events_KeyDown(object? sender, Events.KeyDownEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                objectTransform.Rotation += new Rotation(-1, 0, 0) * 5;
                Log.Information($"Object yaw left: {objectTransform.Rotation}");
            }
            else if (e.Key == Key.Right)
            {
                objectTransform.Rotation += new Rotation(1, 0, 0) * 5;
                Log.Information($"Object yaw right: {objectTransform.Rotation}");
            }
            else if (e.Key == Key.Up)
            {
                objectTransform.Rotation += new Rotation(0, 1, 0) * 5;
                Log.Information($"Object pitch up: {objectTransform.Rotation}");
            }
            else if (e.Key == Key.Down)
            {
                objectTransform.Rotation += new Rotation(0, -1, 0) * 5;
                Log.Information($"Object pitch down: {objectTransform.Rotation}");
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
                rotate = !rotate;
            }
            else if (e.Key == Key.F4)
            {
                materialTest = !materialTest;
            }
            else if (e.Key == Key.F5)
            {
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Shader = shader;
            }
            else if (e.Key == Key.F6)
            {
                _engine.Scene[0].GetComponent<MeshRenderComponent>()!.Shader = faceShader;
            }
        }

    }
}