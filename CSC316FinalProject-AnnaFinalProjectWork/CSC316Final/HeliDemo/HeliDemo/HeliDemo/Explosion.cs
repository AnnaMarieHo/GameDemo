
using Microsoft.Xna.Framework;

namespace HeliDemo
{
    internal class Explosion
    {
        public Vector3 pos;
        public float scale;
        public AnimatedSprite animatedSprite;
        public Explosion(Vector3 p, float s)
        {
            pos = p; 
            scale = s;
        }
    }
}
