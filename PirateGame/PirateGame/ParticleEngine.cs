﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PirateGame
{
    class ParticleEngine : Entity
    {
        bool active = true;
        public Vector2 EmitterLocation { get; set; }
        private List<Particle> particles;
        private Texture2D img;
        float timelimit;
        float xSpeed;
        float ySpeed;
        float xAccl;
        float yAccl;
        float rotateSpeed;
        public int Count;
        public static GameTime GT;

        public ParticleEngine(Texture2D Image, float Timelimit, float XSpeed, float YSpeed, float X_Accl, float Y_Accl)
        {
            active = true;
            img = Image;
            timelimit = Timelimit;
            xSpeed = XSpeed;
            ySpeed = YSpeed;
            xAccl = X_Accl;
            yAccl = Y_Accl;
            rotateSpeed = 0;
            Count = 0;
            GT = new GameTime();
            particles = new List<Particle>();
        }

        public void Update(float DT)
        {
            if (active)
            {
                //add particles
                try {
                    if ((img.Name == "poof1" || img.Name == "poof2") && Count < 1)
                    {
                        particles.Add(new Particle(img, new Vector2(getX(), getY()), (int)timelimit, xSpeed, ySpeed, xAccl, yAccl, rotateSpeed, GT));
                        Count++;
                    }
                    else
                    {
                        if (img.Name != "poof1" && img.Name != "poof2")
                        {
                            particles.Add(new Particle(img, new Vector2(getX(), getY()), (int)timelimit, xSpeed, ySpeed, xAccl, yAccl, rotateSpeed, GT));
                            Count++;
                        }
                    }
                }
                catch
                {

                }
                for (int p = 0; p < particles.Count; p++)
                {
                    particles[p].Update(DT);
                    if (particles[p].TTL <= 0)
                    {
                        particles.RemoveAt(p);
                        p--; //why and how was this here.
                        Count--;
                    }
                }
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (active)
            {
                //draw particles
                for (int i = 0; i < particles.Count; i++)
                {
                    particles[i].Draw(spriteBatch);
                }
            }
        }

        public void removeAll()
        {
            for (int p = 0; p < particles.Count; p++)
            {
                particles.RemoveAt(p);
                p--;
                Count--;
            }
        }

        public void setActive(bool Active)
        {
            active = Active;
        }

        public void setXSpeed(float XSpeed)
        {
            xSpeed = XSpeed;
        }

        public void setYSpeed(float YSpeed)
        {
            ySpeed = YSpeed;
        }

        public void setTimeLimit(float Time)
        {
            timelimit = Time;
        }

        public void setXAccl(float x_accl)
        {
            xAccl = x_accl;
        }

        public void setYAccl(float y_accl)
        {
            yAccl = y_accl;
        }

        public void setRotateSpeed(float rotatespeed)
        {
            rotateSpeed = rotatespeed;
        }

        public bool getActive()
        {
            return active;
        }

        public float getXSpeed()
        {
            return xSpeed;
        }

        public float getYSpeed()
        {
            return ySpeed;
        }

        public float getTimeLimit()
        {
            return timelimit;
        }

        public float getXAccl()
        {
            return xAccl;
        }

        public float getYAccl()
        {
            return yAccl;
        }

    }
}
