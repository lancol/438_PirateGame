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
        #region System/Game Control
        GraphicsDeviceManager graphics;
        Camera camera;
        SpriteBatch spriteBatch;
        //Song backgroundSong;
        //ParticleEngine ow_ShipSprayEffect;
        float DT;
        int gameState;
        int screen_W;
        int screen_H;
        int world_W;
        int world_H;
        #endregion

        #region Environment
        Texture2D OceanTile;
        Texture2D OceanTile48;
        Texture2D OceanWeb;
        Texture2D SailSprayEffect;
        Texture2D[] island;
        int[] isl_x;
        int[] isl_y;
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
            world_H = 5000;
            world_W = 5000;
            screen_H = GraphicsDevice.Viewport.Height;
            screen_W = GraphicsDevice.Viewport.Width;
            Random rand = new Random();
            camera = new Camera(GraphicsDevice.Viewport);
            gameState = 2;
            #endregion

            #region Enviornment
            island = new Texture2D[3];
            isl_x = new int[3];
            isl_y = new int[3];

            for (int i = 0; i < 3; i++)
            {
                isl_x[i] = rand.Next(0, world_W);
                isl_y[i] = rand.Next(3000, world_H);
            }
            #endregion

            #region Player Related
            player = new PlayerShip(2500, 4500, 0);
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

            #region Other Ships
            OtherShip = new NPCShip[20];
            for (int i = 0; i < OtherShip.Length; i++)
            {
                OtherShip[i] = new NPCShip(cannonball, rand.Next(0, 5000), rand.Next(0, 5000), 0, "neutral");
            }
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
                OceanTile = Content.Load<Texture2D>("Ocean_Tile32");
                OceanTile48 = Content.Load<Texture2D>("Ocean_Tiles48");
                OceanWeb = Content.Load<Texture2D>("Ocean_web48_t127");
                SailSprayEffect = Content.Load<Texture2D>("WaterEffectSheet");
                whiteblock = Content.Load<Texture2D>("whiteblock");

                island[0] = Content.Load<Texture2D>("Island1");
                island[1] = Content.Load<Texture2D>("Island2");
                island[2] = Content.Load<Texture2D>("Island3");

                shipImg = new Texture2D[1];
                b_shipImg = new Texture2D[1];

                shipImg[0] = Content.Load<Texture2D>("Ship1v2");
                b_shipImg[0] = Content.Load<Texture2D>("Ship_TopDown136_68");
                cannonball = Content.Load<Texture2D>("Battle_Cannonball16");

                player.setImage(shipImg[0]);
                player.setBattleImage(b_shipImg[0]);

                for (int i = 0; i < OtherShip.Length; i++)
                {
                    OtherShip[i].setImage(shipImg[0]);
                    OtherShip[i].setBattleImage(b_shipImg[0]);
                }

                //backgroundSong = Content.Load<Song>("PirateSong");
            }
            catch
            {
                Debug.WriteLine("Failed to load an image");
            }
            overworld_init(); //depends on some player inits
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
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                spacedown = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.B)) //Temporary until overworld enemy ship collisions
            {
                gameState = 3;
                battle_init(); //temporary
            }

            #endregion

            switch (gameState)
            {
                case 0: //Main Menu
                    #region Main Menu

                    #endregion
                    break;
                case 1: //In town
                    #region In Town

                    #endregion
                    break;
                case 2: //overworld
                    #region Overworld
                    //MediaPlayer.Play(backgroundSong);
                    //MediaPlayer.IsRepeating = true;
                    //check for pause

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

                    //check for pause

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

            switch (gameState)
            {
                case 0: //Main menu
                    #region Main Menu
                        
                    #endregion
                    break;
                case 1: //In Town
                    #region In Town

                    #endregion
                    break;
                case 2: //overworld
                    #region Overworld
                    //draw ocean and ocean effects
                    //int h = ((int)camera.position.Y / OceanTile.Height) * OceanTile.Height;
                    //int w = ((int)camera.position.X / OceanTile.Width) * OceanTile.Width;

                    //for (int y = h; y < (h + screen_H + OceanTile.Height); y += OceanTile.Height)
                    //{
                    //    for (int x = w; x < (w + screen_W + OceanTile.Width); x += OceanTile.Width)
                    //    {
                    //        spriteBatch.Draw(OceanTile, new Vector2(x, y), Color.White);
                    //        spriteBatch.Draw(OceanWeb, new Vector2(x + (float)(stepRadius * Math.Sin(effectT)), y), Color.White);
                    //    }
                    //}
                    int h = ((int)camera.position.Y / OceanTile48.Height) * OceanTile48.Height;
                    int w = ((int)camera.position.X / OceanTile48.Width) * OceanTile48.Width;

                    for (int y = h; y < (h + screen_H + OceanTile48.Height); y += OceanTile48.Height)
                    {
                        for (int x = w; x < (w + screen_W + OceanTile48.Width); x += OceanTile48.Width)
                        {
                            spriteBatch.Draw(OceanTile48, new Vector2(x, y), Color.White);
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
                    spriteBatch.Draw(player.getImage(), new Vector2(player.getX(), player.getY()), null, Color.White,
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
                    #endregion
                    //Draw clouds/wind/weather/anything else
                    #endregion
                    break;
                case 3: //in battle
                    #region In Battle
                    
                    //draw Ocean
                    //int bh = ((int)camera.position.Y / OceanTile.Height) * OceanTile.Height;
                    //int bw = ((int)camera.position.X / OceanTile.Width) * OceanTile.Width;

                    //for (int y = bh; y < (bh + screen_H + OceanTile.Height); y += OceanTile.Height)
                    //{
                    //    for (int x = bw; x < (bw + screen_W + OceanTile.Width); x += OceanTile.Width)
                    //    {
                    //        spriteBatch.Draw(OceanTile, new Vector2(x, y), Color.White);
                    //        spriteBatch.Draw(OceanWeb, new Vector2(x + (float)(stepRadius * Math.Sin(effectT)), y), Color.White);
                    //    }
                    //}
                    int bh = ((int)camera.position.Y / OceanTile48.Height) * OceanTile48.Height;
                    int bw = ((int)camera.position.X / OceanTile48.Width) * OceanTile48.Width;

                    for (int y = bh; y < (bh + screen_H + OceanTile48.Height); y += OceanTile48.Height)
                    {
                        for (int x = bw; x < (bw + screen_W + OceanTile48.Width); x += OceanTile48.Width)
                        {
                            spriteBatch.Draw(OceanTile48, new Vector2(x, y), Color.White);
                            spriteBatch.Draw(OceanWeb, new Vector2(x + (float)(stepRadius * Math.Sin(effectT)), y), Color.White);
                        }
                    }

                    //draw Enemy Ship
                    spriteBatch.Draw(EnemyShip.getBattleImage(), new Vector2(EnemyShip.getX(), EnemyShip.getY()), null, Color.White,
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

                    #endregion
                    break;
                default:
                    Exit();
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void battle_init()
        {
            player.setRotate(0);
            camera.position = new Vector2(player.getX() - (screen_W / 2), player.getY() - (screen_H / 2));

            //EnemyShip = //whatever collided with
            EnemyShip = new NPCShip(cannonball, player.getX() + 200, player.getY() + 200, 270, "pirate");
            EnemyShip.setBattleImage(b_shipImg[0]);
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
            player.setRotate(0);
            camera.position = new Vector2(player.getX() - (screen_W / 2), player.getY() - (screen_H / 2));
            ow_sailSpray = new ParticleEngine(whiteblock, 20, -1, 0, 0, 0);

            player.setCBallImage(cannonball);

            ow_sailSpray.setX(player.getX());
            ow_sailSpray.setY(player.getY());
            ow_sailSpray.setActive(true);

            //b_SailStream.setActive(false);
            //b_SailStream2.setActive(false);

        }

    }
}
