using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace FishGame
{


    
        public class ParticleEngine
        {
            private Random random;
            public float EmitterAng;
            public Vector2 EmitterLocation { get; set; }
            public Vector2 EmitterVel { get; set; }
            private List<Particle> particles;
            private List<Texture2D> textures;

            public ParticleEngine(List<Texture2D> textures, Vector2 location)
            {
                EmitterLocation = location;
                this.textures = textures;
                this.particles = new List<Particle>();
                random = new Random();
            }

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

            public void Draw(SpriteBatch spriteBatch)
            {
               
                for (int index = 0; index < particles.Count; index++)
                {
                    particles[index].Draw(spriteBatch);
                }
                
            }
        }
    }

