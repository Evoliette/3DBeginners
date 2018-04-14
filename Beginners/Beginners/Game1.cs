using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Beginners
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //constants and values
        const int max_models = 5;
        float radius = 2f;
        float range = 10;
        int timer;
        int record;

        //camera
        Vector3 camPosition;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix[] worldMatrix = new Matrix[max_models];

        //geometrics and objects
        Model[] model = new Model[max_models];
        Vector3[] modelPosition = new Vector3[max_models];
        Texture2D looser;
        Texture2D won;
        SpriteFont counter;
        SpriteFont recordCounter;

        //stuff
        Random random = new Random();
        bool hit;
        bool gameOver;
        bool winner;
 
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            //camera
            camPosition = new Vector3(0f, 15f, 10f);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45f), graphics.
                               GraphicsDevice.Viewport.AspectRatio,
                               1f, 1000f);
            viewMatrix = Matrix.CreateLookAt(camPosition, new Vector3(0, 0, -5),
                         new Vector3(0f, 1f, 0f));// Y up

            //positions
            modelPosition[0] = new Vector3(0f, 0f, 0f);

            modelPosition[1] = new Vector3((float)random.NextDouble() * range - range, 0, -30f);
            modelPosition[2] = new Vector3((float)random.NextDouble() * range + range, 0, -30f);
            modelPosition[3] = new Vector3((float)random.NextDouble() * range * 2 - range, 0, -30f);
            modelPosition[4] = new Vector3((float)random.NextDouble() * range * 2 - range, 0, -30f);

            for(int i=1; i<max_models; i++)
            {
            worldMatrix[i] = Matrix.CreateWorld(modelPosition[i], Vector3.Forward, Vector3.Up);
            }

            //timer
            timer = 30000;
            record = 0;
        }

        protected override void LoadContent()
        {
            model[0] = Content.Load<Model>("acagamics6");

            for (int i=1; i<max_models; i++)
            {
                model[i] = Content.Load<Model>("bullets");
            }

            spriteBatch = new SpriteBatch(GraphicsDevice);
            looser = Content.Load<Texture2D>("YouLost2");
            won = Content.Load<Texture2D>("win");
            counter = Content.Load<SpriteFont>("NewSpriteFont");
            recordCounter = Content.Load<SpriteFont>("counterSprite");
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
            //close game


            //gameplay
            if (!gameOver && !winner)
            {

                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    modelPosition[0].X -= 0.3f;
                    if (modelPosition[0].X < -range)
                    {
                        modelPosition[0].X = -range;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    modelPosition[0].X += 0.3f;
                    if (modelPosition[0].X > range)
                    {
                        modelPosition[0].X = range;
                    }
                }

                modelPosition[1].Z += 0.55f;
                modelPosition[2].Z += 0.4f;
                modelPosition[3].Z += 0.45f;
                modelPosition[4].Z += 0.5f;
                //speed of the bullets

                for (int i = 1; i <= max_models - 1; i++)
                {
                    if (modelPosition[i].Z >= 1.5f)
                    {
                        modelPosition[i] = new Vector3((float)random.NextDouble() * range * 2 - range, 0, -30f);
                    }
                }
                //set back the bullets

                timer = timer - gameTime.ElapsedGameTime.Milliseconds;
                if (timer <= 0)
                {
                    winner = true;
                    record++;
                }
                //timer

                //collision
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
                    record = 0;

                }

                //viewMatrix = Matrix.CreateLookAt(camPosition, camTarget,Vector3.Up);
                //camTarget = modelPosition;
                //camera follows player

                for (int i = 0; i < max_models; i++)
                {
                    worldMatrix[i] = Matrix.CreateWorld(modelPosition[i], Vector3.Forward, Vector3.Up);
                }
                //updates positions
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                for (int j = 1; j <= max_models - 1; j++)
                {
                    modelPosition[j] = new Vector3((float)random.NextDouble() * range * 2 - range, 0, -30f);
                }
                gameOver = false;
                winner = false;
                timer = 30000;  
            }
            //restart

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
                        effect.AmbientLightColor = new Vector3(1, 1, 1);    //color of light
                        effect.View = viewMatrix;
                        effect.World = worldMatrix[i];
                        effect.Projection = projectionMatrix;
                        if(i == 0)
                        {
                            effect.DiffuseColor = new Vector3(.7f, .2f, 0); //player = orange
                        }
                        else 
                        {
                            effect.DiffuseColor = new Vector3(.5f,.5f,.5f); //others = white
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
                spriteBatch.Draw(won, new Rectangle(100, 0, graphics.PreferredBackBufferWidth - 200, graphics.PreferredBackBufferHeight), Color.White);
            }

        spriteBatch.DrawString(counter, timer.ToString(), new Vector2(0, 0), Color.White);
        spriteBatch.DrawString(recordCounter,"wins:  "+record.ToString(), new Vector2(0, graphics.PreferredBackBufferHeight-30), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
