using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace PirateGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
            Texture2D OceanTile;

            Texture2D SailSprayEffect;

        PlayerShip player;
        bool facingRight;

        bool moving;

        float DT;
        float t;
        int step;

        int screen_W;
        int screen_H;
        int gameState;
        //GameStates:
        //0 = main menu
        //1 = In town
        //2 = Overworld
        //3 = In battle
        //4 =
        //5 =

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //graphics.IsFullScreen = true;
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

            player = new PlayerShip(150, 250, 0);

            t = 0; //ever incrementing T
            step = 0;
            moving = false;
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

                    break;
                case 3: //In battle
                    break;
                case 4:
                    break;
                default:
                    break;
            }

            Debug.WriteLine(step);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            switch (gameState)
            {
                case 0: //Main menu
                    break;
                case 1: //In Town
                    break;
                case 2: //overworld

                    //draw ocean
                    for (int h = 0; h < screen_H; h += OceanTile.Height)
                    {
                        for (int w = 0; w < screen_W; w += OceanTile.Width)
                        {
                            spriteBatch.Draw(OceanTile, new Vector2(w, h), Color.White);
                        }
                    }
                    //draw any ocean effects

                    //draw islands

                    //draw island extras (towns etc)

                    //Draw enemy ships

                    //Draw player

                    spriteBatch.Draw(player.getImage(), new Vector2(player.getX(), player.getY()), null, Color.White,
                    MathHelper.ToRadians(player.getRotate()), new Vector2(36, 50), 1f, (facingRight) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 1);

                    //Sailing effect
                    if (moving)
                        spriteBatch.Draw(SailSprayEffect, 
                            (facingRight) ? new Rectangle((int)player.getX() - 15, (int)player.getY() - 10, 34, 13) : new Rectangle((int)player.getX() - 20, (int)player.getY() - 10, 34, 13), //this code sucks. Sorry.
                            new Rectangle(step*34,0,34,13), Color.White, 0,new Vector2(0,0),                                                                                    //basically, the offset for the animation is different
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
