using Microsoft.Xna.Framework;

namespace PirateGame
{
    class Cannonball : Entity
    {
        public float TTL { get; set; }
        private bool good;
        private float xSpeed;
        private float ySpeed;

        public Cannonball(int X, int Y, bool Good, float FireDistance, float XSpeed, float YSpeed)
        {
            setX(X);
            setY(Y);
            good = Good;
            TTL = FireDistance;
            xSpeed = XSpeed;
            ySpeed = YSpeed;
        }

        public void Update(float DT)
        {
            TTL--;

            float nextStepX = getX() + xSpeed * DT;
            float nextStepY = getY() + ySpeed * DT;

            setX(nextStepX);
            setY(nextStepY);
        }
    }
}