using System;
using System.Numerics;
using Silk.NET.OpenGL;
using SimpleMeshGraphics;

namespace MeshLibraryTest
{
    public class myBallStickCube : CompositeObject
    {
        private double time = 0;
        public myBallStickCube(GL gl, Camera cam)
        {
            var cube = new myCube(gl, cam);
            var stick = new myCylinder(gl, cam);
            var ball = new mySphere(gl, cam);
            
            ball.Position = Vector3.Zero;
            ball.Scaling = Vector3.One;
            ball.Rotation = Quaternion.Identity;

            stick.Scaling = new Vector3(0.5f,5.0f,0.5f);
            stick.Position = new Vector3(ball.Position.X, ball.Position.Y + (stick.Scaling.Y/2.0f), ball.Position.Z);

            cube.Scaling = Vector3.One;
            cube.Position = new Vector3(ball.Position.X, ball.Position.Y + stick.Scaling.Y, ball.Position.Z);

            this.TryAddComponent(("ball", ball));
            this.TryAddComponent(("stick", stick));
            this.TryAddComponent(("cube", cube));

            this.Rotation = Quaternion.CreateFromYawPitchRoll(0.0f, MathHelper.DegreesToRadians(45), 0.0f);
        }

        public override void Update(double deltaTime)
        {
            time += deltaTime;
            base.Update(deltaTime);
            var speed = 20f;
            float rotationRate = MathHelper.DegreesToRadians(speed * (float)deltaTime);
            Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, rotationRate);

        }
    }
}