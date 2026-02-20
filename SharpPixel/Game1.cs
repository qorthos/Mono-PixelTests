using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SharpPixel
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Effect _sharpEffect;

        private Texture2D _sprite;          // your pixel art texture
        private Vector2 _position = new Vector2(400, 300);
        private float _zoom = 4f;           // start zoomed in a bit
        private float _zoomSpeed = 0.5f;

        // We'll pass per-sprite texture size to shader
        private Vector2 _texSize;
        private Vector2 _invTexSize;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load your pixel art sprite (change name as needed)
            _sprite = Content.Load<Texture2D>("Sprite-0001");   // ← your sprite here

            _sharpEffect = Content.Load<Effect>("SharpPixel");

            _texSize = new Vector2(_sprite.Width, _sprite.Height);
            _invTexSize = Vector2.One / _texSize;

            // Optional: set initial effect parameters (can update every frame if needed)
            _sharpEffect.Parameters["TextureSize"]?.SetValue(_texSize);
            _sharpEffect.Parameters["InvTextureSize"]?.SetValue(_invTexSize);
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Movement (smooth sub-pixel)
            Vector2 move = Vector2.Zero;
            if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A)) move.X -= 1;
            if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D)) move.X += 1;
            if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W)) move.Y -= 1;
            if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S)) move.Y += 1;

            if (move != Vector2.Zero)
            {
                move.Normalize();
                _position += move * 180f * dt;   // pixels per second
            }

            // Zoom with mouse wheel or Q/E
            int wheel = Mouse.GetState().ScrollWheelValue;
            if (wheel != 0)
            {
                _zoom += wheel > 0 ? _zoomSpeed : -_zoomSpeed;
                _zoom = MathHelper.Clamp(_zoom, 1f, 12f);
            }
            if (keyboard.IsKeyDown(Keys.Q)) _zoom -= 3f * dt;
            if (keyboard.IsKeyDown(Keys.E)) _zoom += 3f * dt;
            _zoom = MathHelper.Clamp(_zoom, 1f, 12f);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Optional: update if texture size changes (multi-sprite games)
            // _sharpEffect.Parameters["TextureSize"].SetValue(_texSize);
            // _sharpEffect.Parameters["InvTextureSize"].SetValue(_invTexSize);

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,   // important for base sharpness
                effect: _sharpEffect
            );

            // Draw with arbitrary float position + zoom scale
            // Origin = center for nicer zooming
            Vector2 origin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);

            _spriteBatch.Draw(
                _sprite,
                _position,
                null,
                Color.White,
                0f,
                origin,
                _zoom,                    // arbitrary scale
                SpriteEffects.None,
                0f
            );

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}