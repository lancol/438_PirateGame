using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace PirateGame
{
    public class PlayerShip : Ship
    {
        private float morale;
        private float align;
        private float b_acceleration;
        private float b_speed;
        private float max_speed;
        private float fireDistance;
        public bool canFire;
        public float lastFire;

        Texture2D cBall_image;
        //ParticleEngine PE;
        List<Cannonball> cannonballs = new List<Cannonball>();

        Random rand;

        public PlayerShip(float X, float Y, float Rotate)
        {
            setX(X);
            setY(Y);
            setRotate(Rotate);
            fireDistance = 300;

            fireDistance = 150;

            setAttack(1);
            setDefense(1);
            setHealth(10);
            setGold(100);

            lastFire = 0;
            rand = new Random();

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

        public void raise_Sails(float DT)
        {
            if (b_speed + (b_speed * (b_acceleration * DT)) < max_speed) //if speed is not going over the max speed.
            {
                if (b_speed < 3) //if not moving (unlikely)
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

        public bool fireCannon(NPCShip enemyShip, float DT)
        {
            canFire = false;
            SoundEffectInstance sound;
            sound = cannonFire.CreateInstance();
            float rayXr = getX(); //init start position
            float rayYr = getY();
            float rayXl = getX();
            float rayYl = getY();

            float minL = 1000;
            float minR = 1000;
            float lDist = 1000;
            float rDist = 1000;

            for (int k = 0; k < 10; k++)
            {
                rayXr = getX() + ((k * 10) * (float)Math.Cos(MathHelper.ToRadians(getRotate() + 90))); //right side
                rayYr = getY() + ((k * 10) * (float)Math.Sin(MathHelper.ToRadians(getRotate() + 90))); //right side
                rayXl = getX() + ((k * 10) * (float)Math.Cos(MathHelper.ToRadians(getRotate() - 90))); //left side
                rayYl = getY() + ((k * 10) * (float)Math.Sin(MathHelper.ToRadians(getRotate() - 90))); //left side

                lDist = Vector2.Distance(new Vector2(rayXl, rayYl), enemyShip.getPos());
                rDist = Vector2.Distance(new Vector2(rayXr, rayYr), enemyShip.getPos());

                if (rDist < minR)
                {
                    minR = rDist;
                }
                if (lDist < minL)
                {
                    minL = lDist;
                }
            }

            if (rDist < lDist) //This is temporary
            {
                if (cannonballs.Count < 3)
                {
                    sound.Pitch = rand.Next(-1, 1);
                    sound.Play();
                    cannonballs.Add(new Cannonball((int)getX(), (int)getY(), true, fireDistance, 175 * (float)Math.Cos(MathHelper.ToRadians(getRotate() + 90)), 175 * (float)Math.Sin(MathHelper.ToRadians(getRotate() + 90)))); //0 == good, 1 == bad; FireDistance, xSpeed, ySpeed
                    cannonballs.Add(new Cannonball((int)getX() + 10, (int)getY() + 10, true, fireDistance, 175 * (float)Math.Cos(MathHelper.ToRadians(getRotate() + 90)), 175 * (float)Math.Sin(MathHelper.ToRadians(getRotate() + 90))));
                    cannonballs.Add(new Cannonball((int)getX() - 10, (int)getY() - 10, true, fireDistance, 175 * (float)Math.Cos(MathHelper.ToRadians(getRotate() + 90)), 175 * (float)Math.Sin(MathHelper.ToRadians(getRotate() + 90))));
                }
                return true;
            }
            else //left
            {
                if (cannonballs.Count < 3)
                {
                    sound.Pitch = rand.Next(-1,1);
                    sound.Play();
                    cannonballs.Add(new Cannonball((int)getX(), (int)getY(), true, fireDistance, 175 * (float)Math.Cos(MathHelper.ToRadians(getRotate() - 90)), 175 * (float)Math.Sin(MathHelper.ToRadians(getRotate() - 90)))); //0 == good, 1 == bad; FireDistance, xSpeed, ySpeed
                    cannonballs.Add(new Cannonball((int)getX() + 10, (int)getY() + 10, true, fireDistance, 175 * (float)Math.Cos(MathHelper.ToRadians(getRotate() - 90)), 175 * (float)Math.Sin(MathHelper.ToRadians(getRotate() - 90))));
                    cannonballs.Add(new Cannonball((int)getX() - 10, (int)getY() - 10, true, fireDistance, 175 * (float)Math.Cos(MathHelper.ToRadians(getRotate() - 90)), 175 * (float)Math.Sin(MathHelper.ToRadians(getRotate() - 90))));
                }
                return false;
            }
        }

        public void updateCannonBalls(float DT, NPCShip enemy)
        {
            Vector2[] cb = enemy.getCollisionbox();

            for (int i = 0; i < cannonballs.Count; i++)
            {
                cannonballs[i].Update(DT);

                if ((cannonballs[i].getY() > cb[0].Y) && (cannonballs[i].getY() < cb[3].Y) && cannonballs[i].getX() > cb[1].X && cannonballs[i].getX() < cb[2].X)
                {
                    cannonballs[i].TTL = 0;
                    enemy.setHealth(enemy.getHealth() - 5);
                }

                if (cannonballs[i].TTL <= 0)
                {
                    cannonballs.RemoveAt(i);
                    if (cannonballs.Count < 1) //this needs to change to the timer
                    {
                        canFire = true;
                    }
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
