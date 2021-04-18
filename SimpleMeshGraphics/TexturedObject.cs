using System.Collections.Generic;
using System.Numerics;
using Silk.NET.OpenGL;

namespace SimpleMeshGraphics
{
    public class TexturedObject : MeshedObject
    {
        private Texture _texture;
        protected int textureSlot = 0;
        
        public TexturedObject(string objFile, string textureFile, GL gl, Shader s, Camera cam) : base(objFile, gl, s,cam)
        {
            _texture = new Texture(gl_api, textureFile);
        }

        protected virtual void UpdateTexture(Texture newTexture)
        {
            _texture = newTexture;
        }

        protected virtual void UpdateTexture(string newTextureFile)
        {
            _texture = new Texture(gl_api, newTextureFile);
        }

        
        public override void Render(double deltaTime, Stack<Transform> parentTransformation)
        {
            switch (textureSlot)
            {
                case 0: _texture.Bind(); break;
                case 1: _texture.Bind(TextureUnit.Texture1); break;
                case 2: _texture.Bind(TextureUnit.Texture2); break;
                case 3: _texture.Bind(TextureUnit.Texture3); break;
                case 4: _texture.Bind(TextureUnit.Texture4); break;
                case 5: _texture.Bind(TextureUnit.Texture5); break;
                case 6: _texture.Bind(TextureUnit.Texture6); break;
                case 7: _texture.Bind(TextureUnit.Texture7); break;
                case 8: _texture.Bind(TextureUnit.Texture8); break;
                case 9: _texture.Bind(TextureUnit.Texture9); break;
            }
            base.Render(deltaTime,parentTransformation);
        }

        protected override void ApplyShaderUniforms()
        {
            associatedShader.TrySetUniform("uTexture0",textureSlot);
            base.ApplyShaderUniforms();
        }
    }
}