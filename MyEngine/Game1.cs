using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System.Collections.Generic;
using MonoGameLibrary.Input;
using System;

namespace MyEngine;

public class Game1 : Core
{
    private AnimatedSprite _slime;
    private AnimatedSprite _bat;

    private float movementSpeed = 5.0f;
    private const float MIN_SPEED = 1.5f;
    private const float MAX_SPEED = 8f;

    private Queue<Vector2> _inputBuffer;
    private const int MAX_BUFFER_SIZE = 2;

    private Vector2 _batVelocity;

    public Game1() : base("Dungeon Slime", 1280, 720, false)
    {
        
    }

    protected override void Initialize()
    {
        _inputBuffer = new Queue<Vector2>(MAX_BUFFER_SIZE);

     

        base.Initialize();
    }

    protected override void LoadContent()
    {
        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");

        _slime = atlas.CreateAnimatedSprite("slime-animation");
        SpriteManager.Add(_slime);
        _bat = atlas.CreateAnimatedSprite("bat-animation");
        SpriteManager.Add(_bat);


        _bat.Location = new Vector2(_slime.Width + 10, 0);
        AssignRandomBatVelocity();

        _slime.Scale = new Vector2(4.0f, 4.0f);
        _bat.Scale = new Vector2(4.0f, 4.0f);
        
    }

    //game logic
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _slime.Update(gameTime);
        _bat.Update(gameTime);

        CheckKeyboardInput();
        UpdateSlimePosition();
        BoundingCheck();

        base.Update(gameTime);
    }

    //visuals
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);


        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _slime.Draw(SpriteBatch);
        _bat.Draw(SpriteBatch);

        SpriteBatch.End();


        base.Draw(gameTime);
    }

    private void BoundingCheck()
    {
        Vector2 newBatPosition = _bat.Location + _batVelocity;


        _batVelocity = CollisionManager.HandleBat(_bat.Id, GraphicsDevice.PresentationParameters, newBatPosition, _batVelocity);


        CollisionManager.CheckIsOffScreen(_bat.Id, GraphicsDevice.PresentationParameters);
        _bat.Location = newBatPosition;
        if (CollisionManager.IsColliding(_bat.Id, _slime.Id))
        {
            int totalColumns = GraphicsDevice.PresentationParameters.BackBufferWidth / (int)_bat.Width;
            int totalRows = GraphicsDevice.PresentationParameters.BackBufferHeight / (int)_bat.Height;

            int column = Random.Shared.Next(0, totalColumns);
            int row = Random.Shared.Next(0, totalRows);

            _bat.Location = new Vector2(column * _bat.Width, row * _bat.Height);
            AssignRandomBatVelocity();
        }
    }



    private void AssignRandomBatVelocity()
    {
        // Generate a random angle.
        float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

        // Convert angle to a direction vector.
        float x = (float)Math.Cos(angle);
        float y = (float)Math.Sin(angle);
        Vector2 direction = new Vector2(x, y);

        // Multiply the direction vector by the movement speed.
        _batVelocity = direction * movementSpeed;
    }

    private void UpdateSlimePosition()
    {
        if (_inputBuffer.Count > 0)
        {
            Vector2 nextDirection = _inputBuffer.Dequeue();

            _slime.Location += nextDirection * movementSpeed;

            CollisionManager.CheckIsOffScreen(_slime.Id, GraphicsDevice.PresentationParameters);
        }
    }

    private void CheckKeyboardInput()
    {
        KeyboardState keyboardState = Keyboard.GetState();
        Vector2 newDirection = Vector2.Zero;

        // If the space key is held down, the movement speed increases by 1.5
        if (Input.Keyboard.IsKeyHeldDown(Keys.Space))
        {
            if(movementSpeed < MAX_SPEED)
            {
                movementSpeed *= 1.5f;
            }
        } else
        {
            movementSpeed = movementSpeed <= MIN_SPEED ? MIN_SPEED : movementSpeed / 1.5f;
        }

        // If the W or Up keys are down, move the slime up on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.W) || Input.Keyboard.IsKeyDown(Keys.Up))
        {
            newDirection.Y -= 1;
        }

        // if the S or Down keys are down, move the slime down on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.S) || Input.Keyboard.IsKeyDown(Keys.Down))
        {
            newDirection.Y += 1;
        }

        // If the A or Left keys are down, move the slime left on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.A) || Input.Keyboard.IsKeyDown(Keys.Left))
        {
            newDirection.X -= 1;
        }

        // If the D or Right keys are down, move the slime right on the screen.
        if (Input.Keyboard.IsKeyDown(Keys.D) || Input.Keyboard.IsKeyDown(Keys.Right))
        {
            newDirection.X += 1;
        }

        if (newDirection != Vector2.Zero && _inputBuffer.Count < MAX_BUFFER_SIZE)
        {
            _inputBuffer.Enqueue(Vector2.Normalize(newDirection));
        }
    }
}
