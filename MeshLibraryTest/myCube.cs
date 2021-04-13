using System;
using System.Numerics;
using Silk.NET.OpenGL;
using SimpleMeshGraphics;
using Shader = SimpleMeshGraphics.Shader;

namespace MeshLibraryTest
{
    public class myCube : MeshedObject
    {
        private DateTime start;
        
        public myCube(GL gl) : base("Cube.obj",gl, new Shader(gl, "shaders/shader.vert", "shaders/shader.frag"))
        {}

        public override void OnLoad()
        {
            base.OnLoad();
            Console.WriteLine("Loaded myCube");
            start = DateTime.UtcNow;
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
            var speed = 0.5f * deltaTime;
            this.Scaling += Vector3.UnitY * (float)speed;
            this.Scaling.X = (MathF.Sin(DateTime.UtcNow.Millisecond * (float)speed) + 1)/2.0f;
        }

        protected override void ApplyShaderUniforms(Camera c)
        {
            //associatedShader.TrySetUniform("uColor", Vector4.One * (MathF.Cos((DateTime.UtcNow - start).Milliseconds) + 1.0f)/2.0f); 
            base.ApplyShaderUniforms(c);
        }

        public override void OnClose()
        {
            base.OnClose();
            Console.WriteLine("Closing myCube");
        }
    }
}