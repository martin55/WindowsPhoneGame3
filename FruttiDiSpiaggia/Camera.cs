namespace FruttiDiSpiaggia
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a camera object.
    /// </summary>
    public class Camera
    {
        /* Fields */

        /// <summary>
        /// The camera's position.
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// The camera's speed.
        /// </summary>
        private float speed;

        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera" /> class.
        /// </summary>
        public Camera()
        {
            this.position = new Vector2();
            this.speed = 4.0f;
        }

        /* Accessors */

        /// <summary>
        /// Gets or sets the camera's speed.
        /// </summary>
        public float Speed
        {
            get
            {
                return this.speed;
            }

            set
            {
                this.speed = MathHelper.Clamp(value, 0.5f, 50f);
            }
        }

        /// <summary>
        /// Gets or sets the camera's position.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return this.position;
            }

            set
            {
                this.position.X = MathHelper.Clamp(
                    value.X,
                    0,
                    GameCore.MapWidthInPixels - GameCore.ScreenWidth);
                this.position.Y = MathHelper.Clamp(
                    value.Y,
                    0,
                    GameCore.MapHeightInPixels - GameCore.ScreenHeight);
            }
        }
    }
}
