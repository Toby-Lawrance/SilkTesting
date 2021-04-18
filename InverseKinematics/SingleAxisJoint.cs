using System.Collections.Generic;
using System.Numerics;
using MeshLibraryTest;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using SimpleMeshGraphics;

namespace InverseKinematics
{
    public class SingleAxisJoint : BaseCylinder
    {
        private float _maxAngle = 0.0f;
        private float _minAngle = 0.0f;

        //IS IN RADIANS
        public float MaxAngle
        {
            get => _maxAngle;
            set
            {
                _maxAngle = value;
                MathHelper.Clamp(ref _angle, MaxAngle,MinAngle);
            }
        }

        //IS IN RADIANS
        public float MinAngle
        {
            get => _minAngle;
            set
            {
                _minAngle = value;
                MathHelper.Clamp(ref _angle, MaxAngle,MinAngle);
            }
        }

        private float _angle = 0.0f;
        public float Angle
        {
            get => _angle;
            set
            {
                _angle = value;
                MathHelper.Clamp(ref _angle, MaxAngle,MinAngle);
            }
        }

        protected MeshedObject connectedPiece = null;

        public SingleAxisJoint(GL gl, Camera cam, float maxAngle = 360.0f, float minAngle = 0.0f, float diameter = 1.0f , MeshedObject connection = null) : base(gl, cam)
        {
            connectedPiece = connection;
            MaxAngle = MathHelper.DegreesToRadians(maxAngle);
            MinAngle = MathHelper.DegreesToRadians(minAngle);
            this.Scaling = new Vector3(diameter, 1f, diameter);
        }

        public void SetAngleDegrees(double angle)
        {
            float rads = MathHelper.DegreesToRadians(angle);
            Angle = rads;
        }

        public void SetAngleRadians(float rads)
        {
            Angle = rads;
        }
        
        public override void Render(double deltaTime, Stack<Transform> parentTransformation)
        {
            Transform prettyJoint = new Transform
                {Rotation = Quaternion.CreateFromYawPitchRoll(0f, 0f, MathHelper.DegreesToRadians(90)), Scale = new Vector3(1f, 0.25f, 1f)};
            
            Transform appliedRotation = new Transform();
            Matrix4x4 rotation = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, Angle);
            appliedRotation.Rotation = this.Rotation * Quaternion.CreateFromRotationMatrix(rotation); //My rotation, then apply joint angle

            Transform offsetToEdgeOfJoint = new Transform {Position = Vector3.UnitY * Scaling.X};

            var withPrettyJoint = new Stack<Transform>(parentTransformation);
            withPrettyJoint.Push(prettyJoint);
            
            base.Render(deltaTime, withPrettyJoint);
            parentTransformation.Push(NoScalingTransformation);
            parentTransformation.Push(offsetToEdgeOfJoint);
            parentTransformation.Push(appliedRotation);
            connectedPiece?.Render(deltaTime,parentTransformation);
        }
    }
}