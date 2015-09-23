using System;

namespace PirateGame
{
    class PlayerShip
    {
        private String Ship_img;

        private float x;
        private float y;
        private float x_off; //not used yet
        private float y_off; //same

        private float rot; //rotate
        private float rot_speed;

        private float range;
        private float def;
        private float speed;
        private float maxSpeed;
        private float morale;
        private float align;
        private float atk_speed;
        int crew;

        public PlayerShip(float X, float Y, float Rotate) //Basic constructor
        {
            x = X;
            y = Y;
            rot = Rotate;

            //Set some defaults (usually 10... arbitrarily)
            rot_speed = 5;

            x_off = 0;
            y_off = 0;

            range = 10;
            def = 10;

            speed = 0;
            maxSpeed = 30;

            morale = 5;
            align = 5;

            atk_speed = 10;
            crew = 5;

            //if ship1 1, x_off,y_off = something
            //if ship 2, x_off, y_off = something else, maybe
        }

        public void setPos(float X, float Y)
        {
            x = X;
            y = Y;
        }

        public void setRotateSpeed(float Speed)
        {
            rot_speed = Speed;
        }

        public void setRange(float rng)
        {
            range = rng;
        }

        public void setDef(float deff)
        {
            def = deff;
        }

        public void setSpeed(float Speed)
        {
            speed = Speed;
        }

        public void setMaxSpeed(float maxS)
        {
            maxSpeed = maxS;
        }

        public void setMorale(float mor)
        {
            morale = mor;
        }

        public void setAlign(float ali)
        {
            align = ali;
        }

        public void setAtkSpeed(float atk)
        {
            atk_speed = atk;
        }

        public void setCrew(int people)
        {
            crew = people;
        }

        public void setImage()
        {
            Ship_img = "idk";
        }

        public float getX()
        {
            return x;
        }

        public float getY()
        {
            return y;
        }

        public float getRotateSpeed()
        {
            return rot_speed;
        }

        public float getRange()
        {
            return range;
        }

        public float getDef()
        {
            return def;
        }

        public float getSpeed()
        {
            return speed;
        }

        public float getMaxSpeed()
        {
            return maxSpeed;
        }

        public float getMorale()
        {
            return morale;
        }

        public float getAlign()
        {
            return align;
        }

        public float getAtkSpeed()
        {
            return atk_speed;
        }

        public int getCrew()
        {
            return crew;
        }
    }
}
