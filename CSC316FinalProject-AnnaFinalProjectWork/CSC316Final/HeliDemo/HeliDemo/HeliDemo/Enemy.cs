using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;




namespace HeliDemo
{
    internal class Enemy
    {  

        static Random rand;
        List<Vector3> enemyPos;
        float rot;
        Vector3 pos, velocity;
        float speed;
        double delay;
        List<Projectile> projectiles;
        List<Explosion> explosions;
        Texture2D ex;
        int hitCount;
        bool singleShot;
        float radius;
       
        private SpriteBatch _spriteBatch;





        public enum EnemyBehavior { Random, Freeze, Attack }
        public enum EnemyType { Building, Helicopter }
        
        public BoundingSphere EnemyBounds
        {
            get { return new BoundingSphere(pos, radius);  }
        }

        public int Hits
        {
            get { return hitCount; }
            set { hitCount = value; }
        }
        
        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Vector3 Pos 
        { 
            get { return pos; } 
            set { pos = value; }
        }

        public float Rot 
        { 
            get { return rot; } 
            set { rot = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public List<Projectile> Projectiles
        {
            get { return projectiles; }
            set { projectiles = value; }
        }

        EnemyType type;
        public EnemyType Type 
        { 
            get { return type; } 
            set { type = value; }
        }

        EnemyBehavior behavior;
        public EnemyBehavior Behavior
        {
            get { return behavior; }
            set { behavior = value; }
        }

        public Vector3 enemyForward
        {
            get { return Vector3.Transform(velocity, Matrix.CreateRotationY(rot)); }
        }



        public Enemy(Model model) 
        {

            if (rand == null)
            {
                rand = new Random();
            }

            //radius = model.Meshes[0].BoundingSphere.Radius;
            radius = 5.5f;

            pos = new Vector3(rand.Next(-100, 100), 7f, rand.Next(-200, 200));
            rot = MathHelper.ToRadians(rand.Next(0, 360));
            velocity = Vector3.Backward;

            type = (EnemyType)rand.Next(0, 2);
            behavior = EnemyBehavior.Random;
            speed = 0.2f;
            delay = 0;
            //radius = 20f;
            singleShot= true;
            projectiles = new List<Projectile>();
            explosions = new List<Explosion>();

        }

        public void loadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);

            ex = content.Load<Texture2D>("explosionsheet");

        }


        public void Update(GameTime gameTime, Vector3 playerPos, Texture2D ex, int playerLives)
        {
            delay = delay - 0.04;


            switch (behavior)
            {
                case EnemyBehavior.Random:
                    pos += enemyForward * speed;
                    rot += MathHelper.ToRadians(rand.Next(-3, 3));
                   
                    break;

                case EnemyBehavior.Attack:
                    Shoot(playerPos, gameTime, ex, playerLives);
                   
                    break;

                case EnemyBehavior.Freeze:
                    break;

                default: break;
            }
        }

        public void checkCollision(Enemy other)
        {
            if (new BoundingSphere(this.pos, this.radius).Intersects(new BoundingSphere(other.pos, other.radius)))
            {
                Vector3 axis = other.pos - this.pos;
                float dist = other.radius + this.radius;
                float move = (dist - axis.Length()) / 2f;
                axis.Normalize();

                Vector3 U1x = axis * Vector3.Dot(axis, this.velocity);
                Vector3 U1y = this.velocity - U1x;

                Vector3 U2x = -axis * Vector3.Dot(-axis, other.velocity);
                Vector3 U2y = other.velocity - U2x;

                this.velocity = U1x + U1y;
                other.velocity = U2x + U2y;

                //this.velocity *=  1f;
                //other.velocity *= 1f;

                this.pos -= axis * move;
                other.pos += axis * move;

            }
        }


        public void Shoot(Vector3 playerPos, GameTime gameTime, Texture2D ex, int playerLives)
        {

            //Create new bullet
            if(delay <= 0)
            {
                rot = (float)Math.Atan2(playerPos.X - pos.X, playerPos.Z - pos.Z);
                Projectile newProjectile = new Projectile(pos, enemyForward * MathHelper.Max(Speed, 0.000001f));
                projectiles.Add(newProjectile);
                delay = 2;
            }
            //Update bullet velocity and position, then remove bullet
            for (int z = 0; z < projectiles.Count; z++)
            {
                projectiles[z].Update(gameTime);
                if (projectiles[z].Pos.Y <= 0.0f)
                {
                    projectiles.RemoveAt(z);
                    z--;
                    continue;
                }
            }

        }

        public void RespondToPlayer(Vector3 playerPos)
        {
            if (Vector3.Distance(pos, playerPos) < 40)
            {
                behavior = EnemyBehavior.Freeze;
                behavior = EnemyBehavior.Attack;


            }
            else if (Vector3.Distance(pos, playerPos) < 60)
            {
                speed = 0.45f;

                pos += enemyForward * speed;

                behavior = EnemyBehavior.Attack;
            }
            else
            {
                speed = 0.2f;
                behavior = EnemyBehavior.Random;
            }
        }

        public void Draw( Matrix view, Matrix projection, Model bullet)
        {
            foreach (Projectile p in projectiles)
            {
                bullet.Draw(Matrix.CreateTranslation(p.Pos),
                   view, projection);
            }

        }

    }
}
