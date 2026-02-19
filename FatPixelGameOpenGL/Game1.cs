using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _mySprite;
    private Effect _smoothShader;

    // Movement & Zoom variables
    private float _timer = 0f;
    private Vector2 _spritePos = new Vector2(400, 300);
    private float _zoom = 1.0f;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // Replace with your 16x32 sprite name
        _mySprite = Content.Load<Texture2D>("inventory_chest_slot");
        _smoothShader = Content.Load<Effect>("SmoothPixel");
    }

    protected override void Update(GameTime gameTime)
    {
        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // 1. Smoothly move the sprite in a tiny circle (sub-pixel movement)
        _spritePos.X = 400 + (float)Math.Cos(_timer) * 50.5f;
        _spritePos.Y = 300 + (float)Math.Sin(_timer) * 50.5f;

        // 2. Animate the zoom smoothly between 1x and 4x
        _zoom = 2.5f + (float)Math.Sin(_timer * 0.5f) * 1.5f;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Camera Matrix Calculation
        float screenWidth = _graphics.PreferredBackBufferWidth;
        float screenHeight = _graphics.PreferredBackBufferHeight;

        Matrix cameraMatrix = Matrix.CreateTranslation(new Vector3(-screenWidth / 2, -screenHeight / 2, 0)) *
                              Matrix.CreateScale(_zoom) *
                              Matrix.CreateTranslation(new Vector3(screenWidth / 2, screenHeight / 2, 0));

        // Start SpriteBatch with our Shader
        _spriteBatch.Begin(
            SpriteSortMode.Immediate, // Immediate mode lets us change shader params per-draw
            BlendState.AlphaBlend,
            SamplerState.LinearClamp, // Essential for the sub-pixel smoothing
            null, null,
            _smoothShader,
            cameraMatrix);

        // Pass the specific texture size to the shader
        _smoothShader.Parameters["TextureSize"].SetValue(new Vector2(_mySprite.Width, _mySprite.Height));

        // Draw at a smooth float position
        _spriteBatch.Draw(_mySprite, _spritePos, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}