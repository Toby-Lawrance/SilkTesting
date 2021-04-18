using System;
using System.Collections.Generic;
using System.Numerics;
using MeshLibraryTest;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using SimpleMeshGraphics;

namespace InverseKinematics
{
    public class Arm : BaseCubeFaceOrigin
    {
        private float _length = 1.0f;

        public float Length
        {
            get => _length;
            set
            {
                _length = value;
                this.Scaling = new Vector3(0.25f, this._length, 0.25f);
            }
        }

        protected MeshedObject connectedComponent = null;


        public Arm(GL gl, Camera cam, float length = 1.0f, MeshedObject connection = null) : base(gl, cam)
        {
            this.connectedComponent = connection;
            this.Length = length;
        }

        public override void Render(double deltaTime, Stack<Transform> parentTransformation)
        {
            Transform offset = new Transform {Position = Vector3.UnitY * Scaling.Y};
            base.Render(deltaTime, parentTransformation);
            parentTransformation.Push(NoScalingTransformation);
            parentTransformation.Push(offset);
            connectedComponent?.Render(deltaTime,parentTransformation);
        }
    }
}