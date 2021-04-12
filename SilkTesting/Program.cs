using System;
using System.Linq;
using System.Numerics;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace SilkTesting
{
    internal class Program
    {
        private const int Width = 800;
        private const int Height = 600;
        private static IWindow window;

        private static GL Gl;

        private static IKeyboard primaryKeyboard;

        private static BufferObject<float> Vbo;
        private static BufferObject<uint> Ebo;
        private static VertexArrayObject<float, uint> VaoCube;
        private static Texture texture;
        private static Shader LampShader;
        private static Shader TextureShader;
        private static readonly Vector3 DefaultLampPosition = new(1.2f, 1.0f, 2.0f);
        private static Vector3 LampPosition = new(1.2f, 1.0f, 2.0f);

        private static Camera Camera;

        private static DateTime StartTime;

        private static GraphicalObject cube;

        //Used to track change in mouse movement to allow for moving of the Camera
        private static Vector2 LastMousePosition;

        private static (float[], uint[]) GenCube()
        {
            uint[] indices =
            {
                0, 1, 3,
                1, 2, 3
            };

            float[] vertices =
            {
                //X    Y      Z          Normals            U   V
                -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,
                0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f,
                0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f,
                0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f,
                -0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,

                -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
                0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f,
                0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,
                0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,
                -0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
                -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,

                -0.5f, 0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
                -0.5f, 0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
                -0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
                -0.5f, -0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                -0.5f, 0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f,

                0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
                0.5f, 0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f,
                0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
                0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
                0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,

                -0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f,
                0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 1.0f,
                0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f,
                0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f,
                -0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f,
                -0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f,

                -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
                0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f,
                0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
                0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
                -0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f,
                -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f
            };

            return (vertices, indices);
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Creating Window");
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(Width, Height);
            options.Title = "Hello Window";
            //options.TransparentFramebuffer = true;
            options.WindowBorder = WindowBorder.Fixed;

            window = Window.Create(options);

            Console.WriteLine("Adding event hooks");
            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.Closing += OnClose;


            Console.WriteLine("Running");
            window.Run();
            Console.WriteLine("Done running");
        }

        private static void OnLoad()
        {
            StartTime = DateTime.UtcNow;
            var input = window.CreateInput();
            primaryKeyboard = input.Keyboards.FirstOrDefault();
            if (primaryKeyboard != null)
            {
                primaryKeyboard.KeyDown += KeyDown;
            }

            foreach (var t in input.Mice)
            {
                t.Cursor.CursorMode = CursorMode.Raw;
                t.MouseMove += OnMouseMove;
                t.Scroll += OnMouseWheel;
            }

            Gl = GL.GetApi(window);

            var (vertices, indices) = GenCube();

            Ebo = new BufferObject<uint>(Gl, indices, BufferTargetARB.ElementArrayBuffer);
            Vbo = new BufferObject<float>(Gl, vertices, BufferTargetARB.ArrayBuffer);
            VaoCube = new VertexArrayObject<float, uint>(Gl, Vbo, Ebo);

            VaoCube.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
            VaoCube.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
            VaoCube.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);

            //The Lamp shader uses a fragment shader that just colours it solid white so that we know it is the light source
            LampShader = new Shader(Gl, @"C:\Users\tobyl\RiderProjects\SilkTesting\SilkTesting\shader.vert",
                @"C:\Users\tobyl\RiderProjects\SilkTesting\SilkTesting\shader.frag");

            TextureShader = new Shader(Gl, @"C:\Users\tobyl\RiderProjects\SilkTesting\SilkTesting\transformShader.vert",
                @"C:\Users\tobyl\RiderProjects\SilkTesting\SilkTesting\lightingTextureShader.frag");
            
            cube = new GraphicalObject(@"C:\Users\tobyl\RiderProjects\SilkTesting\SilkTesting\Monke.obj",@"C:\Users\tobyl\RiderProjects\SilkTesting\SilkTesting\Cube.mtl",Gl,TextureShader);

            //Start a camera at position 3 on the Z axis, looking at position -1 on the Z axis
            Camera = new Camera(Vector3.UnitZ * 6, Vector3.UnitZ * -1, Vector3.UnitY, Width / Height);

            texture = new Texture(Gl, @"C:\Users\tobyl\RiderProjects\SilkTesting\SilkTesting\monke.png");
        }

        private static void OnUpdate(double deltaTime)
        {
            var moveSpeed = 2.5f * (float) deltaTime;

            if (primaryKeyboard.IsKeyPressed(Key.W))
            {
                Camera.Position += moveSpeed * Camera.Front;
            }

            if (primaryKeyboard.IsKeyPressed(Key.S))
            {
                Camera.Position -= moveSpeed * Camera.Front;
            }

            if (primaryKeyboard.IsKeyPressed(Key.A))
            {
                Camera.Position -= Vector3.Normalize(Vector3.Cross(Camera.Front, Camera.Up)) * moveSpeed;
            }

            if (primaryKeyboard.IsKeyPressed(Key.D))
            {
                Camera.Position += Vector3.Normalize(Vector3.Cross(Camera.Front, Camera.Up)) * moveSpeed;
            }

            if (primaryKeyboard.IsKeyPressed(Key.Space))
            {
                Camera.Position += moveSpeed * Camera.Up;
            }

            if (primaryKeyboard.IsKeyPressed(Key.ShiftLeft))
            {
                Camera.Position -= moveSpeed * Camera.Up;
            }

            var rotation =  Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(((float)window.Time * 20f) % 360.0f));
            var translationMatrix = Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(rotation);

            LampPosition = Vector3.Transform(DefaultLampPosition, translationMatrix);
        }

        private static void OnRender(double dt)
        {
            Gl.Enable(EnableCap.DepthTest);
            Gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
            
            texture.Bind();

            RenderMyLitCube();
            //RenderLitCube();
            
            VaoCube.Bind();

            RenderLampCube();
        }

        private static void RenderMyLitCube()
        {
            cube.Use();
            //cube.associatedShader.SetUniform("uTexture0", 0);
            //Set up the uniforms needed for the lighting shaders to be able to draw and light the coral cube
            cube.associatedShader.SetUniform("uModel", Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(25f)));
            cube.associatedShader.SetUniform("uView", Camera.GetViewMatrix());
            cube.associatedShader.SetUniform("uProjection", Camera.GetProjectionMatrix());
            
            cube.associatedShader.SetUniform("viewPos",Camera.Position);
            
            cube.associatedShader.SetUniform("material.ambient", new Vector3(1.0f, 1.0f, 1.0f));
            cube.associatedShader.SetUniform("material.diffuse", new Vector3(0.5f, 0.5f, 0.5f));
            cube.associatedShader.SetUniform("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            cube.associatedShader.SetUniform("material.shininess", 16.0f);
            
            //Track the difference in time so we can manipulate variables as time changes
            var difference = (float) (DateTime.UtcNow - StartTime).TotalSeconds;
            var lightColor = Vector3.One;
            //lightColor.X = MathF.Sin(difference * 2.0f);
            //lightColor.Y = MathF.Sin(difference * 0.7f);
            //lightColor.Z = MathF.Sin(difference * 1.3f);

            var diffuseColor = lightColor * new Vector3(0.5f);
            var ambientColor = diffuseColor * new Vector3(0.5f);

            cube.associatedShader.SetUniform("light.ambient", ambientColor);
            cube.associatedShader.SetUniform("light.diffuse", diffuseColor); // darkened
            cube.associatedShader.SetUniform("light.specular", new Vector3(1.0f, 1.0f, 1.0f));
            cube.associatedShader.SetUniform("light.position", LampPosition);
            cube.Draw();
        }
        
        private static void RenderLitCube()
        {
            //Use the 'lighting shader' that is capable of modifying the cubes colours based on ambient lighting and diffuse lighting
            TextureShader.Use();
            TextureShader.SetUniform("uTexture0", 0);
            //Set up the uniforms needed for the lighting shaders to be able to draw and light the coral cube
            TextureShader.SetUniform("uModel", Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(25f)));
            TextureShader.SetUniform("uView", Camera.GetViewMatrix());
            TextureShader.SetUniform("uProjection", Camera.GetProjectionMatrix());
            
            TextureShader.SetUniform("viewPos",Camera.Position);
            
            TextureShader.SetUniform("material.ambient", new Vector3(1.0f, 0.5f, 0.31f));
            TextureShader.SetUniform("material.diffuse", new Vector3(1.0f, 0.5f, 0.31f));
            TextureShader.SetUniform("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            TextureShader.SetUniform("material.shininess", 32.0f);
            
            //Track the difference in time so we can manipulate variables as time changes
            var difference = (float) (DateTime.UtcNow - StartTime).TotalSeconds;
            var lightColor = Vector3.Zero;
            lightColor.X = MathF.Sin(difference * 2.0f);
            lightColor.Y = MathF.Sin(difference * 0.7f);
            lightColor.Z = MathF.Sin(difference * 1.3f);

            var diffuseColor = lightColor * new Vector3(0.5f);
            var ambientColor = diffuseColor * new Vector3(0.2f);

            TextureShader.SetUniform("light.ambient", ambientColor);
            TextureShader.SetUniform("light.diffuse", diffuseColor); // darkened
            TextureShader.SetUniform("light.specular", new Vector3(1.0f, 1.0f, 1.0f));
            TextureShader.SetUniform("light.position", LampPosition);

            //We're drawing with just vertices and no indicies, and it takes 36 verticies to have a six-sided textured cube
            Gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        private static void RenderLampCube()
        {
            //Use the 'main' shader that does not do any lighting calculations to just draw the cube to screen in the requested colours.
            LampShader.Use();

            //The Lamp cube is going to be a scaled down version of the normal cubes verticies moved to a different screen location
            var lampMatrix = Matrix4x4.Identity;
            lampMatrix *= Matrix4x4.CreateScale(0.2f);
            lampMatrix *= Matrix4x4.CreateTranslation(LampPosition);

            //Setup the uniforms needed to draw the Lamp in the correct place on screen
            LampShader.SetUniform("uModel", lampMatrix);
            LampShader.SetUniform("uView", Camera.GetViewMatrix());
            LampShader.SetUniform("uProjection", Camera.GetProjectionMatrix());

            Gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        private static void OnMouseMove(IMouse mouse, Vector2 position)
        {
            var lookSensitivity = 0.1f;
            if (LastMousePosition == default)
            {
                LastMousePosition = position;
            }
            else
            {
                var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
                var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
                LastMousePosition = position;

                Camera.ModifyDirection(xOffset, yOffset);
            }
        }

        private static void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
        {
            Camera.ModifyZoom(scrollWheel.Y);
        }

        private static void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if (arg2 == Key.Escape)
            {
                window.Close();
            }
        }

        private static void OnClose()
        {
            //Remember to delete the buffers.
            Vbo.Dispose();
            Ebo.Dispose();
            VaoCube.Dispose();
            TextureShader.Dispose();
            LampShader.Dispose();
            texture.Dispose();
        }
    }
}