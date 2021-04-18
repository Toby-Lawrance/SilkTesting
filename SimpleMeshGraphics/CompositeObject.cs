using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Silk.NET.Vulkan;

namespace SimpleMeshGraphics
{
    public class CompositeObject : GraphicsObject
    {
        //Each components properties are in relation to the base
        private Dictionary<string,GraphicsObject> components;

        public CompositeObject()
        {
            components = new Dictionary<string,GraphicsObject>();
        }
        
        public CompositeObject(IEnumerable<(string,GraphicsObject)> initial)
        {
            components = new Dictionary<string, GraphicsObject>(initial.Select((tuple =>
                new KeyValuePair<string, GraphicsObject>(tuple.Item1, tuple.Item2))));
        }

        public bool TryAddComponent((string, GraphicsObject) component)
        {
            return !components.ContainsKey(component.Item1) && components.TryAdd(component.Item1, component.Item2);
        }

        public bool TryRemoveComponent(string key)
        {
            return components.Remove(key);
        }
        
        public GraphicsObject GetComponent(string key)
        {
            return !components.ContainsKey(key) ? null : components[key];
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);
            foreach (var (key,component) in components)
            {
                component.Update(deltaTime);
            }
        }
        
        public override void Render(double deltaTime, Stack<Transform> parentTransformation)
        {
            parentTransformation.Push(Transformation);
            foreach (var (key,component) in components)
            {
                component.Render(deltaTime,parentTransformation);
            }
        }
    }
}