namespace SharkGame
{
    using System;
    using System.Collections.Generic;
    using FarseerPhysics.Dynamics;
    using FarseerPhysics.Factories;
    using Microsoft.Devices.Sensors;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Enumerates game objects for use with collection of objects
    /// such as bodies, textures and so on.
    /// </summary>
    public enum Bodies
    {
        /// <summary>
        /// 
        /// </summary>
        topWall,

        /// <summary>
        /// 
        /// </summary>
        blackSharkBody,

        /// <summary>
        /// 
        /// </summary>
        mapSurface
    }

    /// <summary>
    /// Represents basic graphics device initialization, game logic and rendering code object.
    /// </summary>
    public class GameCore : Microsoft.Xna.Framework.Game
    {
        /* Fields */

        /// <summary>
        /// Device's screen width in pixels.
        /// </summary>
        private static int screenWidth;

        /// <summary>
        /// Device's screen height in pixels.
        /// </summary>
        private static int screenHeight;

        /// <summary>
        /// Given map's width in pixels.
        /// </summary>
        private static int mapWidth;

        /// <summary>
        /// Given maps' height in pixels.
        /// </summary>
        private static int mapHeight;

        /// <summary>
        /// Represents how many meters does one pixel represent.
        /// </summary>
        private static float realToVirtualRatio = 100f;

        /// <summary>
        /// Representation of the physical accelerometer.
        /// </summary>
        private Accelerometer _motion;

        /// <summary>
        /// Game camera; at fixed height over the board, movable in different
        /// directions, always following the main character.
        /// </summary>
        private Camera camera;

        /// <summary>
        /// 
        /// </summary>
        GraphicsDeviceManager graphics;

        /// <summary>
        /// Particle generator.
        /// </summary>
        private ParticleEngine particleEngine;

        /// <summary>
        /// Textures for the particle generator.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Collection of game bodies.
        /// </summary>
        private List<Body> bodies;

        /// <summary>
        /// Collection of game textures.
        /// </summary>
        // private List<Texture2D> textures;

        /// <summary>
        /// Map bounding box, imposing map size limits on player's movement.
        /// </summary>
        private Body topWall;

        /// <summary>
        /// Black shark.
        /// </summary>
        private Body blackSharkBody;

        /// <summary>
        /// Surface of the map.
        /// </summary>
        private Body mapSurface;

        /// <summary>
        /// Game world that holds all objects and manages all the physics and collisions.
        /// </summary>
        private World gameWorld;

        // Scrolling map.

        /// <summary>
        /// Collection of sprites for the map.
        /// </summary>
        private List<Texture2D> tiles = new List<Texture2D>();

        // int tileMapWidth;
        // int tileMapHeight;

        // Textures

        /// <summary>
        /// 
        /// </summary>
        private Texture2D lander;

        /// <summary>
        /// 
        /// </summary>
        private Texture2D blueSharkSprite;

        /// <summary>
        /// 
        /// </summary>
        private Texture2D rocket;

        /// <summary>
        /// 
        /// </summary>
        private Texture2D blackSharkSprite;

        /// <summary>
        /// 
        /// </summary>
        private int playerCurrentFrame = 0;

        /// <summary>
        /// 
        /// </summary>
        private int runFrameWidth = 0;

        /// <summary>
        /// 
        /// </summary>
        private int runFrameHeight = 0;

        /// <summary>
        /// 
        /// </summary>
        private float playerAnimationDelay = .1f;

        /// <summary>
        /// 
        /// </summary>
        private float currentPlayerAnimationDelay = 0f;

        /// <summary>
        /// 
        /// </summary>
        private float rotationAngle = 0f;

        // Entities

        /// <summary>
        /// Blue shark's velocity.
        /// </summary>
        private Vector2 blueSharkVelocity = Vector2.Zero;

        /// <summary>
        /// Vector with its initial point set to screen center.
        /// </summary>
        private Vector2 centralVector = new Vector2(screenHeight / 2f, screenWidth / 2f);

        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="GameCore" /> class.
        /// </summary>
        public GameCore()
        {
            this.camera = new Camera();
            this.graphics = new GraphicsDeviceManager(this)
                {
                    PreferredBackBufferWidth = 800,
                    PreferredBackBufferHeight = 480,
                    IsFullScreen = true,
                    SupportedOrientations = DisplayOrientation.LandscapeLeft
                };

            this.Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            this.TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            this.InactiveSleepTime = TimeSpan.FromSeconds(1.0);
        }

        /* Properties */
        
        /// <summary>
        /// Gets device's screen width.
        /// </summary>
        public static int ScreenWidth
        {
            get { return GameCore.screenWidth; }
        }

        /// <summary>
        /// Gets device's screen height.
        /// </summary>
        public static int ScreenHeight
        {
            get { return GameCore.screenHeight; }
        }

        /// <summary>
        /// Gets given map's width in pixels.
        /// </summary>
        public static int MapWidth
        {
            get { return GameCore.mapWidth; }
        }

        /// <summary>
        /// Gets given maps' height in pixels.
        /// </summary>
        public static int MapHeight
        {
            get { return GameCore.mapHeight; }
        }

        /* Methods */

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Set up accelerometer handling.
            this._motion = new Accelerometer();
            this._motion.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(Accelerometer_CurrentValueChanged);
            this._motion.Start();

            // Perform initialization of the remaining components.
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Begin playing the game's soundtrack.
            // Song song = Content.Load<Song>("game_s");
            // MediaPlayer.Play(song);
            // MediaPlayer.IsRepeating = true;

            if (this.gameWorld == null)
            {
                this.gameWorld = new World(Vector2.Zero);
            }
            else
            {
                this.gameWorld.Clear();
            }

            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            // Load particles.
            List<Texture2D> textures = new List<Texture2D>();

            // textures.Add(Content.Load<Texture2D>("circle"));
            textures.Add(this.Content.Load<Texture2D>("a"));

            this.particleEngine = new ParticleEngine(textures, new Vector2(400f, 240f));

            this.tiles.Add(Content.Load<Texture2D>("sand"));
            this.tiles.Add(Content.Load<Texture2D>("sand2"));
            this.tiles.Add(Content.Load<Texture2D>("sand3"));
            this.tiles.Add(Content.Load<Texture2D>("sand4"));
            this.tiles.Add(Content.Load<Texture2D>("sand5"));

            GameCore.screenWidth = GraphicsDevice.Viewport.Width;
            GameCore.screenHeight = GraphicsDevice.Viewport.Height;

            // Load textures
            this.lander = this.Content.Load<Texture2D>("fish");
            this.blackSharkSprite = this.Content.Load<Texture2D>("fish");
            this.blueSharkSprite = this.Content.Load<Texture2D>("runfish");

            this.runFrameWidth = this.blueSharkSprite.Width / 16;
            this.runFrameHeight = this.blueSharkSprite.Height;

            // bounding box
            // Initialize xTile map resources

            this.mapSurface = BodyFactory.CreateCircle(this.gameWorld, 0.4f, 1.0f);
            this.mapSurface.BodyType = BodyType.Dynamic;
            this.mapSurface.Position = this.centralVector;

            this.topWall = BodyFactory.CreateRectangle(this.gameWorld, 0.5f, 1f, 2.0f);
            this.topWall.BodyType = BodyType.Static;
            this.topWall.Position = new Vector2(3f, 2f);

            this.blackSharkBody = BodyFactory.CreateCircle(this.gameWorld, 0.4f, 1.0f);
            this.blackSharkBody.BodyType = BodyType.Dynamic;
            this.blackSharkBody.Position = new Vector2(5f, 3f);

            this.rocket = this.Content.Load<Texture2D>("rocket");

            // this._lander = new GameObject(_landerTexture, new Vector2(300, 200));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            this.Content.Unload();
        }

        /// <summary>
        /// Handles UI updating.
        /// </summary>
        /// <param name="accelerometerReading">New accelerometer data.</param>
        private void UpdateUI(AccelerometerReading accelerometerReading)
        {
            // Set the minimum speed limit (sensitivity) to a sane value,
            // so that the blue shark will not "drift".
            if (Math.Abs(accelerometerReading.Acceleration.Y) > Constants.Speeds.BlueSharkSpeedMin)
            {
                this.blueSharkVelocity.X = -accelerometerReading.Acceleration.Y;
            }

            if (Math.Abs(accelerometerReading.Acceleration.X) > Constants.Speeds.BlueSharkSpeedMin)
            {
                this.blueSharkVelocity.Y = -accelerometerReading.Acceleration.X;
            }
            
            // Limit the velocity.
            this.blueSharkVelocity = Vector2.Clamp(this.blueSharkVelocity, -Constants.Speeds.BallVelocityLimit, Constants.Speeds.BallVelocityLimit);
        }

        /// <summary>
        /// Raised when the accelerometer readings are subject to change.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Detailed state connected with the event.</param>
        // public void Accelerometer_CurrentValueChanged(object sender, AccelerometerReadingEventArgs e)
        public void Accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            // Call UpdateUI on the UI thread and pass the AccelerometerReading event's data.
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() => this.UpdateUI(e.SensorReading));
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Checks whether the Return (Back) button has been pressed;
            // handles both WP7 button and XBox gamepad.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Set the refresh rate to 30 FPS (step one thirtieth of a second in time).
            this.gameWorld.Step(Math.Min(elapsed, 1f / 30f));
            // TODO: Add your update logic here

            // TODO: Add your game logic here.

            this.currentPlayerAnimationDelay += elapsed;

            if (Math.Abs(this.blueSharkVelocity.X) > 0.07f || Math.Abs(this.blueSharkVelocity.Y) > 0.07f)
            {
                while (this.currentPlayerAnimationDelay > this.playerAnimationDelay)
                {
                    ++this.playerCurrentFrame;
                    this.playerCurrentFrame = this.playerCurrentFrame % 16;
                    this.currentPlayerAnimationDelay -= this.playerAnimationDelay;
                }
                // Measure the angle at which the blue shark is moving forward.
                this.rotationAngle = this.AngleF(this.centralVector, this.centralVector + this.blueSharkVelocity) % (MathHelper.Pi * 2f);

                // Generate blue shark's "footsteps".
                this.particleEngine.EmitterAngle = 0f;
                this.particleEngine.EmitterVelocity = new Vector2(-this.blueSharkVelocity.X, -this.blueSharkVelocity.Y);
                this.particleEngine.EmitterLocation = new Vector2(this.mapSurface.Position.X, this.mapSurface.Position.Y);
                this.particleEngine.Update();
            }

            this.centralVector.X += this.blueSharkVelocity.X * Constants.Speeds.Speed;
            this.centralVector.Y += this.blueSharkVelocity.Y * Constants.Speeds.Speed;
            this.blueSharkVelocity *= elapsed;

            // Accept touch
            // TouchCollection touches = TouchPanel.GetState();
            // foreach (TouchLocation touch in touches)
            // {
            //     if (touch.State == TouchLocationState.Pressed)
            //     {
            //         this._lander.Position = touch.Position;
            //         this._lander.Velocity = Vector2.Zero;
            //     }
            // }

            // Update particles
            int xMin = this.lander.Width / 2;
            int xMax = this.graphics.GraphicsDevice.Viewport.Width - (this.lander.Width / 2);
            int yMin = this.lander.Height / 2;
            int yMax = this.graphics.GraphicsDevice.Viewport.Height - (this.lander.Height / 2);

            this.mapSurface.ApplyForce(this.blueSharkVelocity);
            this.mapSurface.Position = this.centralVector * elapsed;

            this.camera.Position = ToDisplayUnits(this.mapSurface.Position);

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws all game objects. This function is called every time
        /// drawing is supposed to happen, in a timely manner.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Adjust origin vector according to the current blue shark's position.
            Vector2 origin = new Vector2((this.blueSharkSprite.Width / 16) / 2, this.blueSharkSprite.Height / 2);

            // Draw the map.
            this.DrawMap();

            // Sprites drawing begins here (performance-awareness).
            this.spriteBatch.Begin();

            // Draw any existing particles.
            this.particleEngine.Draw(this.spriteBatch);

            // Draw the rocket's sprite.
            this.spriteBatch.Draw(
                this.rocket,
                ToDisplayUnits(this.topWall.Position),
                null,
                Color.White,
                this.topWall.Rotation,
                new Vector2(this.rocket.Width / 2f, this.rocket.Height / 2f),
                1f,
                SpriteEffects.None,
                0f);
            
            // Draw the black shark's sprite.
            this.spriteBatch.Draw(
                this.blackSharkSprite,
                ToDisplayUnits(this.blackSharkBody.Position),
                null,
                Color.Red,
                this.blackSharkBody.Rotation,
                new Vector2(this.blackSharkSprite.Width / 2f, this.blackSharkSprite.Height / 2f),
                1f,
                SpriteEffects.None,
                0f);
            
            // Draw the blue shark's sprite.
            this.spriteBatch.Draw(
                this.blueSharkSprite,
                ToDisplayUnits(this.mapSurface.Position),
                new Microsoft.Xna.Framework.Rectangle(this.playerCurrentFrame * this.runFrameWidth, 0, this.runFrameWidth, this.runFrameHeight),
                Color.White,
                this.rotationAngle,
                origin,
                1f,
                SpriteEffects.None,
                0f);

            // Sprites drawing ends here.
            this.spriteBatch.End();

            // Continue with drawing what was the default.
            base.Draw(gameTime);
        }

        /// <summary>
        /// Returns an angle between two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Angle between the two vectors.</returns>
        private float AngleF(Vector2 a, Vector2 b)
        {
            float angle = (float)Math.Atan2((double)(a.Y - b.Y), (double)(a.X - b.X)) - MathHelper.ToRadians(90);
            if (angle < 0)
            {
                angle += MathHelper.ToRadians(360);
            }

            return angle;
        }

        /// <summary>
        /// Draws the map of the game.
        /// </summary>
        private void DrawMap()
        {
            Point cameraPoint = this.VectorToCell(this.camera.Position);
            Point viewPoint = this.VectorToCell(this.camera.Position + this.ViewPortVector());

            Point min = new Point(cameraPoint.X, cameraPoint.Y);
            Point max = new Point(
                (int)Math.Min(viewPoint.X, Constants.Maps.Map.GetLength(1)),
                (int)Math.Min(viewPoint.Y, Constants.Maps.Map.GetLength(0)));

            Microsoft.Xna.Framework.Rectangle tileRectangle =
                new Microsoft.Xna.Framework.Rectangle(
                    0,
                    0,
                    Constants.TileWidth,
                    Constants.TileHeight);

            this.spriteBatch.Begin();
            for (int y = min.Y; y < max.Y; y++)
            {
                for (int x = min.X; x < max.X; x++)
                {
                    tileRectangle.X = (x * Constants.TileWidth) - (int)this.camera.Position.X;
                    tileRectangle.Y = (y * Constants.TileHeight) - (int)this.camera.Position.Y;
                    this.spriteBatch.Draw(
                        this.tiles[Constants.Maps.Map[y, x]],
                        tileRectangle,
                        Color.White);
                }
            }

            this.spriteBatch.End();
        }

        /// <summary>
        /// Returns vector's initial point.
        /// </summary>
        /// <param name="vector">Vector.</param>
        /// <returns>Vector's initial point.</returns>
        private Point VectorToCell(Vector2 vector)
        {
            return new Point(
                (int)(vector.X / Constants.TileWidth),
                (int)(vector.Y / Constants.TileHeight));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Vector2 ViewPortVector()
        {
            return new Vector2(
                GameCore.screenWidth + Constants.TileWidth,
                GameCore.screenHeight + Constants.TileHeight);
        }

        /// <summary>
        /// Returns a vector with display units as values coming from the vector with simulated units as values.
        /// </summary>
        /// <param name="simUnits">Vector with simulated units as values.</param>
        /// <returns>Vector with display units as values.</returns>
        public static Vector2 ToDisplayUnits(Vector2 simUnits)
        {
            return simUnits * GameCore.realToVirtualRatio;
        }
    }
}