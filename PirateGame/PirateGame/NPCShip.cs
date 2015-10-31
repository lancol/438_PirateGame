using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace PirateGame
{
    public class NPCShip : Ship
    {
        string faction;
        string stance;
        float fireDistance = 200;
        int RC_StepSize = 2;
        List<Cannonball> cannonballs = new List<Cannonball>();
        static Texture2D cBall_image;

        public NPCShip(float x, float y, float rotate, string Faction)
        {
            //cBall_image = cBall_Image; //terrible code.
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

        public static void setCBallImage(Texture2D Image)
        {
            cBall_image = Image;
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

        public void runStandardBattleAI(PlayerShip player, float DT)
        {
            //Move forward
            bool raycastHitR = false; //boolean hit
            bool raycastHitL = false;
            float rayXr = getX(); //init start position
            float rayYr = getY();
            float rayXl = getX();
            float rayYl = getY();
            //Start
            Vector2[] cb = player.getCollisionbox(); //bounding box of player
            float distanceY = (float)Math.Sqrt(Math.Pow(cb[0].Y, 2) + Math.Pow(cb[3].Y, 2));
            float distanceX = (float)Math.Sqrt(Math.Pow(cb[1].Y, 2) + Math.Pow(cb[2].Y, 2));
            float midY = cb[3].Y - (distanceY / 2);
            float midX = cb[1].X + (distanceX / 2);
            //Raycasts
            #region Raycasts
            //k is the current step along the raycast
            for (int k = 0; k < (fireDistance / RC_StepSize); k++)
            {
                rayXr = getX() + ((k * RC_StepSize) * (float)Math.Cos(MathHelper.ToRadians(getRotate() + 90))); //right side
                rayYr = getY() + ((k * RC_StepSize) * (float)Math.Sin(MathHelper.ToRadians(getRotate() + 90))); //right side
                rayXl = getX() + ((k * RC_StepSize) * (float)Math.Cos(MathHelper.ToRadians(getRotate() - 90))); //left side
                rayYl = getY() + ((k * RC_StepSize) * (float)Math.Sin(MathHelper.ToRadians(getRotate() - 90))); //left side

                if (rayYr > cb[0].Y && rayYr < cb[3].Y && rayXr > cb[1].X && rayXr < cb[2].X) //if the point is 'below' the highest point and 'above' the lowest
                {
                    //Debug.WriteLine("RaycastHitR");
                    raycastHitR = true;
                    break;
                }
                else if (rayYl > cb[0].Y && rayYl < cb[3].Y && rayXl > cb[1].X && rayXl < cb[2].X) //if the point is 'below' the highest point and 'above' the lowest
                {
                    //Debug.WriteLine("RaycastHitL");
                    raycastHitL = true;
                    break;
                }
            }
            #endregion

            #region Fire if necessary
            //Raycast hit?
            if (raycastHitR == true) //if on right side
            {
                //Fire gun
                if (cannonballs.Count < 1)
                    fireCannon(0);
            }
            else if (raycastHitL == true)
            {
                if (cannonballs.Count < 1)
                    fireCannon(1);
            }
            #endregion

            //if close enough, rotate guns towards center of player
            rotateTowardsPlayer(player, DT);
            //else sail towards a good vantage point +/- k distance from top or bottom of player ship

            //Sail forward
            setX(getX() + (30 * DT) * (float)Math.Cos(MathHelper.ToRadians(getRotate()))); //update positions
            setY(getY() + (30 * DT) * (float)Math.Sin(MathHelper.ToRadians(getRotate())));
        } //needs lots of work

        private void fireCannon(int direction)
        {
            if (direction == 0) //0 == right
            {
                cannonballs.Add(new Cannonball((int)getX(), (int)getY(), false, fireDistance, 175*(float)Math.Cos(MathHelper.ToRadians(getRotate()+90)), 175 * (float)Math.Sin(MathHelper.ToRadians(getRotate()+90)))); //0 == good, 1 == bad; FireDistance, xSpeed, ySpeed
            }
            else //left
            {
                cannonballs.Add(new Cannonball((int)getX(), (int)getY(), false, fireDistance, 175 * (float)Math.Cos(MathHelper.ToRadians(getRotate() - 90)), 175 * (float)Math.Sin(MathHelper.ToRadians(getRotate() - 90)))); //0 == good, 1 == bad; FireDistance, xSpeed, ySpeed
                //cannonballs.Add(new Cannonball((int)getX(), (int)getY(), false, fireDistance, -30, 30)); //0 == good, 1 == bad; FireDistance, xSpeed, ySpeed
            }
        }

        public void rotateTowardsPlayer(PlayerShip player, float DT)
        {
            //check if above or below
            //if player above, rotate cwise
            //if player below, ccwise

            if ((player.getY() < getY() && player.getX() < getX()) || (player.getY() > getY() && player.getX() > getX())) //if player is above enemy and to the left OR below and to the right
            {
                setRotate(getRotate() + 30 * DT);
            }
            else if ((player.getY() > getY() && player.getX() > getX()) || (player.getY() < getY() && player.getX() > getX())) //if player is below enemy and to the left OR above and to the right
            {
                setRotate(getRotate() - 30 * DT);
            }
            
        } //garbage. Needs a lot of work.

        public void updateCannonBalls(float DT, PlayerShip player)
        {

            Vector2[] cb = player.getCollisionbox();

            for (int i = 0; i < cannonballs.Count; i++)
            {
                cannonballs[i].Update(DT);

                if ((cannonballs[i].getY() > cb[0].Y) && (cannonballs[i].getY() < cb[3].Y) && cannonballs[i].getX() > cb[1].X && cannonballs[i].getX() < cb[2].X)
                {
                    cannonballs[i].TTL = 0;
                    player.setHealth(player.getHealth() - 5);
                }

                if (cannonballs[i].TTL <= 0)
                {
                    cannonballs.RemoveAt(i);
                }
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
