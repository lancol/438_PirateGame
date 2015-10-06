using System;
using Microsoft.Xna.Framework.Graphics;

namespace PirateGame
{
    class PlayerShip : Ship
    {
        private float morale;
        private float align;

        public PlayerShip(float X, float Y, float Rotate)
        {
            setX(X);
            setY(Y);
            setRotate(Rotate);
        }

        public float getMorale()
        {
            return morale;
        }

        public float getAlignment()
        {
            return align;
        }

        public void setMorale(float Morale)
        {
            morale = Morale;
        }

        public void setAlignment(float Align)
        {
            align = Align;
        }

        public void setPos(float X, float Y)
        {
            setX(X);
            setY(Y);
        }


    }
}
