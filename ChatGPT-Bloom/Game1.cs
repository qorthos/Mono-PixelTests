using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BloomOpenGL
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D sprite;

        RenderTarget2D sceneTarget;
        RenderTarget2D halfTarget1;
        RenderTarget2D halfTarget2;

        Effect bloomExtract;
        Effect gaussianBlur;
        Effect bloomCombine;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sprite = Content.Load<Texture2D>("Sprite-White");

            bloomExtract = Content.Load<Effect>("BloomExtract");
            gaussianBlur = Content.Load<Effect>("GaussianBlur");
            bloomCombine = Content.Load<Effect>("BloomCombine");

            var pp = GraphicsDevice.PresentationParameters;

            sceneTarget = new RenderTarget2D(GraphicsDevice,
                pp.BackBufferWidth,
                pp.BackBufferHeight);

            halfTarget1 = new RenderTarget2D(GraphicsDevice,
                pp.BackBufferWidth / 2,
                pp.BackBufferHeight / 2);

            halfTarget2 = new RenderTarget2D(GraphicsDevice,
                pp.BackBufferWidth / 2,
                pp.BackBufferHeight / 2);
        }

        protected override void Draw(GameTime gameTime)
        {
            // 1. Render Scene
            GraphicsDevice.SetRenderTarget(sceneTarget);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(sprite, new Vector2(300, 200), Color.White);
            spriteBatch.End();

            // 2. Extract Bright Areas (downsample)
            GraphicsDevice.SetRenderTarget(halfTarget1);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(effect: bloomExtract);

            bloomExtract.Parameters["BloomThreshold"].SetValue(0.2f);
            bloomExtract.Parameters["BloomSoftKnee"].SetValue(0.5f);

            spriteBatch.Draw(sceneTarget,
                Vector2.Zero,
                Color.White);
            spriteBatch.End();

            // 3. Horizontal Blur
            gaussianBlur.Parameters["TexelSize"]
                .SetValue(new Vector2(1f / halfTarget1.Width, 0));
            gaussianBlur.Parameters["BlurAmount"].SetValue(1f);

            GraphicsDevice.SetRenderTarget(halfTarget2);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(effect: gaussianBlur);
            spriteBatch.Draw(halfTarget1, Vector2.Zero, Color.White);
            spriteBatch.End();

            // 4. Vertical Blur
            gaussianBlur.Parameters["TexelSize"]
                .SetValue(new Vector2(0, 1f / halfTarget1.Height));

            GraphicsDevice.SetRenderTarget(halfTarget1);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(effect: gaussianBlur);
            spriteBatch.Draw(halfTarget2, Vector2.Zero, Color.White);
            spriteBatch.End();

            //// 5. Final Combine
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.Textures[1] = halfTarget1;
            bloomCombine.Parameters["BloomIntensity"].SetValue(1.2f);
            bloomCombine.Parameters["BaseIntensity"].SetValue(1.0f);
            bloomCombine.Parameters["BloomSaturation"].SetValue(1.0f);
            bloomCombine.Parameters["BaseSaturation"].SetValue(1.0f);


            spriteBatch.Begin(effect: bloomCombine);
            spriteBatch.Draw(sceneTarget, Vector2.Zero, Color.White);
            spriteBatch.End();


            // debug
            //spriteBatch.Begin();
            //spriteBatch.Draw(sceneTarget, Vector2.Zero, Color.White);
            //spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}