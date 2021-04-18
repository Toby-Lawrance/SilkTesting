using System.Numerics;
using Silk.NET.OpenGL;

namespace SimpleMeshGraphics
{
    public class ColouredObject : MeshedObject
    {
        public Vector4 Colour = Vector4.One;
        
        public ColouredObject(string objFile, GL gl, Shader s, Camera cam) : base(objFile, gl, s, cam)
        {
        }

        protected override void ApplyShaderUniforms()
        {
            associatedShader.TrySetUniform("uColor", Colour);
            base.ApplyShaderUniforms();
        }
    }
}