using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PirateGame
{
    public class Ship : Entity
    {
        private Texture2D image;
        private Texture2D battleImage;
        private int gold;
        private int crew;
        private float speed;
        private float maxSpeed;
        private float defense;
        private float health;
        private float attack;
        private float PowerLevel;
        
        #region Getters
        public Texture2D getImage()
        {
            return image;
        }
        
        public Texture2D getBattleImage()
        {
            return battleImage;
        }

        public Vector2 getPos()
        {
            return new Vector2(getX(), getY());
        }

        public float getPowerlvl()
        {
            PowerLevel = (0.6f * health) + (0.5f)*attack + (0.5f)*defense + (crew / 3) + (speed / 5); //a bit arbitrary...
            return PowerLevel;
        }

        public int getGold()
        {
            return gold;
        }

        public float getCrew()
        {
            return crew;
        }

        public float getSpeed()
        {
            return speed;
        }

        public float getMaxSpeed()
        {
            return maxSpeed;
        }

        public float getDefense()
        {
            return defense;
        }

        public float getAttack()
        {
            return attack;
        }

        public float getHealth()
        {
            return health;
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
            y1 = getY() - 1 + 65.5f * (float)Math.Sin(MathHelper.ToRadians(getRotate() + 148.23f));

            x2 = getX() + 73 * (float)Math.Cos(MathHelper.ToRadians(getRotate() - 27.65f)); //opposite p1
            y2 = getY() - 1 + 73 * (float)Math.Sin(MathHelper.ToRadians(getRotate() - 27.65f));

            x3 = getX() + 65.5f * (float)Math.Cos(MathHelper.ToRadians(getRotate() - 148.23f)); //opposite p4
            y3 = getY() - 1 + 65.5f * (float)Math.Sin(MathHelper.ToRadians(getRotate() - 148.23f));

            x4 = getX() + 73 * (float)Math.Cos(MathHelper.ToRadians(getRotate() + 27.65f)); //opposite p3
            y4 = getY() - 1 + 73 * (float)Math.Sin(MathHelper.ToRadians(getRotate() + 27.65f));

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
            for (int i = 0; i < 4; i++)
            {
                if (p[i].Y < p[highest].Y) //Highest is lowest... on y axis... on computers at least.
                {
                    highest = i;
                }
            }

            int second_highest = -1;
            for (int i = 0; i < 4; i++)
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

        #endregion

        #region Setters

        public void setPos(float X, float Y)
        {
            setX(X);
            setY(Y);
        }

        public void setImage(Texture2D Image)
        {
            image = Image;
        }

        public void setBattleImage(Texture2D Image)
        {
            battleImage = Image;
        }

        public void setGold(int Gold)
        {
            gold = Gold;
        }

        public void setCrew(int Crew)
        {
            crew = Crew;
        }

        public void setSpeed(float Speed)
        {
            speed = Speed;
        }

        public void setMaxSpeed(float MaxSpeed)
        {
            maxSpeed = MaxSpeed;
        }

        public void setDefense(float Defense)
        {
            defense = Defense;
        }

        public void setHealth(float Health)
        {
            health = Health;
        }

        public void setAttack(float Attack)
        {
            attack = Attack;
        }
        #endregion
    }
}
