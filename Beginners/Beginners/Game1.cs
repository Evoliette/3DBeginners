using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Beginners
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int max_models = 5;

        //Camera
        Vector3 camPosition;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix[] worldMatrix = new Matrix[max_models];

        //Geometric info
        Model[] model = new Model[max_models];
        Vector3[] modelPosition = new Vector3[max_models];
        float range = 10;
        
        //Orbit
        bool orbit = false;

        //Sonstiges
        Random random = new Random();
        bool hit;
        float radius = 2f;
        bool gameOver;
        bool winner;
        Texture2D looser;
        Texture2D won;
        SpriteFont counter;
        int timer = 10000;
       

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            //Setup Camera
            camPosition = new Vector3(0f, 15f, 10f);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45f), graphics.
                               GraphicsDevice.Viewport.AspectRatio,
                1f, 1000f);
            viewMatrix = Matrix.CreateLookAt(camPosition, new Vector3(0, 0, -5),
                         new Vector3(0f, 1f, 0f));// Y up

            modelPosition[0] = new Vector3(0f, 0f, 0f);
            modelPosition[1] = new Vector3((float)random.NextDouble() * range - range, 0, -30f);
            modelPosition[2] = new Vector3((float)random.NextDouble() * range + range, 0, -30f);
            modelPosition[3] = new Vector3((float)random.NextDouble() * range * 2 - range, 0, -30f);
            modelPosition[4] = new Vector3((float)random.NextDouble() * range * 2 - range, 0, -30f);

            worldMatrix[0] = Matrix.CreateWorld(modelPosition[0], Vector3.
                          Forward, Vector3.Up);
            worldMatrix[1] = Matrix.CreateWorld(modelPosition[1], Vector3.
                          Forward, Vector3.Up);
            worldMatrix[2] = Matrix.CreateWorld(modelPosition[2], Vector3.
                          Forward, Vector3.Up);
            worldMatrix[3] = Matrix.CreateWorld(modelPosition[3], Vector3.
                          Forward, Vector3.Up);
            worldMatrix[4] = Matrix.CreateWorld(modelPosition[3], Vector3.
                          Forward, Vector3.Up);

            model[0] = Content.Load<Model>("acagamics6");
            model[1] = Content.Load<Model>("bullets");
            model[2] = Content.Load<Model>("bullets");
            model[3] = Content.Load<Model>("bullets");
            model[4] = Content.Load<Model>("bullets");



        }
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            looser = Content.Load<Texture2D>("YouLost2");
            won = Content.Load<Texture2D>("win");
            counter = Content.Load<SpriteFont>("NewSpriteFont");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed || Keyboard.GetState().IsKeyDown(
                Keys.Escape))
                Exit();

        if (!gameOver && !winner) { 
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                modelPosition[0].X -= 0.3f;
                if(modelPosition[0].X < -range)
                {
                    modelPosition[0].X = -range;
                }
            }

            timer = timer - gameTime.ElapsedGameTime.Milliseconds;

            modelPosition[1].Z += 0.55f;
            modelPosition[2].Z += 0.4f;
            modelPosition[3].Z += 0.45f;
            modelPosition[4].Z += 0.5f;

            for (int i = 1; i <= max_models-1; i++)
            {
                if (modelPosition[i].Z >= 1.5f)
                {
                    modelPosition[i] = new Vector3((float)random.NextDouble() * range * 2 - range, 0, -30f);
                }
            }
        

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                modelPosition[0].X += 0.3f;
                if(modelPosition[0].X > range)
                {
                    modelPosition[0].X = range;
                }
            }
   
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                orbit = !orbit;
            }

            if (orbit)
            {
                Matrix rotationMatrix = Matrix.CreateRotationY(
                                        MathHelper.ToRadians(1f));
                camPosition = Vector3.Transform(camPosition,
                              rotationMatrix);
            }

            if (timer == 0)
                {
                    winner = true;
                }

            hit = false;

                for (int i = 1; i <= max_models - 1; i++)
                {

                    if (!hit)
                    {
                        if (Math.Abs(modelPosition[i].X - modelPosition[0].X) <= radius && Math.Abs(modelPosition[i].Z - modelPosition[0].Z) <= radius)
                        {
                            hit = true;
                        }
                    }
                }

                if (hit)
                {
                    gameOver = true;
                       
                   }

         
    //viewMatrix = Matrix.CreateLookAt(camPosition, camTarget,
    //             Vector3.Up);

    //camTarget = modelPosition;
    //so guckt die Kamera auf Position des Spielers

    worldMatrix[0] = Matrix.CreateWorld(modelPosition[0], Vector3.Forward, Vector3.Up);
            worldMatrix[1] = Matrix.CreateWorld(modelPosition[1], Vector3.Forward, Vector3.Up);
            worldMatrix[2] = Matrix.CreateWorld(modelPosition[2], Vector3.Forward, Vector3.Up);
            worldMatrix[3] = Matrix.CreateWorld(modelPosition[3], Vector3.Forward, Vector3.Up);
            worldMatrix[4] = Matrix.CreateWorld(modelPosition[4], Vector3.Forward, Vector3.Up);
                //aktualisiert die Figurpositionen
            }

        else if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                for (int j = 1; j <= max_models - 1; j++)
                {
                    modelPosition[j] = new Vector3((float)random.NextDouble() * range * 2 - range, 0, -30f);
                }
                gameOver = false;
                winner = false;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            for (int i = 0; i < model.Length; i++)
            {
                foreach (ModelMesh mesh in model[i].Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.AmbientLightColor = new Vector3(1, 1, 1);
                        effect.View = viewMatrix;
                        effect.World = worldMatrix[i];
                        effect.Projection = projectionMatrix;
                        if(i == 0)
                        {
                            effect.DiffuseColor = new Vector3(.7f, .2f, 0);
                        }
                        else if (i == 1)
                        {
                            effect.DiffuseColor = new Vector3(.5f,.5f,.5f);
                        }
                    }
                    mesh.Draw();
                }
            }
            spriteBatch.Begin();
            if (gameOver)
            {         
                spriteBatch.Draw(looser, new Rectangle(0,0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            }
            if(winner)
             { 
                spriteBatch.Draw(won, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
             }

        spriteBatch.DrawString(counter, timer.ToString(), new Vector2(0, 0), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
