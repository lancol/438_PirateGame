using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PirateGame
{
    class Ship : Entity
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

        #region Getters
        public Texture2D getImage()
        {
            return image;
        }
        
        public Texture2D getBattleImage()
        {
            return battleImage;
        }

        public float getGold()
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
        #endregion

        #region Setters
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
