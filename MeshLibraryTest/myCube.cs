using System;
using System.Numerics;
using Silk.NET.OpenGL;
using SimpleMeshGraphics;
using Shader = SimpleMeshGraphics.Shader;

namespace MeshLibraryTest
{
    public class myCube : MeshedObject
    {
        public myCube(GL gl,Camera cam) : base("Cube.obj",gl, new Shader(gl, "shaders/shader.vert", "shaders/shader.frag"),cam)
        {}

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
            Rotation *= Quaternion.CreateFromYawPitchRoll(0, MathHelper.DegreesToRadians(60.0f * (float)deltaTime), 0);
        }
    }
}