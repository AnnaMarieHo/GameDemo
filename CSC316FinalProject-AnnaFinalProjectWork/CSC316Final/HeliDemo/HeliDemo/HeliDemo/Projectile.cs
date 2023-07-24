using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace HeliDemo
{
    internal class Projectile
    {
        Vector3 pos;
        Vector3 dir;
        float speed;
        float radius;

        public BoundingSphere ProjectileBounds
        {
            get { return new BoundingSphere(pos, radius); }
        }

        public Vector3 Pos { get { return pos; }  }

        public Vector3 Velocity { 
            get { return dir * speed;}
            set
            {
                dir = Vector3.Normalize(value);
                speed = value.Length();
            }
        }

        public Projectile(Vector3 p, Vector3 v)
        {
            radius = 2f;
            pos = p;
            dir = Vector3.Normalize(v);
            speed = v.Length() + 2f;
        }

        public void Update(GameTime gameTime) {
            Velocity += new Vector3(0, -0.05f, 0);
            pos += Velocity;
        }
    }
}
