using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        [Flags]
        enum gameState{mainMenu,overWorld,battle,inTown}


        #region System/Game Control
        GraphicsDeviceManager graphics;
        Camera camera;
        SpriteBatch spriteBatch;
        Song OverworldSong;
        //ParticleEngine ow_ShipSprayEffect;
        float DT;
        gameState currentState = new gameState();
        int screen_W;
        int screen_H;
        int world_W;
        int world_H;
        bool mapOpen;
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

        Texture2D Map;

        Vector2 startButtonPosition;
        Vector2 exitButtonPosition;
        Vector2 continueButtonPosition;
        Vector2 instructionsButtonPosition;
        Vector2 logoPosition;

        MouseState mouseState;
        MouseState previousMouseState;
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
        NPCShip EnemyShip;
        Texture2D cannonball;
        Texture2D flag;
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

        #region General Status Bar Elements
        Texture2D statusBarBase;
        SpriteFont VinerHand;
        Texture2D AlignmentBar;
        Texture2D HealthBar; // full bar
        Texture2D MoraleBar; // full bar
        Texture2D MenuSlider;
        #endregion

        #region Shop Window
      //  Store shop_window;
        Texture2D shop_window_background;
        Texture2D shop_back_button;
        Texture2D shop_repair_button;
        Texture2D shop_item_image;
        Texture2D shop_label;
        Texture2D crew_label;
        Texture2D upgrades_label;

        Vector2 shop_back_button_position;
        Vector2 shop_repair_button_position;
        Vector2 shop_item_one_button_position;
        Vector2 shop_item_two_button_position;
        Vector2 shop_item_three_button_position;
        #endregion

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.IsFullScreen = false;
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
            world_H = 3000;
            world_W = 3000;
            screen_H = GraphicsDevice.Viewport.Height;
            screen_W = GraphicsDevice.Viewport.Width;
            Random rand = new Random();
            camera = new Camera(GraphicsDevice.Viewport);
            currentState = gameState.mainMenu;
            #endregion

            #region Environment
            island = new Texture2D[5];
            isl_x = new int[5];
            isl_y = new int[5];

            isl_x[0] = 600;  // First island hard-coded
            isl_y[0] = 2300;

            isl_x[1] = 1900; // Second island hard-coded
            isl_y[1] = 1850;

            isl_x[2] = 680; // Third island hard-coded
            isl_y[2] = 1250;

            isl_x[3] = 1400; // Fourth island hard-coded
            isl_y[3] = 750;

            isl_x[4] = 2300; // Fifth island hard-coded
            isl_y[4] = 300;
            #endregion

            #region Player Related
            player = new PlayerShip(1000, 2300, 0);
            player.set_bSpeed(0);
            player.set_bAcceleration(1f);
            player.set_maxSpeed(30);
            player.setHealth(100);
            player.setMorale(100);
            player.setAlignment(30);
            player.setAttack(50);
            player.setDefense(50);
            player.setCrew(20);
            facingRight = true;
            moving = false;
            #endregion

            #region Animation related
            t = 0; //ever incrementing T
            step = 0; //used in animating. Hopefully will merge with t at somepoint
            stepRadius = 3; //used only in the ocean shifting effect
            effectT = 0; //also only used in ocean shifting effect
            #endregion

            #region Other Ships
            OtherShip = new NPCShip[20];
            for (int i = 0; i < OtherShip.Length; i++)
            {
                OtherShip[i] = new NPCShip(rand.Next(0, world_W), rand.Next(0, world_H), 0, "Pirate");
                OtherShip[i].setAttack(rand.Next(20, 80));
                OtherShip[i].setDefense(rand.Next(20, 80));
            }
            #endregion

            #region Main Menu

            //set the position of main menu items
            continueButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 40, 450); // middle of screen, width then height
            startButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 48, 525);
            instructionsButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 50, 600);
            exitButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 17, 675);
            logoPosition = new Vector2((GraphicsDevice.Viewport.Width / 30) - 48, 5);

            //get the mouse state
            mouseState = Mouse.GetState();
            previousMouseState = mouseState;

/*
            //set the position shop menu items
            shop_back_button_position = new Vector2((GraphicsDevice.Viewport.Width / 2) - 505, 640);
            shop_repair_button_position = new Vector2((GraphicsDevice.Viewport.Width / 2) - 710, 160);
            shop_item_one_button_position = new Vector2((GraphicsDevice.Viewport.Width / 2) - 210,300);
            shop_item_two_button_position = new Vector2((GraphicsDevice.Viewport.Width / 2) - 210, 420);
            shop_item_three_button_position = new Vector2((GraphicsDevice.Viewport.Width / 2) - 210, 54);


            
            shop_back_button_position = new Vector2(camera.position.X + 505, camera.position.Y + 640);
            shop_repair_button_position = new Vector2(camera.position.X + 710, camera.position.Y + 160);
            shop_item_one_button_position = new Vector2(camera.position.X + 210, camera.position.Y + 300);
            shop_item_two_button_position = new Vector2(camera.position.X + 210, camera.position.Y + 420);
            shop_item_three_button_position = new Vector2(camera.position.X + 210, camera.position.Y + 54);
*/
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

                OceanTile = Content.Load<Texture2D>("Ocean_Tiles48");
                OceanWeb = Content.Load<Texture2D>("Ocean_web48_t127");
                SailSprayEffect = Content.Load<Texture2D>("WaterEffectSheet");
                whiteblock = Content.Load<Texture2D>("whiteblock");

                //loads general status bar
                statusBarBase = Content.Load<Texture2D>("Top_Menu_Bar_Cropped");
                MoraleBar = Content.Load<Texture2D>("Morale_Status_Bar_Resized");
                AlignmentBar = Content.Load<Texture2D>("Alignment_Status_Bar_Resized");
                HealthBar = Content.Load<Texture2D>("Health_Status_Bar_Resized");
                MenuSlider = Content.Load<Texture2D>("Slider");

                //loads all islands
                island[0] = Content.Load<Texture2D>("Island150p");
                island[1] = Content.Load<Texture2D>("Island250p");
                island[2] = Content.Load<Texture2D>("Island350p");
                island[3] = Content.Load<Texture2D>("Island450p");
                island[4] = Content.Load<Texture2D>("Island550p");

                //loads combat images
                shipImg = new Texture2D[1];
                b_shipImg = new Texture2D[1];

                //loads combat images
                shipImg[0] = Content.Load<Texture2D>("Ship1v3");
                b_shipImg[0] = Content.Load<Texture2D>("Ship_TopDown136_68");
                cannonball = Content.Load<Texture2D>("Battle_Cannonball16");

                // loads shop elements
                shop_window_background = Content.Load<Texture2D>("Shop_Window_Background_Biggest");
                shop_back_button = Content.Load<Texture2D>("Back_Button");
                shop_repair_button = Content.Load<Texture2D>("Repair_Ship_Button");
                shop_item_image = Content.Load<Texture2D>("Shop_ItemBox");
                shop_label = Content.Load<Texture2D>("Shop_Label");
                crew_label = Content.Load<Texture2D>("Hire_Crew_Label");
                upgrades_label = Content.Load<Texture2D>("Upgrades_Label");
                

                //loads flag
                flag = Content.Load<Texture2D>("flag");

                //loads minimap
                Map = Content.Load<Texture2D>("Map678");

                player.setImage(shipImg[0]);
                player.setBattleImage(b_shipImg[0]);

                NPCShip.setCBallImage(cannonball);
                for (int i = 0; i < OtherShip.Length; i++)
                {
                    OtherShip[i].setImage(shipImg[0]);
                    OtherShip[i].setBattleImage(b_shipImg[0]);
                }


                OverworldSong = Content.Load<Song>("Piratev2");
            }
            catch
            {
                Debug.WriteLine("Failed to load an image");
            }
            //overworld_init(); //depends on some player inits
            //ow_Player_CollBox = new Rectangle((int)player.getX(), (int)player.getY(), player.getImage().Width, player.getImage().Height);
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
            bool spacedown;

            bool Collision;

            int xStep;
            int yStep;

            int nextPosX;
            int nextPosY;

            Random rand = new Random();

            moving = false;

            mapOpen = false;

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
            spacedown = false;
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
                facingRight = false;
                moving = true;
                xStep = -60;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                rightdown = true;
                facingRight = true;
                moving = true;
                xStep = 60;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                spacedown = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.M)) //Temporary until overworld enemy ship collisions
            {
                mapOpen = true;
            }
            else
            {
                mapOpen = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S)) //Temporary until overworld island interaction
            {
                currentState = gameState.inTown;
            }

            #endregion

            switch (currentState) //this gameState is for loading
            {
                case gameState.mainMenu: //Main Menu
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
                case gameState.overWorld: //overworld
                    #region Overworld
                    //MediaPlayer.Play(backgroundSong);
                    //MediaPlayer.IsRepeating = true;
                    //check for pause

                    //Update enemy positions
                    for (int i = 0; i < OtherShip.Length; i++)
                        OtherShip[i].runOverworldAI(player, DT);

                    //update ocean effects (if applicable)

                    //update weather effects (if applicable)
                    IsMouseVisible = false;


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

                    if (nextPosX < 0)
                        player.setPos(world_W, player.getY());
                    else if(nextPosX > world_W)
                        player.setPos(0, player.getY());

                    if (nextPosY < 0)
                        Collision = true;
                    else if (nextPosY > world_H)
                        Collision = true;
                                            
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
                    for (int i = 0; i < OtherShip.Length; i++)
                    {
                        float distance = 0;
                        float Px = player.getX() + player.getImage().Width / 2;
                        float Py = player.getY() + player.getImage().Height / 2;
                        float Ex = OtherShip[i].getX() + OtherShip[i].getImage().Width / 2;
                        float Ey = OtherShip[i].getY() + OtherShip[i].getImage().Height / 2;

                        distance = Vector2.Distance(new Vector2(Px, Py), new Vector2(Ex, Ey));
                        if (distance < 45) //optimize this distance
                        {
                            //change gamestate and set enemy ship to othership[i]
                            currentState = gameState.battle;
                            battle_init(OtherShip[i]);
                        }
                    }
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
                    }
                    ow_sailSpray.Update(DT);
                    #endregion

                    //update camera
                    camera.position = new Vector2(player.getX() - (screen_W / 2), player.getY() - (screen_H / 2));

                    #endregion
                    break;
                case gameState.battle: //In battle
                    #region In Battle

                    //check for pause

                    //check if player is dead
                    if (player.getHealth() <= 0)
                    {
                        currentState = gameState.overWorld;
                    }
                    //check if enemy is dead
                    if (EnemyShip.getHealth() <= 0)
                    {
                        //ship is inactive
                        currentState = gameState.overWorld;
                    }

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

                    player.updateCannonBalls(DT, EnemyShip);

                    #region sailing particles
                    //Update ParticleEnginePosition
                    b_SailStream.setX(player.getX() + (-46.1f * (float)Math.Cos(MathHelper.ToRadians(player.getRotate()) + .61f))); //PlayerX + ([angle length of Hypotenuse] * Cos([playerRotate] + [angle between desired position and origin])
                    b_SailStream.setY(player.getY() + (-46.1f * (float)Math.Sin(MathHelper.ToRadians(player.getRotate()) + .61f)));
                    b_SailStream.setActive(true);
                    b_SailStream.setTimeLimit(player.get_bSpeed() / 2);
                    b_SailStream2.setX(player.getX() + (-46.1f * (float)Math.Cos(MathHelper.ToRadians(player.getRotate()) - .61f)));
                    b_SailStream2.setY(player.getY() + (-46.1f * (float)Math.Sin(MathHelper.ToRadians(player.getRotate()) - .61f)));
                    b_SailStream2.setActive(true);
                    b_SailStream2.setTimeLimit(player.get_bSpeed() / 2);
                    //b_SailStream2.setTimeLimit(rand.Next(0, ((int)player.get_bSpeed() * 2))); //Semi-good idea, maybe revisit this.

                    //Set spawn position relative to ship rotation
                    b_SailStream.setXSpeed(2 * (float)Math.Cos(MathHelper.ToRadians(player.getRotate() + 180)));
                    b_SailStream.setYSpeed(2 * (float)Math.Sin(MathHelper.ToRadians(player.getRotate() + 180)));

                    b_SailStream2.setXSpeed(2 * (float)Math.Cos(MathHelper.ToRadians(player.getRotate() + 180)));
                    b_SailStream2.setYSpeed(2 * (float)Math.Sin(MathHelper.ToRadians(player.getRotate() + 180)));

                    //Set x/y speeds relative to ship rotation
                    b_SailStream.Update(DT);
                    b_SailStream2.Update(DT);
                    #endregion

                    //Player Cannon Fire
                    if (spacedown)
                        player.fireCannon(EnemyShip, DT);

                    //EnemyShip
                    EnemyShip.runStandardBattleAI(player, DT);
                    EnemyShip.updateCannonBalls(DT, player);

                    #endregion
                    break;
                case gameState.inTown: //inTown  

                    break;   
                default:
                    Exit();
                    break;
            }
            //Debug.WriteLine(1/(float)gameTime.ElapsedGameTime.TotalSeconds); //FPS
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

            switch (currentState) //this gameState is for drawing
            {
                case gameState.mainMenu: //Main menu
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
                case gameState.overWorld: //overworld
                    #region Overworld
                    //draw ocean and ocean effects

                    int h = ((int)camera.position.Y / OceanTile.Height) * OceanTile.Height;
                    int w = (((int)camera.position.X / OceanTile.Width) * OceanTile.Width)-OceanTile.Width;

                    for (int y = h; y < (h + screen_H + OceanTile.Height); y += OceanTile.Height)
                    {
                        for (int x = w; x < (w + screen_W + (OceanTile.Width*2)); x += OceanTile.Width)
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
                    {   //change distance to stance, but I'd prefer if stances were an enum first
                        spriteBatch.Draw(OtherShip[n].getImage(), OtherShip[n].getPos(), null, Color.White,  
                                        MathHelper.ToRadians(player.getRotate()), new Vector2(34, 50), 1f, ((player.getX() < OtherShip[n].getX()) && (Vector2.Distance(player.getPos(), OtherShip[n].getPos()) < 250)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1);
                        
                        //Draw Flags
                        int r, g, b;
                        float cDistance;
                        cDistance = player.getPowerlvl() - OtherShip[n].getPowerlvl();

                        if (cDistance > 0) //Player is stronger
                        {
                            r = 0;
                            g = 0;
                            b = 255;
                        }
                        else //Enemy is stronger
                        {
                            r = 255;
                            g = 0;
                            b = 0;
                        }

                        //44 * Math.Cos(player.getRotate()-90)

                        //Vector2 shipPos = new Vector2(OtherShip[n].getX() - OtherShip[n].getImage().Width / 2, OtherShip[n].getY() - (OtherShip[n].getImage().Height / 2) - 10);

                        Vector2 shipPos = new Vector2(OtherShip[n].getX() + (45 * (float)Math.Cos(MathHelper.ToRadians(player.getRotate() - 90))), OtherShip[n].getY() + (45 * (float)Math.Sin(MathHelper.ToRadians(player.getRotate() - 90))));
                        Color newColor = new Color(r,g,b); //Work on this.

                        spriteBatch.Draw(flag, shipPos, null, newColor,
                                        MathHelper.ToRadians(player.getRotate()), new Vector2(flag.Width, flag.Height/2), 1f, SpriteEffects.None, 1);
                    }

                    //Draw player
                    spriteBatch.Draw(player.getImage(), new Vector2(player.getX(), player.getY()), null, Color.White,
                    MathHelper.ToRadians(player.getRotate()), new Vector2(34, 50), 1f, (facingRight) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 1);

                    //Sailing effect
                    #region particle crap
                    if (moving)
                    {
                        ow_sailSpray.Draw(spriteBatch);               
                    }
                    #endregion

                    #region Status bar
                    // draw status bar

                    spriteBatch.Draw(statusBarBase, new Vector2(camera.position.X, camera.position.Y), Color.White);

                    spriteBatch.Draw(MoraleBar, new Vector2(camera.position.X + 735, camera.position.Y + 10), new Rectangle(0, 0, (int)((player.getMorale() / 100f) * MoraleBar.Width), MoraleBar.Height), Color.White);

                    spriteBatch.Draw(HealthBar, new Vector2(camera.position.X + 180, camera.position.Y + 10), new Rectangle(0, 0, (int)((player.getHealth() / 100f) * HealthBar.Width), HealthBar.Height), Color.White);

                    spriteBatch.Draw(AlignmentBar, new Vector2(camera.position.X + 180, camera.position.Y + 40), Color.White); // always same

                    spriteBatch.Draw(MenuSlider, new Vector2(camera.position.X + 285, camera.position.Y + 40), Color.White);
                    #endregion
                    //Draw clouds/wind/weather/anything else

                    //Draw Map
                    if (mapOpen)
                    {
                        Vector2 mapPos = new Vector2(player.getX() - (screen_W / 2) + 169, player.getY() - (screen_H / 2) + 89);
                        spriteBatch.Draw(Map, mapPos, Color.White);
                        //.226
                        Vector2 islPos;
                        for (int i = 0; i < 5; i++)
                        {
                            islPos = new Vector2(mapPos.X + isl_x[i] *.226f, mapPos.Y + isl_y[i] * .226f);
                            spriteBatch.Draw(island[i], islPos, null, Color.White,0,new Vector2(0,0), 0.226f, SpriteEffects.None,1);
                        }
                        
                        spriteBatch.Draw(whiteblock, mapPos + player.getPos()*.226f, Color.Red);
                    }
                    #endregion
                    break;
                case gameState.battle: //in battle
                    #region In Battle

                    //draw Ocean
                    int bh = ((int)camera.position.Y / OceanTile.Height) * OceanTile.Height;
                    int bw = (((int)camera.position.X / OceanTile.Width) * OceanTile.Width) - OceanTile.Width;

                    for (int y = bh; y < (bh + screen_H + OceanTile.Height); y += OceanTile.Height)
                    {
                        for (int x = bw; x < (bw + screen_W + (OceanTile.Width * 2)); x += OceanTile.Width)
                        {
                            spriteBatch.Draw(OceanTile, new Vector2(x, y), Color.White);
                            spriteBatch.Draw(OceanWeb, new Vector2(x + (float)(stepRadius * Math.Sin(effectT)), y), Color.White);
                        }
                    }

                    //draw Enemy Ship
                    spriteBatch.Draw(EnemyShip.getBattleImage(), new Vector2(EnemyShip.getX(), EnemyShip.getY()), null, Color.DimGray,
                                        MathHelper.ToRadians(EnemyShip.getRotate()), new Vector2(55, 34), 1f, SpriteEffects.None, 1);
                    EnemyShip.drawCannonBalls(spriteBatch);

                    //draw any of its cannonballs
                    EnemyShip.drawCannonBalls(spriteBatch);

                    //draw particles
                    b_SailStream.Draw(spriteBatch);
                    b_SailStream2.Draw(spriteBatch);

                    //Draw your ship
                    spriteBatch.Draw(player.getBattleImage(), new Vector2(player.getX(), player.getY()), null, Color.White,
                                        MathHelper.ToRadians(player.getRotate()), new Vector2(55, 34), 1f, SpriteEffects.None, 1);

                    //Draw player cannonballs
                    player.drawCannonBalls(spriteBatch);

                    //draws collision box vertices
                    Vector2[] p = player.getCollisionbox();
                    for (int i = 0; i < 4; i++)
                    {
                        spriteBatch.Draw(whiteblock, new Vector2(p[i].X, p[i].Y), null, Color.Red);
                    }

                    #region Status bar
                    //status bar
                    spriteBatch.Draw(statusBarBase, new Vector2(camera.position.X, camera.position.Y), Color.White);

                    spriteBatch.Draw(MoraleBar, new Vector2(camera.position.X + 735, camera.position.Y + 10), new Rectangle(0, 0, (int)((player.getMorale() / 100f) * MoraleBar.Width), MoraleBar.Height), Color.White);

                    spriteBatch.Draw(HealthBar, new Vector2(camera.position.X + 180, camera.position.Y + 10), new Rectangle(0, 0, (int)((player.getHealth() / 100f) * HealthBar.Width), HealthBar.Height), Color.White);

                    spriteBatch.Draw(AlignmentBar, new Vector2(camera.position.X + 180, camera.position.Y + 40), Color.White); // always same

                    spriteBatch.Draw(MenuSlider, new Vector2(camera.position.X + 285, camera.position.Y + 40), Color.White);
                    #endregion
                    #endregion
                    break;
                case gameState.inTown: //Shop 
                    #region Shop
                    IsMouseVisible = true;

                    #region Draw Ocean Background 
                    int ab = ((int)camera.position.Y / OceanTile.Height) * OceanTile.Height;
                    int bc = (((int)camera.position.X / OceanTile.Width) * OceanTile.Width) - OceanTile.Width;

                    for (int y = ab; y < (ab + screen_H + OceanTile.Height); y += OceanTile.Height)
                    {
                        for (int x = bc; x < (bc + screen_W + (OceanTile.Width * 2)); x += OceanTile.Width)
                        {
                            spriteBatch.Draw(OceanTile, new Vector2(x, y), Color.White);
                            spriteBatch.Draw(OceanWeb, new Vector2(x + (float)(stepRadius * Math.Sin(effectT)), y), Color.White);
                        }
                    }
                    #endregion

                    #region Draw Status Bar 
                    spriteBatch.Draw(statusBarBase, new Vector2(camera.position.X, camera.position.Y), Color.White);
                    spriteBatch.Draw(MoraleBar, new Vector2(camera.position.X + 735, camera.position.Y + 10), new Rectangle(0, 0, (int)((player.getMorale() / 100f) * MoraleBar.Width), MoraleBar.Height), Color.White);
                    spriteBatch.Draw(HealthBar, new Vector2(camera.position.X + 180, camera.position.Y + 10), new Rectangle(0, 0, (int)((player.getHealth() / 100f) * HealthBar.Width), HealthBar.Height), Color.White);
                    spriteBatch.Draw(AlignmentBar, new Vector2(camera.position.X + 180, camera.position.Y + 40), Color.White); // always same
                    spriteBatch.Draw(MenuSlider, new Vector2(camera.position.X + 285, camera.position.Y + 40), Color.White);

                    //draw shop window objects
                    spriteBatch.Draw(shop_window_background, new Vector2(camera.position.X + 170, camera.position.Y + 115), Color.White);
                    spriteBatch.Draw(shop_label, new Vector2(camera.position.X + 445, camera.position.Y + 140), Color.White);
                    spriteBatch.Draw(upgrades_label, new Vector2(camera.position.X + 250, camera.position.Y + 215), Color.White);
                    spriteBatch.Draw(crew_label, new Vector2(camera.position.X + 630, camera.position.Y + 215), Color.White);

                    /*
                    spriteBatch.Draw(shop_item_image, shop_item_one_button_position, Color.White);
                    spriteBatch.Draw(shop_item_image, shop_item_two_button_position, Color.White);
                    spriteBatch.Draw(shop_item_image, shop_item_three_button_position, Color.White);
                    spriteBatch.Draw(shop_repair_button, shop_repair_button_position, Color.White);
                    spriteBatch.Draw(shop_back_button, shop_back_button_position, Color.White);
                    */
                    
                    spriteBatch.Draw(shop_item_image, new Vector2(camera.position.X + 210, camera.position.Y + 300), Color.White);
                    spriteBatch.Draw(shop_item_image, new Vector2(camera.position.X + 210, camera.position.Y + 420), Color.White);
                    spriteBatch.Draw(shop_item_image, new Vector2(camera.position.X + 210, camera.position.Y + 540), Color.White);
                    spriteBatch.Draw(shop_repair_button, new Vector2(camera.position.X + 710, camera.position.Y + 160), Color.White);
                    spriteBatch.Draw(shop_back_button, new Vector2(camera.position.X + 505, camera.position.Y + 640), Color.White);
                    

                    #endregion
                    break;
                default:
                    Exit();
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void MouseClicked(int x, int y)
        {
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);

            //check the shop menu
            if (currentState == gameState.inTown)
            {

                Rectangle backButtonRect = new Rectangle((int)shop_back_button_position.X,
                      (int)shop_back_button_position.Y, 100, 20);
                Rectangle repairShipButtonRect = new Rectangle((int)shop_repair_button_position.X,
                                      (int)shop_repair_button_position.Y, 100, 20);
                Rectangle itemOneRect = new Rectangle((int)shop_item_one_button_position.X,
                                      (int)shop_item_one_button_position.Y, 100, 20);
                Rectangle itemTwoRect = new Rectangle((int)shop_item_two_button_position.X,
                                      (int)shop_item_two_button_position.Y, 100, 20);
                Rectangle itemThreeRect = new Rectangle((int)shop_item_three_button_position.X,
                      (int)shop_item_three_button_position.Y, 100, 20);
                      

                if (mouseClickRect.Intersects(backButtonRect)) //player clicked start button
                {
                    currentState = gameState.overWorld;
                    overworld_init();
                }
                else if (mouseClickRect.Intersects(itemOneRect))
                {
                    currentState = gameState.overWorld;
                    overworld_init();
                }

                else if (mouseClickRect.Intersects(itemTwoRect))
                {
                    currentState = gameState.overWorld;
                    overworld_init();
                }

                else if (mouseClickRect.Intersects(itemThreeRect)) //player clicked exit button
                {
                    currentState = gameState.overWorld;
                    overworld_init();
                }
            }
                    


            //check the startmenu
            if (currentState == gameState.mainMenu)
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
                    currentState = gameState.overWorld;
                    overworld_init();
                }
                else if (mouseClickRect.Intersects(continueButtonRect))
                {
                    currentState = gameState.overWorld;
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

        protected void battle_init(NPCShip Enemy)
        {
            player.setRotate(0);
            camera.position = new Vector2(player.getX() - (screen_W / 2), player.getY() - (screen_H / 2));

            //EnemyShip = //whatever collided with
            //EnemyShip = new NPCShip(cannonball, player.getX() - 250, player.getY() + 50, 270, "pirate");
            EnemyShip = Enemy;
            EnemyShip.setX(player.getX() - 250);
            EnemyShip.setY(player.getY() + 50);
            EnemyShip.setRotate(270);
            //EnemyShip.setBattleImage(b_shipImg[0]);

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

        protected void overworld_init()
        {
            //MediaPlayer.Play(OverworldSong);
            MediaPlayer.Volume = 1.0f;
            MediaPlayer.IsRepeating = true;

            player.setRotate(0);
            camera.position = new Vector2(player.getX() - (screen_W / 2), player.getY() - (screen_H / 2));
            ow_sailSpray = new ParticleEngine(whiteblock, 20, -1, 0, 0, 0);
          
            player.setCBallImage(cannonball);

            ow_sailSpray.setX(player.getX());
            ow_sailSpray.setY(player.getY());
            ow_sailSpray.setActive(true);
        }
    }
}
#endregion