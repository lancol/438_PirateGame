using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace PirateGame
{
    class NPCShip : Ship
    {
        string faction;
        string stance;
        float fireDistance = 200;
        int RC_StepSize = 2;
        List<Cannonball> cannonballs = new List<Cannonball>();
        Texture2D cBall_image;


        public NPCShip(Texture2D cBall_Image, float x, float y, float rotate, string Faction)
        {
            cBall_image = cBall_Image; //terrible code.
            setX(x);
            setY(y);
            setRotate(rotate);
            //defaults
            setAttack(1);
            setDefense(1);
            setHealth(10);
            setGold(100);
            faction = Faction;
            stance = "passive";
        }

        public void setFaction(string Faction)
        {
            faction = Faction;
        }

        public void setStance(string Stance)
        {
            stance = Stance;
        }

        public string getFaction()
        {
            return faction;
        }

        public string getStance()
        {
            return stance;
        }

        public void runStandardBattleAI(PlayerShip player)
        {
            bool raycastHitR = false;
            bool raycastHitL = false;
            float rayXr = getX();
            float rayYr = getY();
            float rayXl = getX();
            float rayYl = getY();
            //Start

            //Raycasts
            #region Raycasts
            //k is the current step along the raycast
            for (int k = 0; k < (fireDistance/RC_StepSize); k++)
            {
                rayXr = (getX() + k * RC_StepSize) * (float)Math.Cos(MathHelper.ToRadians(getRotate() - 90)); //right side
                rayYr = (getY() + k * RC_StepSize) * (float)Math.Sin(MathHelper.ToRadians(getRotate() - 90)); //right side
                //if raycast right point is within collisionbox, raycastHit = true, break;
                rayXl = (getX() + k * RC_StepSize) * (float)Math.Cos(MathHelper.ToRadians(getRotate() + 90)); //left side
                rayYl = (getY() + k * RC_StepSize) * (float)Math.Sin(MathHelper.ToRadians(getRotate() + 90)); //left side
                //if raycast right point is within collisionbox, raycastHit = true, break;

                Vector2[] cb = player.getCollisionbox();

                //the following part doesn't ring true, when it should.
                //begin printing out raycast positions
                //print out cb positions. Run manual cases
                //Debug.WriteLine("player: " + cb[0] + ", " + cb[1] + ", " + cb[2] + ", " + cb[3]);
                //Debug.WriteLine("RaycastL: " + rayXl + ", " + rayYl);

                
                /*
                Raycast Y is being made really small.

                player: {X:2521.664 Y:4680.367}, {X:2520.698 Y:4800.714}, {X:2590.631 Y:4681.27}, {X:2588.447 Y:4801.601}
                RaycastL: 2704, 0.0008224735
                player: {X:2521.664 Y:4680.367}, {X:2520.698 Y:4800.714}, {X:2590.631 Y:4681.27}, {X:2588.447 Y:4801.601}
                RaycastL: 2706, 0.0008228231
                player: {X:2521.664 Y:4680.367}, {X:2520.698 Y:4800.714}, {X:2590.631 Y:4681.27}, {X:2588.447 Y:4801.601}
                RaycastL: 2708, 0.0008231729
                player: {X:2521.664 Y:4680.367}, {X:2520.698 Y:4800.714}, {X:2590.631 Y:4681.27}, {X:2588.447 Y:4801.601}
                RaycastL: 2710, 0.0008235226
                player: {X:2521.664 Y:4680.367}, {X:2520.698 Y:4800.714}, {X:2590.631 Y:4681.27}, {X:2588.447 Y:4801.601}
                RaycastL: 2712, 0.0008238722
                player: {X:2521.664 Y:4680.367}, {X:2520.698 Y:4800.714}, {X:2590.631 Y:4681.27}, {X:2588.447 Y:4801.601}
                RaycastL: 2714, 0.0008242219
                */


                if (rayYr > cb[0].Y && rayYr < cb[3].Y) //if the point is 'below' the highest point and 'above' the lowest
                {
                    if (rayXr > cb[0].X && rayXr < cb[2].X)
                    {
                        Debug.WriteLine("RaycastHitR");
                        raycastHitR = true;
                        break;
                    }
                }
                else if (rayYl > cb[0].Y && rayYl < cb[3].Y) //if the point is 'below' the highest point and 'above' the lowest
                {
                    if (rayXl > cb[0].X && rayXl < cb[2].X)
                    {
                        Debug.WriteLine("RaycastHitL");
                        raycastHitL = true;
                        break;
                    }
                }
            }
            #endregion

            //Raycast hit?
            if (raycastHitR == true) //if on right side
            {
                //Fire gun
                fireCannon(0);
            }
            else if (raycastHitL == true)
            {
                fireCannon(1);
            }
            else
            {
                //Rotate step towards perpendicular to player
            }
            //Sail forward
        }

        private void fireCannon(int direction)
        {
            Debug.WriteLine("Fire!");
            Debug.WriteLine(cBall_image.Name);
            if (direction == 0) //0 == right
            {
                cannonballs.Add(new Cannonball(false, fireDistance, 5, 5)); //0 == good, 1 == bad; FireDistance, xSpeed, ySpeed
            }
            else //left
            {
                cannonballs.Add(new Cannonball(false, fireDistance, 5, 5)); //0 == good, 1 == bad; FireDistance, xSpeed, ySpeed
            }
        }

        public void updateCannonBalls(float DT)
        {
            for (int i = 0; i < cannonballs.Count; i++)
            {
                if (cannonballs[i].TTL <= 0)
                {
                    cannonballs.RemoveAt(i);
                }
                cannonballs[i].Update(DT);
            }
        }

        public void drawCannonBalls(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < cannonballs.Count; i++)
            {
                spriteBatch.Draw(cBall_image, new Vector2(cannonballs[i].getX(), cannonballs[i].getY()), Color.White);
            }
        }
    }
}
