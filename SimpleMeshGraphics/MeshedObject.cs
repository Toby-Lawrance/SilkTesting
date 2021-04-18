using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using JeremyAnsel.Media.WavefrontObj;
using Silk.NET.OpenGL;

namespace SimpleMeshGraphics
{
    public class MeshedObject : GraphicsObject
    {
        protected GL gl_api;
        protected VertexArray<float, uint> arrayObject;
        protected Shader associatedShader;

        protected Matrix4x4 transformationMatrix;

        private uint indicesLength = 0;
        
        protected ObjFile obj;
        
        public MeshedObject(string objFile, GL gl, Shader s, Camera cam)
        {
            obj = ObjFile.FromFile(objFile);
            gl_api = gl;
            associatedShader = s;
            CameraRelevant = cam;

            var (vertices, indices) = CreateVertexInfo();

            indicesLength = (uint) indices.Length;
            arrayObject = new VertexArray<float, uint>(gl_api, vertices, BufferTargetARB.ArrayBuffer, indices,
                BufferTargetARB.ElementArrayBuffer);

            arrayObject.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
            arrayObject.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
            arrayObject.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);
        }

        public override unsafe void Render(double deltaTime, Stack<Transform> parentTransformations)
        {
            arrayObject.Bind();
            associatedShader.Use();
            parentTransformations.Push(Transformation);
            transformationMatrix =  parentTransformations.Select(t => t.ViewMatrix).Aggregate(Matrix4x4.Identity, (m1,m2) => m1*m2);
            ApplyShaderUniforms();

            gl_api.DrawElements(PrimitiveType.Triangles,indicesLength,DrawElementsType.UnsignedInt,null);
        }

        protected virtual void ApplyShaderUniforms()
        {
            associatedShader.TrySetUniform("uWorld", transformationMatrix);
            associatedShader.TrySetUniform("uView", CameraRelevant.GetViewMatrix());
            associatedShader.TrySetUniform("uProjection", CameraRelevant.GetProjectionMatrix());
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