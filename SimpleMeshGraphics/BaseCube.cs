using Silk.NET.OpenGL;
using SimpleMeshGraphics;
using Shader = SimpleMeshGraphics.Shader;

namespace MeshLibraryTest
{
    public class BaseCube : ColouredObject
    {
        public BaseCube(GL gl, Camera cam) : base("Cube.obj", gl, new Shader(gl,"shaders/shader.vert","shaders/shader.frag"), cam)
        {}
    }
}