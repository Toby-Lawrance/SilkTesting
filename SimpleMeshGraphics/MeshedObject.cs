using System;
using System.Collections.Generic;
using System.Linq;
using JeremyAnsel.Media.WavefrontObj;
using Silk.NET.OpenGL;

namespace SimpleMeshGraphics
{
    public class MeshedObject : GraphicsObject
    {
        protected GL gl_api;
        protected BufferObject<uint> indicesBuffer;
        protected BufferObject<float> vertexBuffer;
        protected VertexArrayObject<float, uint> arrayObject;
        protected Shader associatedShader;
        
        protected ObjFile obj;
        
        public MeshedObject(string objFile, GL gl, Shader s)
        {
            obj = ObjFile.FromFile(objFile);
            gl_api = gl;
            associatedShader = s;

            var (vertices, indices) = CreateVertexInfo();

            indicesBuffer = new BufferObject<uint>(gl_api, indices, BufferTargetARB.ElementArrayBuffer);
            vertexBuffer = new BufferObject<float>(gl_api, vertices, BufferTargetARB.ArrayBuffer);
            arrayObject = new VertexArrayObject<float, uint>(gl_api, vertexBuffer, indicesBuffer);
            
            arrayObject.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
            arrayObject.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
            arrayObject.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);
        }

        public override unsafe void Render(double deltaTime, Camera c)
        {
            indicesBuffer.Bind();
            vertexBuffer.Bind();
            arrayObject.Bind();
            associatedShader.Use();
            
            ApplyShaderUniforms(c);
            
            gl_api.DrawElements(PrimitiveType.Triangles,indicesBuffer.GetSize(),DrawElementsType.UnsignedInt,null);
        }

        protected virtual void ApplyShaderUniforms(Camera c)
        {
            var currentTransform = new Transform
            {
                Position = Position, Rotation = Rotation, Scale = Scaling
            };
            
            associatedShader.TrySetUniform("uModel", currentTransform.ViewMatrix);
            associatedShader.TrySetUniform("uView", c.GetViewMatrix());
            associatedShader.TrySetUniform("uProjection", c.GetProjectionMatrix());
        }
        
        private (float[], uint[]) CreateVertexInfo()
        {
            var triplets = new List<ObjTriplet>();

            foreach (var face in obj.Faces)
            {
                if (face.Vertices.Count != 3)
                {
                    throw new Exception($"Expected 3 vertices per face, got {face.Vertices.Count}");
                }

                triplets.AddRange(face.Vertices);
            }

            var finalTriplets = triplets.Distinct().ToList();

            var compiledVertices = (from triplet in finalTriplets
                let v = obj.Vertices[triplet.Vertex - 1].Position
                let t = triplet.Texture != 0 ? obj.TextureVertices[triplet.Texture - 1] : new ObjVector3()
                let n = triplet.Normal != 0 ? obj.VertexNormals[triplet.Normal - 1] : new ObjVector3()
                select new[] {v.X, v.Y, v.Z, n.X, n.Y, n.Z, t.X, t.Y}).ToList();

            var doneVertices = compiledVertices.SelectMany(v => v).ToArray();
            var finalIndices = obj.Faces.SelectMany(face =>
                face.Vertices
                    .Select(v => (uint) finalTriplets.FindIndex(tr => tr == v))).ToArray();
            
            return (doneVertices, finalIndices);
        }
    }
}