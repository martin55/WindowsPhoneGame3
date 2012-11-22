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
    /// Represents basic graphics device initialization, game logic and rendering code object.
    /// </summary>
    public class GamePage : Microsoft.Xna.Framework.Game
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
        /// Representation of the physical accelerometer sensor.
        /// </summary>
        private Accelerometer accelerometer;

        /// <summary>
        /// Game camera; at fixed height over the board, movable in different
        /// directions, always following the main character.
        /// </summary>
        private Camera camera;

        /// <summary>
        /// Object for handling and configuration of the graphics device.
        /// </summary>
        private GraphicsDeviceManager graphics;

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
        /// Vector with its initial point set to screen center.
        /// </summary>
        private Vector2 centralVector = new Vector2(screenHeight / 2f, screenWidth / 2f);

        /// <summary>
        /// Blue shark's velocity.
        /// </summary>
        private Vector2 blueSharkVelocity = Vector2.Zero;

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
        /// 
        /// </summary>
        private float rotationAngle = 0f;

        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePage" /> class.
        /// </summary>
        public GamePage()
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

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Set up accelerometer handling.
            this.accelerometer = new Accelerometer();
            this.accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(this.Accelerometer_CurrentValueChanged);
            this.accelerometer.Start();

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

            // Get the display size.
            GamePage.screenWidth = GraphicsDevice.Viewport.Width;
            GamePage.screenHeight = GraphicsDevice.Viewport.Height;

            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            // Load particles.
            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(this.Content.Load<Texture2D>("a"));

            // Initializes a new instance of the particle emitter with textures.
            this.particleEngine = new ParticleEmitter(textures, new Vector2(400f, 240f));

            // Load tiles.
            this.tiles = new List<Texture2D>();

            this.tiles.Add(Content.Load<Texture2D>("sand"));
            this.tiles.Add(Content.Load<Texture2D>("sand2"));
            this.tiles.Add(Content.Load<Texture2D>("sand3"));
            this.tiles.Add(Content.Load<Texture2D>("sand4"));
            this.tiles.Add(Content.Load<Texture2D>("sand5"));

            // Load bodies.
            this.bodies = new List<Body>();

            this.bodies.Add(BodyFactory.CreateCircle(this.gameWorld, 0.4f, 1f));
            this.bodies[Constants.GameObjects.BlueShark].BodyType = BodyType.Dynamic;
            this.bodies[Constants.GameObjects.BlueShark].Position = this.centralVector;

            this.bodies.Add(BodyFactory.CreateCircle(this.gameWorld, 0.4f, 1f));
            this.bodies[Constants.GameObjects.BlackShark].BodyType = BodyType.Dynamic;
            this.bodies[Constants.GameObjects.BlackShark].Position = new Vector2(5f, 3f);

            this.bodies.Add(BodyFactory.CreateRectangle(this.gameWorld, 0.5f, 1f, 2f));
            this.bodies[Constants.GameObjects.Rocket].BodyType = BodyType.Static;
            this.bodies[Constants.GameObjects.Rocket].Position = new Vector2(3f, 2f);

            // Load textures.
            this.sprites = new List<Texture2D>();

            this.sprites.Add(this.Content.Load<Texture2D>("runfish"));
            this.sprites.Add(this.Content.Load<Texture2D>("fish"));
            this.sprites.Add(this.Content.Load<Texture2D>("rocket"));

            this.runFrameWidth = this.sprites[Constants.GameObjects.BlueShark].Width / 16;
            this.runFrameHeight = this.sprites[Constants.GameObjects.BlueShark].Height;
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
                this.rotationAngle = this.centralVector.AngleBetween(this.centralVector + this.blueSharkVelocity) % (MathHelper.Pi * 2f);

                // Generate blue shark's "footsteps".
                this.particleEngine.EmitterAngle = 0f;
                this.particleEngine.EmitterVelocity = new Vector2(-this.blueSharkVelocity.X, -this.blueSharkVelocity.Y);
                this.particleEngine.EmitterLocation = this.bodies[Constants.GameObjects.BlueShark].Position;
                this.particleEngine.Update();
            }

            this.centralVector.X += this.blueSharkVelocity.X * Constants.Speeds.Speed;
            this.centralVector.Y += this.blueSharkVelocity.Y * Constants.Speeds.Speed;
            this.blueSharkVelocity *= elapsed;

            this.bodies[Constants.GameObjects.BlueShark].ApplyForce(this.blueSharkVelocity);
            this.bodies[Constants.GameObjects.BlueShark].Position = this.centralVector * elapsed;

            // Adjust camera position so that it follows the blue shark.
            this.camera.Position = this.bodies[Constants.GameObjects.BlueShark].Position.ToRealVector();

            // Continue with what was the default action.
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
            Vector2 origin = new Vector2(
                (this.sprites[Constants.GameObjects.BlueShark].Width / 16) / 2,
                this.sprites[Constants.GameObjects.BlueShark].Height / 2);

            // Draw the map.
            this.DrawMap();

            this.spriteBatch.Begin();

            // Draw any existing particles.
            this.particleEngine.Draw(this.spriteBatch);

            // Draw the blue shark's sprite.
            this.spriteBatch.Draw(
                this.sprites[Constants.GameObjects.BlueShark],
                this.bodies[Constants.GameObjects.BlueShark].Position.ToRealVector(),
                new Microsoft.Xna.Framework.Rectangle(this.playerCurrentFrame * this.runFrameWidth, 0, this.runFrameWidth, this.runFrameHeight),
                Color.White,
                this.rotationAngle,
                origin,
                1f,
                SpriteEffects.None,
                0f);

            // Draw the black shark's sprite.
            this.spriteBatch.Draw(
                this.sprites[Constants.GameObjects.BlackShark],
                this.bodies[Constants.GameObjects.BlackShark].Position.ToRealVector(),
                null,
                Color.Red,
                this.bodies[Constants.GameObjects.BlackShark].Rotation,
                new Vector2(
                    this.sprites[Constants.GameObjects.BlackShark].Width / 2f,
                    this.sprites[Constants.GameObjects.BlackShark].Height / 2f),
                1f,
                SpriteEffects.None,
                0f);

            // Draw the rocket's sprite.
            this.spriteBatch.Draw(
                this.sprites[Constants.GameObjects.Rocket],
                this.bodies[Constants.GameObjects.Rocket].Position.ToRealVector(),
                null,
                Color.White,
                this.bodies[Constants.GameObjects.Rocket].Rotation,
                new Vector2(
                    this.sprites[Constants.GameObjects.Rocket].Width / 2f,
                    this.sprites[Constants.GameObjects.Rocket].Height / 2f),
                1f,
                SpriteEffects.None,
                0f);

            this.spriteBatch.End();

            // Continue with what was the default action.
            base.Draw(gameTime);
        }

        /// <summary>
        /// Draws the map of the game.
        /// </summary>
        private void DrawMap()
        {
            // Get the upper-left and lower-right points of the visible area.
            Point cameraPoint = this.camera.Position.ToCell();
            Point viewPoint = (this.camera.Position + this.ViewPortVector()).ToCell();

            // Get the upper-left and lower-right points of the visible part of the map.
            Point min = new Point(cameraPoint.X, cameraPoint.Y);
            Point max = new Point(
                (int)Math.Min(viewPoint.X, Constants.Maps.Map.GetLength(1)),
                (int)Math.Min(viewPoint.Y, Constants.Maps.Map.GetLength(0)));

            Microsoft.Xna.Framework.Rectangle tileRectangle =
                new Microsoft.Xna.Framework.Rectangle(
                    0,
                    0,
                    Constants.Maps.TileWidth,
                    Constants.Maps.TileHeight);

            this.spriteBatch.Begin();
            for (int y = min.Y; y < max.Y; ++y)
            {
                for (int x = min.X; x < max.X; ++x)
                {
                    tileRectangle.X = (x * Constants.Maps.TileWidth) - (int)this.camera.Position.X;
                    tileRectangle.Y = (y * Constants.Maps.TileHeight) - (int)this.camera.Position.Y;
                    this.spriteBatch.Draw(
                        this.tiles[Constants.Maps.Map[y, x]],
                        tileRectangle,
                        Color.White);
                }
            }

            this.spriteBatch.End();
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
            this.blueSharkVelocity = Vector2.Clamp(this.blueSharkVelocity, -Constants.Speeds.BlueSharkVelocityMax, Constants.Speeds.BlueSharkVelocityMax);
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
    }
}