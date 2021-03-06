using System;
using System.IO;
using System.Numerics;
using Silk.NET.OpenGL;

namespace SimpleMeshGraphics
{
    public class Shader : IDisposable
    {
        private uint _handle;
        private GL _gl;

        public Shader(GL gl, string vertexPath, string fragmentPath)
        {
            _gl = gl;
            
            //Load the individual shaders.
            uint vertex = LoadShader(ShaderType.VertexShader, vertexPath);
            uint fragment = LoadShader(ShaderType.FragmentShader, fragmentPath);
            //Create the shader program.
            _handle = _gl.CreateProgram();
            //Attach the individual shaders.
            _gl.AttachShader(_handle, vertex);
            _gl.AttachShader(_handle, fragment);
            _gl.LinkProgram(_handle);
            //Check for linking errors.
            _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(_handle)}");
            }
            //Detach and delete the shaders
            _gl.DetachShader(_handle, vertex);
            _gl.DetachShader(_handle, fragment);
            _gl.DeleteShader(vertex);
            _gl.DeleteShader(fragment);
        }
        
        public void Use()
        {
            _gl.UseProgram(_handle);
        }
        
        public bool TrySetUniform(string name, int value)
        {
            //Setting a uniform on a shader using a name.
            var location = _gl.GetUniformLocation(_handle, name);
            if (location == -1) //If GetUniformLocation returns -1 the uniform is not found.
            {
                Console.Error.WriteLine($"{name} uniform not found on shader.");
                return false;
            }
            _gl.Uniform1(location, value);
            return true;
        }
        
        public unsafe bool TrySetUniform(string name, Matrix4x4 value)
        {
            //A new overload has been created for setting a uniform so we can use the transform in our shader.
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                Console.Error.WriteLine($"{name} uniform not found on shader.");
                return false;
            }
            _gl.UniformMatrix4(location, 1, false, (float*) &value);
            return true;
        }
        
        public bool TrySetUniform(string name, float value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                Console.Error.WriteLine($"{name} uniform not found on shader.");
                return false;
            }
            _gl.Uniform1(location, value);
            return true;
        }
        
        public bool TrySetUniform(string name, Vector3 value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                Console.Error.WriteLine($"{name} uniform not found on shader.");
                return false;
            }
            _gl.Uniform3(location, value.X, value.Y, value.Z);
            return true;
        }
        
        public bool TrySetUniform(string name, Vector4 value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                Console.Error.WriteLine($"{name} uniform not found on shader.");
                return false;
            }
            _gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
            return true;
        }
        
        private uint LoadShader(ShaderType type, string path)
        {
            //To load a single shader we need to load
            //1) Load the shader from a file.
            //2) Create the handle.
            //3) Upload the source to opengl.
            //4) Compile the shader.
            //5) Check for errors.
            string src = File.ReadAllText(path);
            uint handle = _gl.CreateShader(type);
            _gl.ShaderSource(handle, src);
            _gl.CompileShader(handle);
            string infoLog = _gl.GetShaderInfoLog(handle);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
            }

            return handle;
        }
        
        public void Dispose()
        {
            //Remember to delete the program when we are done.
            _gl.DeleteProgram(_handle);
        }
    }
}