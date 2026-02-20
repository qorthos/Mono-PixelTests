using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CopilotPixel
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Add fields for character texture and renderer
        private Texture2D _characterTexture;
        private Effect _superSamplingEffect;
        private SuperSamplingRenderer _renderer;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load character texture and shader effect
            _characterTexture = Content.Load<Texture2D>("Sprite-0001");
            _superSamplingEffect = Content.Load<Effect>("SuperSampling");
            _renderer = new SuperSamplingRenderer(GraphicsDevice, _superSamplingEffect, _characterTexture);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Example: render character at non-integer scale and position
            float scale = 2.35f;
            Vector2 position = new Vector2(100.5f, 150.75f);

            _renderer.Draw(_spriteBatch, position, scale);

            base.Draw(gameTime);
        }
    }
}
