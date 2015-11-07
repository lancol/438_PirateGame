using System;
namespace PirateGame
{
    public class Entity
    {
        private float x;
        private float y;
        private float rotate;

        public float getX()
        {
            return x;
        }

        public float getY()
        {
            return y;
        }

        public float getRotate()
        {
            return rotate;
        }

        public void setX(float X)
        {
            x = X;
        }

        public void setY(float Y)
        {
            y = Y;
        }

        public void setRotate(float Rotate)
        {
            rotate = Rotate;

            if (rotate > 360)
            {
                rotate %= 360;
            }
            else if (rotate < 0)
            {
                rotate %= 360;
                rotate = 360+(rotate % 360);
            }
        }

    }
}