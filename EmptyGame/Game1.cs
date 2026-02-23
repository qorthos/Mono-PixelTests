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
        KeyboardState _lastKS;

        bool _isFullScreen = false;

        public bool IsFullScreen { get => _isFullScreen; set 
            { 
                _isFullScreen = value; 

                if (!_isFullScreen)
                {

                    _graphics.IsFullScreen = false;
                    _graphics.PreferredBackBufferWidth = 1280;
                    _graphics.PreferredBackBufferHeight =800;
                    //_graphics.SynchronizeWithVerticalRetrace = true;
                    _graphics.ApplyChanges();
                }
                else
                {

                    _graphics.IsFullScreen = true;
                    _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    //_graphics.SynchronizeWithVerticalRetrace = true;
                    _graphics.ApplyChanges();
                }


            } 
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();

            IsFixedTimeStep = false;
            //TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 60.0);

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

            if (ks.IsKeyDown(Keys.Enter) && (_lastKS.IsKeyDown(Keys.Enter) == false))
            {
                IsFullScreen = !IsFullScreen;
            }

            cameraPosition += input * 60 * (float)gameTime.ElapsedGameTime.TotalSeconds;



            _lastKS = ks;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var camPosSnapped = new Vector2(
                MathF.Round(cameraPosition.X),
                MathF.Round(cameraPosition.Y)
            );

            var matrix = Matrix.CreateTranslation(new Vector3(-camPosSnapped, 0)) * Matrix.CreateScale(3);

            _spriteBatch.Begin(transformMatrix: matrix, samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(map, new Vector2(0, 0), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
