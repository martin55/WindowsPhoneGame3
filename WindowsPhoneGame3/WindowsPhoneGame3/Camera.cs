namespace FishGame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// 
    /// </summary>
    class Camera
    {
        /// <summary>
        /// 
        /// </summary>
        Vector2 position;

        /// <summary>
        /// 
        /// </summary>
        float speed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera" /> class.
        /// </summary>
        public Camera()
        {
            this.position = new Vector2();
            this.speed = 4.0f;
        }

        /// <summary>
        /// Gets or sets the speed of the camera.
        /// </summary>
        public float Speed
        {
            get { return speed; }
            set
            {
                speed = MathHelper.Clamp(value, 0.5f, 50f);
            }
        }

        /// <summary>
        /// Gets or sets the position of the camera.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }

            set
            {
                position.X = MathHelper.Clamp(value.X,
                        0,
                        Game1.MapWidthInPixels - Game1.ScreenWidth);
                position.Y = MathHelper.Clamp(value.Y,
                        0,
                        Game1.MapHeightInPixels - Game1.ScreenHeight);
            }
        }




    }
}
