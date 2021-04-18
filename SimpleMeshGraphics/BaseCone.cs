using Silk.NET.OpenGL;
using SimpleMeshGraphics;
using Shader = SimpleMeshGraphics.Shader;

namespace MeshLibraryTest
{
    public class BaseCone : ColouredObject
    {
        public BaseCone(GL gl, Camera cam) : base("Cone.obj", gl, new Shader(gl,"shaders/shader.vert","shaders/shader.frag"), cam)
        {}
    }
}