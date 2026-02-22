using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace EmptyGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Texture2D map;

        Vector2 cameraPosition;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            map = Content.Load<Texture2D>("map_hollow");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var ks = Keyboard.GetState();
            Vector2 input = Vector2.Zero;
            // wasd movement, 60 pixels per second
            if (ks.IsKeyDown(Keys.W))
            {
                input += new Vector2(0, -1);
            }
            if (ks.IsKeyDown(Keys.S))
            {
                input += new Vector2(0, 1);
            }
            if (ks.IsKeyDown(Keys.A))
            {
                input += new Vector2(-1, 0);
            }
            if (ks.IsKeyDown(Keys.D))
            {
                input += new Vector2(1, 0);
            }

            cameraPosition += input * 60 * (float)gameTime.ElapsedGameTime.TotalSeconds;



            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var matrix = Matrix.CreateTranslation(new Vector3(-cameraPosition, 0)) * Matrix.CreateScale(3);

            _spriteBatch.Begin(transformMatrix: matrix, samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(map, new Vector2(0, 0), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
