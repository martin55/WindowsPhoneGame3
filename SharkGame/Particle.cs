namespace SharkGame
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Represents a single in-game particle.
    /// </summary>
    public class Particle
    {
        /* Fields */

        /// <summary>
        /// Particle position.
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// Particle velocity.
        /// </summary>
        private Vector2 velocity;

        /// <summary>
        /// Particle angle.
        /// </summary>
        private float angle;

        /// <summary>
        /// Particle angular velocity.
        /// </summary>
        private float angularVelocity;

        /// <summary>
        /// Particle size.
        /// </summary>
        private float size;

        /// <summary>
        /// Particle gradient.
        /// </summary>
        private Color gradient;

        /// <summary>
        /// Particle sprite.
        /// </summary>
        private Texture2D sprite;

        /// <summary>
        /// Particle lifespan.
        /// </summary>
        private int lifespan;

        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="Particle" /> class.
        /// </summary>
        /// <param name="position">Position of the particle.</param>
        /// <param name="velocity">Velocity of the particle.</param>
        /// <param name="angle">Angle of the particle.</param>
        /// <param name="angularVelocity">Angular velocity of the particle.</param>
        /// <param name="size">Texture size.</param>
        /// <param name="gradient">Color for the particle.</param>
        /// <param name="sprite">Texture for the particle.</param>
        /// <param name="lifespan">Lifespan of the particle.</param>
        public Particle(
            Vector2 position,
            Vector2 velocity,
            float angle,
            float angularVelocity,
            float size,
            Color gradient,
            Texture2D sprite,
            int lifespan)
        {
            this.sprite = sprite;
            this.position = position;
            this.velocity = velocity;
            this.angle = angle;
            this.angularVelocity = angularVelocity;
            this.gradient = gradient;
            this.size = size;
            this.lifespan = lifespan;
        }

        /* Accessors */

        /// <summary>
        /// Gets the position associated with the particle.
        /// </summary>
        public Vector2 Position
        {
            get { return this.position; }
            private set { this.position = value; }
        }

        /// <summary>
        /// Gets the velocity associated with the particle.
        /// </summary>
        public Vector2 Velocity
        {
            get { return this.velocity; }
            private set { this.velocity = value; }
        }

        /// <summary>
        /// Gets the angle associated with the particle.
        /// </summary>
        public float Angle
        {
            get { return this.angle; }
            private set { this.angle = value; }
        }

        /// <summary>
        /// Gets the angular velocity associated with the particle.
        /// </summary>
        public float AngularVelocity
        {
            get { return this.angularVelocity; }
            private set { this.angularVelocity = value; }
        }

        /// <summary>
        /// Gets the size associated with the particle.
        /// </summary>
        public float Size
        {
            get { return this.size; }
            private set { this.size = value; }
        }

        /// <summary>
        /// Gets the color associated with the particle.
        /// </summary>
        public Color Gradient
        {
            get { return this.gradient; }
            private set { this.gradient = value; }
        }

        /// <summary>
        /// Gets the texture associated with the particle.
        /// </summary>
        public Texture2D Sprite
        {
            get { return this.sprite; }
            private set { this.sprite = value; }
        }

        /// <summary>
        /// Gets the lifespan time associated with the particle.
        /// </summary>
        public int Lifespan
        {
            get { return this.lifespan; }
            private set { this.lifespan = value; }
        }

        /// <summary>
        /// Updates particle data.
        /// </summary>
        public void Update()
        {
            --this.lifespan;
            this.position += this.velocity;
            this.angle += this.angularVelocity;
        }

        /* Methods */

        /// <summary>
        /// Draws particle with the sprite given.
        /// </summary>
        /// <param name="spriteBatch">Sprite for the particle.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Save sprite's size as a rectangle.
            Rectangle sourceRectangle = new Rectangle(0, 0, this.sprite.Width, this.sprite.Height);

            // Create a vector centered in sprite.
            Vector2 origin = new Vector2(this.sprite.Width / 2, this.sprite.Height / 2);

            spriteBatch.Draw(
                this.sprite,
                this.position,
                sourceRectangle,
                this.gradient,
                this.angle,
                origin,
                this.size,
                SpriteEffects.None,
                0f);
        }
    }
}