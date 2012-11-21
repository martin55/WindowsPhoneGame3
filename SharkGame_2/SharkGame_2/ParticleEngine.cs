namespace SharkGame_2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Represents a particle generator object.
    /// </summary>
    public class ParticleEmitter
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
        /// Initializes a new instance of the <see cref="ParticleEmitter" /> class
        /// with the given sprites' collection and position for the emitter.
        /// </summary>
        /// <param name="textures">Collection of sprites.</param>
        /// <param name="location">New location for the emitter.</param>
        public ParticleEmitter(List<Texture2D> textures, Vector2 emitterLocation)
        {
            this.emitterLocation = emitterLocation;
            this.textures = textures;
            this.particles = new List<Particle>();
            this.random = new Random();
        }

        /* Properties */

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
            this.particles.ForEach(particle => particle.Draw(spriteBatch));
        }

        /// <summary>
        /// Generates some and updates all existing particles.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < 10; ++i)
            {
                this.particles.Add(this.GenerateNewParticle());
            }

            // Iterate over particles' list, update and remove "dead" particles.
            for (int i = this.particles.Count - 1; i >= 0; --i)
            {
                this.particles[i].Update();
                if (this.particles[i].Lifespan <= 0)
                {
                    this.particles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Generates a new, predefined particle.
        /// </summary>
        /// <returns>A new, predefined particle.</returns>
        private Particle GenerateNewParticle()
        {
            System.Diagnostics.Debug.WriteLine("Emitter: {0}", this.emitterLocation);
            return new Particle(
                this.emitterLocation,
                this.emitterVelocity,
                this.emitterAngle,
                0f,
                1f,
                Color.SandyBrown,
                this.textures[this.random.Next(this.textures.Count)],
                10 + this.random.Next(20));
        }
    }
}