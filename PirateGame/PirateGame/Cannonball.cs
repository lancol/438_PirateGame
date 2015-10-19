namespace PirateGame
{
    class Cannonball : Entity
    {
        public float TTL { get; set; }
        private bool good;
        private float xSpeed;
        private float ySpeed;

        public Cannonball(bool Good, float FireDistance, float XSpeed, float YSpeed)
        {
            good = Good;
            TTL = FireDistance;
            xSpeed = XSpeed;
            ySpeed = YSpeed;
        }

        public void Update(float DT)
        {
            TTL--;
            setX(getX() + xSpeed*DT);
            setY(getY() + ySpeed*DT);
        }
    }
}