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

    /// <summary>
    /// Represents a page for the game itself, accessible from the main menu.
    /// </summary>
    public partial class GamePage : PhoneApplicationPage
    {
        int tileMapWidth;
        int tileMapHeight;

        static int mapWidthInPixels;
        static int mapHeightInPixels;

        public static int MapWidthInPixels
        {
            get { return mapWidthInPixels; }
        }
        public static int MapHeightInPixels
        {
            get { return mapHeightInPixels; }
        }


        /* Fields */

        /// <summary>
        /// Device's screen width in pixels.
        /// </summary>
        private static readonly int screenWidth = 800;

        /// <summary>
        /// Device's screen height in pixels.
        /// </summary>
        private static readonly int screenHeight = 480;

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
        /// Game world that holds all objects and manages all the physics and collisions.
        /// </summary>
        private World gameWorld;

        /// <summary>
        /// Textures for all game objects.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Collection of game bodies.
        /// </summary>
        private List<Body> bodies;
        private List<Body> humans;
        private List<Body> pools;
        private List<Body> traps;

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
        /// Collection of fixtures for the game objects.
        /// </summary>
        /// 
        private List<Fixture> fixtures;

        /// <summary>
        /// Vector with its initial point set to screen center.
        /// </summary>
        private Vector2 centralVector = Conversions.ToSimVector(new Vector2(screenHeight / 2f, screenWidth / 2f));

        /// <summary>
        /// Shark's velocity.
        /// </summary>
        private Vector2 sharkVelocity = Vector2.Zero;

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
        /// 
        /// </summary>
        private float playerAnimationDelay = .1f;

        /// <summary>
        /// 
        /// </summary>
        private float currentPlayerAnimationDelay = 0f;

        /// <summary>
        /// Shark's rotation angle in degrees.
        /// </summary>
        private float sharkRotation = 0f;

        /// <summary>
        /// 
        /// </summary>
        private ContentManager contentManager;

        /// <summary>
        /// 
        /// </summary>
        private GameTimer timer;

        /// <summary>
        /// Time left until the shark suffocates.
        /// </summary>
        private TimeSpan timeLeft;

        /// <summary>
        /// Amount of points the player has collected throughout the current level.
        /// </summary>
        private int points;

        /// <summary>
        /// Timer for eating sound effect.
        /// </summary>
        private float timeSinceEatingSound;

        /// <summary>
        /// Timer for trap sound effect.
        /// </summary>
        private float timeSinceTrapSound;

        /// <summary>
        /// Timer for water sound effect.
        /// </summary>
        private float timeSinceWaterSound;

        bool backPressed;
        bool pausedGame = false;

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

            // Create a timer for this page.
            this.timer = new GameTimer();
            this.timer.UpdateInterval = TimeSpan.FromTicks(333333);
            this.timer.Update += this.OnUpdate;
            this.timer.Draw += this.OnDraw;

            // Start the time countdown and reset points.
            this.timeLeft = TimeSpan.FromSeconds(30.0);
            this.points = 0;

            // Set sound effect timers to arbitrary positive number so that early collisions will trigger them as well.
            // Five [seconds], a number higher than any sound effect timer, should do.
            this.timeSinceEatingSound = 5f;
            this.timeSinceTrapSound = 5f;
            this.timeSinceWaterSound = 5f;

            // Set up accelerometer handling.
            this.accelerometer = new Accelerometer();
            this.accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(this.Accelerometer_CurrentValueChanged);
            this.accelerometer.Start();
        }

        /* Properties */

        /// <summary>
        /// Gets device's screen width.
        /// </summary>
        public static int ScreenWidth
        {
            get { return GamePage.screenWidth; }
        }

        /// <summary>
        /// Gets device's screen height.
        /// </summary>
        public static int ScreenHeight
        {
            get { return GamePage.screenHeight; }
        }

        /* Methods */





        void GamePage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Pause the game and return to the menu.
            backPressed = true;
            HandleInput(backPressed);
            e.Cancel = true;
        }

        //two times pressed back button exit game(one time just paused game. not working yet properly)
        public void HandleInput(bool shouldPause)
        {

            if (pausedGame)
            {
                FinishCurrentGame();
                pausedGame = false;
                backPressed = false;
            }
            else
            {
                pausedGame = true;

            }

        }

        private void FinishCurrentGame()
        {
            backPressed = false;
            NavigationService.GoBack();
        }





        /// <summary>
        /// Handles navigating to the game.
        /// </summary>
        /// <param name="e">Information passed to the event.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            // Begin playing the game's soundtrack.
            Song song = this.contentManager.Load<Song>("game");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;

            if (this.gameWorld == null)
            {
                this.gameWorld = new World(Microsoft.Xna.Framework.Vector2.Zero);
            }
            else
            {
                this.gameWorld.Clear();
            }

            tileMapWidth = Constants.Maps.Map.GetLength(1);
            tileMapHeight = Constants.Maps.Map.GetLength(0);
            mapWidthInPixels = tileMapWidth * Constants.Maps.TileWidth;
            mapHeightInPixels = tileMapHeight * Constants.Maps.TileHeight;

            // Load particles.
            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(this.contentManager.Load<Texture2D>("shadow"));

            // Initializes a new instance of the particle emitter with textures.
            this.particleEngine = new ParticleEmitter(textures, new Vector2(400f, 240f));

            // Load tiles.
            this.tiles = new List<Texture2D>();

            this.tiles.Add(this.contentManager.Load<Texture2D>("sand"));
            this.tiles.Add(this.contentManager.Load<Texture2D>("sea_sand"));
            this.tiles.Add(this.contentManager.Load<Texture2D>("sea"));

            // Load bodies.
            this.bodies = new List<Body>();

            this.bodies.Add(BodyFactory.CreateCircle(this.gameWorld, 0.6f, 1f));
            this.bodies[Constants.GameObjects.Shark].BodyType = BodyType.Dynamic;
            this.bodies[Constants.GameObjects.Shark].Position = this.centralVector;
            this.bodies[Constants.GameObjects.Shark].CollidesWith = Category.All;

            //this.bodies.Add(BodyFactory.CreateRectangle(this.gameWorld, 0.9f, 0.9f, 1f));
            //this.bodies[Constants.GameObjects.Human].BodyType = BodyType.Dynamic;
            //this.bodies[Constants.GameObjects.Human].Position = new Vector2(2f, 4f);



            //this.bodies.Add(BodyFactory.CreateCircle(this.gameWorld, 0.9f, 1f, 1f));
            //this.bodies[Constants.GameObjects.Pool].Position = new Vector2(3f, 2f);

            //this.bodies.Add(BodyFactory.CreateCircle(this.gameWorld, 0.9f, 1f, 1f));
            //this.bodies[Constants.GameObjects.Trap].Position = new Vector2(1f, 1f);

            this.humans = new List<Body>();
            this.pools = new List<Body>();
            this.traps = new List<Body>();
           int humanIndex=0;
           int poolIndex = 0;
           int trapIndex = 0;
          
            
            

            for (int y = 0; y < Constants.Maps.Map.GetLength(1); y++)
            {
                for (int x = 0; x < Constants.Maps.Map.GetLength(0); x++)

                {

                     switch (Constants.Maps.ObjectMap[y, x])
                        {
                            case 1:
                                this.humans.Add(BodyFactory.CreateRectangle(this.gameWorld, 0.9f, 0.9f, 1f));
                                   
                                    this.humans[humanIndex].BodyType = BodyType.Dynamic;
                                    
                                    this.humans[humanIndex].Position =
                                          ( new Vector2(x * Constants.Maps.TileWidth, y * Constants.Maps.TileHeight)
                                           - new Vector2(screenWidth / 2, screenHeight / 2) 
                                           + new Vector2(Constants.Maps.TileWidth / 2, Constants.Maps.TileHeight / 2)).ToSimVector();
                                    
                              
                                    humanIndex++;
                                    
                                break;
                            case 2: this.pools.Add(BodyFactory.CreateCircle(this.gameWorld, 0.9f, 1f, 1f));
                                this.pools[poolIndex].BodyType = BodyType.Dynamic;
                                this.pools[poolIndex].Position =
                                        (new Vector2(x * Constants.Maps.TileWidth, y * Constants.Maps.TileHeight)
                                           - new Vector2(screenWidth / 2, screenHeight / 2)
                                           + new Vector2(Constants.Maps.TileWidth / 2, Constants.Maps.TileHeight / 2)).ToSimVector();
                                poolIndex++;
                                break;
                           
                         case 3: this.traps.Add(BodyFactory.CreateCircle(this.gameWorld, 0.9f, 1f, 1f));
                                this.traps[trapIndex].BodyType = BodyType.Dynamic;
                                this.traps[trapIndex].Position =
                                       (new Vector2(x * Constants.Maps.TileWidth, y * Constants.Maps.TileHeight)
                                           - new Vector2(screenWidth / 2, screenHeight / 2)
                                           + new Vector2(Constants.Maps.TileWidth / 2, Constants.Maps.TileHeight / 2)).ToSimVector();
                                trapIndex++;
                                break;



                              
                        }
                }
            }

            float width = 8f;
            float height = 5f;

            walls = new List<Body>();
            walls.Add(BodyFactory.CreateEdge(this.gameWorld, new Vector2(0f, 0f), new Vector2(width, 0f)));
            walls.Add(BodyFactory.CreateEdge(this.gameWorld, new Vector2(0f, 0f), new Vector2(0f, height)));
            walls.Add(BodyFactory.CreateEdge(this.gameWorld, new Vector2(width, height), new Vector2(width, 0f)));
            walls.Add(BodyFactory.CreateEdge(this.gameWorld, new Vector2(width, height), new Vector2(0f, height)));

            //Vertices aBorders = new Vertices(4);
            //aBorders.Add(new Vector2(0, 0));
            //aBorders.Add(new Vector2(0, height));
            //aBorders.Add(new Vector2(width, height));
            //aBorders.Add(new Vector2(width, 0));

            //this.bodies.Add(BodyFactory.CreateLoopShape(this.gameWorld, aBorders));
            //this.bodies[Constants.GameObjects.Wall].CollisionCategories = Category.All;
            //this.bodies[Constants.GameObjects.Wall].CollidesWith = Category.All;

            // Load textures.
            this.sprites = new List<Texture2D>();

            this.sprites.Add(this.contentManager.Load<Texture2D>("shark"));
            this.sprites.Add(this.contentManager.Load<Texture2D>("people"));
            this.sprites.Add(this.contentManager.Load<Texture2D>("pool"));
            this.sprites.Add(this.contentManager.Load<Texture2D>("trap"));
            this.sprites.Add(this.contentManager.Load<Texture2D>("wall"));

            
            
            //Create fixtures.
            this.fixtures = new List<Fixture>();
            this.fixtures.Add(FixtureFactory.AttachCircle(0.3f, 1f, this.bodies[Constants.GameObjects.Shark]));
            this.fixtures.Add(FixtureFactory.AttachCircle(0.5f, 1f, this.humans[humanIndex]));
            this.fixtures[Constants.GameObjects.Human].OnCollision += HumanEaten;
            //this.fixtures.Add(FixtureFactory.AttachCircle(0.5f, 1f, this.bodies[Constants.GameObjects.Pool]));
            //this.fixtures[Constants.GameObjects.Pool].OnCollision += TimerReplenished;

            //this.fixtures.Add(FixtureFactory.AttachCircle(0.5f, 1f, this.bodies[Constants.GameObjects.Trap]));
            //this.fixtures[Constants.GameObjects.Trap].OnCollision += SharkDead;    


            //this.fixtures.Add(FixtureFactory.AttachLoopShape(aBorders, this.bodies[Constants.GameObjects.Wall]));
            //this.fixtures[Constants.GameObjects.Wall].CollisionCategories = Category.All;
            //this.fixtures[Constants.GameObjects.Wall].CollidesWith = Category.All;
            //this.fixtures[Constants.GameObjects.Wall].Restitution = 1f;
            //this.fixtures[Constants.GameObjects.Wall].Friction = 0f;
            //this.fixtures[Constants.GameObjects.Wall].OnCollision += WallHit;

            this.runFrameWidth = this.sprites[Constants.GameObjects.Shark].Width / 16;
            this.runFrameHeight = this.sprites[Constants.GameObjects.Shark].Height;

            // Create a default camera object centered on the shark.
            this.camera = new Camera(this.bodies[Constants.GameObjects.Shark].Position.ToRealVector());

            // Start the timer.
            this.timer.Start();

            base.OnNavigatedTo(e);
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
            if (fixtureB.CollisionCategories == Category.Cat1 && this.timeSinceTrapSound > 1f)
            {
                Debug.WriteLine("Shark dead");
                this.gameOver();
                this.contentManager.Load<SoundEffect>("trapsound").Play();
                this.timeSinceTrapSound = 0f;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles game over screen display.
        /// </summary>
        private void gameOver()
        {
            this.timer.Stop();
            this.spriteBatch.Begin();
            this.spriteBatch.DrawString(this.contentManager.Load<SpriteFont>("DisplayFont"), "You failed\nmiserably!", new Vector2(2f, 2f).ToRealVector(), Color.Red);
            this.spriteBatch.DrawString(this.contentManager.Load<SpriteFont>("DisplayFont"), "Points: ", new Vector2(2f, 2f).ToRealVector(), Color.Red);
            this.spriteBatch.End();

            // TODO: Delay for three seconds?

            // Check high-scores - if the score for this level was high enough, induct player to the high scores list.
            Dictionary<string, int> highScores;
            IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;
            try
            {

                highScores = userSettings["High scores"] as Dictionary<string, int>;
            }
            catch (KeyNotFoundException exception)
            {
                Debug.WriteLine("High scores data not found; " + exception.Message);
                highScores = new Dictionary<string, int>();
            }

            int lowestScore = 0;

            if (highScores.Count > 0)
            {
                lowestScore = highScores.Aggregate(highScores.First(), (min, curr) => curr.Value < min.Value ? curr : min).Value;
            }

            if (this.points > lowestScore)
            {
                // Induct the player to high scores list. If more than 8 elements, remove the last element.
                string currentDate = DateTime.Now.ToString("YY-MM-DD HH:mm");
                highScores.Add(currentDate, this.points);
                highScores = highScores.OrderByDescending(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                if (highScores.Count > 8)
                {
                    highScores.Remove(highScores.Aggregate(highScores.First(), (min, curr) => curr.Value < min.Value ? curr : min).Key);
                }

                // TODO: make this highlighting working by finding the index of added entry...
                // might be impossible since dictionary indices are undefined. :<
                int position = 3;

                // This shark was a hero indeed. Go to the high scores page.
                NavigationService.Navigate(new Uri("/ScorePage.xaml?highlight=" + position, UriKind.Relative));
            }

            // Shark is not a cat, has only one life, so a dead one means a dead one. Go to the main menu screen.
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
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
            if (fixtureB.CollisionCategories == Category.Cat1 && this.timeSinceEatingSound > 1f)
            {
                Debug.WriteLine("Human eaten");
                this.points += 5;
                this.contentManager.Load<SoundEffect>("eating").Play();
                this.timeSinceEatingSound = 0f;
                return true;
            }

            return false;
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
            if (fixtureB.CollisionCategories == Category.Cat1 && this.timeSinceWaterSound > 2f)
            {
                Debug.WriteLine("Timer replenished");
                this.timeLeft += TimeSpan.FromSeconds(2.0);
                this.contentManager.Load<SoundEffect>("water").Play();
                this.timeSinceWaterSound = 0f;
                return true;
            }

            return false;
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
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Information passed to the event.</param>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            float elapsed = (float)e.ElapsedTime.TotalSeconds;

            // Set the refresh rate to 30 FPS (step one thirtieth of a second in time).
            this.gameWorld.Step(Math.Min(elapsed, 1f / 30f));

            this.timeLeft -= e.ElapsedTime;
            if (this.timeLeft < TimeSpan.FromSeconds(0.0))
            {
                this.gameOver();
            }

            this.currentPlayerAnimationDelay += elapsed;
            this.timeSinceEatingSound += elapsed;
            this.timeSinceTrapSound += elapsed;
            this.timeSinceWaterSound += elapsed;

            if (Math.Abs(this.sharkVelocity.X) > 0.07f || Math.Abs(this.sharkVelocity.Y) > 0.07f)
            {
                while (this.currentPlayerAnimationDelay > this.playerAnimationDelay)
                {
                    ++this.playerCurrentFrame;
                    this.playerCurrentFrame = this.playerCurrentFrame % 16;
                    this.currentPlayerAnimationDelay -= this.playerAnimationDelay;
                }

                // Measure the angle at which the shark is moving forward.
                this.sharkRotation = this.centralVector.AngleBetween(this.centralVector + this.sharkVelocity) % (MathHelper.Pi * 2f);

                // Generate shark's "footprints".
                this.particleEngine.Generate();
            }

            // Generate shark's "footprints".
            this.particleEngine.EmitterAngle = 0f;
            this.particleEngine.EmitterVelocity = new Vector2(-this.sharkVelocity.X, -this.sharkVelocity.Y);
            this.particleEngine.EmitterLocation = this.bodies[Constants.GameObjects.Shark].Position;
            this.particleEngine.Update();

            this.centralVector.X += this.sharkVelocity.X * Constants.Speeds.BlueSharkSpeedMultiplier;
            this.centralVector.Y += this.sharkVelocity.Y * Constants.Speeds.BlueSharkSpeedMultiplier;
            this.sharkVelocity *= elapsed;

            this.bodies[Constants.GameObjects.Shark].ApplyForce(this.sharkVelocity);
            this.bodies[Constants.GameObjects.Shark].Position = this.centralVector * elapsed;


            //update camera position
            camera.position = this.bodies[Constants.GameObjects.Shark].Position.ToRealVector();
            //clamp camera movement only on the map size
            camera.position.X = MathHelper.Clamp(camera.position.X,
                      0,
                     MapWidthInPixels - ScreenWidth);
            camera.position.Y = MathHelper.Clamp(camera.position.Y,
                      0,
                        MapHeightInPixels - ScreenHeight);
        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Information passed to the event.</param>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Adjust origin vector according to the current shark's position.
            Vector2 origin = new Vector2(
                (this.sprites[Constants.GameObjects.Shark].Width / 16) / 2,
                this.sprites[Constants.GameObjects.Shark].Height / 2);

            // Draw the map.
            this.DrawMap();

            this.spriteBatch.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        null,
                        null,
                        null,
                        null,
                        camera.get_transformation());

            // Draw any existing particles.
            this.particleEngine.Draw(this.spriteBatch);

            Microsoft.Xna.Framework.Point cameraPoint = Conversions.ToCell(camera.position);
            Microsoft.Xna.Framework.Point viewPoint = Conversions.ToCell(camera.position + ViewPortVector());
            Microsoft.Xna.Framework.Point min = new Microsoft.Xna.Framework.Point();
            Microsoft.Xna.Framework.Point max = new Microsoft.Xna.Framework.Point();
            min.X = cameraPoint.X;
            min.Y = cameraPoint.Y;
            max.X = (int)Math.Min(viewPoint.X, Constants.Maps.Map.GetLength(1));
            max.Y = (int)Math.Min(viewPoint.Y, Constants.Maps.Map.GetLength(0));

            for (int y = min.Y; y < max.Y; y++)
            {
                for (int x = min.X; x < max.X; x++)

                {
                   

                           switch (Constants.Maps.ObjectMap[y, x])
                        {
                            case 1:  // Draw humans
                                foreach (Body h in humans)
                                {
                                    this.spriteBatch.Draw(
                                     this.sprites[Constants.Maps.ObjectMap[y, x]],
                                   h.Position.ToRealVector(),
                                    null,
                                    Color.White,
                                    h.Rotation,
                                    new Vector2(
                                    this.sprites[Constants.Maps.ObjectMap[y, x]].Width / 2f,
                                    this.sprites[Constants.Maps.ObjectMap[y, x]].Height / 2f),
                                     1f,
                                     SpriteEffects.None,
                                    1f);
                                }
                                
                                break;
                            case 2: foreach (Body p in pools)
                                {
                                    this.spriteBatch.Draw(
                                     this.sprites[Constants.Maps.ObjectMap[y, x]],
                                   p.Position.ToRealVector(),
                                    null,
                                    Color.White,
                                    p.Rotation,
                                    new Vector2(
                                    this.sprites[Constants.Maps.ObjectMap[y, x]].Width / 2f,
                                    this.sprites[Constants.Maps.ObjectMap[y, x]].Height / 2f),
                                     1f,
                                     SpriteEffects.None,
                                    1f);
                                }
                                break;
                                case 3: foreach (Body t in traps)
                                {
                                    this.spriteBatch.Draw(
                                     this.sprites[Constants.Maps.ObjectMap[y, x]],
                                   t.Position.ToRealVector(),
                                    null,
                                    Color.White,
                                    t.Rotation,
                                    new Vector2(
                                    this.sprites[Constants.Maps.ObjectMap[y, x]].Width / 2f,
                                    this.sprites[Constants.Maps.ObjectMap[y, x]].Height / 2f),
                                     1f,
                                     SpriteEffects.None,
                                    1f);
                                }
                                break;
                                             

                        }

                        
                                   
                
                       
                    }
                }


            //// Draw the human sprite.
            //this.spriteBatch.Draw(
            //    this.sprites[Constants.GameObjects.Human],
            //    this.bodies[Constants.GameObjects.Human].Position.ToRealVector(),
            //    null,
            //    Color.Yellow,
            //    this.bodies[Constants.GameObjects.Human].Rotation,
            //    new Vector2(
            //        this.sprites[Constants.GameObjects.Human].Width / 2f,
            //        this.sprites[Constants.GameObjects.Human].Height / 2f),
            //    1f,
            //    SpriteEffects.None,
            //    0f);

          

            // Draw the shark sprite.
            this.spriteBatch.Draw(
                this.sprites[Constants.GameObjects.Shark],
                this.bodies[Constants.GameObjects.Shark].Position.ToRealVector(),
                new Microsoft.Xna.Framework.Rectangle(this.playerCurrentFrame * this.runFrameWidth, 0, this.runFrameWidth, this.runFrameHeight),
                Color.White,
                this.sharkRotation,
                origin,
                1.5f,
                SpriteEffects.None,
                0.5f);
            this.spriteBatch.End();

            // Draw the HUD.
            this.DrawHud();

        }

        /// <summary>
        /// Draws the map of the game.
        /// </summary>
        private void DrawMap()
        {
            this.spriteBatch.Begin();
            Microsoft.Xna.Framework.Point cameraPoint = Conversions.ToCell(camera.position);
            Microsoft.Xna.Framework.Point viewPoint = Conversions.ToCell(camera.position + ViewPortVector());
            Microsoft.Xna.Framework.Point min = new Microsoft.Xna.Framework.Point();
            Microsoft.Xna.Framework.Point max = new Microsoft.Xna.Framework.Point();
            min.X = cameraPoint.X;
            min.Y = cameraPoint.Y;
            max.X = (int)Math.Min(viewPoint.X, Constants.Maps.Map.GetLength(1));
            max.Y = (int)Math.Min(viewPoint.Y, Constants.Maps.Map.GetLength(0));
            Rectangle tileRectangle = new Rectangle(
                    0,
                    0,
                    Constants.Maps.TileWidth,
                    Constants.Maps.TileHeight);

            for (int y = min.Y; y < max.Y; ++y)
            {
                for (int x = min.X; x < max.X; ++x)
                {
                    tileRectangle.X = x * Constants.Maps.TileWidth - (int)camera.position.X;
                    tileRectangle.Y = y * Constants.Maps.TileHeight - (int)camera.position.Y;
                    spriteBatch.Draw(tiles[Constants.Maps.Map[y, x]],
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
            this.spriteBatch.DrawString(this.contentManager.Load<SpriteFont>("DisplayFont"), this.timeLeft.ToString("ss"), new Vector2(0.2f, 0.2f).ToRealVector(), Color.Blue);
            this.spriteBatch.DrawString(this.contentManager.Load<SpriteFont>("DisplayFont"), this.points.ToString(), new Vector2(7.4f, 4.0f).ToRealVector(), Color.Yellow);
            this.spriteBatch.End();
        }

        /// <summary>
        /// Returns the viewport vector, representing the visible portion of the canvas (map).
        /// </summary>
        /// <returns>Viewport vector.</returns>
        private Vector2 ViewPortVector()
        {
            return new Vector2(
                GamePage.screenWidth + 2*Constants.Maps.TileWidth,
                GamePage.screenHeight + 2*Constants.Maps.TileHeight);
        }

        /// <summary>
        /// Raised when the accelerometer readings are subject to change.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Detailed state connected with the event.</param>
        private void Accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            // Call UpdateUI on the UI thread and pass the AccelerometerReading event's data.
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() => this.UpdateUI(e.SensorReading));
        }

        /// <summary>
        /// Handles UI updating.
        /// </summary>
        /// <param name="accelerometerReading">New accelerometer data.</param>
        private void UpdateUI(AccelerometerReading accelerometerReading)
        {
            // Set the minimum speed limit (sensitivity) to a sane value,
            // so that the shark will not "drift".
            if (Math.Abs(accelerometerReading.Acceleration.Y) > Constants.Speeds.BlueSharkSpeedMin)
            {
                this.sharkVelocity.X = -accelerometerReading.Acceleration.Y;
            }

            if (Math.Abs(accelerometerReading.Acceleration.X) > Constants.Speeds.BlueSharkSpeedMin)
            {
                this.sharkVelocity.Y = -accelerometerReading.Acceleration.X;
            }

            // Limit the velocity.
            this.sharkVelocity = Vector2.Clamp(this.sharkVelocity, -Constants.Speeds.BlueSharkVelocityMax, Constants.Speeds.BlueSharkVelocityMax);
        }
    }
}