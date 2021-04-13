using System.Numerics;
using Silk.NET.Input;

namespace SimpleMeshGraphics
{
    public abstract class GraphicsObject
    {
        public Vector3 Position = Vector3.Zero;
        public Quaternion Rotation = Quaternion.Identity;
        public Vector3 Scaling = Vector3.One;
        public Camera CameraRelevant;

        public Transform Transformation => new Transform { Position = Position, Rotation = Rotation, Scale = Scaling};

        public virtual void OnLoad() {}
        public virtual void Update(double deltaTime) {}
        public abstract void Render(double deltaTime);
        public abstract void Render(double deltaTime, Transform parentTransformation);
        public virtual void OnClose() {}

        public virtual void OnKeyPress(IKeyboard keyboard, Key key, int arg3) {}
        public virtual void OnMouseClick(IMouse mouse, Vector2 position) {}
    }
}