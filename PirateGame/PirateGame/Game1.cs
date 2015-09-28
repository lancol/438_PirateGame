using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PirateGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        PlayerShip player;

        Texture2D OceanTile;

        int gameState;
        //GameStates:
        //0 = main menu
        //1 = In town
        //2 = Overworld
        //3 = In battle
        //4 =
        //5 =

        int screen_W;
        int screen_H;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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

            screen_H = GraphicsDevice.Viewport.Height;
            screen_W = GraphicsDevice.Viewport.Width;

            player = new PlayerShip(150, 250, 0);

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

            player.setImage(Content.Load<Texture2D>("Ship1_Design"));
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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            rightdown = false;
            updown = false;
            leftdown = false;
            downdown = false;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                updown = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                downdown = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                leftdown = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                rightdown = true;

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
                    player.setPos(player.getX(), player.getY());

                    //Adjust player sprite




                    break;
                case 3: //In battle
                    break;
                case 4:
                    break;
                default:
                    break;
            }


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
                    MathHelper.ToRadians(player.getRotate()), new Vector2(33, 37), .1f, SpriteEffects.None, 1);

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
