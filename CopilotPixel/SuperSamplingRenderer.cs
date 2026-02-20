using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CopilotPixel
{
    public class SuperSamplingRenderer
    {
        private Effect _effect;
        private Texture2D _texture;
        private GraphicsDevice _graphicsDevice;

        public SuperSamplingRenderer(GraphicsDevice graphicsDevice, Effect effect, Texture2D texture)
        {
            _graphicsDevice = graphicsDevice;
            _effect = effect;
            _texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float scale)
        {
            _effect.Parameters["TextureSize"]?.SetValue(new Vector2(_texture.Width, _texture.Height));
            _effect.Parameters["Scale"]?.SetValue(scale);
            _effect.Parameters["Position"]?.SetValue(position);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, _effect);
            spriteBatch.Draw(_texture, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.End();
        }
    }
}