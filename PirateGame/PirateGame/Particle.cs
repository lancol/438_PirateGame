using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PirateGame
{
    public class Particle : Entity
    {
        //Source: http://rbwhitaker.wikidot.com/2d-particle-engine-2
        public Texture2D Texture { get; set; }        
        public Vector2 Position { get; set; }               
        public int TTL { get; set; }                //Time to Live
        public float xSpeed;
        public float ySpeed;
        public float xAccl;
        public float yAccl;
        public float rotateSpeed;
        public float angle;
        GameTime GT;

        public Particle(Texture2D texture, Vector2 position, int ttl, float XSpeed, float YSpeed, float X_Accl, float Y_Accl, float rotatespeed, GameTime gt)
        {
            Texture = texture;
            Position = position;
            TTL = ttl;
            xSpeed = XSpeed;
            ySpeed = YSpeed;
            xAccl = X_Accl;
            yAccl = Y_Accl;
            rotateSpeed = rotatespeed;
            angle = 0;
            GT = gt;
        }

        public void Update(float DT)
        {
            TTL--;
            
            if (xAccl != 0 || yAccl != 0)
            {
                xSpeed += xAccl * DT;
                ySpeed += yAccl * DT;
                angle = rotateSpeed * (TTL * DT) * DT;
                //Console.WriteLine(rotateSpeed +", " + t);
                Position = new Vector2(Position.X + xSpeed, Position.Y + ySpeed);
            }
            else
            {
                Position = new Vector2(Position.X + xSpeed, Position.Y + ySpeed);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            spriteBatch.Draw(Texture, Position, sourceRectangle, Color.White,
                angle, origin, 1, SpriteEffects.None, 0f);
        }
    }
}
