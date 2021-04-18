using Silk.NET.OpenGL;
using SimpleMeshGraphics;
using Shader = SimpleMeshGraphics.Shader;

namespace MeshLibraryTest
{
    public class BaseCubeFaceOrigin : ColouredObject
    {
        public BaseCubeFaceOrigin(GL gl, Camera cam) : base("FaceOriginCube.obj", gl, new Shader(gl,"shaders/shader.vert","shaders/shader.frag"), cam)
        {}
    }
}