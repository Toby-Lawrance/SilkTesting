using Silk.NET.OpenGL;
using SimpleMeshGraphics;
using Shader = SimpleMeshGraphics.Shader;

namespace MeshLibraryTest
{
    public class BaseSphere : ColouredObject
    {
        public BaseSphere(GL gl, Camera cam) : base("Sphere.obj", gl, new Shader(gl,"shaders/shader.vert","shaders/shader.frag"), cam)
        {}
    }
}