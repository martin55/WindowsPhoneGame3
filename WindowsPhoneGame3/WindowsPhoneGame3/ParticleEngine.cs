namespace FishGame
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// 
    /// </summary>
    public class ParticleEngine
    {
        /// <summary>
        /// 
        /// </summary>
        public float EmitterAng;

        /// <summary>
        /// Gets or sets the emitter location associated with the particle.
        /// </summary>
        public Vector2 EmitterLocation { get; set; }

        /// <summary>
        /// Gets or sets the emitter velocity associated with the particle.
        /// </summary>
        public Vector2 EmitterVel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private Random random;

        /// <summary>
        /// 
        /// </summary>
        private List<Particle> particles;

        /// <summary>
        /// 
        /// </summary>
        private List<Texture2D> textures;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleEngine" /> class.
        /// </summary>
        /// <param name="textures"></param>
        /// <param name="location"></param>
        public ParticleEngine(List<Texture2D> textures, Vector2 location)
        {
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            random = new Random();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            int total = 10;

            for (int i = 0; i < total; i++)
            {
                particles.Add(GenerateNewParticle());
            }

            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].TTL <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Particle GenerateNewParticle()
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = EmitterVel;
            float angle = EmitterAng;

            Color color = Color.SandyBrown;
            float size = 1;
            int ttl = 10 + random.Next(20);

            return new Particle(texture, position, velocity, angle, 0, color, size, ttl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(spriteBatch);
            }
        }
    }
}

