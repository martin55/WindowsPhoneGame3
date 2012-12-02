namespace SharkGame
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Navigation;
    using FarseerPhysics.Dynamics;
    using FarseerPhysics.Factories;
    using Microsoft.Devices.Sensors;
    using Microsoft.Phone.Controls;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Media;
    using FarseerPhysics.Dynamics.Contacts;
    using System.Diagnostics;
    
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
        private static int screenWidth = 800;

        /// <summary>
        /// Device's screen height in pixels.
        /// </summary>
        private static int screenHeight = 480;

        /// <summary>
        /// Representation of the physical accelerometer sensor.
        /// </summary>
        private Accelerometer accelerometer;

        /// <summary>
        /// Game camera; at fixed height over the board, movable in different
        /// directions, always following the main character.
        /// </summary>
        Camera camera ;
       

        /// <summary>
        /// Particle generator.
        /// </summary>
        private ParticleEmitter particleEngine;

        /// <summary>
        /// Game world that holds all objects and manages all the physics and collisions.
        /// </summary>
        private World gameWorld;

        /// <summary>
        /// Textures for the particle generator.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Collection of game bodies.
        /// </summary>
        private List<Body> bodies;

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





        bool backPressed;
        bool pausedGame = false;






        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePage" /> class.
        /// </summary>
        public GamePage()
        {
            this.InitializeComponent();

            // Create a default camera object.
            //
           
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Get the content manager from the application
            this.contentManager = (Application.Current as App).Content;

            // Create a timer for this page
            this.timer = new GameTimer();
            this.timer.UpdateInterval = TimeSpan.FromTicks(333333);
            this.timer.Update += this.OnUpdate;
            this.timer.Draw += this.OnDraw;

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
           // Song song = this.contentManager.Load<Song>("game_s");
            //MediaPlayer.Play(song);
            //MediaPlayer.IsRepeating = true;

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
            this.tiles.Add(this.contentManager.Load<Texture2D>("sand"));
            this.tiles.Add(this.contentManager.Load<Texture2D>("sand"));

            // Load bodies.
            this.bodies = new List<Body>();

            this.bodies.Add(BodyFactory.CreateCircle(this.gameWorld, 0.6f, 1f));
            this.bodies[Constants.GameObjects.Shark].BodyType = BodyType.Dynamic;
            this.bodies[Constants.GameObjects.Shark].Position = this.centralVector;

            this.bodies.Add(BodyFactory.CreateRectangle(this.gameWorld, 0.9f, 0.9f, 1f));
            this.bodies[Constants.GameObjects.Human].BodyType = BodyType.Dynamic;
            this.bodies[Constants.GameObjects.Human].Position = new Vector2(5f, 3f);

            this.bodies.Add(BodyFactory.CreateCircle(this.gameWorld, 0.9f, 1f, 1f));
            this.bodies[Constants.GameObjects.Pool].BodyType = BodyType.Dynamic;
            this.bodies[Constants.GameObjects.Pool].Position = new Vector2(3f, 2f);
            //this.bodies[Constants.GameObjects.Pool].

            this.bodies.Add(BodyFactory.CreateCircle(this.gameWorld, 0.9f, 1f, 1f));
            this.bodies[Constants.GameObjects.Trap].BodyType = BodyType.Dynamic;
            this.bodies[Constants.GameObjects.Trap].Position = new Vector2(7f, 5f);

            // Load textures.
            this.sprites = new List<Texture2D>();

            this.sprites.Add(this.contentManager.Load<Texture2D>("shark"));
            this.sprites.Add(this.contentManager.Load<Texture2D>("people"));
            this.sprites.Add(this.contentManager.Load<Texture2D>("pool"));
            this.sprites.Add(this.contentManager.Load<Texture2D>("trap"));
            this.sprites.Add(this.contentManager.Load<Texture2D>("wall"));

            // Create fixtures.
            this.fixtures = new List<Fixture>();

            this.fixtures.Add(FixtureFactory.AttachCircle(0.6f, 1f, this.bodies[Constants.GameObjects.Shark]));
            this.fixtures[Constants.GameObjects.Shark].CollisionCategories = Category.Cat1;

            this.fixtures.Add(FixtureFactory.AttachCircle(0.6f, 1f, this.bodies[Constants.GameObjects.Human]));
            this.fixtures[Constants.GameObjects.Human].CollisionCategories = Category.Cat2;
            this.fixtures[Constants.GameObjects.Human].OnCollision += HumanEaten;

            this.fixtures.Add(FixtureFactory.AttachCircle(1f, 10f, this.bodies[Constants.GameObjects.Pool]));
            this.fixtures[Constants.GameObjects.Pool].CollisionCategories = Category.Cat3;
            this.fixtures[Constants.GameObjects.Pool].OnCollision += TimerReplenished;

            this.fixtures.Add(FixtureFactory.AttachCircle(1f, 10f, this.bodies[Constants.GameObjects.Trap]));
            this.fixtures[Constants.GameObjects.Trap].CollisionCategories = Category.Cat4;
            this.fixtures[Constants.GameObjects.Trap].OnCollision += SharkDead;

            //FixtureFactory.AttachEdge(this.gameWorld.

            this.runFrameWidth = this.sprites[Constants.GameObjects.Shark].Width / 16;
            this.runFrameHeight = this.sprites[Constants.GameObjects.Shark].Height;

            camera = new Camera(this.bodies[Constants.GameObjects.Shark].Position.ToRealVector());
            // Start the timer.
            this.timer.Start();

            base.OnNavigatedTo(e);
        }

        private bool SharkDead(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.CollisionCategories == Category.Cat1)
            {
                Debug.WriteLine("Shark dead");
                return false;
            }

            return false;
        }

        private bool HumanEaten(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.CollisionCategories == Category.Cat1)
            {
                Debug.WriteLine("Human eaten");
                return false;
            }

            return false;
        }

        private bool TimerReplenished(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.CollisionCategories == Category.Cat1)
            {
                Debug.WriteLine("Timer replenished");
                return false;
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

            this.currentPlayerAnimationDelay += elapsed;

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


            
            //draw all objects in viewpoint
            Microsoft.Xna.Framework.Point cameraPoint = Conversions.ToCell(camera.position);
            Microsoft.Xna.Framework.Point viewPoint = Conversions.ToCell(camera.position+
                                    ViewPortVector());
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

                    if (Constants.Maps.ObjectMap[y, x] > 0)
                    {
                        this.spriteBatch.Draw(
                    this.sprites[Constants.Maps.ObjectMap[y, x]],
                    new Vector2(x * Constants.Maps.TileWidth - this.sprites[Constants.Maps.ObjectMap[y, x]].Width / 2f,
                                 y * Constants.Maps.TileHeight - this.sprites[Constants.Maps.ObjectMap[y, x]].Height / 2f),
                    null,
                    Color.White,
                    this.bodies[Constants.Maps.ObjectMap[y, x]].Rotation,
                    new Vector2(
                        this.sprites[Constants.Maps.ObjectMap[y, x]].Width / 2f,
                        this.sprites[Constants.Maps.ObjectMap[y, x]].Height / 2f),
                    0.8f,
                    SpriteEffects.None,
                    0f);
                    }
                }
            }
           

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
                0f);

            this.spriteBatch.End();
        }

        /// <summary>
        /// Draws the map of the game.
        /// </summary>
        private void DrawMap()
        {


            Microsoft.Xna.Framework.Point cameraPoint = Conversions.ToCell(camera.position);
            Microsoft.Xna.Framework.Point viewPoint = Conversions.ToCell(camera.position +
                                    ViewPortVector());
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
            
            this.spriteBatch.Begin();

            for (int y = min.Y; y < max.Y; y++)
            {
                for (int x = min.X; x < max.X; x++)
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
        /// Returns the viewport vector, representing the visible portion of the canvas (map).
        /// </summary>
        /// <returns>Viewport vector.</returns>
        private Vector2 ViewPortVector()
        {
            return new Vector2(
                GamePage.screenWidth + Constants.Maps.TileWidth,
                GamePage.screenHeight + Constants.Maps.TileHeight);
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