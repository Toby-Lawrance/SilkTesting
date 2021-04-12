using System;
using System.Collections.Generic;
using System.Linq;
using JeremyAnsel.Media.WavefrontObj;
using Silk.NET.OpenGL;

namespace SilkTesting
{
    public class GraphicalObject
    {
        private readonly BufferObject<uint> Ebo;

        private readonly GL Gl;
        private readonly uint[] indices;
        private readonly ObjFile obj;
        private readonly VertexArrayObject<float, uint> Vao;
        private readonly BufferObject<float> Vbo;
        private readonly float[] vertices;

        public Shader associatedShader;
        private ObjMaterialFile mtl;

        public GraphicalObject(string objFile, string mtlFile, GL Gl, Shader s)
        {
            obj = ObjFile.FromFile(objFile);
            mtl = ObjMaterialFile.FromFile(mtlFile);

            associatedShader = s;
            this.Gl = Gl;

            (vertices, indices) = CreateVertexInfo();

            Ebo = new BufferObject<uint>(Gl, indices, BufferTargetARB.ElementArrayBuffer);
            Vbo = new BufferObject<float>(Gl, vertices, BufferTargetARB.ArrayBuffer);
            Vao = new VertexArrayObject<float, uint>(Gl, Vbo, Ebo);

            Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
            Vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
            Vao.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);
        }

        public void Use()
        {
            Ebo.Bind();
            Vbo.Bind();
            Vao.Bind();
            associatedShader.Use();
        }

        public unsafe void Draw()
        {
            Gl.DrawElements(PrimitiveType.Triangles,(uint)indices.Length,DrawElementsType.UnsignedInt,null);
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