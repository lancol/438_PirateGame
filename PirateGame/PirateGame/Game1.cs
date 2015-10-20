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
        #region Class Variables
        #region System/Game Control
        GraphicsDeviceManager graphics;
        Camera camera;
        SpriteBatch spriteBatch;
        // ParticleEngine ow_ShipSprayEffect; // was commented out by Dylan???
        float DT;
        int gameState;
        int screen_W;
        int screen_H;
        int world_W;
        int world_H;
        #endregion

        #region Environment
        Texture2D OceanTile;
        Texture2D OceanWeb;
        Texture2D SailSprayEffect;
        Texture2D[] island;
        int[] isl_x;
        int[] isl_y;
        #endregion

        #region Menu Related
        // menu
        Texture2D continueButton;
        Texture2D startButton;
        Texture2D exitButton;
        Texture2D instructionsButton;
        Texture2D menuBackground;
        Texture2D logo;

        Vector2 startButtonPosition;
        Vector2 exitButtonPosition;
        Vector2 continueButtonPosition;
        Vector2 instructionsButtonPosition;
        Vector2 logoPosition;

        MouseState mouseState;
        MouseState previousMouseState;
        #endregion

        #region General Status Bar Elements
        Texture2D statusBarBase;
        SpriteFont VinerHand;
        Texture2D AlignmentBar;
        Texture2D HealthBar; // full bar
        Texture2D MoraleBar; // full bar
        Texture2D MenuSlider;

        #endregion

        #region Morale Bar Levels
        Texture2D MoraleBarOne;
        Texture2D MoraleBarTwo;
        Texture2D MoraleBarThree;
        Texture2D MoraleBarFour;
        Texture2D MoraleBarFive;
        Texture2D MoraleBarSix;
        Texture2D MoraleBarSeven;
        Texture2D MoraleBarEight;
        Texture2D MoraleBarNine;
        #endregion

        #region Health Bar Levels
        Texture2D HealthBarOne;
        Texture2D HealthBarTwo;
        Texture2D HealthBarThree;
        Texture2D HealthBarFour;
        Texture2D HealthBarFive;
        Texture2D HealthBarSix;
        Texture2D HealthBarSeven;
        Texture2D HealthBarEight;
        Texture2D HealthBarNine;
        #endregion

        #region Player Related
        PlayerShip player;
        Rectangle ow_Player_CollBox; //overworld Player collisionbox
        bool facingRight;
        bool moving;
        #endregion

        #region Ships and NPCS
        Texture2D[] shipImg;
        Texture2D[] b_shipImg;
        NPCShip[] OtherShip;
        #endregion

        #region Animation related
        float t;
        float effectT;
        int step;   //I need to do something about merging all these step variables.
        int stepRadius;
        ParticleEngine ow_sailSpray;
        ParticleEngine b_SailStream;
        ParticleEngine b_SailStream2;
        Texture2D whiteblock;
        #endregion

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.IsFullScreen = false; // switch to full screen, bounding boxes for buttons fix
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            #region System/Game Control
            world_H = 5000;
            world_W = 5000;
            screen_H = GraphicsDevice.Viewport.Height;
            screen_W = GraphicsDevice.Viewport.Width;
            Random rand = new Random();
            camera = new Camera(GraphicsDevice.Viewport);
            
            //set the gamestate to start menu
            gameState = 0;          

           // gameState = 2;
            #endregion

            #region Enviornment
            island = new Texture2D[5];
            isl_x = new int[5];
            isl_y = new int[5];

            isl_x[0] = 1000;  // First island hard-coded
            isl_y[0] = 4500;

            isl_x[1] = 3000; // Second island hard-coded
            isl_y[1] = 3500;

            isl_x[2] = 800; // Third island hard-coded
            isl_y[2] = 1800;

            isl_x[3] = 2000; // Fourth island hard-coded
            isl_y[3] = 1000;

            isl_x[4] = 4500; // Fifth island hard-coded
            isl_y[4] = 500;

            #endregion

            #region Player Related
            player = new PlayerShip(1000, 4500, 0); // hardcoded ship starting point
            player.set_bSpeed(0);
            player.set_bAcceleration(1f);
            player.set_maxSpeed(30);
            facingRight = true;
            moving = false;
            #endregion

            #region Animation related
            t = 0; //ever incrementing T
            step = 0; //used in animating. Hopefully will merge with t at somepoint
            stepRadius = 3; //used only in the ocean shifting effect
            effectT = 0; //also only used in ocean shifting effect
            #endregion

            #region
            OtherShip = new NPCShip[20];
            for (int i = 0; i < OtherShip.Length; i++)
            {
                OtherShip[i] = new NPCShip(rand.Next(0, 5000), rand.Next(0, 5000), 0, "neutral");
            }
            #endregion

            #region Main Menu

            //set the position of the buttons
            

            // if not full screen
            continueButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 40, 450); // middle of screen, width then height
            startButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 48, 525);
            instructionsButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 50, 600);
            exitButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 17, 675);
            logoPosition = new Vector2((GraphicsDevice.Viewport.Width/30) - 48, 5);

            /*
            // if full screen
             continueButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2 - 50 ), GraphicsDevice.Viewport.Height / 2 + 70); // middle of screen, width then height
             startButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2 - 57), GraphicsDevice.Viewport.Height / 2 + 150);
             instructionsButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2 - 60), GraphicsDevice.Viewport.Height / 2 + 220);
             exitButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2- 25 ), GraphicsDevice.Viewport.Height / 2 + 290);
             logoPosition = new Vector2((GraphicsDevice.Viewport.Width / 80) - 30, GraphicsDevice.Viewport.Height / 40);
             */

            //get the mouse state
            mouseState = Mouse.GetState();
            previousMouseState = mouseState;

            #endregion

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            try{
                //load the buttonimages into the content pipeline
                continueButton = Content.Load<Texture2D>("Continue");
                startButton = Content.Load<Texture2D>("NewGame");
                instructionsButton = Content.Load<Texture2D>("Instructions");
                exitButton = Content.Load<Texture2D>("Quit");
                menuBackground = Content.Load<Texture2D>("Menu");
                logo = Content.Load<Texture2D>("Logo");

                // loads ocean background and water effects
                OceanTile = Content.Load<Texture2D>("Ocean_Tile32");
                OceanWeb = Content.Load<Texture2D>("Ocean_web32t127");
                SailSprayEffect = Content.Load<Texture2D>("WaterEffectSheet");
                whiteblock = Content.Load<Texture2D>("whiteblock");

                //loads general status bar
                statusBarBase = Content.Load<Texture2D>("Top_Menu_Bar_Cropped");
                MoraleBar = Content.Load<Texture2D>("Morale_Status_Bar_Resized");
                AlignmentBar = Content.Load<Texture2D>("Alignment_Status_Bar_Resized");
                HealthBar = Content.Load<Texture2D>("Health_Status_Bar_Resized");
                MenuSlider = Content.Load<Texture2D>("Slider");

                //loads morale
                MoraleBarOne = Content.Load<Texture2D>("Morale_Status_Bar_One");
                MoraleBarTwo = Content.Load<Texture2D>("Morale_Status_Bar_Two");
                MoraleBarThree = Content.Load<Texture2D>("Morale_Status_Bar_Three");
                MoraleBarFour = Content.Load<Texture2D>("Morale_Status_Bar_Four");
                MoraleBarFive = Content.Load<Texture2D>("Morale_Status_Bar_Five");
                MoraleBarSix = Content.Load<Texture2D>("Morale_Status_Bar_Six");
                MoraleBarSeven = Content.Load<Texture2D>("Morale_Status_Bar_Seven");
                MoraleBarEight = Content.Load<Texture2D>("Morale_Status_Bar_Eight");
                MoraleBarNine = Content.Load<Texture2D>("Morale_Status_Bar_Nine");

                //loads health
                HealthBarOne = Content.Load<Texture2D>("Health_Status_Bar_One");
                HealthBarTwo = Content.Load<Texture2D>("Health_Status_Bar_Two");
                HealthBarThree = Content.Load<Texture2D>("Health_Status_Bar_Three");
                HealthBarFour = Content.Load<Texture2D>("Health_Status_Bar_Four");
                HealthBarFive = Content.Load<Texture2D>("Health_Status_Bar_Five");
                HealthBarSix = Content.Load<Texture2D>("Health_Status_Bar_Six");
                HealthBarSeven = Content.Load<Texture2D>("Health_Status_Bar_Seven");
                HealthBarEight = Content.Load<Texture2D>("Health_Status_Bar_Eight");
                HealthBarNine = Content.Load<Texture2D>("Health_Status_Bar_Nine");
                
                //add gold value
                //use text sprite

                //loads all islands
                island[0] = Content.Load<Texture2D>("Island150p");
                island[1] = Content.Load<Texture2D>("Island250p");
                island[2] = Content.Load<Texture2D>("Island350p");
                island[3] = Content.Load<Texture2D>("Island450p"); 
                island[4] = Content.Load<Texture2D>("Island550p"); 

                shipImg = new Texture2D[1];
                b_shipImg = new Texture2D[1];

                shipImg[0] = Content.Load<Texture2D>("Ship1v2");
                b_shipImg[0] = Content.Load<Texture2D>("Ship_TopDown136_68");
                player.setImage(shipImg[0]);
                player.setBattleImage(b_shipImg[0]);

                for (int i = 0; i < OtherShip.Length; i++)
                {
                    OtherShip[i].setImage(shipImg[0]);
                    OtherShip[i].setBattleImage(b_shipImg[0]);
                }
            }
            catch
            {
                Debug.WriteLine("Failed to load an image");
            }

           // ow_Player_CollBox = new Rectangle((int)player.getX(), (int)player.getY(), player.getImage().Width, player.getImage().Height);
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
            #region Variables
            bool rightdown;
            bool updown;
            bool leftdown;
            bool downdown;

            bool Collision;

            int xStep;
            int yStep;

            int nextPosX;
            int nextPosY;

            Random rand = new Random();

            moving = false;

            xStep = 0;
            yStep = 0;

            nextPosX = (int)player.getX();
            nextPosY = (int)player.getY();

            Collision = false;

            DT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            t += DT;//gameTime.TotalGameTime.Seconds;

            effectT += (float)(Math.PI / 2) * DT;

            if (effectT > 9999999)
                effectT = 0;

            step = (int)(10 * t) % 5;

            rightdown = false;
            updown = false;
            leftdown = false;
            downdown = false;

            #endregion

            #region Input
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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
            if (Keyboard.GetState().IsKeyDown(Keys.B)) //Temporary until overworld enemy ship collisions
            {
                gameState = 3;
                battle_init();
            }
            #endregion

            switch (gameState)
            {
                case 0: //Main Menu
                    #region Main Menu

                    IsMouseVisible = true; //enables mouse pointer
                 //   main_menu();

                    //wait for mouseclick
                    mouseState = Mouse.GetState();                  

                    if (previousMouseState.LeftButton == ButtonState.Pressed &&
                    mouseState.LeftButton == ButtonState.Released)
                    {
                    MouseClicked(mouseState.X, mouseState.Y);
                    }
                     
                    previousMouseState = mouseState; 

                    #endregion
                    break;
                case 1: //In town
                    #region In Town

                    #endregion
                    break;
                case 2: //overworld
                    #region Overworld
                    //Update enemy positions

                    //update ocean effects (if applicable)

                    //update weather effects (if applicable)

                    //Check for collisions
                    #region Get Next Position
                    if (updown)
                    {
                        nextPosY = (int)(player.getY() + (yStep * DT) + 1);
                    }
                    if (rightdown)
                    {
                        nextPosX = (int)(player.getX() + (xStep * DT) + 1);
                    }
                    if (downdown)
                    {
                        nextPosY = (int)(player.getY() + (yStep * DT) + 1);
                    }
                    if (leftdown)
                    {
                        nextPosX = (int)(player.getX() + (xStep * DT) + 1);
                    }
                    #endregion
                    //  if next step is an island, stop
                    for (int i = 0; i < island.Length; i++) //must improve collision box on final islands.
                    {
                        if (nextPosX < (isl_x[i] + island[i].Width) && nextPosX > isl_x[i])
                        {
                            if (nextPosY < (isl_y[i] + island[i].Height) && nextPosY > isl_y[i])
                            {
                                Collision = true;
                            }
                        }
                    }
                    //  if next step is a collision with other ship, go into battle with them

                    //  if next step is a collision with a town, go into town or open town menu

                    if (Collision == false)
                    {
                        player.setPos(player.getX() + (xStep * DT), player.getY() + (yStep * DT));
                        ow_Player_CollBox.X = (int)player.getX();
                        ow_Player_CollBox.Y = (int)player.getY();
                    }

                    //Adjust player sprite
                    //  draw wake, use sin function to update wake. step 5 degrees*dt, then every ~3 seconds it'll be 15 degrees, then reverse
                    player.setRotate((float)(5 * Math.Sin(t)));

                    //Particle Effects
                    #region Particle Crap
                    if (updown == false && downdown == false)
                    {
                        ow_sailSpray.setX((facingRight) ? player.getX() + 18 : player.getX() - 18);
                        ow_sailSpray.setY(player.getY());
                        ow_sailSpray.setXSpeed((facingRight) ? -.1f : .1f);
                        ow_sailSpray.setXAccl((facingRight) ? -2 : 2);
                        ow_sailSpray.setYSpeed(-.3f);
                        ow_sailSpray.setYAccl(.1f * rand.Next(4, 8));// .5f);
                        ow_sailSpray.setTimeLimit(rand.Next(10, 20));
                    }
                    else// if (updown == true)
                    {
                        ow_sailSpray.removeAll();
                        //ow_sailSpray.setX((facingRight) ? player.getX() + 18 : player.getX() - 18);
                        //ow_sailSpray.setY(player.getY());
                        //ow_sailSpray.setXSpeed((facingRight) ? -.1f : .1f);
                        //ow_sailSpray.setXAccl((facingRight) ? -2 : 2);
                        //ow_sailSpray.setYSpeed(20 - .3f);
                        //ow_sailSpray.setYAccl(.5f);
                        //ow_sailSpray.setTimeLimit(rand.Next(10, 20));
                    }
                    ow_sailSpray.Update(DT);
                    #endregion

                    //update camera
                    camera.position = new Vector2(player.getX() - (screen_W / 2), player.getY() - (screen_H / 2));

                    #endregion
                    break;
                case 3: //In battle
                    #region In Battle

                    //Move Player
                    if (updown)
                    {
                        player.raise_Sails(DT);
                    }
                    if (downdown)
                    {
                        player.lower_Sails(DT);
                    }
                    if (rightdown)
                    {
                        player.rotate_Cwise(DT);
                    }
                    if (leftdown)
                    {
                        player.rotate_CCwise(DT);
                    }

                    player.setX(player.getX() + (player.get_bSpeed() * DT) * (float)Math.Cos(MathHelper.ToRadians(player.getRotate()))); //update positions
                    player.setY(player.getY() + (player.get_bSpeed() * DT) * (float)Math.Sin(MathHelper.ToRadians(player.getRotate())));
                    
                    #region sailing particles
                    //Update ParticleEnginePosition
                    b_SailStream.setX(player.getX() + (-46.1f * (float)Math.Cos(MathHelper.ToRadians(player.getRotate())+.61f))); //PlayerX + ([angle length of Hypotenuse] * Cos([playerRotate] + [angle between desired position and origin])
                    b_SailStream.setY(player.getY() + (-46.1f * (float)Math.Sin(MathHelper.ToRadians(player.getRotate())+.61f)));
                    b_SailStream.setActive(true);
                    b_SailStream.setTimeLimit(player.get_bSpeed() / 2);
                    b_SailStream2.setX(player.getX() + (-46.1f * (float)Math.Cos(MathHelper.ToRadians(player.getRotate())-.61f)));
                    b_SailStream2.setY(player.getY() + (-46.1f * (float)Math.Sin(MathHelper.ToRadians(player.getRotate())-.61f)));
                    b_SailStream2.setActive(true);
                    b_SailStream2.setTimeLimit(player.get_bSpeed() / 2);
                    //b_SailStream2.setTimeLimit(rand.Next(0, ((int)player.get_bSpeed() * 2))); //Semi-good idea, maybe revisit this.

                    //Set spawn position relative to ship rotation
                    b_SailStream.setXSpeed(2*(float)Math.Cos(MathHelper.ToRadians(player.getRotate() + 180)));
                    b_SailStream.setYSpeed(2*(float)Math.Sin(MathHelper.ToRadians(player.getRotate() + 180)));

                    b_SailStream2.setXSpeed(2*(float)Math.Cos(MathHelper.ToRadians(player.getRotate() + 180)));
                    b_SailStream2.setYSpeed(2*(float)Math.Sin(MathHelper.ToRadians(player.getRotate() + 180)));

                    //Set x/y speeds relative to ship rotation
                    b_SailStream.Update(DT);
                    b_SailStream2.Update(DT);
                    #endregion

                    #endregion
                    break;

                default:
                    Exit();
                    break;
            }
            //Debug.WriteLine(1/(float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            var viewMatrix = camera.GetViewMatrix();//Camera stuff
            spriteBatch.Begin(transformMatrix: viewMatrix);

            switch (gameState)
            {
                case 0: //Main menu
                    #region Main Menu

                    //draw menu
                    IsMouseVisible = true;

                    //draw the start menu
                    spriteBatch.Draw(menuBackground, new Rectangle(0, 0, menuBackground.Width, menuBackground.Height), Color.White);
                    spriteBatch.Draw(continueButton, continueButtonPosition, Color.White);
                    spriteBatch.Draw(startButton, startButtonPosition, Color.White);
                    spriteBatch.Draw(instructionsButton, instructionsButtonPosition, Color.White);
                    spriteBatch.Draw(exitButton, exitButtonPosition, Color.White);
                    spriteBatch.Draw(logo, logoPosition, Color.White);

                    #endregion
                    break;
                case 1: //In Town
                    #region In Town

                    //draw menu

                    #endregion
                    break;
                case 2: //overworld
                    #region Overworld
                    //draw ocean and ocean effects
                    int h = ((int)camera.position.Y / OceanTile.Height) * OceanTile.Height;
                    int w = ((int)camera.position.X / OceanTile.Width) * OceanTile.Width;

                    for (int y = h; y < (h + screen_H + OceanTile.Height); y += OceanTile.Height)
                    {
                        for (int x = w; x < (w + screen_W + OceanTile.Width); x += OceanTile.Width)
                        {
                            spriteBatch.Draw(OceanTile, new Vector2(x, y), Color.White);
                            spriteBatch.Draw(OceanWeb, new Vector2(x + (float)(stepRadius * Math.Sin(effectT)), y), Color.White);
                        }
                    }

                
                    //draw islands
                    for (int i = 0; i < island.Length; i++)
                        spriteBatch.Draw(island[i], new Vector2(isl_x[i], isl_y[i]), Color.White);
                    //draw island extras (towns etc)

                    //Draw enemy ships
                    for (int n = 0; n < OtherShip.Length; n++)
                    {
                        spriteBatch.Draw(OtherShip[n].getImage(), new Vector2(OtherShip[n].getX(), OtherShip[n].getY()), null, Color.White,
                                        MathHelper.ToRadians(player.getRotate()), new Vector2(55, 34), 1f, SpriteEffects.None, 1);
                    }

                    //Draw player
                    spriteBatch.Draw(player.getImage(), new Vector2(player.getX(), player.getY()), null, Color.White,   // null uses whole image 
                    MathHelper.ToRadians(player.getRotate()), new Vector2(36, 50), 1f, (facingRight) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 1);

                    //Sailing effect
                    #region particle crap
                    if (moving)
                    {
                        ow_sailSpray.Draw(spriteBatch);
                        //spriteBatch.Draw(SailSprayEffect,
                        //(facingRight) ? new Rectangle((int)player.getX() - 14, (int)player.getY() - 11, 34, 13) : new Rectangle((int)player.getX() - 20, (int)player.getY() - 11, 34, 13), //this code sucks. Sorry.
                        //new Rectangle(step * 34, 0, 34, 13), Color.White, 0, new Vector2(0, 0),                                                                                    //basically, the offset for the animation is different
                        //(facingRight) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 1);                  
                    }

                    // draw status bar

                    // player.getHealth();

                    spriteBatch.Draw(statusBarBase, new Vector2(camera.position.X, camera.position.Y), Color.White);

                    spriteBatch.Draw(MoraleBar, new Vector2(camera.position.X + 735, camera.position.Y + 10), Color.White);

                    spriteBatch.Draw(HealthBar, new Vector2(camera.position.X + 180, camera.position.Y + 10), Color.White);

                    spriteBatch.Draw(AlignmentBar, new Vector2(camera.position.X + 180, camera.position.Y + 40), Color.White); // always same

                    spriteBatch.Draw(MenuSlider, new Vector2(camera.position.X + 285, camera.position.Y + 40), Color.White);


                    #endregion
                    //Draw clouds/wind/weather/anything else
                    #endregion
                    break;
                case 3: //in battle
                    #region In Battle

                    //draw Ocean
                    int bh = ((int)camera.position.Y / OceanTile.Height) * OceanTile.Height;
                    int bw = ((int)camera.position.X / OceanTile.Width) * OceanTile.Width;

                    for (int y = bh; y < (bh + screen_H + OceanTile.Height); y += OceanTile.Height)
                    {
                        for (int x = bw; x < (bw + screen_W + OceanTile.Width); x += OceanTile.Width)
                        {
                            spriteBatch.Draw(OceanTile, new Vector2(x, y), Color.White);
                            spriteBatch.Draw(OceanWeb, new Vector2(x + (float)(stepRadius * Math.Sin(effectT)), y), Color.White);
                        }
                    }

                    //draw Enemy Ship

                    //draw particles
                    b_SailStream.Draw(spriteBatch);
                    b_SailStream2.Draw(spriteBatch); 

                    //Draw your ship
                    spriteBatch.Draw(player.getBattleImage(), new Vector2(player.getX(), player.getY()), null, Color.White,
                                        MathHelper.ToRadians(player.getRotate()), new Vector2(55, 34), 1f, SpriteEffects.None, 1);

                    #endregion
                    break;

                default:
                    Exit();
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void battle_init()
        {
            player.setRotate(0);
            camera.position = new Vector2(player.getX() - (screen_W / 2), player.getY() - (screen_H / 2));

            b_SailStream = new ParticleEngine(whiteblock, 20, -1, 0,0,0);
            b_SailStream.setX(player.getX());
            b_SailStream.setY(player.getY()-30);
            b_SailStream.setActive(true);
            b_SailStream2 = new ParticleEngine(whiteblock, 20, -1, 0,0,0);
            b_SailStream2.setX(player.getX());
            b_SailStream2.setY(player.getY()+50);
            b_SailStream2.setActive(true);

            ow_sailSpray.setActive(false);
        }

        protected void MouseClicked(int x, int y)
        {
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);


            //check the startmenu
            if (gameState == 0)
            {

                        Rectangle continueButtonRect = new Rectangle((int)continueButtonPosition.X,
                                              (int)continueButtonPosition.Y, 100, 20);
                        Rectangle startButtonRect = new Rectangle((int)startButtonPosition.X,
                                              (int)startButtonPosition.Y, 100, 20);
                        Rectangle instructionsButtonRect = new Rectangle((int)instructionsButtonPosition.X,
                                              (int)instructionsButtonPosition.Y, 100, 20);
                        Rectangle exitButtonRect = new Rectangle((int)exitButtonPosition.X,
                                              (int)exitButtonPosition.Y, 100, 20);

                    if (mouseClickRect.Intersects(startButtonRect)) //player clicked start button
                    {
                        gameState = 2;
                        overworld_init();
                    }
                    else if (mouseClickRect.Intersects(continueButtonRect))
                    {
                        gameState = 2;
                        overworld_init();
                    }

                    else if (mouseClickRect.Intersects(instructionsButtonRect))
                    {
                        //gameState = 5;
                        Exit();
                    }

                    else if (mouseClickRect.Intersects(exitButtonRect)) //player clicked exit button
                    {
                        Exit();
                    }

            }
        }

        protected void overworld_init()
        {
            player.setRotate(0);
            camera.position = new Vector2(player.getX() - (screen_W / 2), player.getY() - (screen_H / 2));
            ow_sailSpray = new ParticleEngine(whiteblock, 20, -1, 0, 0, 0);

            ow_sailSpray.setX(player.getX());
            ow_sailSpray.setY(player.getY());
            ow_sailSpray.setActive(true);

            // b_SailStream.setActive(false); // Dylan commented out
            //b_SailStream2.setActive(false); // Dylan commented out

        }
  }

    }

