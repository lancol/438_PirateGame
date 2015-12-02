using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Diagnostics;
using System.IO;

using System.IO.IsolatedStorage;


namespace PirateGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region Class Variables
        [Flags]
        enum gameState { mainMenu, overWorld, battle, inTown, instructions, savefiles, credits }

        #region System/Game Control
        GraphicsDeviceManager graphics;
        Camera camera;
        SpriteBatch spriteBatch;
        float DT;
        gameState currentState = new gameState();
        int screen_W;
        int screen_H;
        int world_W;
        int world_H;
        bool mapOpen;
        bool buyOptionOpen;
        bool healthFull;
        bool moraleFull;
        bool returnError;
        #endregion

        #region Particle Engine
        ParticleEngine PE;
        ParticleEngine PE2;
        bool cannonfired;
        #endregion

        #region Sound
        Song OverworldSong;
        SoundEffect cannonFire;
        #endregion

        #region Environment
        Texture2D OceanTile;
        Texture2D OceanWeb;
        Texture2D SailSprayEffect;
        Texture2D[] island;
        int[] isl_x;
        int[] isl_y;
        #endregion

        #region Island Labels
        Texture2D butterflyLabel;
        Texture2D capeCoastLabel;
        Texture2D chickenNuggetLabel;
        Texture2D croissantLabel;
        Texture2D rattataLabel;
        #endregion

        #region Menu Related
        // menu
        Texture2D continueButton;
        Texture2D startButton;
        Texture2D exitButton;
        Texture2D instructionsButton;
        Texture2D menuBackground;
        Texture2D logo;

        Texture2D InstructionsLabel;
        Texture2D Map;

        Vector2 startButtonPosition;
        Vector2 exitButtonPosition;
        Vector2 continueButtonPosition;
        Vector2 instructionsButtonPosition;
        Vector2 logoPosition;

        MouseState mouseState;
        MouseState previousMouseState;

        bool instructionsOpen;
        int previousStateInstructions;
        bool drawSign;

        #endregion

        #region Enter Town Notification
        Texture2D townNotificationSign;
        Texture2D yesButton;
        Texture2D noButton;
        #endregion

        #region Player Related
        PlayerShip player;
        Rectangle ow_Player_CollBox; //overworld Player collisionbox
        bool facingRight;
        bool moving;
        Vector2 last_Coord;
        #endregion

        #region Ships and NPCS
        Texture2D[] shipImg;
        Texture2D[] b_shipImg;
        NPCShip[] OtherShip;
        NPCShip EnemyShip;
        Texture2D cannonball;
        Texture2D flag;
        Texture2D portrait;
        Texture2D enemyHealth;
        #endregion

        #region Instructions menu
        Texture2D objectiveMessage;
        Texture2D instructionTips;
        Texture2D hotkeys;
        #endregion

        #region Save File elements
        Texture2D saveFilesLabel;
        Texture2D file1Label;
        Texture2D file2Label;
        Texture2D file3Label;
        String savekey;
        bool noEmptySaveFileCheck = false;
        #endregion

        //error messages
        Texture2D moraleFullMessage;
        Texture2D healthFullMessage;

        //credits stuff
        Texture2D creditsButton;
        Texture2D creditsLabel;
        Texture2D credits;

        #region Animation related
        float t;
        float effectT;
        int step;   //I need to do something about merging all these step variables.
        int stepRadius;
        ParticleEngine ow_sailSpray;
        ParticleEngine b_SailStream;
        ParticleEngine b_SailStream2;
        Texture2D whiteblock;
        Texture2D[] smoke;
        #endregion

        #region General Status Bar Elements
        Texture2D statusBarBase;
        //SpriteFont VinerHand;
        Texture2D AlignmentBar;
        Texture2D HealthBar; // full bar
        Texture2D MoraleBar; // full bar
        Texture2D MenuSlider;
        Texture2D CrewIcon;
        Texture2D GoldIcon;
        #endregion

        #region Shop Window
        Texture2D shop_window_background;
        Texture2D shop_back_button;
        Texture2D shop_repair_button;
        Texture2D shop_item_image;
        Texture2D shop_label;
        Texture2D crew_label;
        Texture2D upgrades_label;
        int crewAdding = 0;
        Texture2D shipStats;
        Texture2D crewStats;
        Texture2D costLabel;
        Texture2D cannonStats;
        Texture2D beerStats;
        Texture2D buyButton;
        Texture2D hireButton;
        Texture2D line;
        int firstItemCost; // Write algorithm to calculate, proportional to current gold ammount
        int secondItemCost;
        int thirdItemCost;
        int itemSelected;
        int repairCost;
        #endregion

        int attackUpgrade = 10;
        int defenseUpgrade = 10;
        int speedUpgrade = 10;
        int maxAccelerationUpgrade = 10;
        int moraleUpgrade = 10;
        int reloadSpeedUpgrade = 10;
        int accelerationUpgrade = 10;
        int crewCost;
        bool notEnoughGold = false;

        Texture2D popUpBackground;
        Texture2D yesButtonSmaller;
        Texture2D noButtonSmaller;
        Texture2D insufficientFundsMessage;
        Texture2D errorBackButton;

        SpriteFont ourfont;
        Texture2D upArrow;
        Texture2D downArrow;
        Texture2D shopShip;
        Texture2D shopCannon;
        Texture2D beerIcon;
        #endregion


        public void setSavekey(String key)
        {
            savekey = key;
        }

        public string getSavekey()
        {
            return savekey;
        }

        private bool noSaveFiles()
        {
            return noEmptySaveFileCheck;
        }

        private void setNoSaveFiles(bool v)
        {
            noEmptySaveFileCheck = v;
        }

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

            PE = new ParticleEngine(null, -1, 0, 0, 0, 0);
            PE2 = new ParticleEngine(null, -1, 0, 0, 0, 0);

            #region Islands
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
            isl_y[3] = 750; //Nohely has 700

            isl_x[4] = 2300; // Fifth island hard-coded
            isl_y[4] = 300;
            #endregion

            #region Player Related
            player = new PlayerShip(1000, 2300, 0);



            player.set_bSpeed(0);
            facingRight = true;
            moving = false;
            last_Coord = new Vector2(1000, 2300);
            #endregion

            #region Animation related
            t = 0; //ever incrementing T
            step = 0; //used in animating. Hopefully will merge with t at somepoint
            stepRadius = 3; //used only in the ocean shifting effect
            effectT = 0; //also only used in ocean shifting effect

            smoke = new Texture2D[2];
            smoke[0] = Content.Load<Texture2D>("poof1");
            smoke[1] = Content.Load<Texture2D>("poof2");
            #endregion

            #region Other Ships
            OtherShip = new NPCShip[20];

            for (int i = 0; i < OtherShip.Length; i++)
            {

                int W, H, k;
                bool valid = true;

                do
                {
                    W = rand.Next(0, world_W);
                    H = rand.Next(0, world_H);

                    for (k = 0; k < 4; k++)
                    {
                        if (W > isl_x[k] && W < isl_x[k] + 396 && (H > isl_y[k] && H < isl_y[k] + 396))
                        {
                            valid = false;
                            break;
                        }
                        else
                        {
                            valid = true;
                        }
                        OtherShip[i] = new NPCShip(W, H, 0, "Pirate");

                        OtherShip[i].setAttack(rand.Next(20, 80));
                        OtherShip[i].setDefense(rand.Next(20, 80));
                        OtherShip[i].setPath(rand.Next(0, 2));
                    }
                } while (valid == false || i > OtherShip.Length);
            }
            #endregion

            #region Main Menu

            // if not full screen
            continueButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 40, 450); // middle of screen, width then height
            startButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 48, 525);
            instructionsButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 50, 600);
            exitButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 17, 675);
            logoPosition = new Vector2((GraphicsDevice.Viewport.Width / 30) - 48, 5);

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

            try
            {

                //Sound
                OverworldSong = Content.Load<Song>("Piratev2");
                cannonFire = Content.Load<SoundEffect>("CannonFire");

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
                CrewIcon = Content.Load<Texture2D>("Sword_Icon_Small");
                GoldIcon = Content.Load<Texture2D>("Gold");

                //loads all islands
                island[0] = Content.Load<Texture2D>("Island150p");
                island[1] = Content.Load<Texture2D>("Island250p");
                island[2] = Content.Load<Texture2D>("Island350p");
                island[3] = Content.Load<Texture2D>("Island450p");
                island[4] = Content.Load<Texture2D>("Island550p");

                //Loads Ship images
                shipImg = new Texture2D[1]; //maybe move up to init?
                b_shipImg = new Texture2D[1];
                shipImg[0] = Content.Load<Texture2D>("Ship1v3");
                b_shipImg[0] = Content.Load<Texture2D>("Ship_TopDown136_68");
                cannonball = Content.Load<Texture2D>("Battle_Cannonball16");
                portrait = Content.Load<Texture2D>("Pirate128v2");
                enemyHealth = Content.Load<Texture2D>("pirateHealth");

                // loads shop elements
                shop_window_background = Content.Load<Texture2D>("Shop_Window_Background_Biggest");
                shop_back_button = Content.Load<Texture2D>("Back_Button");
                shop_repair_button = Content.Load<Texture2D>("Repair_Ship_Button");
                shop_item_image = Content.Load<Texture2D>("Shop_ItemBox");
                shop_label = Content.Load<Texture2D>("Shop_Label");
                crew_label = Content.Load<Texture2D>("Hire_Crew_Label");
                upgrades_label = Content.Load<Texture2D>("Upgrades_Label");
                shopShip = Content.Load<Texture2D>("Ship_Small");
                shopCannon = Content.Load<Texture2D>("Cannonball_Small");
                beerStats = Content.Load<Texture2D>("Beer Stats Label");
                line = Content.Load<Texture2D>("Line");

                popUpBackground = Content.Load<Texture2D>("Pop Up");
                yesButtonSmaller = Content.Load<Texture2D>("Yes Smaller");
                noButtonSmaller = Content.Load<Texture2D>("No Smaller");
                insufficientFundsMessage = Content.Load<Texture2D>("Error Message");
                errorBackButton = Content.Load<Texture2D>("Error_Back_Button");

                // load island labels
                butterflyLabel = Content.Load<Texture2D>("Butterfly Label");
                capeCoastLabel = Content.Load<Texture2D>("Cape Coast Label");
                chickenNuggetLabel = Content.Load<Texture2D>("Chicken Nugget Label");
                croissantLabel = Content.Load<Texture2D>("Croissant Label");
                rattataLabel = Content.Load<Texture2D>("Rattata Label");

                // load enter town elements
                townNotificationSign = Content.Load<Texture2D>("Enter Town Prompt");
                noButton = Content.Load<Texture2D>("No");
                yesButton = Content.Load<Texture2D>("Yes");

                //load blank flag
                flag = Content.Load<Texture2D>("flag");

                //load map
                Map = Content.Load<Texture2D>("Map678");

                //load font
                ourfont = Content.Load<SpriteFont>("font");

                // load shop arrows
                upArrow = Content.Load<Texture2D>("Arrow_Up_Resized");
                downArrow = Content.Load<Texture2D>("Arrow_Down_Resized");

                beerIcon = Content.Load<Texture2D>("beer");

                shipStats = Content.Load<Texture2D>("Ship Stats Label");
                crewStats = Content.Load<Texture2D>("Crew Stats Label");
                costLabel = Content.Load<Texture2D>("Cost Label");
                cannonStats = Content.Load<Texture2D>("Attack Label");
                hireButton = Content.Load<Texture2D>("Hire Button");
                buyButton = Content.Load<Texture2D>("Buy Button");


                credits = Content.Load<Texture2D>("Credits");
                creditsLabel = Content.Load<Texture2D>("Credits Label");
                creditsButton = Content.Load<Texture2D>("Credits Button");
                moraleFullMessage = Content.Load<Texture2D>("Morale Error");
                healthFullMessage = Content.Load<Texture2D>("Ship Repair Error");

                //load save file elements
                saveFilesLabel = Content.Load<Texture2D>("Saved Games Label");
                file1Label = Content.Load<Texture2D>("File 1 Label");
                file2Label = Content.Load<Texture2D>("File 2 Label");
                file3Label = Content.Load<Texture2D>("File 3 Label");

                //loads instructions label;
                InstructionsLabel = Content.Load<Texture2D>("Instructions Label");
                objectiveMessage = Content.Load<Texture2D>("Objective");
                instructionTips = Content.Load<Texture2D>("Tips");
                hotkeys = Content.Load<Texture2D>("Hotkeys");

                //Set the players ship image
                player.setImage(shipImg[0]);
                player.setBattleImage(b_shipImg[0]);

                //Set up OtherShip
                NPCShip.setCBallImage(cannonball);
                for (int i = 0; i < OtherShip.Length; i++)
                {
                    OtherShip[i].setImage(shipImg[0]);
                    OtherShip[i].setBattleImage(b_shipImg[0]);
                }

                player.setCBallImage(cannonball);
                Ship.cannonFire = cannonFire;
            }
            catch
            {
                Debug.WriteLine("Failed to load an image");
            }
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

            instructionsOpen = false;

            xStep = 0;
            yStep = 0;

            nextPosX = (int)player.getX();
            nextPosY = (int)player.getY();

            Collision = false;

            DT = (float)gameTime.ElapsedGameTime.TotalSeconds;
            t += DT;//gameTime.TotalGameTime.Seconds;

            effectT += (float)(Math.PI / 2) * DT;

            if (effectT > 9999999) //unneccesary, but not broken. So not touching.
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
            {
                String[] saveinfo = System.IO.File.ReadAllLines("save.txt");
                for (int i = 0; i < saveinfo.Length; i++)
                {
                    if (saveinfo[i] == getSavekey())
                    {
                        Object[] newsaveinfo =
                                { getSavekey(), player.getMorale(), player.getAlignment(), player.getHealth(),
                                    player.getAttack(), player.getDefense(), player.get_bAcceleration(),
                                    player.get_bSpeed(), player.getGold(), player.getCrew(),
                                    (float)gameTime.ElapsedGameTime.TotalSeconds };
                        for (int j = 1; j < 11; j++)
                        {
                            saveinfo[i + j] = newsaveinfo[j].ToString();
                        }
                    }
                }
                System.IO.File.WriteAllLines("save.txt", saveinfo); //write new info to the text file
                Exit();
            }

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
            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                mapOpen = true;
            }
            else
            {
                mapOpen = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.I))
            {
                instructionsOpen = true;
                currentState = gameState.instructions;
            }
            else
            {
                instructionsOpen = false;
            }

            #endregion

            switch (currentState) //this gameState is for loading
            {
                case gameState.mainMenu: //Main Menu
                    #region Main Menu

                    crewAdding = 0;
                    buyOptionOpen = false;

                    IsMouseVisible = true; //enables mouse pointer

                    //wait for mouseclick
                    mouseState = Mouse.GetState();

                    if (previousMouseState.LeftButton == ButtonState.Pressed &&
                    mouseState.LeftButton == ButtonState.Released)
                    {
                        MouseClicked(mouseState.X, mouseState.Y);
                    }

                    previousMouseState = mouseState;

                    previousStateInstructions = 0;
                    #endregion
                    break;

                case gameState.overWorld: //overworld
                    #region Overworld

                    buyOptionOpen = false;
                    crewAdding = 0;

                    IsMouseVisible = false;

                    //wait for mouseclick
                    mouseState = Mouse.GetState();

                    if (previousMouseState.LeftButton == ButtonState.Pressed &&
                    mouseState.LeftButton == ButtonState.Released)
                    {
                        MouseClicked(mouseState.X, mouseState.Y);
                    }

                    previousMouseState = mouseState;

                    //Update enemy positions
                    for (int i = 0; i < OtherShip.Length; i++)
                        OtherShip[i].runOverworldAI(player, DT);

                    previousStateInstructions = 1;

                    //Check for collisions
                    #region Collision Checks
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

                    //If next step is the edge of the world, stop
                    if ((nextPosY < 0) || (nextPosY > world_H) || (nextPosX < 0) || (nextPosX > world_W))
                        Collision = true;

                    //  if next step is an island, stop
                    #region nextStep an island?

                    for (int i = 0; i < island.Length; i++) //must improve collision box on final islands.
                    {
                        if (nextPosX < (isl_x[i] + island[i].Width) && nextPosX > isl_x[i])
                        {
                            if (nextPosY < (isl_y[i] + island[i].Height) && nextPosY > isl_y[i])
                            {
                                Collision = true;
                                drawSign = true;
                            }
                        }
                    }
                    #endregion

                    //  if next step is a collision with other ship, go into battle with them
                    #region nextStep another ship?
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
                            last_Coord = new Vector2(player.getX(), player.getY());
                            battle_init(OtherShip[i]);
                        }
                    }
                    #endregion
                    //  if next step is a collision with a town, go into town or open town menu
                    if (Collision == false && drawSign == false)
                    {
                        player.setPos(player.getX() + (xStep * DT), player.getY() + (yStep * DT));
                        ow_Player_CollBox.X = (int)player.getX();
                        ow_Player_CollBox.Y = (int)player.getY();
                    }
                    #endregion

                    //Adjust player sprite
                    //  draw wake, use sin function to update wake. step 5 degrees*dt, then every ~3 seconds it'll be 15 degrees, then reverse
                    player.setRotate((float)(5 * Math.Sin(t)));

                    //Particle Effects
                    #region Particle Crap
                    ow_sailSpray.setX((facingRight) ? player.getX() + 18 : player.getX() - 18);
                    ow_sailSpray.setY(player.getY());
                    ow_sailSpray.setXSpeed((facingRight) ? -.1f : .1f);
                    ow_sailSpray.setXAccl((facingRight) ? -2 : 2);
                    ow_sailSpray.setYSpeed((-.3f) + yStep * DT);
                    ow_sailSpray.setYAccl(.1f * rand.Next(4, 8));// .5f);
                    ow_sailSpray.setTimeLimit(rand.Next(10, 20));

                    if ((updown || downdown) && !(leftdown || rightdown))
                    {
                        ow_sailSpray.setXSpeed(ow_sailSpray.getXSpeed() + ((facingRight) ? -60 * DT : 60 * DT));
                    }

                    ow_sailSpray.Update(DT);
                    #endregion

                    //update camera
                    camera.position = new Vector2(player.getX() - (screen_W / 2), player.getY() - (screen_H / 2));

                    #endregion
                    break;

                case gameState.battle: //In battle
                    #region In Battle

                    buyOptionOpen = false;
                    crewAdding = 0;

                    //check for pause

                    //check if player is dead
                    if (player.getHealth() <= 0)
                    {
                        currentState = gameState.overWorld;
                        overworld_init();
                        player.setHealth(50);
                        player.setPos(1000, 2300);
                        player.setGold((int)(player.getGold() * .75f));
                        if (player.getMorale() >= 10)
                            player.setMorale(player.getMorale() - 10f);
                    }
                    //check if enemy is dead
                    if (EnemyShip.getHealth() <= 0)
                    {
                        //ship is inactive
                        currentState = gameState.overWorld;
                        overworld_init();
                        player.setGold(player.getGold() + EnemyShip.getGold());
                        if (player.getMorale() < 100)
                            player.setMorale(player.getMorale() + 10f);

                        EnemyShip.setPos(2000, 5000);
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
                    float reloadSpeed;

                    reloadSpeed = 100 / player.getCrew();
                    reloadSpeed *= 1 / (player.getMorale() / 50);

                    if (reloadSpeed > 10)
                        reloadSpeed = 10;

                    if (spacedown && PE.Count < 1 && PE2.Count < 1 && player.canFire && (gameTime.TotalGameTime.TotalSeconds - player.lastFire) > reloadSpeed)
                    {
                        bool fireRight = player.fireCannon(EnemyShip, DT);
                        player.lastFire = (float)gameTime.TotalGameTime.TotalSeconds;
                        if (fireRight)
                        {
                            PE = new ParticleEngine(smoke[rand.Next(0, 1)], 50, 2 * (float)Math.Cos(MathHelper.ToRadians(player.getRotate() + 90)), 1.25f * (float)Math.Sin(MathHelper.ToRadians(player.getRotate() + 90)), -2 * (float)Math.Cos(MathHelper.ToRadians(player.getRotate() + 90)), -2 * (float)Math.Sin(MathHelper.ToRadians(player.getRotate() + 90)));
                            PE2 = new ParticleEngine(smoke[rand.Next(0, 1)], 50, 2 * (float)Math.Cos(MathHelper.ToRadians(player.getRotate() + 90)), 1.25f * (float)Math.Sin(MathHelper.ToRadians(player.getRotate() + 90)), -2 * (float)Math.Cos(MathHelper.ToRadians(player.getRotate() + 90)), -2 * (float)Math.Sin(MathHelper.ToRadians(player.getRotate() + 90)));

                        }
                        else
                        {
                            PE = new ParticleEngine(smoke[rand.Next(0, 1)], 50, 2 * (float)Math.Cos(MathHelper.ToRadians(player.getRotate() - 90)), 1.25f * (float)Math.Sin(MathHelper.ToRadians(player.getRotate() - 90)), -2 * (float)Math.Cos(MathHelper.ToRadians(player.getRotate() - 90)), -2 * (float)Math.Sin(MathHelper.ToRadians(player.getRotate() - 90)));
                            PE2 = new ParticleEngine(smoke[rand.Next(0, 1)], 50, 2 * (float)Math.Cos(MathHelper.ToRadians(player.getRotate() - 90)), 1.25f * (float)Math.Sin(MathHelper.ToRadians(player.getRotate() - 90)), -2 * (float)Math.Cos(MathHelper.ToRadians(player.getRotate() - 90)), -2 * (float)Math.Sin(MathHelper.ToRadians(player.getRotate() - 90)));

                        }
                        //PE = new ParticleEngine(smoke[rand.Next(0, 1)], 50, 1, 1, -2, -2); //doesn't work


                        PE.setX(player.getX());
                        PE.setY(player.getY() + 10);
                        PE.setRotateSpeed(60);
                        PE.EmitterLocation = player.getPos();
                        PE.setActive(true);

                        PE2.setX(player.getX());
                        PE2.setY(player.getY() - 10);
                        PE2.setRotateSpeed(60);
                        PE2.EmitterLocation = player.getPos();
                        cannonfired = true;
                        PE2.setActive(true);
                    }
                    if (PE.Count < 0)
                    {
                        PE.removeAll();
                        PE.setActive(false);
                    }
                    else
                    {
                        PE.Update(DT);
                    }
                    if (PE2.Count < 0)
                    {
                        PE2.removeAll();
                        PE2.setActive(false);
                    }
                    else
                    {
                        PE2.Update(DT);
                    }

                    //EnemyShip
                    EnemyShip.runStandardBattleAI(player, DT);
                    EnemyShip.updateCannonBalls(DT, player);

                    previousStateInstructions = 2;
                    #endregion
                    break;

                case gameState.inTown: //inTown  
                    #region In Town
                    IsMouseVisible = true; //enables mouse pointer

                    //wait for mouseclick
                    mouseState = Mouse.GetState();

                    crewCost = (int)((.5) * (double)player.getGold() * (double)crewAdding); // determines cost of crew members
                    reloadSpeedUpgrade = (int)(1.2 * (double)crewAdding);
                    accelerationUpgrade = (int)((1.5 * (double)player.get_bAcceleration()) * (double)crewAdding);

                    firstItemCost = attackUpgrade * 20;
                    secondItemCost = defenseUpgrade * 20;
                    thirdItemCost = player.getCrew() * 5;

                    repairCost = 20 * (100 - (int)player.getHealth());

                    if (previousMouseState.LeftButton == ButtonState.Pressed &&
                    mouseState.LeftButton == ButtonState.Released)
                    {
                        MouseClicked(mouseState.X, mouseState.Y);
                    }

                    previousMouseState = mouseState;

                    previousStateInstructions = 3;

                    drawSign = false;
                    #endregion
                    break;

                case gameState.instructions: //Instructions pulled up 
                    #region Instructions

                    buyOptionOpen = false;
                    IsMouseVisible = true; //enables mouse pointer

                    //wait for mouseclick
                    mouseState = Mouse.GetState();

                    if (previousMouseState.LeftButton == ButtonState.Pressed &&
                    mouseState.LeftButton == ButtonState.Released)
                    {
                        MouseClicked(mouseState.X, mouseState.Y);
                    }

                    previousMouseState = mouseState;

                    if (previousStateInstructions != 0 && previousStateInstructions != 1 && previousStateInstructions != 2 && previousStateInstructions != 3 && previousStateInstructions != 4)
                        previousStateInstructions = 5;

                    #endregion
                    break;
                case gameState.savefiles: //if save files are displayed
                    #region Save Files
                    buyOptionOpen = false;

                    IsMouseVisible = true; //enables mouse pointer

                    //wait for mouseclick
                    mouseState = Mouse.GetState();

                    if (previousMouseState.LeftButton == ButtonState.Pressed &&
                    mouseState.LeftButton == ButtonState.Released)
                    {
                        MouseClicked(mouseState.X, mouseState.Y);
                    }

                    previousMouseState = mouseState;

                    previousStateInstructions = 4;
                    #endregion
                    break;

                case gameState.credits:
                    buyOptionOpen = false;

                    IsMouseVisible = true; //enables mouse pointer

                    //wait for mouseclick
                    mouseState = Mouse.GetState();

                    if (previousMouseState.LeftButton == ButtonState.Pressed &&
                    mouseState.LeftButton == ButtonState.Released)
                    {
                        MouseClicked(mouseState.X, mouseState.Y);
                    }

                    previousMouseState = mouseState;

                    previousStateInstructions = 6;

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
                    spriteBatch.Draw(creditsButton, new Vector2(920, 10), Color.CadetBlue);

                    if (instructionsOpen)
                        currentState = gameState.instructions;

                    #endregion
                    break;
                case gameState.overWorld: //overworld
                    #region Overworld
                    #region Draws Ocean and Ocean Effects
                    int h = ((int)camera.position.Y / OceanTile.Height) * OceanTile.Height;
                    int w = (((int)camera.position.X / OceanTile.Width) * OceanTile.Width) - OceanTile.Width;

                    for (int y = h; y < (h + screen_H + OceanTile.Height); y += OceanTile.Height)
                    {
                        for (int x = w; x < (w + screen_W + (OceanTile.Width * 2)); x += OceanTile.Width)
                        {
                            spriteBatch.Draw(OceanTile, new Vector2(x, y), Color.White);
                            spriteBatch.Draw(OceanWeb, new Vector2(x + (float)(stepRadius * Math.Sin(effectT)), y), Color.White);
                        }
                    }
                    #endregion

                    #region Draw Islands
                    for (int i = 0; i < island.Length; i++)
                        spriteBatch.Draw(island[i], new Vector2(isl_x[i], isl_y[i]), Color.White);
                    #endregion

                    //Draw enemy ships
                    #region Draw enemy ships
                    for (int n = 0; n < OtherShip.Length; n++)
                    {   //change distance to stance, but I'd prefer if stances were an enum first
                        SpriteEffects flip = new SpriteEffects();
                        flip = ((player.getX() < OtherShip[n].getX()) && (Vector2.Distance(player.getPos(), OtherShip[n].getPos()) < 250) || !OtherShip[n].facingRight) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                        spriteBatch.Draw(OtherShip[n].getImage(), OtherShip[n].getPos(), null, Color.White,
                                        MathHelper.ToRadians(player.getRotate()), new Vector2(34, 50), 1f, flip, 1);
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

                        Vector2 flagPos;
                        if (flip == SpriteEffects.FlipHorizontally)
                        {   //May not be super efficient, but that's okay.
                            flagPos = new Vector2(OtherShip[n].getX() + 14 + (45 * (float)Math.Cos(MathHelper.ToRadians(player.getRotate() - 90))), OtherShip[n].getY() + (45 * (float)Math.Sin(MathHelper.ToRadians(player.getRotate() - 90))));
                        }
                        else
                        {
                            flagPos = new Vector2(OtherShip[n].getX() + (45 * (float)Math.Cos(MathHelper.ToRadians(player.getRotate() - 90))), OtherShip[n].getY() + (45 * (float)Math.Sin(MathHelper.ToRadians(player.getRotate() - 90))));
                        }


                        Color newColor = new Color(r, g, b); //Work on this.

                        spriteBatch.Draw(flag, flagPos, null, newColor,
                                        MathHelper.ToRadians(player.getRotate()), new Vector2(flag.Width, flag.Height / 2), 1f, SpriteEffects.None, 1);
                    }
                    #endregion

                    //Draw player
                    #region Draw Player
                    spriteBatch.Draw(player.getImage(), new Vector2(player.getX(), player.getY()), null, Color.White,
                    MathHelper.ToRadians(player.getRotate()), new Vector2(34, 50), 1f, (facingRight) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 1);
                    #endregion

                    //Sailing effect
                    #region particle crap
                    if (moving)
                    {
                        ow_sailSpray.Draw(spriteBatch);
                    }
                    #endregion

                    #region Draws Island Labels
                    //Draw Island Labels
                    spriteBatch.Draw(capeCoastLabel, new Vector2(710, 2410), Color.White);
                    spriteBatch.Draw(chickenNuggetLabel, new Vector2(1975, 1940), Color.White);
                    spriteBatch.Draw(butterflyLabel, new Vector2(750, 1390), Color.White);
                    spriteBatch.Draw(rattataLabel, new Vector2(1490, 900), Color.White);
                    spriteBatch.Draw(croissantLabel, new Vector2(2300, 460), Color.White);
                    #endregion

                    // draw status bar
                    #region Status bar
                    spriteBatch.Draw(statusBarBase, new Vector2(camera.position.X, camera.position.Y), Color.White);
                    spriteBatch.Draw(MoraleBar, new Vector2(camera.position.X + 735, camera.position.Y + 10), new Rectangle(0, 0, (int)((player.getMorale() / 100f) * MoraleBar.Width), MoraleBar.Height), Color.White);
                    spriteBatch.Draw(HealthBar, new Vector2(camera.position.X + 180, camera.position.Y + 10), new Rectangle(0, 0, (int)((player.getHealth() / 100f) * HealthBar.Width), HealthBar.Height), Color.White);
                    spriteBatch.Draw(AlignmentBar, new Vector2(camera.position.X + 180, camera.position.Y + 40), Color.White); // always same
                    spriteBatch.Draw(MenuSlider, new Vector2(camera.position.X + 285, camera.position.Y + 40), Color.White);
                    spriteBatch.Draw(CrewIcon, new Vector2(camera.position.X + 795, camera.position.Y + 40), Color.White);
                    spriteBatch.DrawString(ourfont, Convert.ToString(player.getCrew()), new Vector2(camera.position.X + 765, camera.position.Y + 45), Color.White);
                    spriteBatch.DrawString(ourfont, Convert.ToString(player.getGold()), new Vector2(camera.position.X + 910, camera.position.Y + 45), Color.White);
                    #endregion

                    //Draw Map
                    #region Draw Map
                    if (mapOpen)
                    {
                        Vector2 mapPos = new Vector2(player.getX() - (screen_W / 2) + 169, player.getY() - (screen_H / 2) + 89);
                        spriteBatch.Draw(Map, mapPos, Color.White);
                        //.226
                        Vector2 islPos;
                        for (int i = 0; i < 5; i++)
                        {
                            islPos = new Vector2(mapPos.X + isl_x[i] * .226f, mapPos.Y + isl_y[i] * .226f);
                            spriteBatch.Draw(island[i], islPos, null, Color.White, 0, new Vector2(0, 0), 0.226f, SpriteEffects.None, 1);
                        }

                        for (int k = 0; k < OtherShip.Length; k++)
                        {
                            spriteBatch.Draw(whiteblock, mapPos + OtherShip[k].getPos() * .226f, Color.Red);

                        }

                        spriteBatch.Draw(whiteblock, mapPos + player.getPos() * .226f, Color.LawnGreen);
                    }
                    #endregion

                    if (instructionsOpen == true)
                    {
                        spriteBatch.Draw(menuBackground, new Vector2(camera.position.X, camera.position.Y), Color.White);
                        spriteBatch.Draw(shop_window_background, new Vector2(camera.position.X + 160, camera.position.Y + 60), Color.White);
                        spriteBatch.Draw(InstructionsLabel, new Vector2(camera.position.X + 380, camera.position.Y + 110), Color.White);
                        spriteBatch.Draw(shop_back_button, new Vector2(camera.position.X + 480, camera.position.Y + 620), Color.White);
                        spriteBatch.Draw(objectiveMessage, new Vector2(290, 170), Color.White);
                        spriteBatch.Draw(instructionTips, new Vector2(280, 400), Color.White);
                        spriteBatch.Draw(hotkeys, new Vector2(430, 260), Color.White);
                    }

                    #region Draws Enter Town prompt
                    // draw when collide with island
                    if (drawSign == true)
                    {
                        //draws sign
                        spriteBatch.Draw(townNotificationSign, new Vector2(camera.position.X + 280, camera.position.Y + 180), Color.White);
                        spriteBatch.Draw(noButton, new Vector2(camera.position.X + 310, camera.position.Y + 400), Color.White);
                        spriteBatch.Draw(yesButton, new Vector2(camera.position.X + 550, camera.position.Y + 400), Color.White);
                        IsMouseVisible = true;
                    }
                    #endregion

                    #region Tells to Change Gamestate for Drawing Instructions
                    if (instructionsOpen)
                        currentState = gameState.instructions;
                    #endregion

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

                    if (PE.Count < 1)
                    {
                        cannonfired = false;
                        PE.setActive(false);
                    }

                    if (PE.Count > 0)
                    {
                        PE.Draw(spriteBatch);
                    }

                    if (PE2.Count < 1)
                    {
                        cannonfired = false;
                        PE2.setActive(false);
                    }

                    if (PE2.Count > 0)
                    {
                        PE2.Draw(spriteBatch);
                    }

                    //draws collision box vertices
                    //Vector2[] p = player.getCollisionbox();
                    //for (int i = 0; i < 4; i++)
                    //{
                    //    spriteBatch.Draw(whiteblock, new Vector2(p[i].X, p[i].Y), null, Color.Red);
                    //}

                    #region Status bar
                    //status bar
                    spriteBatch.Draw(statusBarBase, new Vector2(camera.position.X, camera.position.Y), Color.White);
                    spriteBatch.Draw(MoraleBar, new Vector2(camera.position.X + 735, camera.position.Y + 10), new Rectangle(0, 0, (int)((player.getMorale() / 100f) * MoraleBar.Width), MoraleBar.Height), Color.White);
                    spriteBatch.Draw(HealthBar, new Vector2(camera.position.X + 180, camera.position.Y + 10), new Rectangle(0, 0, (int)((player.getHealth() / 100f) * HealthBar.Width), HealthBar.Height), Color.White);
                    spriteBatch.Draw(AlignmentBar, new Vector2(camera.position.X + 180, camera.position.Y + 40), Color.White); // always same
                    spriteBatch.Draw(MenuSlider, new Vector2(camera.position.X + 285, camera.position.Y + 40), Color.White);
                    spriteBatch.Draw(CrewIcon, new Vector2(camera.position.X + 795, camera.position.Y + 40), Color.White);
                    spriteBatch.DrawString(ourfont, Convert.ToString(player.getCrew()), new Vector2(camera.position.X + 765, camera.position.Y + 45), Color.White);
                    spriteBatch.DrawString(ourfont, Convert.ToString(player.getGold()), new Vector2(camera.position.X + 910, camera.position.Y + 45), Color.White);
                    #endregion

                    #region portrait
                    bool transparent = false;
                    if (Vector2.Distance(new Vector2(camera.position.X, camera.position.Y + screen_H), player.getPos()) < 200)
                        transparent = true;
                    else if (Vector2.Distance(new Vector2(camera.position.X, camera.position.Y + screen_H), EnemyShip.getPos()) < 200)
                        transparent = true;

                    spriteBatch.Draw(portrait, new Vector2(camera.position.X, camera.position.Y + screen_H - portrait.Height), (!transparent) ? Color.White : new Color(255, 255, 255, 125));
                    spriteBatch.Draw(enemyHealth, new Vector2(camera.position.X + 7, camera.position.Y + screen_H - portrait.Height + 139), new Rectangle(0, 0, (int)((EnemyShip.getHealth() / 100f) * enemyHealth.Width), enemyHealth.Height), (!transparent) ? Color.White : new Color(255, 255, 255, 125));

                    #endregion
                    #endregion
                    break;
                case gameState.inTown: //Shopping
                    #region In Shop

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
                    spriteBatch.Draw(CrewIcon, new Vector2(camera.position.X + 795, camera.position.Y + 40), Color.White);
                    spriteBatch.DrawString(ourfont, Convert.ToString(player.getCrew()), new Vector2(camera.position.X + 765, camera.position.Y + 45), Color.White);
                    spriteBatch.DrawString(ourfont, Convert.ToString(player.getGold()), new Vector2(camera.position.X + 910, camera.position.Y + 45), Color.White);
                    #endregion

                    #region Draw Shop Items
                    //draw shop window objects
                    spriteBatch.Draw(shop_window_background, new Vector2(camera.position.X + 170, camera.position.Y + 115), Color.White);
                    spriteBatch.Draw(shop_label, new Vector2(camera.position.X + 445, camera.position.Y + 140), Color.White);
                    spriteBatch.Draw(upgrades_label, new Vector2(camera.position.X + 325, camera.position.Y + 215), Color.White);
                    spriteBatch.Draw(crew_label, new Vector2(camera.position.X + 643, camera.position.Y + 215), Color.White);

                    //draw buttons
                    spriteBatch.Draw(shop_back_button, new Vector2(camera.position.X + 730, camera.position.Y + 645), Color.White);
                    spriteBatch.Draw(shop_repair_button, new Vector2(camera.position.X + 667, camera.position.Y + 540), Color.White);

                    //draw item background boxes
                    spriteBatch.Draw(shop_item_image, new Vector2(camera.position.X + 210, camera.position.Y + 300), Color.White);
                    spriteBatch.Draw(shop_item_image, new Vector2(camera.position.X + 210, camera.position.Y + 420), Color.White);
                    spriteBatch.Draw(shop_item_image, new Vector2(camera.position.X + 210, camera.position.Y + 540), Color.White);

                    //draw items for purchase icons
                    spriteBatch.Draw(shopShip, new Vector2(camera.position.X + 213, camera.position.Y + 427), Color.White);
                    spriteBatch.Draw(shopCannon, new Vector2(camera.position.X + 213, camera.position.Y + 302), Color.White);
                    spriteBatch.Draw(beerIcon, new Vector2(camera.position.X + 214, camera.position.Y + 544), Color.White);

                    //draw crew number
                    spriteBatch.DrawString(ourfont, Convert.ToString(crewAdding), new Vector2(camera.position.X + 705, camera.position.Y + 298), Color.Black);

                    //draw crew spinner arrows
                    spriteBatch.Draw(upArrow, new Vector2(camera.position.X + 727, camera.position.Y + 280), Color.White);
                    spriteBatch.Draw(downArrow, new Vector2(camera.position.X + 727, camera.position.Y + 300), Color.White);

                    //draw stat labels
                    spriteBatch.Draw(shipStats, new Vector2(camera.position.X + 300, camera.position.Y + 415), Color.White);
                    spriteBatch.Draw(cannonStats, new Vector2(camera.position.X + 300, camera.position.Y + 315), Color.White);
                    spriteBatch.Draw(beerStats, new Vector2(camera.position.X + 300, camera.position.Y + 555), Color.White);
                    spriteBatch.Draw(crewStats, new Vector2(camera.position.X + 660, camera.position.Y + 335), Color.White);

                    //draw purchase buttons
                    spriteBatch.Draw(hireButton, new Vector2(camera.position.X + 695, camera.position.Y + 435), Color.White);
                    spriteBatch.Draw(buyButton, new Vector2(camera.position.X + 490, camera.position.Y + 345), Color.White);
                    spriteBatch.Draw(buyButton, new Vector2(camera.position.X + 490, camera.position.Y + 455), Color.White);
                    spriteBatch.Draw(buyButton, new Vector2(camera.position.X + 490, camera.position.Y + 580), Color.White);

                    //draw gold icons
                    spriteBatch.Draw(GoldIcon, new Vector2(camera.position.X + 735, camera.position.Y + 390), Color.White);
                    spriteBatch.Draw(GoldIcon, new Vector2(camera.position.X + 540, camera.position.Y + 410), Color.White);
                    spriteBatch.Draw(GoldIcon, new Vector2(camera.position.X + 540, camera.position.Y + 298), Color.White);
                    spriteBatch.Draw(GoldIcon, new Vector2(camera.position.X + 540, camera.position.Y + 535), Color.White);

                    // draw line divider
                    spriteBatch.Draw(line, new Vector2(camera.position.X + 600, camera.position.Y + 290), Color.Brown);

                    spriteBatch.DrawString(ourfont, Convert.ToString(firstItemCost), new Vector2(camera.position.X + 510, camera.position.Y + 310), Color.Black);
                    spriteBatch.DrawString(ourfont, Convert.ToString(secondItemCost), new Vector2(camera.position.X + 510, camera.position.Y + 420), Color.Black);
                    spriteBatch.DrawString(ourfont, Convert.ToString(thirdItemCost), new Vector2(camera.position.X + 510, camera.position.Y + 550), Color.Black);
                    spriteBatch.DrawString(ourfont, Convert.ToString(crewCost), new Vector2(camera.position.X + 705, camera.position.Y + 405), Color.Black);


                    spriteBatch.DrawString(ourfont, Convert.ToString(attackUpgrade), new Vector2(camera.position.X + 375, camera.position.Y + 317), Color.Black);
                    spriteBatch.DrawString(ourfont, Convert.ToString(defenseUpgrade), new Vector2(camera.position.X + 375, camera.position.Y + 420), Color.Black);
                    spriteBatch.DrawString(ourfont, Convert.ToString(speedUpgrade), new Vector2(camera.position.X + 360, camera.position.Y + 445), Color.Black);
                    spriteBatch.DrawString(ourfont, Convert.ToString(maxAccelerationUpgrade), new Vector2(camera.position.X + 450, camera.position.Y + 470), Color.Black);
                    spriteBatch.DrawString(ourfont, Convert.ToString(moraleUpgrade), new Vector2(camera.position.X + 370, camera.position.Y + 557), Color.Black);
                    spriteBatch.DrawString(ourfont, Convert.ToString(reloadSpeedUpgrade), new Vector2(camera.position.X + 780, camera.position.Y + 340), Color.Black);
                    spriteBatch.DrawString(ourfont, Convert.ToString(accelerationUpgrade), new Vector2(camera.position.X + 780, camera.position.Y + 365), Color.Black);


                    // Write algorithm to calculate, proportional to current gold ammount

                    if (buyOptionOpen == true)
                    {
                        if ((itemSelected != 3 && moraleFull != false) && ((itemSelected == 3 && healthFull == false)))
                            spriteBatch.Draw(noButtonSmaller, new Vector2(camera.position.X + 340, camera.position.Y + 430), Color.White);
                        spriteBatch.Draw(yesButtonSmaller, new Vector2(camera.position.X + 580, camera.position.Y + 430), Color.White);
                        returnError = false;
                    }


                    #endregion

                    if (notEnoughGold == true)
                    {
                        spriteBatch.Draw(popUpBackground, new Vector2(camera.position.X + 205, camera.position.Y + 320), Color.White);
                        spriteBatch.Draw(insufficientFundsMessage, new Vector2(camera.position.X + 255, camera.position.Y + 380), Color.White);
                        spriteBatch.Draw(errorBackButton, new Vector2(camera.position.X + 500, camera.position.Y + 440), Color.White);
                    }



                    if (healthFull == true)
                    {
                        buyOptionOpen = false;
                        spriteBatch.Draw(popUpBackground, new Vector2(camera.position.X + 205, camera.position.Y + 320), Color.White);
                        spriteBatch.Draw(healthFullMessage, new Vector2(camera.position.X + 440, camera.position.Y + 390), Color.White);
                        spriteBatch.Draw(errorBackButton, new Vector2(camera.position.X + 500, camera.position.Y + 440), Color.White);
                        returnError = true;
                    }

                    if (moraleFull == true)
                    {
                        buyOptionOpen = false;
                        spriteBatch.Draw(popUpBackground, new Vector2(camera.position.X + 205, camera.position.Y + 320), Color.White);
                        spriteBatch.Draw(moraleFullMessage, new Vector2(camera.position.X + 410, camera.position.Y + 390), Color.White);
                        spriteBatch.Draw(errorBackButton, new Vector2(camera.position.X + 500, camera.position.Y + 440), Color.White);
                        returnError = true;
                    }


                    #region Draws Buy Item prompt
                    // draw when collide with island
                    if (buyOptionOpen == true)
                    {

                        //draws sign
                        spriteBatch.Draw(popUpBackground, new Vector2(camera.position.X + 205, camera.position.Y + 320), Color.White);
                        spriteBatch.DrawString(ourfont, "Are you sure you want to spend ", new Vector2(camera.position.X + 415, camera.position.Y + 355), Color.Black);
                        spriteBatch.Draw(GoldIcon, new Vector2(camera.position.X + 565, camera.position.Y + 380), Color.White);
                        spriteBatch.Draw(noButtonSmaller, new Vector2(camera.position.X + 340, camera.position.Y + 430), Color.White);
                        spriteBatch.Draw(yesButtonSmaller, new Vector2(camera.position.X + 580, camera.position.Y + 430), Color.White);
                        IsMouseVisible = true;

                        {
                            switch (itemSelected)
                            {
                                case 1:
                                    spriteBatch.DrawString(ourfont, (Convert.ToString(firstItemCost)), new Vector2(camera.position.X + 520, camera.position.Y + 390), Color.Black);
                                    break;

                                case 2:
                                    spriteBatch.DrawString(ourfont, (Convert.ToString(secondItemCost)), new Vector2(camera.position.X + 520, camera.position.Y + 390), Color.Black);
                                    break;

                                case 3:
                                    spriteBatch.DrawString(ourfont, (Convert.ToString(thirdItemCost)), new Vector2(camera.position.X + 520, camera.position.Y + 390), Color.Black);
                                    break;

                                case 4:
                                    spriteBatch.DrawString(ourfont, (Convert.ToString(crewCost)), new Vector2(camera.position.X + 520, camera.position.Y + 390), Color.Black);
                                    break;
                                case 5:
                                    spriteBatch.DrawString(ourfont, (Convert.ToString(repairCost)), new Vector2(camera.position.X + 520, camera.position.Y + 390), Color.Black);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    #endregion
                    #endregion
                    break;
                case gameState.instructions: // Instructions
                    #region Draws instructions 
                    spriteBatch.Draw(menuBackground, new Vector2(camera.position.X, camera.position.Y), Color.White);
                    spriteBatch.Draw(shop_window_background, new Vector2(camera.position.X + 160, camera.position.Y + 60), Color.White);
                    spriteBatch.Draw(InstructionsLabel, new Vector2(camera.position.X + 380, camera.position.Y + 110), Color.White);
                    spriteBatch.Draw(shop_back_button, new Vector2(camera.position.X + 480, camera.position.Y + 620), Color.White);
                    spriteBatch.Draw(objectiveMessage, new Vector2(camera.position.X + 290, camera.position.Y + 170), Color.White); //breaks here
                    spriteBatch.Draw(instructionTips, new Vector2(camera.position.X + 280, camera.position.Y + 400), Color.White);
                    spriteBatch.Draw(hotkeys, new Vector2(camera.position.X + 430, camera.position.Y + 260), Color.White);

                    #endregion
                    break;
                case gameState.savefiles:
                    #region Draw Save Files Elements
                    spriteBatch.Draw(menuBackground, new Vector2(camera.position.X, camera.position.Y), Color.White);
                    spriteBatch.Draw(shop_window_background, new Vector2(camera.position.X + 160, camera.position.Y + 60), Color.White);
                    spriteBatch.Draw(saveFilesLabel, new Vector2(camera.position.X + 380, camera.position.Y + 110), Color.White);
                    spriteBatch.Draw(file1Label, new Vector2(camera.position.X + 220, camera.position.Y + 200), Color.White);
                    spriteBatch.Draw(file2Label, new Vector2(camera.position.X + 220, camera.position.Y + 350), Color.White);
                    spriteBatch.Draw(file3Label, new Vector2(camera.position.X + 220, camera.position.Y + 500), Color.White);
                    spriteBatch.Draw(shop_back_button, new Vector2(camera.position.X + 490, camera.position.Y + 605), Color.White);
                    spriteBatch.Draw(continueButton, new Vector2(camera.position.X + 675, camera.position.Y + 215), Color.White);
                    spriteBatch.Draw(continueButton, new Vector2(camera.position.X + 675, camera.position.Y + 365), Color.White);
                    spriteBatch.Draw(continueButton, new Vector2(camera.position.X + 675, camera.position.Y + 515), Color.White);
                    #endregion
                    break;
                case gameState.credits:
                    spriteBatch.Draw(menuBackground, new Vector2(camera.position.X, camera.position.Y), Color.White);
                    spriteBatch.Draw(shop_window_background, new Vector2(camera.position.X + 160, camera.position.Y + 60), Color.White);
                    spriteBatch.Draw(creditsLabel, new Vector2(camera.position.X + 410, camera.position.Y + 125), Color.White);
                    spriteBatch.Draw(shop_back_button, new Vector2(camera.position.X + 480, camera.position.Y + 560), Color.White);
                    spriteBatch.Draw(credits, new Vector2(camera.position.X + 370, camera.position.Y + 200), Color.White);
                    break;
                default:
                    //change gameTime 
                    String[] saveinfo = System.IO.File.ReadAllLines("save.txt");
                    for (int i = 0; i < saveinfo.Length; i++)
                    {
                        if (saveinfo[i] == getSavekey())
                        {
                            saveinfo[i + 10] = gameTime.ToString();
                        }
                    }
                    //write new info to the text file
                    System.IO.File.WriteAllLines("save.txt", saveinfo);
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

            if (currentState == gameState.overWorld && drawSign == true)
            {
                Rectangle yesEnterIslandRect = new Rectangle(550, 400, 209, 118);
                Rectangle noEnterIslandRect = new Rectangle(310, 400, 210, 121);

                if (mouseClickRect.Intersects(yesEnterIslandRect)) // click yes
                {
                    currentState = gameState.inTown;
                }
                else
                {
                    currentState = gameState.overWorld;
                    IsMouseVisible = false;
                    drawSign = false;
                    overworld_init();
                }

                if (mouseClickRect.Intersects(noEnterIslandRect)) // click no
                {
                    currentState = gameState.overWorld;
                    IsMouseVisible = false;
                    drawSign = false;
                    overworld_init();
                }
            }
            if (currentState == gameState.instructions) //wonky
            {
                Rectangle instructionBackRect = new Rectangle(480, 620, 73, 40);


                if (mouseClickRect.Intersects(instructionBackRect))
                {
                    if (previousStateInstructions == 0)
                        currentState = gameState.mainMenu;
                    else if (previousStateInstructions == 1)
                        currentState = gameState.overWorld; // issues
                    else if (previousStateInstructions == 2)
                        currentState = gameState.battle;
                    else if (previousStateInstructions == 3)
                        currentState = gameState.inTown;
                    else if (previousStateInstructions == 4)
                        currentState = gameState.savefiles;
                    else if (previousStateInstructions == 5)
                        currentState = gameState.mainMenu;
                    else if (previousStateInstructions == 6)
                        currentState = gameState.mainMenu;
                }
            }

            //check the shop menu
            if (currentState == gameState.inTown)
            {
                Rectangle repairShipButtonRect = new Rectangle(667, 540, 138, 40);
                Rectangle backButtonRect = new Rectangle(730, 645, 73, 40);
                Rectangle upArrowRect = new Rectangle(727, 280, 20, 17);
                Rectangle downArrowRect = new Rectangle(727, 300, 20, 17);
                Rectangle buyOneRect = new Rectangle(490, 455, 77, 40);
                Rectangle buyTwoRect = new Rectangle(490, 345, 77, 40);
                Rectangle buyThreeRect = new Rectangle(490, 580, 77, 40);
                Rectangle hireButRect = new Rectangle(695, 435, 81, 40);

                if (returnError == true)
                {
                    Rectangle closeErrorRect = new Rectangle(500, 440, 73, 40);

                    if (mouseClickRect.Intersects(closeErrorRect))
                    {
                        IsMouseVisible = false;
                        currentState = gameState.inTown;
                        healthFull = false;
                        moraleFull = false;
                    }

                }
                //500, 440

                if (buyOptionOpen == true)
                {

                    Rectangle noButtonSmallerRect = new Rectangle(340, 430, 126, 73);
                    Rectangle yesButtonSmallerRect = new Rectangle(580, 430, 125, 71);

                    if (mouseClickRect.Intersects(yesButtonSmallerRect))
                    {
                        IsMouseVisible = false;
                        currentState = gameState.inTown;

                        if (itemSelected == 1 && ((int)((player.getGold() - firstItemCost)) >= 0))
                        {
                            player.setGold(player.getGold() - firstItemCost);
                            notEnoughGold = false;
                            player.setAttack(player.getAttack() + attackUpgrade);
                        }
                        else if (itemSelected == 1 && ((int)((player.getGold() - firstItemCost)) < 0))
                            notEnoughGold = true;

                        if (itemSelected == 2 && ((int)((player.getGold() - secondItemCost)) >= 0))
                        {
                            player.setGold(player.getGold() - secondItemCost);
                            notEnoughGold = false;
                            player.setDefense(player.getAttack() + attackUpgrade);
                            player.set_bSpeed(player.get_bSpeed() + speedUpgrade);
                            player.set_bAcceleration(player.get_bAcceleration() + accelerationUpgrade);
                        }
                        else if (itemSelected == 2 && ((int)((player.getGold() - secondItemCost)) < 0))
                            notEnoughGold = true;



                        if (itemSelected == 3 && player.getMorale() < 100 && ((int)((player.getGold() - thirdItemCost)) >= 0))
                        {
                            player.setGold(player.getGold() - thirdItemCost);
                            notEnoughGold = false;
                            player.setMorale(100);
                        }
                        else if (itemSelected == 3 && player.getMorale() < 100 && ((int)((player.getGold() - thirdItemCost)) < 0))
                            notEnoughGold = true;
                        else if (itemSelected == 3 && player.getMorale() >= 100)
                            moraleFull = true;

                        if (itemSelected == 4 && ((int)((player.getGold() - crewCost)) >= 0))
                        {
                            player.setGold(player.getGold() - crewCost);
                            player.setCrew(player.getCrew() + crewAdding);
                            notEnoughGold = false;
                        }
                        else if (itemSelected == 4 && ((int)((player.getGold() - crewCost)) < 0))
                            notEnoughGold = true;

                        if (itemSelected == 5 && player.getHealth() < 100 && (((int)(player.getGold() - repairCost)) >= 0))
                        {
                            player.setGold(player.getGold() - repairCost);
                            notEnoughGold = false;
                        }
                        else if (itemSelected == 5 && player.getHealth() < 100 && ((int)((player.getGold() - repairCost)) < 0))
                            notEnoughGold = true;
                        else if (itemSelected == 5 && player.getHealth() >= 100)
                            healthFull = true;

                        buyOptionOpen = false;
                    }


                    if (mouseClickRect.Intersects(noButtonSmallerRect))
                    {
                        IsMouseVisible = false;
                        currentState = gameState.inTown;
                        buyOptionOpen = false;
                        notEnoughGold = false;
                    }
                }

                if (mouseClickRect.Intersects(repairShipButtonRect)) //player clicked repair button
                {
                    buyOptionOpen = true;
                    itemSelected = 5;
                    //currentState = gameState.overWorld;
                    //pop up to confirm w/ gold price proportional to damage
                }
                else if (mouseClickRect.Intersects(backButtonRect)) // click back, sends to overworld
                {
                    currentState = gameState.overWorld;
                }
                else if (mouseClickRect.Intersects(upArrowRect)) //Stops working after one click
                {
                    crewAdding++;
                }
                else if (mouseClickRect.Intersects(downArrowRect)) //stops working after one click
                {
                    if (crewAdding > 0)
                    {
                        crewAdding--;
                    }
                }
                else if (mouseClickRect.Intersects(buyOneRect)) // click back, sends to overworld
                {
                    buyOptionOpen = true;
                    itemSelected = 1;
                    //currentState = gameState.overWorld;
                }
                else if (mouseClickRect.Intersects(buyTwoRect)) // click back, sends to overworld
                {
                    buyOptionOpen = true;
                    itemSelected = 2;
                    // currentState = gameState.overWorld;
                }
                else if (mouseClickRect.Intersects(buyThreeRect)) // click back, sends to overworld
                {
                    buyOptionOpen = true;
                    itemSelected = 3;
                    //currentState = gameState.overWorld;
                }
                else if (mouseClickRect.Intersects(hireButRect)) // click back, sends to overworld
                {
                    buyOptionOpen = true;
                    itemSelected = 4;
                    //currentState = gameState.overWorld;
                }
                else
                {
                    currentState = gameState.inTown;
                }
                // }
            }

            if (currentState == gameState.credits)
            {


                Rectangle goBacktoMainRect = new Rectangle(480, 560, 73, 40);

                if (mouseClickRect.Intersects(goBacktoMainRect)) // click back, sends to overworld
                {
                    currentState = gameState.mainMenu;
                }
                else
                {
                    currentState = gameState.credits;
                }

            }

            //check the startmenu
            if (currentState == gameState.mainMenu)
            {

                Rectangle continueButtonRect = new Rectangle((int)continueButtonPosition.X,
                                      (int)continueButtonPosition.Y, 131, 40);
                Rectangle startButtonRect = new Rectangle((int)startButtonPosition.X,
                                      (int)startButtonPosition.Y, 149, 40);
                Rectangle instructionsButtonRect = new Rectangle((int)instructionsButtonPosition.X,
                                      (int)instructionsButtonPosition.Y, 157, 40);
                Rectangle exitButtonRect = new Rectangle((int)exitButtonPosition.X,
                                      (int)exitButtonPosition.Y, 83, 40);
                Rectangle creditsButtonRect = new Rectangle(920, 10, 108, 39);

                if (mouseClickRect.Intersects(startButtonRect)) //player clicked start button
                {

                    String[] saveinfo = System.IO.File.ReadAllLines("save.txt");
                    try
                    {
                        if (saveinfo[saveinfo.Length - 1].Contains("o")) //if there is at least one empty file
                        {
                            String lastline = saveinfo[saveinfo.Length - 1]; //checks last line of save.txt
                            int savefileno = lastline.IndexOf('o'); //finds first empty file, indicated by 'o' and saves to that file
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            for (int i = 0; i < savefileno; i++)
                            {
                                sb.Append(lastline[i]);
                            }
                            for (int i = savefileno; i < lastline.Length; i++)
                            {
                                if (i == savefileno)
                                {
                                    sb.Append('x');
                                }
                                else
                                {
                                    sb.Append(lastline[i]);
                                }
                            }
                            setNoSaveFiles(false);
                            saveinfo[saveinfo.Length - 1] = sb.ToString();
                            System.IO.File.WriteAllLines("save.txt", saveinfo);
                            setSavekey("SF" + ((savefileno + 1).ToString()));
                            newPlayerShip(player);
                            currentState = gameState.overWorld;
                        }
                        else //if there are no empty save files
                        {
                            setNoSaveFiles(true);
                            currentState = gameState.savefiles;

                        }
                    }
                    catch (Exception e)
                    {
                        Console.Write(e);
                    }

                    //MediaPlayer.Play(OverworldSong);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = 1.0f;
                    overworld_init();
                }
                else if (mouseClickRect.Intersects(continueButtonRect))
                {
                    currentState = gameState.savefiles;
                }

                else if (mouseClickRect.Intersects(instructionsButtonRect))
                {
                    currentState = gameState.mainMenu;
                    instructionsOpen = true;
                }

                else if (mouseClickRect.Intersects(exitButtonRect)) //player clicked exit button
                {
                    //save information to the appropriate text file
                    String[] saveinfo = System.IO.File.ReadAllLines("save.txt");
                    for (int i = 0; i < saveinfo.Length; i++)
                    {
                        if (saveinfo[i] == getSavekey())
                        {
                            for (int j = 1; j < 11; j++)
                            {
                                Object[] newsaveinfo =
                                    {   getSavekey(), player.getMorale(), player.getAlignment(), player.getHealth(),
                                        player.getAttack(), player.getDefense(), player.get_bAcceleration(),
                                        player.get_bSpeed(), player.getGold(), player.getCrew() };

                                saveinfo[i + j] = newsaveinfo[j].ToString();
                            }

                        }

                    }
                    //write new info to the text file
                    System.IO.File.WriteAllLines("save.txt", saveinfo);

                    Exit();
                }
                else if (mouseClickRect.Intersects(creditsButtonRect)) //player clicked credits button
                {
                    currentState = gameState.credits;
                    //Exit(); // Change to brings up credits page
                }

            }

            if (currentState == gameState.savefiles)
            {
                Rectangle gobackRect = new Rectangle(490, 605, 75, 40);
                Rectangle savefile1Rect = new Rectangle(675, 215, 138, 40);
                Rectangle savefile2Rect = new Rectangle(675, 365, 138, 40);
                Rectangle savefile3Rect = new Rectangle(675, 515, 138, 40);

                if (noSaveFiles()) //if there are no free save files, choose a file to overwrite
                {
                    //spriteBatch.DrawString(ourfont, "Select a file to overwrite:", new Vector2(camera.position.X + 405, camera.position.Y + 175), Color.Black);

                    if (mouseClickRect.Intersects(gobackRect)) //the back button is not working correctly..
                    {
                        currentState = gameState.mainMenu;
                    }

                    if (mouseClickRect.Intersects(savefile1Rect))
                    {
                        setSavekey("SF1");
                        currentState = gameState.overWorld;
                        newPlayerShip(player);
                        overworld_init();
                    }

                    if (mouseClickRect.Intersects(savefile2Rect))
                    {
                        setSavekey("SF2");
                        currentState = gameState.overWorld;
                        newPlayerShip(player);
                        overworld_init();
                    }

                    if (mouseClickRect.Intersects(savefile3Rect))
                    {
                        setSavekey("SF3");
                        currentState = gameState.overWorld;
                        newPlayerShip(player);
                        overworld_init();
                    }
                }

                else if (noSaveFiles() == false) //otherwise, the new game overwrite event wouldn't have triggered; continue game as usual
                {
                    if (mouseClickRect.Intersects(gobackRect))
                    {
                        currentState = gameState.mainMenu;
                    }

                    if (mouseClickRect.Intersects(savefile1Rect))
                    {
                        setSavekey("SF1");
                        currentState = gameState.overWorld;
                        loadPlayerStats(player);
                        overworld_init();
                    }

                    if (mouseClickRect.Intersects(savefile2Rect))
                    {
                        setSavekey("SF2");
                        currentState = gameState.overWorld;
                        loadPlayerStats(player);
                        overworld_init();
                    }

                    if (mouseClickRect.Intersects(savefile3Rect))
                    {
                        setSavekey("SF3");
                        currentState = gameState.overWorld;
                        loadPlayerStats(player);
                        overworld_init();
                    }
                }
            }
        }

        protected void battle_init(NPCShip Enemy)
        {
            player.setRotate(0);
            player.canFire = true;
            camera.position = new Vector2(player.getX() - (screen_W / 2), player.getY() - (screen_H / 2));

            //EnemyShip = //whatever collided with
            //EnemyShip = new NPCShip(cannonball, player.getX() - 250, player.getY() + 50, 270, "pirate");
            EnemyShip = Enemy;
            EnemyShip.setX(player.getX() - 250);
            EnemyShip.setY(player.getY() + 50);
            EnemyShip.setRotate(270);
            //EnemyShip.setBattleImage(b_shipImg[0]);

            b_SailStream = new ParticleEngine(whiteblock, 20, -1, 0, 0, 0);
            b_SailStream.setX(player.getX());
            b_SailStream.setY(player.getY() - 30);
            b_SailStream.setActive(true);
            b_SailStream2 = new ParticleEngine(whiteblock, 20, -1, 0, 0, 0);
            b_SailStream2.setX(player.getX());
            b_SailStream2.setY(player.getY() + 50);
            b_SailStream2.setActive(true);

            PE.removeAll();
            PE2.removeAll();

            ow_sailSpray.removeAll();
            ow_sailSpray.setActive(false);
        }

        protected void overworld_init()
        {
            //MediaPlayer.Play(OverworldSong);
            //MediaPlayer.Volume = 1.0f;
            //MediaPlayer.IsRepeating = true;
            player.setPos(last_Coord.X, last_Coord.Y);
            player.setRotate(0);
            camera.position = new Vector2(player.getX() - (screen_W / 2), player.getY() - (screen_H / 2));
            ow_sailSpray = new ParticleEngine(whiteblock, 20, -1, 0, 0, 0);

            ow_sailSpray.setX(player.getX());
            ow_sailSpray.setY(player.getY());
            ow_sailSpray.setActive(true);
        }

        protected void loadPlayerStats(PlayerShip player)
        {
            //read information from text file

            String[] saveinfo = System.IO.File.ReadAllLines("save.txt");
            for (int i = 0; i < saveinfo.Length; i++)
            {
                if (saveinfo[i] == getSavekey())
                {
                    player.setMorale(float.Parse(saveinfo[i + 1]));
                    player.setAlignment(float.Parse(saveinfo[i + 2]));
                    player.setHealth(float.Parse(saveinfo[i + 3]));
                    player.setAttack(float.Parse(saveinfo[i + 4]));
                    player.setDefense(float.Parse(saveinfo[i + 5]));
                    player.set_bAcceleration(float.Parse(saveinfo[i + 6]));
                    player.set_maxSpeed(float.Parse(saveinfo[i + 7]));
                    player.setGold(int.Parse(saveinfo[i + 8]));
                    player.setCrew(int.Parse(saveinfo[i + 9]));
                }
            }
        }

        protected void newPlayerShip(PlayerShip player)
        {
            try
            {
                string[] saveinfo = File.ReadAllLines("save.txt");
                for (int i = 0; i < saveinfo.Length; i++)
                {
                    if (saveinfo[i] == getSavekey())
                    {
                        player.setMorale(50);
                        player.setAlignment(50);
                        player.setHealth(100);
                        player.setAttack(50);
                        player.setDefense(50);
                        player.set_bAcceleration(1);
                        player.set_maxSpeed(30);
                        player.setGold(100);
                        player.setCrew(20);
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Couldn't find save doc");
            }
        }
    }
}
