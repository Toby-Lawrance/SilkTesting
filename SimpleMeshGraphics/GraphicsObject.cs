using System.Collections.Generic;
using System.Numerics;
using Silk.NET.Input;

namespace SimpleMeshGraphics
{
    public abstract class GraphicsObject
    {
        public Vector3 Position = Vector3.Zero;
        private Quaternion _rotation = Quaternion.Identity;
        public Quaternion Rotation { get => _rotation;
            set => _rotation = Quaternion.Normalize(value);
        }
        public Vector3 Scaling = Vector3.One;
        public Camera CameraRelevant;

        public Transform Transformation => new Transform { Position = Position, Rotation = Rotation, Scale = Scaling};
        public Transform NoScalingTransformation => new Transform {Position = Position, Rotation = Rotation};

        public virtual void OnLoad() {}
        public virtual void Update(double deltaTime) {}

        public virtual void Render(double deltaTime)
        {
            Render(deltaTime,new Stack<Transform>());
        }
        public abstract void Render(double deltaTime, Stack<Transform> transforms);
        public virtual void OnClose() {}

        public virtual void OnKeyPress(IKeyboard keyboard, Key key, int arg3) {}
        public virtual void OnMouseClick(IMouse mouse, Vector2 position) {}
    }
}