
namespace PirateGame
{
    class PlayerShip : Ship
    {
        private float morale;
        private float align;
        private float b_acceleration;
        private float b_speed;
        private float max_speed;

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
                b_speed -= (float)(b_acceleration * DT) * b_speed; //subtract 50% (or current acc_rate) of ships speed per second (DT will make the effect happen more or less over the course of a second)
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

    }
}
