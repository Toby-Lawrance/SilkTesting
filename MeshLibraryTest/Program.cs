using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Xml;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using SimpleMeshGraphics;

namespace MeshLibraryTest
{
    class Program
    {
        private static int width = 1920;
        private static int height = 1080;
        private static IWindow window;

        private static GL gl_api;
        private static Camera cam;
        private static IKeyboard keyboard;
        private static IMouse mouse;
        private static List<GraphicsObject> myObjects;
        
        private static Vector2 LastMousePosition;
        
        static void Main(string[] args)
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(width, height);
            options.Title = "Mesh Test";
            options.WindowBorder = WindowBorder.Fixed;

            window = Window.Create(options);

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
            gl_api = GL.GetApi(window);
            var input = window.CreateInput();
            keyboard = input.Keyboards.FirstOrDefault();
            if (keyboard is not null)
            {
                keyboard.KeyDown += KeyDown;
            }

            mouse = input.Mice.FirstOrDefault();
            if (mouse is not null)
            {
                mouse.Cursor.CursorMode = CursorMode.Raw;
                mouse.MouseMove += OnMouseMove;
                mouse.Scroll += OnMouseWheel;
            }
            
            cam = new Camera(Vector3.UnitZ * 6, Vector3.UnitZ * -1, Vector3.UnitY, (float)width / height);
            myObjects = new List<GraphicsObject>();
            
            myObjects.Add(new myBallStickCube(gl_api,cam));
            myObjects.Add(new mySphere(gl_api,cam));
            myObjects[1].Position = Vector3.One * 10;
            myObjects[1].Scaling = Vector3.One * 2;
            myObjects[1].Rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.DegreesToRadians(30),MathHelper.DegreesToRadians(30),MathHelper.DegreesToRadians(30));
            myObjects.Add(new myCylinder(gl_api, cam));
            myObjects[2].Scaling = new Vector3(0.1f,100,0.1f);
            
            foreach (var graphicsObject in myObjects)
            {
                graphicsObject.OnLoad();
            }
            
            Console.WriteLine($"Zero translation {Matrix4x4.CreateTranslation(Vector3.Zero)}");
            Console.WriteLine($"(1,1,1) translation {Matrix4x4.CreateTranslation(Vector3.One)}");
        }

        private static void OnUpdate(double deltaTime)
        {
            foreach (var graphicsObject in myObjects)
            {
                graphicsObject.Update(deltaTime);
            }
            
            var moveSpeed = 2.5f * (float) deltaTime;

            if (keyboard.IsKeyPressed(Key.W))
            {
                cam.Position += moveSpeed * cam.Front;
            }

            if (keyboard.IsKeyPressed(Key.S))
            {
                cam.Position -= moveSpeed * cam.Front;
            }

            if (keyboard.IsKeyPressed(Key.A))
            {
                cam.Position -= Vector3.Normalize(Vector3.Cross(cam.Front, cam.Up)) * moveSpeed;
            }

            if (keyboard.IsKeyPressed(Key.D))
            {
                cam.Position += Vector3.Normalize(Vector3.Cross(cam.Front, cam.Up)) * moveSpeed;
            }

            if (keyboard.IsKeyPressed(Key.Space))
            {
                cam.Position += moveSpeed * cam.Up;
            }

            if (keyboard.IsKeyPressed(Key.ShiftLeft))
            {
                cam.Position -= moveSpeed * cam.Up;
            }
        }

        private static void OnRender(double deltaTime)
        {
            gl_api.Enable(EnableCap.DepthTest);
            gl_api.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
            
            foreach (var graphicsObject in myObjects)
            {
                graphicsObject.Render(deltaTime);
            }
        }

        private static void OnClose()
        {
            foreach (var graphicsObject in myObjects)
            {
                graphicsObject.OnClose();
            }
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

                cam.ModifyDirection(xOffset, yOffset);
            }
        }
        
        private static void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
        {
            cam.ModifyZoom(scrollWheel.Y);
        }

        private static void KeyDown(IKeyboard keyb, Key k, int code)
        {
            if (k == Key.Escape)
            {
                window.Close();
            }
        }
    }
}