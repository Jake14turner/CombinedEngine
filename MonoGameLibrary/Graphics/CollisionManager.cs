using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGameLibrary.Graphics
{
    public class CollisionManager : SpriteManager
    {
        public CollisionManager()
            : base()   
        {
        }

        public CollisionManager(List<Sprite> sprites)
            : base()
        {
            Sprites = sprites;    
        }

        public bool IsColliding(int s1, int s2)
        {
            if (s1 < 0 || s1 >= Sprites.Count + 1 || s2 < 0 || s2 >= Sprites.Count)
                return false;

            Sprite spriteOne = Sprites.Where(x => x.Id == s1).First();
            Sprite spriteTwo = Sprites.Where(x => x.Id == s2).First();

            Circle s1Box = new Circle(
                (int)(spriteOne.Location.X + spriteOne.Width * 0.5f),
                (int)(spriteOne.Location.Y + spriteOne.Height * 0.5f),
                (int)(spriteOne.Width * 0.5f)
            );

            Circle s2Box = new Circle(
                (int)(spriteTwo.Location.X + spriteTwo.Width * 0.5f),
                (int)(spriteTwo.Location.Y + spriteTwo.Height * 0.5f),
                (int)(spriteTwo.Width * 0.5f)
            );

            return s1Box.Intersects(s2Box);
        }

        public void CheckIsOffScreen(int s1, PresentationParameters screen)
        {
            if (s1 < 0 || s1 >= Sprites.Count)
                return;

            Sprite spriteOne = Sprites.Where(x => x.Id == s1).FirstOrDefault();

            Rectangle screenBounds = new Rectangle(0, 0,
                screen.BackBufferWidth,
                screen.BackBufferHeight);

            Circle s1Box = new Circle(
                (int)(spriteOne.Location.X + spriteOne.Width * 0.5f),
                (int)(spriteOne.Location.Y + spriteOne.Height * 0.5f),
                (int)(spriteOne.Width * 0.5f)
            );

            Vector2 loc = spriteOne.Location;

            // Clamp X
            if (s1Box.Left < screenBounds.Left)
                loc.X = screenBounds.Left;
            else if (s1Box.Right > screenBounds.Right)
                loc.X = screenBounds.Right - spriteOne.Width;

            // Clamp Y
            if (s1Box.Top < screenBounds.Top)
                loc.Y = screenBounds.Top;
            else if (s1Box.Bottom > screenBounds.Bottom)
                loc.Y = screenBounds.Bottom - spriteOne.Height;

            spriteOne.Location = loc;
        }

        public Vector2 HandleBat(int s1, PresentationParameters screen, Vector2 newBatPosition, Vector2 batVelocity)
        {
            Sprite _bat = Sprites.Where(x => x.Id == s1).FirstOrDefault();

            Circle batBounds = new Circle(
            (int)(newBatPosition.X + (_bat.Width * 0.5f)),
            (int)(newBatPosition.Y + (_bat.Height * 0.5f)),
            (int)(_bat.Width * 0.5f)
            );
            Vector2 normal = Vector2.Zero;

            Rectangle screenBounds = new Rectangle(0, 0,
                screen.BackBufferWidth,
                screen.BackBufferHeight);

            // Use distance based checks to determine if the bat is within the
            // bounds of the game screen, and if it is outside that screen edge,
            // reflect it about the screen edge normal.
            if (batBounds.Left < screenBounds.Left)
            {
                normal.X = Vector2.UnitX.X;
                newBatPosition.X = screenBounds.Left;
            }
            else if (batBounds.Right > screenBounds.Right)
            {
                normal.X = -Vector2.UnitX.X;
                newBatPosition.X = screenBounds.Right - _bat.Width;
            }

            if (batBounds.Top < screenBounds.Top)
            {
                normal.Y = Vector2.UnitY.Y;
                newBatPosition.Y = screenBounds.Top;
            }
            else if (batBounds.Bottom > screenBounds.Bottom)
            {
                normal.Y = -Vector2.UnitY.Y;
                newBatPosition.Y = screenBounds.Bottom - _bat.Height;
            }

            // If the normal is anything but Vector2.Zero, this means the bat had
            // moved outside the screen edge so we should reflect it about the
            // normal.
            if (normal != Vector2.Zero)
            {
                normal.Normalize();
                batVelocity = Vector2.Reflect(batVelocity, normal);
            }

            _bat.Location = newBatPosition;

            return batVelocity;
        }
    }
}