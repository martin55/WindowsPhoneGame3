namespace FishGame
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// 
    /// </summary>
    public class Particle
    {
        /// <summary>
        /// Gets or sets the texture associated with the particle.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Gets or sets the position associated with the particle.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the velocity associated with the particle.
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// Gets or sets the angle associated with the particle.
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// Gets or sets the angular velocity associated with the particle.
        /// </summary>
        public float AngularVelocity { get; set; }

        /// <summary>
        /// Gets or sets the color associated with the particle.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the size associated with the particle.
        /// </summary>
        public float Size { get; set; }

        /// <summary>
        /// Gets or sets the lifespan time associated with the particle.
        /// </summary>
        public int TTL { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Particle" /> class.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <param name="angle"></param>
        /// <param name="angularVelocity"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="ttl"></param>
        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Color color, float size, int ttl)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            Size = size;
            TTL = ttl;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            TTL--;
            Position += Velocity;
            Angle += AngularVelocity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            spriteBatch.Draw(Texture, Position, sourceRectangle, Color,
                Angle, origin, Size, SpriteEffects.None, 0f);
        }
    }
}

