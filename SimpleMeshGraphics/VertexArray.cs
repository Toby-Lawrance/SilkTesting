using System;
using Silk.NET.OpenGL;
using Buffer = System.Buffer;

namespace SimpleMeshGraphics
{
    public class VertexArray<TVertexType, TIndexType> : IDisposable
        where TVertexType : unmanaged
        where TIndexType : unmanaged
    {
        private uint _handle;
        private GL _gl;
        private BufferObject<TVertexType> vbo;
        private BufferObject<TIndexType> ebo;
        
        public VertexArray(GL gl, Span<TVertexType> vertexData, BufferTargetARB vertexBufferType, Span<TIndexType> indexData, BufferTargetARB indexBufferType)
        {
            //Saving the GL instance.
            _gl = gl;

            //Setting out handle and binding the VBO and EBO to this VAO.
            _handle = _gl.GenVertexArray();
            Bind();
            vbo = new BufferObject<TVertexType>(gl,vertexData, vertexBufferType);
            ebo = new BufferObject<TIndexType>(gl, indexData, indexBufferType);
        }
        
        public unsafe void VertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offSet)
        {
            //Setting up a vertex attribute pointer
            _gl.VertexAttribPointer(index, count, type, false, vertexSize * (uint) sizeof(TVertexType), (void*) (offSet * sizeof(TVertexType)));
            _gl.EnableVertexAttribArray(index);
        }
        
        public void Bind()
        {
            //Binding the vertex array.
            _gl.BindVertexArray(_handle);
        }

        public void Dispose()
        {
            _gl.DeleteVertexArray(_handle);
        }
    }
}