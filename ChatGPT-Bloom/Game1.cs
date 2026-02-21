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
        Texture2D emissionTexture;

        RenderTarget2D sceneTarget;
        RenderTarget2D emissionTarget;
        RenderTarget2D halfTarget1;
        RenderTarget2D halfTarget2;

        Effect bloomExtract;
        Effect gaussianBlur;
        Effect bloomCombine;

        // Cache effect parameters
        EffectParameter bloomThresholdParameter;
        EffectParameter bloomSoftKneeParameter;
        EffectParameter texelSizeParameter;
        EffectParameter blurAmountParameter;
        EffectParameter bloomTextureParameter;
        EffectParameter bloomIntensityParameter;
        EffectParameter bloomSaturationParameter;
        EffectParameter baseIntensityParameter;
        EffectParameter baseSaturationParameter;

        Vector2 cameraPosition = Vector2.Zero;
        const float CameraSpeed = 60f; // pixels per second

        KeyboardState previousKeyboardState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            previousKeyboardState = Keyboard.GetState();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sprite = Content.Load<Texture2D>("Sprite-0001");
            emissionTexture = Content.Load<Texture2D>("Sprite-White");

            bloomExtract = Content.Load<Effect>("BloomExtract");
            gaussianBlur = Content.Load<Effect>("GaussianBlur");
            bloomCombine = Content.Load<Effect>("BloomCombine");

            // Cache effect parameters (once)
            bloomThresholdParameter = bloomExtract.Parameters["BloomThreshold"];
            bloomSoftKneeParameter = bloomExtract.Parameters["BloomSoftKnee"];
            texelSizeParameter = gaussianBlur.Parameters["TexelSize"];
            blurAmountParameter = gaussianBlur.Parameters["BlurAmount"];
            bloomTextureParameter = bloomCombine.Parameters["BloomTexture"];
            bloomIntensityParameter = bloomCombine.Parameters["BloomIntensity"];
            bloomSaturationParameter = bloomCombine.Parameters["BloomSaturation"];
            baseIntensityParameter = bloomCombine.Parameters["BaseIntensity"];
            baseSaturationParameter = bloomCombine.Parameters["BaseSaturation"];

            var pp = GraphicsDevice.PresentationParameters;

            sceneTarget = new RenderTarget2D(GraphicsDevice,
                pp.BackBufferWidth,
                pp.BackBufferHeight);

            // New render target for emission data
            emissionTarget = new RenderTarget2D(GraphicsDevice,
                pp.BackBufferWidth,
                pp.BackBufferHeight);

            halfTarget1 = new RenderTarget2D(GraphicsDevice,
                pp.BackBufferWidth / 2,
                pp.BackBufferHeight / 2);

            halfTarget2 = new RenderTarget2D(GraphicsDevice,
                pp.BackBufferWidth / 2,
                pp.BackBufferHeight / 2);
        }

        protected override void Update(GameTime gameTime)
        {
            UpdateCamera(gameTime);
            base.Update(gameTime);
        }

        private void UpdateCamera(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 move = Vector2.Zero;

            if (state.IsKeyDown(Keys.W)) move.Y -= 1;
            if (state.IsKeyDown(Keys.S)) move.Y += 1;
            if (state.IsKeyDown(Keys.A)) move.X -= 1;
            if (state.IsKeyDown(Keys.D)) move.X += 1;

            if (move != Vector2.Zero)
            {
                move.Normalize();
                cameraPosition += move * CameraSpeed * delta;
            }

            previousKeyboardState = state;
        }

        protected override void Draw(GameTime gameTime)
        {
            // 1. Render Scene (main diffuse/albedo)
            GraphicsDevice.SetRenderTarget(sceneTarget);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(sprite, new Vector2(300, 200) - cameraPosition, Color.White);
            spriteBatch.End();

            // 2. Render Emission Data
            GraphicsDevice.SetRenderTarget(emissionTarget);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            // Render emission texture - this contains the data that should bloom
            spriteBatch.Draw(emissionTexture, new Vector2(300, 200) - cameraPosition, Color.White);
            // You can add more emission sources here if needed
            spriteBatch.End();

            // 3. Extract Emission Areas for Bloom
            bloomThresholdParameter.SetValue(0.8f); 
            bloomSoftKneeParameter.SetValue(0.8f);

            // Calculate the scale factor to downsample properly
            float scaleX = (float)halfTarget1.Width / emissionTarget.Width;
            float scaleY = (float)halfTarget1.Height / emissionTarget.Height;
            
            spriteBatch.Begin(effect: bloomExtract);
            spriteBatch.Draw(emissionTarget, 
                Vector2.Zero, 
                null, 
                Color.White, 
                0f, 
                Vector2.Zero, 
                new Vector2(scaleX, scaleY), 
                SpriteEffects.None, 
                0f);
            spriteBatch.End();

            // 4. Horizontal Blur
            texelSizeParameter.SetValue(new Vector2(1f / halfTarget1.Width, 0));
            blurAmountParameter.SetValue(1f);

            GraphicsDevice.SetRenderTarget(halfTarget2);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(effect: gaussianBlur);
            spriteBatch.Draw(halfTarget1, Vector2.Zero, Color.White);
            spriteBatch.End();

            // 5. Vertical Blur
            texelSizeParameter.SetValue(new Vector2(0, 1f / halfTarget1.Height));

            GraphicsDevice.SetRenderTarget(halfTarget1);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(effect: gaussianBlur);
            spriteBatch.Draw(halfTarget2, Vector2.Zero, Color.White);
            spriteBatch.End();

            // 6. Final Combine (scene + bloom from emission)
            bloomTextureParameter.SetValue(halfTarget1);
            bloomIntensityParameter.SetValue(1.2f);
            baseIntensityParameter.SetValue(0.5f);
            bloomSaturationParameter.SetValue(1.0f);
            baseSaturationParameter.SetValue(0.5f);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            // Set the bloom texture to sampler 1
            spriteBatch.Begin(effect: bloomCombine);
            spriteBatch.Draw(sceneTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            sceneTarget?.Dispose();
            emissionTarget?.Dispose();
            halfTarget1?.Dispose();
            halfTarget2?.Dispose();
            base.UnloadContent();
        }
    }
}