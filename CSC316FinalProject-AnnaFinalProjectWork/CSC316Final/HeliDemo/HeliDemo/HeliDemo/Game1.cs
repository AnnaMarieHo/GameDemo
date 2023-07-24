using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace HeliDemo
{
    public class Game1 : Game
    {
        public static GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //abcd
        Matrix world;
        Matrix view;
        Matrix projection;
        int hits;

        Model HelicopterModel, Zebu;
        float rotY, playerRadius;
        Vector3 heliPos;
        float heliSpeed;
        bool heliFlying;
        bool heliJustStart;
        bool singleShot;
        double delay;
        SpriteFont font;
        float XDist, YDist, actualDist;

        Model buildingModel, terrainModel, bulletModel;
        List<Vector3> enemyPos;

        List<Projectile> projectiles;
        KeyboardState preKb;
        MouseState click;

        //Texture2D explosion;
        List<Explosion> explosions;

        Texture2D ex;
        AnimatedSprite explosion;

        Viewport defaultViewport, miniViewport;

        List<Enemy> enemies;
        List<Projectile> enemyProjectile;
        double enemyDelay;
        String multi, single, shotType;

        Matrix view2;

        public BoundingSphere PlayerBounds
        {
            get { return new BoundingSphere(heliPos, playerRadius); }
        }


        public Vector3 HeliForward
        {
            get
            {
                return Vector3.Transform(Vector3.Backward, Matrix.CreateRotationY(rotY));
            }
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            singleShot = true;
            delay = 0;
            enemyDelay= 0;
            rotY = 0;
            heliSpeed = 0;
            heliFlying = false;
            heliJustStart = true;
            heliPos = new Vector3(10, 5, 10);
            explosions = new List<Explosion>();
            hits = 30;
            shotType = "";


            defaultViewport = GraphicsDevice.Viewport;
            miniViewport = new Viewport(10, 10, 160, 120);

            projectiles = new List<Projectile>();
            enemyProjectile = new List<Projectile>();

            preKb = Keyboard.GetState();
            click = Mouse.GetState();

          
            view = Matrix.CreateLookAt(
                    new Vector3(45, 45, 45),
                    Vector3.Zero,
                    new Vector3(0, 0, 1));

            view2 = Matrix.CreateLookAt(
                        new Vector3(45, 45, 45),
                        Vector3.Zero,
                        new Vector3(0, 0, 1));

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                1,
                0.001f,
                1000f);

            world = Matrix.CreateScale(2) * Matrix.CreateTranslation(heliPos);
           

            enemies = new List<Enemy>();

          
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            HelicopterModel = Content.Load<Model>("heli");
            terrainModel = Content.Load<Model>("Ground");
            buildingModel = Content.Load<Model>("building");
            bulletModel = Content.Load<Model>("bullet");
            Zebu = Content.Load<Model>("Animal_Rigged_Zebu_01");
            ex = Content.Load<Texture2D>("explosionsheet");
            font = Content.Load<SpriteFont>("galleryFont2");

            for (int i = 0; i < 10; i++)
            {
                enemies.Add(new Enemy(HelicopterModel));
            }

        }

       

        protected override void Update(GameTime gameTime)
        {
            delay = delay - 0.04;
            enemyDelay = enemyDelay - 0.04;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            #region PlayerMovement
            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.A))
                rotY += 0.07f;
            else if (kb.IsKeyDown(Keys.D))
                rotY -= 0.07f;

            if (kb.IsKeyDown(Keys.W)) { 
                
                heliPos += HeliForward * heliSpeed;
                heliPos.Y += 0.5f;
                heliSpeed += 0.007f; //Original Speed: 0.015f
                heliSpeed = MathHelper.Min(heliSpeed, 0.6f); //Original Speed: 1.6f
                heliPos.Y = MathHelper.Min(heliPos.Y, 13f);
            }
            else
            {
                heliPos += HeliForward * heliSpeed;
                heliSpeed -= 0.025f;
                heliSpeed = MathHelper.Max(heliSpeed, 0f);

                heliPos.Y -= 0.5f;
                heliPos.Y = MathHelper.Max(heliPos.Y, 5f);
            }


            if (heliSpeed > 0)
                heliFlying = true;
            else
                heliFlying = false;
            #endregion

            #region PlayerProjectiles
            if (kb.IsKeyDown(Keys.LeftShift))
            {
                singleShot = !singleShot;

            }
           



            MouseState click = Mouse.GetState();


            // Projectiles
            if (click.LeftButton == ButtonState.Pressed && delay <=0 && singleShot)
            {
                projectiles.Add(new Projectile(heliPos,
                    HeliForward * MathHelper.Max(heliSpeed, 0.000001f)));

                delay = 1;
                
            }

            if (click.LeftButton == ButtonState.Pressed && delay <= 0 && !singleShot)
            {
                float angle = MathHelper.ToRadians(10); 
                Vector3 left = Vector3.Transform(HeliForward, Matrix.CreateRotationZ(-angle));
                Vector3 right = Vector3.Transform(HeliForward, Matrix.CreateRotationZ(angle)); 

                projectiles.Add(new Projectile(heliPos, HeliForward * MathHelper.Max(heliSpeed, 0.000001f)));
                projectiles.Add(new Projectile(heliPos, left * MathHelper.Max(heliSpeed, 0.000001f)));
                projectiles.Add(new Projectile(heliPos, right * MathHelper.Max(heliSpeed, 0.000001f)));

                delay = 2;

            }

            // update projectiles 
            
            for (int p=0; p < projectiles.Count; p++)
            {
                projectiles[p].Update(gameTime);

                // hit ground
                if (projectiles[p].Pos.Y <= 0.0f)
                {
                    explosions.Add(new Explosion(projectiles[p].Pos, 0.2f));
                    SpriteSheet explosionsheet = new SpriteSheet(ex);
                    explosions[explosions.Count - 1].animatedSprite =
                        new AnimatedSprite(explosionsheet, 100, 100, 0, 3, 5, Point.Zero, 15);

                    projectiles.RemoveAt(p);
                    p--;
                    continue;
                }
                //hit building
                for (int b = 0; b < enemies.Count; b++)
                {
                    if (Vector3.Distance(enemies[b].Pos, projectiles[p].Pos) < 20f)
                    {
                        explosions.Add(new Explosion(projectiles[p].Pos, 1f));
                        SpriteSheet explosionsheet = new SpriteSheet(ex);
                        explosions[explosions.Count - 1].animatedSprite =
                            new AnimatedSprite(explosionsheet, 100, 100, 0, 3, 5, Point.Zero, 15);

                        projectiles.RemoveAt(p);
                        enemies.RemoveAt(b);
                        p--;
                        break;
                    }
                }
            }



            #endregion


            if (gameTime.TotalGameTime.Milliseconds % 10 == 0)
            {
                for (int e = 0; e < explosions.Count; e++)
                {
                    if (explosions[e].animatedSprite != null)
                    {
                        explosions[e].animatedSprite.IncrementAnimationFrame();
                        if (explosions[e].animatedSprite.CurrentFrame == 14)
                        {
                            explosions.RemoveAt(e);
                            e--;
                            continue;
                        }
                    }
                }
            }

            world = Matrix.CreateRotationY(MathHelper.PiOver2) *
                Matrix.CreateScale(2) * 
                Matrix.CreateRotationY(rotY) *
                Matrix.CreateTranslation(heliPos);

            // Update the view matrix

            view = Matrix.CreateLookAt(heliPos + Vector3.Transform(new Vector3(-5, 40, -75), Matrix.CreateRotationY(rotY)), heliPos, Vector3.Up);

            view2 = Matrix.CreateLookAt(heliPos + Vector3.Transform(new Vector3(-5, 90, -100), Matrix.CreateRotationY(rotY)), heliPos, Vector3.Up);

            preKb = kb;

            foreach (Enemy e in enemies)
            {
                e.RespondToPlayer(heliPos);
                e.Update(gameTime, heliPos, ex, hits);

                foreach(Enemy e2 in enemies)
                {
                    if (e != e2)
                    {
                        e.checkCollision(e2);
                    }

                }
                
                for (int p = 0; p < e.Projectiles.Count; p++)
                {
                    if (Vector3.Distance(heliPos, e.Projectiles[p].Pos) < 20f && Vector3.Distance(heliPos, e.Projectiles[p].Pos) > 19f)
                    {
                        hits--;
                    }
                    p--;
                    break;
                }
                
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            #region DefaultViewport
            GraphicsDevice.Viewport = defaultViewport;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            terrainModel.Draw(Matrix.CreateScale(0.005f), view, projection);

            //Draw Enemies
            foreach (Enemy e in enemies)
            {
                foreach (ModelMesh building in buildingModel.Meshes)
                {
                    foreach (BasicEffect effects in building.Effects)
                    {

                        effects.World = Matrix.CreateScale(10f) *
                                        Matrix.CreateRotationY(e.Rot) *
                                        Matrix.CreateTranslation(e.Pos);

                        effects.View = view;
                        effects.Projection = projection;
                        effects.EnableDefaultLighting();

                    }

                    building.Draw();
                }
               
                e.Draw(view, projection, bulletModel);
            }



            //Draw helicopter
            foreach (ModelMesh mesh in HelicopterModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    if (mesh.Name == "pCube3" && heliFlying)
                    {
                        effect.World = Matrix.CreateScale(2)
                            * Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalMilliseconds)
                            * Matrix.CreateTranslation(heliPos);
                    }
                    else
                        effect.World = world;
                        effect.View = view;
                        effect.Projection = projection;
                        effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }

            foreach( Projectile p in projectiles)
            {
                bulletModel.Draw(Matrix.CreateTranslation(p.Pos),
                    view, projection);
            }

            
            _spriteBatch.Begin();
            
            foreach (Explosion e in explosions)
            {
                Vector3 p = _graphics.GraphicsDevice.Viewport.Project(e.pos,
                    projection, view, Matrix.Identity);
                
                Vector2 screenP = new Vector2(p.X, p.Y) -
                    new Vector2(100 / 2f, 100) * e.scale;

                
                e.animatedSprite.Position = screenP;
                e.animatedSprite.ScaleValue = new Vector2(e.scale);
                e.animatedSprite.Draw(_spriteBatch, Color.White);
            }
            
            foreach (Enemy e in enemies)
            {
                _spriteBatch.DrawString(font, "Lives: " + hits, new Vector2(625, 40), Color.White);
            }
            if (enemies.Count <= 0 && hits > 0)
            {
                _spriteBatch.DrawString(font, "All Enemies Cleared", new Vector2(290, 140), Color.White);
            }
            if (hits <= 0)
            {
                _spriteBatch.DrawString(font, "You Lose", new Vector2(320, 160), Color.White);
                for (int b = 0; b < enemies.Count; b++)
                {
                    
                    enemies.RemoveAt(b);
                       
                }
            }

            _spriteBatch.End();
            #endregion

            
            #region MiniMap


            GraphicsDevice.Viewport = miniViewport;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            terrainModel.Draw(Matrix.CreateScale(0.005f), view2, projection);

            //MiniMap Draw Enemy
            foreach (Enemy e in enemies)
            {
                foreach (ModelMesh building in buildingModel.Meshes)
                {
                    foreach (BasicEffect effects in building.Effects)
                    {

                        effects.World = Matrix.CreateScale(10) *
                                        Matrix.CreateRotationY(e.Rot) *
                                        Matrix.CreateTranslation(e.Pos);

                        effects.View = view2;
                        effects.Projection = projection;
                        effects.EnableDefaultLighting();

                    }

                    building.Draw();
                }
                e.Draw(view2, projection, bulletModel);
            }
             

            //MiniMap Draw Player
            foreach (ModelMesh mesh in HelicopterModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    if (mesh.Name == "pCube3" && heliFlying)
                    {
                        effect.World = Matrix.CreateScale(2)
                            * Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalMilliseconds)
                            * Matrix.CreateTranslation(heliPos);
                    }
                    else
                    effect.World = world;
                    effect.View = view2;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }
            

            _spriteBatch.Begin();
            foreach (Explosion e in explosions)
            {
                Vector3 p = _graphics.GraphicsDevice.Viewport.Project(e.pos,
                    projection, view2, Matrix.Identity);
                
                Vector2 screenP = new Vector2(p.X, p.Y) -
                    new Vector2(100 / 2f, 100) * e.scale;

                e.animatedSprite.Position = screenP;
                e.animatedSprite.ScaleValue = new Vector2(e.scale * .5f);
                e.animatedSprite.Draw(_spriteBatch, Color.White);
            }
            _spriteBatch.End();
            
            #endregion
            

            base.Draw(gameTime);
        }
    }
}