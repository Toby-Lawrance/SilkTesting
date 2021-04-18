using Silk.NET.OpenGL;
using SimpleMeshGraphics;
using Shader = SimpleMeshGraphics.Shader;

namespace MeshLibraryTest
{
    public class BaseCylinder : ColouredObject
    {
        public BaseCylinder(GL gl, Camera cam) : base("Cylinder.obj", gl, new Shader(gl,"shaders/shader.vert","shaders/shader.frag"), cam)
        {}
    }
}