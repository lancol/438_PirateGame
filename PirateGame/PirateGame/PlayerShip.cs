using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

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
        Texture2D cBall_image;
        ParticleEngine PE;
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
