using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PirateGame
{
    public class Camera
    {
        private readonly Viewport viewport;
        public Vector2 origin { get; set; }
        public Vector2 position { get; set; }

        public Camera(Viewport view_port)
        {
            viewport = view_port;

            origin = new Vector2(viewport.Width / 2f, viewport.Height / 2);
            position = Vector2.Zero;
        }

        public Matrix GetViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-position, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
                Matrix.CreateRotationZ(0) *
                Matrix.CreateScale(1, 1, 1) *
                Matrix.CreateTranslation(new Vector3(origin, 0.0f));
        }
    }
}