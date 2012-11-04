namespace SharkGame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a camera object for a two-dimensional game. Supports movement
    /// while staying at a fixed height over the game map.
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
            this.position = Vector2.Zero;
            this.speed = Constants.Speeds.CameraSpeed;
        }

        /* Properties */

        /// <summary>
        /// Gets or sets the camera's speed.
        /// </summary>
        /// <value>
        /// Values are restricted to [0.5f, 50f] range.
        /// </value>
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
        /// <value>
        /// Values are restricted to [0, map width - screen width] range.
        /// </value>
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
                    Constants.Maps.MapWidth - GameCore.ScreenWidth);
                this.position.Y = MathHelper.Clamp(
                    value.Y,
                    0,
                    Constants.Maps.MapHeight - GameCore.ScreenHeight);
            }
        }
    }
}
