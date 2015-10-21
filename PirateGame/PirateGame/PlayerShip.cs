using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace PirateGame
{
    class PlayerShip : Ship
    {
        private float morale;
        private float align;
        private float b_acceleration;
        private float b_speed;
        private float max_speed;
        private float fireDistance;
        Texture2D cBall_image;
        List<Cannonball> cannonballs = new List<Cannonball>();

        public PlayerShip(float X, float Y, float Rotate)
        {
            setX(X);
            setY(Y);
            setRotate(Rotate);
            fireDistance = 300;

            setAttack(1);
            setDefense(1);
            setHealth(10);
            setGold(100);
        }

        public float getMorale()
        {
            return morale;
        }

        public float getAlignment()
        {
            return align;
        }

        public float get_bAcceleration()
        {
            return b_acceleration;
        }

        public float get_bSpeed()
        {
            return b_speed;
        }

        public float get_maxSpeed()
        {
            return max_speed;
        }

        public void setMorale(float Morale)
        {
            morale = Morale;
        }

        public void setCBallImage(Texture2D CballImage)
        {
            cBall_image = CballImage;
        }

        public void setAlignment(float Align)
        {
            align = Align;
        }

        public void set_bAcceleration(float battleAcceleration)
        {
            b_acceleration = battleAcceleration;
        }

        public void set_bSpeed(float battle_Speed)
        {
            b_speed = battle_Speed;
        }

        public void set_maxSpeed(float max_Speed)
        {
            max_speed = max_Speed;
        }

        public void setPos(float X, float Y)
        {
            setX(X);
            setY(Y);
        }

        public void raise_Sails(float DT)
        {
            if (b_speed + (b_speed * (b_acceleration * DT)) < max_speed) //if speed is not going over the max speed.
            {
                if (b_speed == 0.0) //if not moving (unlikely)
                {
                    b_speed = 3f; //Starting speed of 3, which is quite slow
                }
                else
                {
                    b_speed += (float)(b_acceleration * DT) * b_speed; //accelerates at 50% (or current acc_rate)
                }
            }
            else //if moving faster than max_speed, reset down to max_speed.
            {
                b_speed = max_speed;
            }
        } //Battle Mode, speed up

        public void lower_Sails(float DT) //slow ship down
        {
            if (b_speed - (b_speed * (b_acceleration * DT)) > 0) //if slowing it down doesn't put it below the speed of 0, so it doesn't go into reverse at any point
            {
                b_speed -= (b_acceleration * DT) * b_speed; //subtract 50% (or current acc_rate) of ships speed per second (DT will make the effect happen more or less over the course of a second)
            }
            else
            {
                b_speed = 0;
            }
        }

        public void rotate_Cwise(float DT)
        {
            setRotate(getRotate() + (15 * DT));
        }

        public void rotate_CCwise(float DT)
        {
            setRotate(getRotate() - (15 * DT));
        }

        public Vector2[] getCollisionbox() //returns an array of vectors that have been sorted top to bottom.
        {                                   
            /*
                I don't expect anyone to follow my code in this method. But basically what happens is that
                it calculates the 4 edge points of the rectangle at whatever rotation it's currently at.
                It then sorts them by top to bottom, so when it returns you can do some collision logic
                knowing about where things are in relation to eachother.
            */

            float x1, y1, x2, y2, x3, y3, x4, y4;
            Vector2[] p = new Vector2[4];           //points
            Vector2[] s_p = new Vector2[4];         //sorted points

            x1 = getX() + 65.5f * (float)Math.Cos(MathHelper.ToRadians(getRotate() + 148.23f)); //opposite p2
            y1 = getY()-1 + 65.5f * (float)Math.Sin(MathHelper.ToRadians(getRotate() + 148.23f));

            x2 = getX() + 73 * (float)Math.Cos(MathHelper.ToRadians(getRotate() - 27.65f)); //opposite p1
            y2 = getY()-1 + 73 * (float)Math.Sin(MathHelper.ToRadians(getRotate() - 27.65f));

            x3 = getX() + 65.5f * (float)Math.Cos(MathHelper.ToRadians(getRotate() - 148.23f)); //opposite p4
            y3 = getY()-1 + 65.5f * (float)Math.Sin(MathHelper.ToRadians(getRotate() - 148.23f));

            x4 = getX() + 73 * (float)Math.Cos(MathHelper.ToRadians(getRotate() + 27.65f)); //opposite p3
            y4 = getY()-1 + 73 * (float)Math.Sin(MathHelper.ToRadians(getRotate() + 27.65f));

            p[0] = new Vector2(x1, y1);
            p[1] = new Vector2(x2, y2);
            p[2] = new Vector2(x3, y3);
            p[3] = new Vector2(x4, y4);

            s_p[0] = new Vector2(x1, y1);
            s_p[1] = new Vector2(x2, y2);
            s_p[2] = new Vector2(x3, y3);
            s_p[3] = new Vector2(x4, y4);

            //sort by Highest
            int highest = 0;
            for(int i = 0; i < 4; i++)
            {
                if (p[i].Y < p[highest].Y) //Highest is lowest... on y axis... on computers at least.
                {
                    highest = i;
                }
            }

            int second_highest = -1;
            for(int i = 0; i < 4; i++)
            {
                if (p[i].Y > p[highest].Y) //defaulting to 0
                {
                    if (second_highest == -1)
                        second_highest = i;
                    
                    if (p[i].Y <= p[second_highest].Y)
                    {
                        second_highest = i;
                    }
                }
            }

            s_p[0] = p[highest];
            s_p[1] = p[second_highest];

            switch (highest)
            {
                case 0:
                    s_p[3] = p[1];
                    break;
                case 1:
                    s_p[3] = p[0];
                    break;
                case 2:
                    s_p[3] = p[3];
                    break;
                case 3:
                    s_p[3] = p[2];
                    break;
            }
            switch (second_highest)
            {
                case 0:
                    s_p[2] = p[1];
                    break;
                case 1:
                    s_p[2] = p[0];
                    break;
                case 2:
                    s_p[2] = p[3];
                    break;
                case 3:
                    s_p[2] = p[2];
                    break;
            }

            if (s_p[1].X > s_p[2].X) //make the smaller x go first (farthest left)
            {
                Vector2 temp1;
                Vector2 temp2;
                temp1 = new Vector2(s_p[1].X, s_p[1].Y);
                temp2 = new Vector2(s_p[2].X, s_p[2].Y);

                s_p[1] = temp2;
                s_p[2] = temp1;
            }

            return s_p;
        }

        public void fireCannon(NPCShip enemyShip, float DT)
        {
            if (getX() < enemyShip.getX()) //This is temporary
            {
                //float xSpeed = ((b_speed + 50) * (float)Math.Cos(MathHelper.ToRadians(getRotate())))*DT;
                //float ySpeed = ((b_speed + 50) * (float)Math.Sin(MathHelper.ToRadians(getRotate())))*DT;
                if (cannonballs.Count < 3)
                {
                    cannonballs.Add(new Cannonball((int)getX(), (int)getY(), true, fireDistance, 175 * (float)Math.Cos(MathHelper.ToRadians(getRotate() + 90)), 175 * (float)Math.Sin(MathHelper.ToRadians(getRotate() + 90)))); //0 == good, 1 == bad; FireDistance, xSpeed, ySpeed
                    cannonballs.Add(new Cannonball((int)getX() + 10, (int)getY() + 10, true, fireDistance, 175 * (float)Math.Cos(MathHelper.ToRadians(getRotate() + 90)), 175 * (float)Math.Sin(MathHelper.ToRadians(getRotate() + 90))));
                    cannonballs.Add(new Cannonball((int)getX() - 10, (int)getY() - 10, true, fireDistance, 175 * (float)Math.Cos(MathHelper.ToRadians(getRotate() + 90)), 175 * (float)Math.Sin(MathHelper.ToRadians(getRotate() + 90))));

                }
            }
            else //left
            {
                //float xSpeed = ((b_speed + 50) * (float)Math.Cos(MathHelper.ToRadians(getRotate())))*DT;
                //float ySpeed = ((b_speed + 50) * (float)Math.Sin(MathHelper.ToRadians(getRotate())))*DT;
                if (cannonballs.Count < 3)
                {
                    cannonballs.Add(new Cannonball((int)getX(), (int)getY(), true, fireDistance, 175 * (float)Math.Cos(MathHelper.ToRadians(getRotate() - 90)), 175 * (float)Math.Sin(MathHelper.ToRadians(getRotate() - 90)))); //0 == good, 1 == bad; FireDistance, xSpeed, ySpeed
                    cannonballs.Add(new Cannonball((int)getX() + 10, (int)getY() + 10, true, fireDistance, 175 * (float)Math.Cos(MathHelper.ToRadians(getRotate() - 90)), 175 * (float)Math.Sin(MathHelper.ToRadians(getRotate() - 90))));
                    cannonballs.Add(new Cannonball((int)getX() - 10, (int)getY() - 10, true, fireDistance, 175 * (float)Math.Cos(MathHelper.ToRadians(getRotate() - 90)), 175 * (float)Math.Sin(MathHelper.ToRadians(getRotate() - 90))));
                }
            }
        }

        public void updateCannonBalls(float DT, NPCShip enemy)
        {
            for (int i = 0; i < cannonballs.Count; i++)
            {
                cannonballs[i].Update(DT);
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
