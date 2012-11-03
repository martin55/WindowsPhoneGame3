namespace SharkGame
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Represents a particle generator object.
    /// </summary>
    public class ParticleEngine
    {
        /* Fields */

        /// <summary>
        /// Angle of the particle emitter.
        /// </summary>
        private float emitterAngle;

        /// <summary>
        /// Location of the particle emitter.
        /// </summary>
        private Vector2 emitterLocation;

        /// <summary>
        /// Velocity of the particle emitter.
        /// </summary>
        private Vector2 emitterVelocity;

        /// <summary>
        /// List of particles.
        /// </summary>
        private List<Particle> particles;

        /// <summary>
        /// Pseudo-random number generator for textures and TTL properties of particles.
        /// </summary>
        private Random random;

        /// <summary>
        /// List of textures.
        /// </summary>
        private List<Texture2D> textures;

        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleEngine" /> class.
        /// </summary>
        /// <param name="textures"></param>
        /// <param name="location"></param>
        public ParticleEngine(List<Texture2D> textures, Vector2 location)
        {
            this.EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            this.random = new Random();
        }

        /* Accessors */

        /// <summary>
        /// Gets or sets the angle of the particle emitter.
        /// </summary>
        public float EmitterAngle
        {
            get { return this.emitterAngle; }
            set { this.emitterAngle = value; }
        }

        /// <summary>
        /// Gets or sets the location of the particle emitter.
        /// </summary>
        public Vector2 EmitterLocation
        {
            get { return this.emitterLocation; }
            set { this.emitterLocation = value; }
        }

        /// <summary>
        /// Gets or sets the velocity of the particle emitter.
        /// </summary>
        public Vector2 EmitterVelocity
        {
            get { return this.emitterVelocity; }
            set { this.emitterVelocity = value; }
        }

        /* Methods */

        /// <summary>
        /// Draws all existing particles.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < this.particles.Count; ++index)
            {
                this.particles[index].Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Generates some and updates all existing particles.
        /// </summary>
        public void Update()
        {
            int total = 10;

            for (int i = 0; i < total; ++i)
            {
                this.particles.Add(this.GenerateNewParticle());
            }

            for (int particle = 0; particle < this.particles.Count; ++particle)
            {
                this.particles[particle].Update();
                if (this.particles[particle].Lifespan <= 0)
                {
                    this.particles.RemoveAt(particle);
                    --particle;
                }
            }
        }

        /// <summary>
        /// Generates a new, predefined particle.
        /// </summary>
        /// <returns>A new, predefined particle.</returns>
        private Particle GenerateNewParticle()
        {
            return new Particle(
                this.EmitterLocation,
                this.EmitterVelocity,
                this.EmitterAngle,
                0f,
                1f,
                Color.SandyBrown,
                this.textures[this.random.Next(this.textures.Count)],
                10 + this.random.Next(20));
        }
    }
}