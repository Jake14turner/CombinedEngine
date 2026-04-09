using System.Collections.Generic;

namespace MonoGameLibrary.Graphics
{
    public class SpriteManager
    {
        public List<Sprite> Sprites { get; set; } = new();

        public Sprite Add(Sprite s)
        {
            Sprites.Add(s);   // This works for both Sprite and AnimatedSprite
            return s;
        }

        // Optional: You can keep a convenience overload if you really want it,
        // but it's not necessary.
        // public AnimatedSprite Add(AnimatedSprite s) => (AnimatedSprite)Add(s);
    }
}