namespace SharkGame
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Windows;
    using System.Windows.Navigation;
    using FarseerPhysics.Dynamics;
    using FarseerPhysics.Dynamics.Contacts;
    using FarseerPhysics.Factories;
    using Microsoft.Devices.Sensors;
    using Microsoft.Phone.Controls;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Media;
    using SharkGameLib;

    /// <summary>
    /// Represents a page for the game itself, accessible from the main menu.
    /// </summary>
    public partial class GamePage : PhoneApplicationPage
    {
        /* Fields */

        /// <summary>
        /// Content manager which fetches media from the LibContent project.
        /// </summary>
        private ContentManager contentManager;

        /// <summary>
        /// Representation of the physical accelerometer sensor.
        /// </summary>
        private Accelerometer accelerometer;

        /// <summary>
        /// Game camera; at fixed height over the board, movable in different
        /// directions, always following the main character.
        /// </summary>
        private Camera camera;

        /// <summary>
        /// Particle generator.
        /// </summary>
        private ParticleEmitter particleEngine;

        /// <summary>
        /// Game world that holds all objects and manages all the physics
        /// and collisions.
        /// </summary>
        private World gameWorld;

        /// <summary>
        /// Textures for all game objects.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Game object body for the shark, game's "protagonist".
        /// </summary>
        private Body shark;

        /// <summary>
        /// A collection of game object bodies for humans,
        /// game's "antagonists" and prey.
        /// </summary>
        private List<Body> humans;

        /// <summary>
        /// A collection of game object bodies for pools,
        /// timer replenishers for the shark.
        /// </summary>
        private List<Body> pools;

        /// <summary>
        /// A collection of game object bodies for traps,
        /// shark insta-killers.
        /// </summary>
        private List<Body> traps;

        /// <summary>
        /// A collection of game object bodies for crates,
        /// movement hindering and annoying things.
        /// </summary>
        private List<Body> crates;

        /// <summary>
        /// A collection of game object bodies for walls,
        /// movement restraining invisible barriers.
        /// </summary>
        private List<Body> walls;

        /// <summary>
        /// Collection of game sprites for corresponding bodies.
        /// </summary>
        private List<Texture2D> sprites;

        /// <summary>
        /// Collection of sprites for the map.
        /// </summary>
        private List<Texture2D> tiles;

        /// <summary>
        /// Current frame for the shark's movement animation.
        /// </summary>
        private int playerCurrentFrame = 0;

        /// <summary>
        /// Width of the running fish's single animation frame.
        /// </summary>
        private int runFrameWidth;

        /// <summary>
        /// Height of the running fish's single animation frame.
        /// </summary>
        private int runFrameHeight;

        /// <summary>
        /// Delay for the shark's movement animation (optimization aspects).
        /// </summary>
        private readonly float playerAnimationDelay = .1f;

        /// <summary>
        /// Current delay for the shark's movement animation.
        /// </summary>
        private float currentPlayerAnimationDelay = 0f;

        /// <summary>
        /// Main game timer for update events.
        /// </summary>
        private GameTimer timer;

        /// <summary>
        /// Time left until the shark suffocates.
        /// </summary>
        private TimeSpan timeLeft;

        /// <summary>
        /// An amount of points the player has collected throughout
        /// the current level.
        /// </summary>
        private int points;

        /// <summary>
        /// An amount of time that should pass before the eating
        /// sound effect can be played again.
        /// </summary>
        private float timeSinceEatingSound;

        /// <summary>
        /// An amount of time that should pass before the trap 
        /// sound effect can be played again.
        /// </summary>
        private float timeSinceTrapSound;

        /// <summary>
        /// An amount of time that should pass before the water
        /// sound effect can be played again.
        /// </summary>
        private float timeSinceWaterSound;

        /// <summary>
        /// A value indicating whether the game is over.
        /// </summary>
        private bool isGameOver;

        /// <summary>
        /// An amount of time that should pass before the game over screen
        /// fades.
        /// </summary>
        private float timeSinceGameOver;

        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePage" /> class.
        /// </summary>
        public GamePage()
        {
            this.InitializeComponent();

            // Set the sharing mode of the graphics device to turn on XNA rendering.
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Get the content manager from the application.
            this.contentManager = (Application.Current as App).Content;

            // Begin playing the game's soundtrack.
            Song song = this.contentManager.Load<Song>("game");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;

            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(
                SharedGraphicsDeviceManager.Current.GraphicsDevice);

            // Initialize the game world with no gravity.
            this.gameWorld = new World(Vector2.Zero);

            // Load particles.
            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(this.contentManager.Load<Texture2D>("shadow"));

            // Initializes a new instance of the particle emitter with textures.
            this.particleEngine = new ParticleEmitter(
                textures,
                new Vector2(400f, 240f));

            this.InitializeMap();

            this.InitializeBodies();

            this.InitializeSprites();

            // Create a default camera object centered on the shark.
            this.camera = new Camera(this.shark.Position.ToRealVector());

            // Create a timer for this page.
            this.timer = new GameTimer();
            this.timer.UpdateInterval = TimeSpan.FromTicks(333333);
            this.timer.Update += this.OnUpdate;
            this.timer.Draw += this.OnDraw;

            // Start the time countdown and reset points.
            this.timeLeft = TimeSpan.FromSeconds(40.0);
            this.points = 0;

            // Set sound effect timers to arbitrary positive number so that
            // early collisions will trigger them as well. Five [seconds],
            // a number higher than any sound effect timer, should do.
            this.timeSinceEatingSound = 5f;
            this.timeSinceTrapSound = 5f;
            this.timeSinceWaterSound = 5f;
            this.timeSinceGameOver = 0f;

            // Set up accelerometer handling.
            this.accelerometer = new Accelerometer();
            this.accelerometer.Start();
        }

        /* Methods */

        /// <summary>
        /// Handles navigating to the game.
        /// </summary>
        /// <param name="e">Information passed to the event.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Start the timer.
            this.timer.Start();

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Handles navigating from the game to any other page or app.
        /// </summary>
        /// <param name="e">Information passed to the event.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer.
            this.timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Initialize all game object sprites.
        /// </summary>
        private void InitializeSprites()
        {
            this.sprites = new List<Texture2D>();

            this.sprites.Add(this.contentManager.Load<Texture2D>("shark"));
            this.sprites.Add(this.contentManager.Load<Texture2D>("people"));
            this.sprites.Add(this.contentManager.Load<Texture2D>("pool"));
            this.sprites.Add(this.contentManager.Load<Texture2D>("trap"));
            this.sprites.Add(this.contentManager.Load<Texture2D>("crate"));
            this.sprites.Add(this.contentManager.Load<Texture2D>("wall"));

            this.runFrameWidth = this.sprites[Constants.GameObjects.Shark].Width / 16;
            this.runFrameHeight = this.sprites[Constants.GameObjects.Shark].Height;
        }

        /// <summary>
        /// Initializes all visible game objects' bodies: shark, humans, pools, traps and crates.
        /// </summary>
        private void InitializeBodies()
        {
            // Initialize the shark body.
            this.shark = BodyFactory.CreateCircle(this.gameWorld, 0.8f, 1f);
            this.shark.BodyType = BodyType.Dynamic;
            this.shark.Position = Conversions.ToSimVector(
                new Vector2(
                    Constants.ScreenHeight / 2f,
                    Constants.ScreenWidth / 2f));
            this.shark.LinearDamping = 1f;
            this.shark.CollidesWith = Category.All;
            this.shark.CollisionCategories = Category.Cat1;

            // Initialize other objects' bodies.
            this.humans = new List<Body>();
            this.pools = new List<Body>();
            this.traps = new List<Body>();
            this.crates = new List<Body>();

            int humanIndex = 0;
            int poolIndex = 0;
            int trapIndex = 0;
            int crateIndex = 0;

            Microsoft.Xna.Framework.Point maximum
                = new Microsoft.Xna.Framework.Point(
                    Constants.Maps.Map.GetLength(0),
                    Constants.Maps.Map.GetLength(1));

            for (int y = 0; y < maximum.Y; ++y)
            {
                for (int x = 0; x < maximum.X; ++x)
                {
                    switch (Constants.Maps.ObjectMap[y, x])
                    {
                        case 1:
                            this.humans.Add(BodyFactory.CreateRectangle(this.gameWorld, 0.9f, 0.9f, 1f));
                            this.humans[humanIndex].Position = (new Vector2(
                                x * Constants.Maps.TileSize - Constants.ScreenWidth / 2 + Constants.Maps.TileSize / 2,
                                y * Constants.Maps.TileSize - Constants.ScreenHeight / 2 + Constants.Maps.TileSize / 2)).ToSimVector();
                            this.humans[humanIndex].CollidesWith = Category.None;
                            this.humans[humanIndex].CollisionCategories = Category.Cat2;
                            FixtureFactory.AttachCircle(0.5f, 1f, this.humans[humanIndex]).OnCollision += this.HumanEaten;
                            ++humanIndex;
                            break;

                        case 2:
                            this.pools.Add(BodyFactory.CreateCircle(this.gameWorld, 0.9f, 1f, 1f));
                            this.pools[poolIndex].Position = (new Vector2(
                                x * Constants.Maps.TileSize - Constants.ScreenWidth / 2 + Constants.Maps.TileSize / 2,
                                y * Constants.Maps.TileSize - Constants.ScreenHeight / 2 + Constants.Maps.TileSize / 2)).ToSimVector();
                            this.pools[poolIndex].CollidesWith = Category.None;
                            this.pools[poolIndex].CollisionCategories = Category.Cat3;
                            FixtureFactory.AttachCircle(0.9f, 1f, this.pools[poolIndex]).OnCollision += this.TimerReplenished;
                            ++poolIndex;
                            break;

                        case 3:
                            this.traps.Add(BodyFactory.CreateCircle(this.gameWorld, 0.9f, 1f, 1f));
                            this.traps[trapIndex].Position = (new Vector2(
                                x * Constants.Maps.TileSize - Constants.ScreenWidth / 2 + Constants.Maps.TileSize / 2,
                                y * Constants.Maps.TileSize - Constants.ScreenHeight / 2 + Constants.Maps.TileSize / 2)).ToSimVector();
                            this.traps[trapIndex].CollidesWith = Category.None;
                            this.traps[trapIndex].CollisionCategories = Category.Cat4;
                            FixtureFactory.AttachCircle(0.1f, 0.5f, this.traps[trapIndex]).OnCollision += this.SharkDead;
                            ++trapIndex;
                            break;

                        case 4:
                            this.crates.Add(BodyFactory.CreateRectangle(this.gameWorld, 1f, 1f, 1f));
                            this.crates[crateIndex].BodyType = BodyType.Dynamic;
                            this.crates[crateIndex].Position = (new Vector2(
                                x * Constants.Maps.TileSize - Constants.ScreenWidth / 2 + Constants.Maps.TileSize / 2,
                                y * Constants.Maps.TileSize - Constants.ScreenHeight / 2 + Constants.Maps.TileSize / 2)).ToSimVector();
                            this.crates[crateIndex].CollidesWith = Category.Cat1;
                            this.crates[crateIndex].CollisionCategories = Category.Cat5;
                            this.crates[crateIndex].LinearDamping = 1f;

                            ++crateIndex;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes tiles for the map, the map itself and the invisible walls
        /// to restrict shark's movement to the map only.
        /// </summary>
        private void InitializeMap()
        {
            // Load tiles.
            this.tiles = new List<Texture2D>();

            this.tiles.Add(this.contentManager.Load<Texture2D>("sand"));
            this.tiles.Add(this.contentManager.Load<Texture2D>("sea_sand"));
            this.tiles.Add(this.contentManager.Load<Texture2D>("sea"));

            // Create map bounds - invisible walls that catch shark within
            // to prevent "leaving" the screen.
            Microsoft.Xna.Framework.Point upperLeft = new Microsoft.Xna.Framework.Point(
                -(Constants.ScreenWidth / 2),
                -(Constants.ScreenHeight / 2));
            Microsoft.Xna.Framework.Point lowerRight = new Microsoft.Xna.Framework.Point(
                Constants.Maps.MapWidth - Constants.ScreenWidth / 2,
                Constants.Maps.MapHeight - Constants.ScreenHeight / 2);

            this.walls = new List<Body>();
            walls = new List<Body>();
            walls.Add(BodyFactory.CreateEdge(this.gameWorld, new Vector2(upperLeft.X, upperLeft.Y).ToSimVector(), new Vector2(lowerRight.X, upperLeft.Y).ToSimVector()));
            walls.Add(BodyFactory.CreateEdge(this.gameWorld, new Vector2(upperLeft.X, upperLeft.Y).ToSimVector(), new Vector2(upperLeft.X, lowerRight.Y).ToSimVector()));
            walls.Add(BodyFactory.CreateEdge(this.gameWorld, new Vector2(lowerRight.X, lowerRight.Y).ToSimVector(), new Vector2(lowerRight.X, upperLeft.Y).ToSimVector()));
            walls.Add(BodyFactory.CreateEdge(this.gameWorld, new Vector2(lowerRight.X, lowerRight.Y).ToSimVector(), new Vector2(upperLeft.X, lowerRight.Y).ToSimVector()));
            foreach (Body wallFragment in this.walls)
            {
                wallFragment.CollidesWith = Category.All;
                wallFragment.CollisionCategories = Category.Cat6;
            }
        }

        /// <summary>
        /// Handles collision of the shark and a wall.
        /// </summary>
        /// <param name="fixtureA">One of the colliding objects - wall.</param>
        /// <param name="fixtureB">One of the colliding objects - shark.</param>
        /// <param name="contact">Detailed info about the contact made.</param>
        /// <returns>True if collision passes, false if it is canceled.</returns>
        private bool WallHit(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.CollisionCategories == Category.Cat1)
            {
                Debug.WriteLine("Wall hit");
                fixtureA.Body.LinearVelocity = Vector2.Zero;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles collision of the shark and a trap.
        /// </summary>
        /// <param name="fixtureA">One of the colliding objects - trap.</param>
        /// <param name="fixtureB">One of the colliding objects - shark.</param>
        /// <param name="contact">Detailed info about the contact made.</param>
        /// <returns>True if collision passes, false if it is canceled.</returns>
        private bool SharkDead(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.CollisionCategories == Category.Cat1)
            {
                if (this.timeSinceTrapSound > 1f)
                {
                    Debug.WriteLine("Shark dead");
                    this.isGameOver = true;
                    this.contentManager.Load<SoundEffect>("trapsound").Play();
                    this.timeSinceTrapSound = 0f;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Handles collision of the shark and a human.
        /// </summary>
        /// <param name="fixtureA">One of the colliding objects - human.</param>
        /// <param name="fixtureB">One of the colliding objects - shark.</param>
        /// <param name="contact">Detailed info about the contact made.</param>
        /// <returns>True if collision passes, false if it is canceled.</returns>
        private bool HumanEaten(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.CollisionCategories == Category.Cat1)
            {
                if (this.timeSinceEatingSound > 1f)
                {
                    this.contentManager.Load<SoundEffect>("eating").Play();
                }

                Debug.WriteLine("Human eaten");
                this.points += 5;
                this.timeSinceEatingSound = 0f;
                fixtureA.Body.Enabled = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Handles collision of the shark and a pool of water.
        /// </summary>
        /// <param name="fixtureA">One of the colliding objects - a pool of water.</param>
        /// <param name="fixtureB">One of the colliding objects - shark.</param>
        /// <param name="contact">Detailed info about the contact made.</param>
        /// <returns>True if collision passes, false if it is canceled.</returns>
        private bool TimerReplenished(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.CollisionCategories == Category.Cat1)
            {
                if (this.timeSinceWaterSound > 2f)
                {
                    Debug.WriteLine("Timer replenished");
                    this.timeLeft += TimeSpan.FromSeconds(2.0);
                    this.contentManager.Load<SoundEffect>("water").Play();
                    this.timeSinceWaterSound = 0f;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Handles game over screen display.
        /// </summary>
        private void GameOver()
        {
            // Stop the game timer and the music.
            MediaPlayer.Stop();
            this.timer.Stop();

            // Check high-scores - if the score for this level was high enough, induct player to the high scores list.
            List<HighScore> highScores;
            IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;
            try
            {
                highScores = userSettings["High Scores"] as List<HighScore>;
            }
            catch (KeyNotFoundException exception)
            {
                Debug.WriteLine("High scores data not found; " + exception.Message);
                highScores = new List<HighScore>();
            }

            if (highScores.Count < 8 || (highScores.Count > 0 && this.points > highScores.Min(item => item.Points)))
            {
                // This shark was a hero indeed. Save the result in the Isolated Storage
                // and go to the high scores page.
                HighScore currentScore = new HighScore("player", DateTime.Now, this.points);

                // Induct the player into high scores list. If more than 8 elements, remove the last element.
                highScores.Add(currentScore);
                highScores = highScores.OrderByDescending(x => x.Points).ToList();
                if (highScores.Count > 8)
                {
                    highScores.Remove(highScores[highScores.Count - 1]);
                }

                if (userSettings.Contains("High Scores"))
                {
                    userSettings["High Scores"] = highScores;
                }
                else
                {
                    userSettings.Add("High Scores", highScores);
                }

                userSettings.Save();

                int position = highScores.IndexOf(currentScore);
                NavigationService.Navigate(new Uri("/ScorePage.xaml?highlight=" + position, UriKind.Relative));
            }
            else
            {
                // Shark is not a cat, has only one life, so a dead one means a dead one.
                // Go back to the main menu screen.
                NavigationService.GoBack();
            }
        }

        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Information passed to the event.</param>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            float elapsed = (float)e.ElapsedTime.TotalSeconds;

            if (!this.isGameOver)
            {
                // Set the refresh rate to 30 FPS (step one thirtieth of a second in time).
                this.gameWorld.Step(Math.Min(elapsed, 1f / 30f));

                this.timeLeft -= e.ElapsedTime;
                if (this.timeLeft < TimeSpan.FromSeconds(0.0))
                {
                    this.isGameOver = true;
                }

                this.currentPlayerAnimationDelay += elapsed;
                this.timeSinceEatingSound += elapsed;
                this.timeSinceTrapSound += elapsed;
                this.timeSinceWaterSound += elapsed;

                if (Math.Abs(this.shark.LinearVelocity.X) > 0.07f
                    || Math.Abs(this.shark.LinearVelocity.Y) > 0.07f)
                {
                    while (this.currentPlayerAnimationDelay > this.playerAnimationDelay)
                    {
                        ++this.playerCurrentFrame;
                        this.playerCurrentFrame = this.playerCurrentFrame % 16;
                        this.currentPlayerAnimationDelay -= this.playerAnimationDelay;
                    }

                    // Measure the angle at which the shark is moving forward.
                    this.shark.Rotation = Vector2.UnitX.AngleBetween(Vector2.UnitX + this.shark.LinearVelocity) % (MathHelper.Pi * 2f);

                    // Generate shark's "footprints".
                    this.particleEngine.Generate();
                }

                // Generate shark's "footprints".
                this.particleEngine.EmitterAngle = this.shark.Rotation;
                this.particleEngine.EmitterVelocity = -this.shark.LinearVelocity;
                this.particleEngine.EmitterLocation = this.shark.Position;
                this.particleEngine.Update();


                // If the device is tilted sufficiently, adjust shark's velocity.
                // Get the accelerometer readings and clamp them to a certain range
                // so that the player will not benefit too much from extreme tilting.
                // We also need to swap the accelerometer readings because
                // a Landscape orientation is used (X becomes Y, Y becomes X).
                float horizontalReading = 0f;
                float verticalReading = 0f;
                if (Math.Abs(accelerometer.CurrentValue.Acceleration.Y) > Constants.Speeds.SharkSpeedMin)
                {
                    horizontalReading = MathHelper.Clamp(-accelerometer.CurrentValue.Acceleration.Y, -0.6f, 0.6f);
                }

                if (Math.Abs(accelerometer.CurrentValue.Acceleration.X) > Constants.Speeds.SharkSpeedMin)
                {
                    verticalReading = MathHelper.Clamp(-accelerometer.CurrentValue.Acceleration.X, -0.6f, 0.6f);
                }

                // Reset angular velocity (we're not gonna need that)
                // and apply force according to the shark's speed multiplier.
                this.shark.AngularVelocity = 0f;
                this.shark.ApplyForce(new Vector2(horizontalReading, verticalReading) * Constants.Speeds.SharkSpeedMultiplier);

                // Update camera position.
                this.camera.Position = Vector2.Clamp(
                    this.shark.Position.ToRealVector(),
                    Vector2.Zero,
                    new Vector2(
                        Constants.Maps.MapWidth - Constants.ScreenWidth,
                        Constants.Maps.MapHeight - Constants.ScreenHeight));
            }
            else
            {
                // After three seconds of showing the game over screen, proceed with GameOver() method.
                this.timeSinceGameOver += elapsed;
                if (this.timeSinceGameOver > 3f)
                {
                    this.GameOver();
                }
            }
        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Information passed to the event.</param>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.CornflowerBlue);

            if (this.isGameOver)
            {
                this.spriteBatch.Begin();

                this.spriteBatch.DrawString(
                    this.contentManager.Load<SpriteFont>("DisplayFont"),
                    "Game over!",
                    new Vector2(2f, 1.5f).ToRealVector(),
                    Color.Red);
                this.spriteBatch.DrawString(
                    this.contentManager.Load<SpriteFont>("DisplayFont"),
                    "Points: " + this.points,
                    new Vector2(2.2f, 3.5f).ToRealVector(),
                    Color.Red);

                this.spriteBatch.End();
            }
            else
            {
                // Adjust origin vector according to the current shark's position.
                Vector2 origin = new Vector2(
                    (this.sprites[Constants.GameObjects.Shark].Width / 16) / 2,
                    this.sprites[Constants.GameObjects.Shark].Height / 2);

                Microsoft.Xna.Framework.Point cameraPoint = this.camera.Position.ToCell();
                Microsoft.Xna.Framework.Point viewPoint = (this.camera.Position + this.ViewPortVector()).ToCell();
                Microsoft.Xna.Framework.Point min = new Microsoft.Xna.Framework.Point(
                    cameraPoint.X, cameraPoint.Y);
                Microsoft.Xna.Framework.Point max = new Microsoft.Xna.Framework.Point(
                    (int)Math.Min(viewPoint.X, Constants.Maps.Map.GetLength(1)),
                    (int)Math.Min(viewPoint.Y, Constants.Maps.Map.GetLength(0)));

                // Draw the map.
                this.DrawMap(min, max);

                this.spriteBatch.Begin(
                    SpriteSortMode.BackToFront,
                    BlendState.AlphaBlend,
                    null,
                    null,
                    null,
                    null,
                    this.camera.GetTransformationMatrix());

                // Draw any existing particles.
                this.particleEngine.Draw(this.spriteBatch);

                Texture2D deadPeople = this.contentManager.Load<Texture2D>("dead_people");

                for (int y = min.Y; y < max.Y; ++y)
                {
                    for (int x = min.X; x < max.X; ++x)
                    {
                        switch (Constants.Maps.ObjectMap[y, x])
                        {
                            case 1:
                                // Draw each human object in the collection.
                                foreach (Body human in this.humans)
                                {
                                    this.spriteBatch.Draw(
                                        human.Enabled ? this.sprites[Constants.Maps.ObjectMap[y, x]] : deadPeople,
                                        human.Position.ToRealVector(),
                                        null,
                                        Color.White,
                                        human.Rotation,
                                        new Vector2(
                                            this.sprites[Constants.Maps.ObjectMap[y, x]].Width / 2f,
                                            this.sprites[Constants.Maps.ObjectMap[y, x]].Height / 2f),
                                            0.7f,
                                            SpriteEffects.None,
                                            1f);
                                }

                                break;

                            case 2:
                                // Draw each pool object in the collection.
                                foreach (Body pool in this.pools)
                                {
                                    this.spriteBatch.Draw(
                                        this.sprites[Constants.Maps.ObjectMap[y, x]],
                                        pool.Position.ToRealVector(),
                                        null,
                                        Color.White,
                                        pool.Rotation,
                                        new Vector2(
                                            this.sprites[Constants.Maps.ObjectMap[y, x]].Width / 2f,
                                            this.sprites[Constants.Maps.ObjectMap[y, x]].Height / 2f),
                                            1f,
                                            SpriteEffects.None,
                                            1f);
                                }

                                break;

                            case 3:
                                // Draw each trap object in the collection.
                                foreach (Body trap in this.traps)
                                {
                                    this.spriteBatch.Draw(
                                        this.sprites[Constants.Maps.ObjectMap[y, x]],
                                        trap.Position.ToRealVector(),
                                        null,
                                        Color.White,
                                        trap.Rotation,
                                        new Vector2(
                                            this.sprites[Constants.Maps.ObjectMap[y, x]].Width / 2f,
                                            this.sprites[Constants.Maps.ObjectMap[y, x]].Height / 2f),
                                            0.6f,
                                            SpriteEffects.None,
                                            1f);
                                }

                                break;

                            case 4:
                                // Draw each crate object in the collection.
                                foreach (Body crate in this.crates)
                                {
                                    this.spriteBatch.Draw(
                                        this.sprites[Constants.Maps.ObjectMap[y, x]],
                                        crate.Position.ToRealVector(),
                                        null,
                                        Color.White,
                                        crate.Rotation,
                                        new Vector2(
                                            this.sprites[Constants.Maps.ObjectMap[y, x]].Width / 2f,
                                            this.sprites[Constants.Maps.ObjectMap[y, x]].Height / 2f),
                                            0.5f,
                                            SpriteEffects.None,
                                            1f);
                                }

                                break;
                        }
                    }
                }

                // Draw the shark sprite.
                this.spriteBatch.Draw(
                    this.sprites[Constants.GameObjects.Shark],
                    this.shark.Position.ToRealVector(),
                    new Microsoft.Xna.Framework.Rectangle(this.playerCurrentFrame * this.runFrameWidth, 0, this.runFrameWidth, this.runFrameHeight),
                    Color.White,
                    this.shark.Rotation,
                    origin,
                    1.4f,
                    SpriteEffects.None,
                    0f);
                this.spriteBatch.End();

                // Draw the HUD.
                this.DrawHud();
            }
        }

        /// <summary>
        /// Draws the map tiles in the area specified by min and max points.
        /// </summary>
        /// <param name="min">Upper-left-most point of the drawing rectangle.</param>
        /// <param name="max">Lower-right-most point of the drawing rectangle.</param>
        private void DrawMap(Microsoft.Xna.Framework.Point min, Microsoft.Xna.Framework.Point max)
        {
            this.spriteBatch.Begin();
            Rectangle tileRectangle = new Rectangle(
                0,
                0,
                Constants.Maps.TileSize,
                Constants.Maps.TileSize);

            for (int y = min.Y; y < max.Y; ++y)
            {
                for (int x = min.X; x < max.X; ++x)
                {
                    tileRectangle.X = (x * Constants.Maps.TileSize) - (int)this.camera.Position.X;
                    tileRectangle.Y = (y * Constants.Maps.TileSize) - (int)this.camera.Position.Y;
                    this.spriteBatch.Draw(
                        this.tiles[Constants.Maps.Map[y, x]],
                        tileRectangle,
                        Color.White);
                }
            }

            this.spriteBatch.End();
        }

        /// <summary>
        /// Draws the display, such as time left and points collected so far.
        /// </summary>
        private void DrawHud()
        {
            this.spriteBatch.Begin();

            // Draw a timer - how many seconds are left in the stage.
            this.spriteBatch.DrawString(
                this.contentManager.Load<SpriteFont>("DisplayFont"),
                this.timeLeft.ToString("ss"),
                new Vector2(0.2f, 0.2f).ToRealVector(),
                Color.Blue);

            // Draw a points counter - how many points did the player collect
            // so far in the stage.
            this.spriteBatch.DrawString(
                this.contentManager.Load<SpriteFont>("DisplayFont"),
                this.points.ToString(),
                new Vector2(7.4f, 4.0f).ToRealVector(),
                Color.Yellow);

            this.spriteBatch.End();
        }

        /// <summary>
        /// Returns the viewport vector, representing the visible portion of the canvas (map).
        /// </summary>
        /// <returns>Viewport vector.</returns>
        private Vector2 ViewPortVector()
        {
            return new Vector2(
                Constants.ScreenWidth + (2 * Constants.Maps.TileSize),
                Constants.ScreenHeight + (2 * Constants.Maps.TileSize));
        }
    }
}