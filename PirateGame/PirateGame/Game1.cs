using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace PirateGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        Camera camera;

        SpriteBatch spriteBatch;
            Texture2D OceanTile;
            Texture2D SailSprayEffect;
        Texture2D[] island;

        PlayerShip player;
        bool facingRight;
        bool moving;

        float DT;
        float t;
        int step;

        int[] isl_x;
        int[] isl_y;


        int screen_W;
        int screen_H;

        int world_W;
        int world_H;

        int gameState;
        //GameStates:
        //0 = Main Menu
        //1 = In town
        //2 = Overworld
        //3 = In battle
        //4 = Pub
        //5 = Merchant

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            gameState = 2;

            facingRight = true;

            screen_H = GraphicsDevice.Viewport.Height;
            screen_W = GraphicsDevice.Viewport.Width;

            world_H = 5000;
            world_W = 5000;

            player = new PlayerShip(2500, 4500, 0);
            island = new Texture2D[5];
            isl_x = new int[5];
            isl_y = new int[5];

            Random rand = new Random();

            /* for (int i = 0; i < 5; i++)
            {
                isl_x[i] = rand.Next(0, world_W);
                isl_y[i] = rand.Next(0, world_H);
            } */

            isl_x[0] = rand.Next(0, world_W);  // First island randomized location within range 
            isl_y[0] = rand.Next(0, 999);

            isl_x[1] = rand.Next(0, world_W); // Second island
            isl_y[1] = rand.Next(1000, 1999);

            isl_x[2] = rand.Next(0, world_W); // Third island
            isl_y[2] = rand.Next(2000, 2999);

            isl_x[3] = rand.Next(0, world_W); // Fourth island
            isl_y[3] = rand.Next(3000, 3999);

            isl_x[4] = rand.Next(0, world_W); // Fifth island 
            isl_y[4] = rand.Next(4000, 4999);

            t = 0; //ever incrementing T
            step = 0; //used in animating. Hopefully will merge with t at somepoint
            moving = false;

            camera = new Camera(GraphicsDevice.Viewport);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


            OceanTile = Content.Load<Texture2D>("Ocean_Tile32");

            SailSprayEffect = Content.Load<Texture2D>("WaterEffectSheet");

            island[0] = Content.Load<Texture2D>("Island1");
            island[1] = Content.Load<Texture2D>("Island2");
            island[2] = Content.Load<Texture2D>("Island3");
            island[3] = Content.Load<Texture2D>("Island2");
            island[4] = Content.Load<Texture2D>("Island3");


            player.setImage(Content.Load<Texture2D>("Ship1v2"));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            bool rightdown;
            bool updown;
            bool leftdown;
            bool downdown;
            int xStep;
            int yStep;

            moving = false;

            xStep = 0;
            yStep = 0;

            DT = (float)gameTime.ElapsedGameTime.TotalSeconds;

            t += 1 * DT;

            if (t > 9999999) //this can be refined
                t = 0;


            step = (int) ((10*t)%5);

            //every x steps increment
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            rightdown = false;
            updown = false;
            leftdown = false;
            downdown = false;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                updown = true;
                yStep = -60;
                moving = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                downdown = true;
                yStep = 60;
                moving = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                leftdown = true;
                xStep = -60;
                facingRight = false;
                moving = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                rightdown = true;
                xStep = 60;
                facingRight = true;
                moving = true;
            }
            switch (gameState)
            {
                case 0: //Main Menu
                    break;
                case 1: //In town
                    break;
                case 2: //overworld

                    //Update enemy positions

                    //update ocean effects (if applicable)

                    //update weather effects (if applicable)

                    //Check for collisions
                    //  if next step is a town, stop

                    //  if next step is a collision with other ship, go into battle with them

                    //  if next step is a collision with a town, go into town or open town menu

                    //else
                    //  Update player Position
                    player.setPos(player.getX() + (xStep * DT), player.getY() + (yStep * DT));

                    //Adjust player sprite
                    //  draw wake, use sin function to update wake. step 5 degrees*dt, then every ~3 seconds it'll be 15 degrees, then reverse
                    player.setRotate((float)(5*System.Math.Sin(t)));

                    //update camera
                    camera.position = new Vector2(player.getX() - (screen_W/2), player.getY() - (screen_H/2));
                    break;
                case 3: //In battle
                    break;
                case 4:
                    break;
                default:
                    break;
            }

            //Debug.WriteLine(step);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {//http://www.dylanwilson.net/implementing-a-2d-camera-in-monogame <-- look into this
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var viewMatrix = camera.GetViewMatrix();
            spriteBatch.Begin(transformMatrix: viewMatrix);

            switch (gameState)
            {
                case 0: //Main menu
                    break;
                case 1: //In Town
                    break;
                case 2: //overworld

                    //draw ocean
                    for (int h = 0; h < world_H; h += OceanTile.Height)
                    {
                        for (int w = 0; w < world_W; w += OceanTile.Width)
                        {
                            spriteBatch.Draw(OceanTile, new Vector2(w, h), Color.White);
                        }
                    }
                    //draw any ocean effects

                    //draw islands
                    for (int i = 0; i < island.Length; i++)
                        spriteBatch.Draw(island[i], new Vector2(isl_x[i], isl_y[i]), Color.White);
                    //draw island extras (towns etc)

                    //Draw enemy ships

                    //Draw player

                    spriteBatch.Draw(player.getImage(), new Vector2(player.getX(), player.getY()), null, Color.White,
                    MathHelper.ToRadians(player.getRotate()), new Vector2(36, 50), 1f, (facingRight) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 1);

                    //Sailing effect
                    if (moving)
                        spriteBatch.Draw(SailSprayEffect,
                        (facingRight) ? new Rectangle((int)player.getX() - 14, (int)player.getY() - 11, 34, 13) : new Rectangle((int)player.getX() - 20, (int)player.getY() - 11, 34, 13), //this code sucks. Sorry.
                        new Rectangle(step * 34, 0, 34, 13), Color.White, 0, new Vector2(0, 0),                                                                                    //basically, the offset for the animation is different
                        (facingRight) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 1);                                                                            //if it's going left or right.

                    //Draw clouds/wind/weather/anything else

                    break;
                case 3: //in battle
                    break;
                case 4:
                    break;
                default:
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
