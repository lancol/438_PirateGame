using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateGame
{
    class Entity
    {
        float x, y;
        float rot; //rotate

        public Entity(float X, float Y)
        {
            x = X;
            y = Y;
            rot = 0;
        }
        public Entity(float X, float Y, float Rotate)
        {
            x = X;
            y = Y;
            rot = Rotate;
        }

    }
}
