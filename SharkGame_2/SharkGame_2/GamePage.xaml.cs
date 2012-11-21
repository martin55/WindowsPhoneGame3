using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Devices.Sensors;
using SharkGame_2;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Factories;
using Microsoft.Devices;



namespace SharkGame_2
{
    public partial class GamePage : PhoneApplicationPage
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
        public Vector2 blueSharkVelocity = Vector2.Zero;

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

        private float playerAnimationDelay = .1f;

        private float currentPlayerAnimationDelay = 0f;

        private float rotationAngle = 0f;

        ContentManager contentManager;
        GameTimer timer;


        /// <summary>
        /// Representation of the physical accelerometer sensor.
        /// </summary>
        private Accelerometer accelerometer;
        SpriteBatch spriteBatch;
        bool backPressed;
        public GamePage()
        {
            


            this.camera = new Camera();
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);



            // Set up accelerometer handling.
            this.accelerometer = new Accelerometer();
            this.accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(this.Accelerometer_CurrentValueChanged);
            this.accelerometer.Start();

            this.BackKeyPress += new EventHandler<System.ComponentModel.CancelEventArgs>(GamePage_BackKeyPress);

            contentManager = (Application.Current as App).Content;
            contentManager.RootDirectory = "Content";

            InitializeComponent();



            // Create a timer for this page
            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromTicks(333333);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;
        }


        /* Properties */

        /// <summary>
        /// Gets device's screen width.
        /// </summary>
        public static int ScreenWidth
        {
            get { return screenWidth; }
        }

        /// <summary>
        /// Gets device's screen height.
        /// </summary>
        public static int ScreenHeight
        {
            get { return screenHeight; }
        }

        void GamePage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            backPressed = true;
            e.Cancel = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>







        protected override void OnNavigatedTo(NavigationEventArgs e)
        {



            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            backPressed = false;



            // Start the timer
            timer.Start();

            // Begin playing the game's soundtrack.
            // Song song = Content.Load<Song>("game_s");
            // MediaPlayer.Play(song);
            // MediaPlayer.IsRepeating = true;

            if (this.gameWorld == null)
            {
                this.gameWorld = new World(Microsoft.Xna.Framework.Vector2.Zero);
            }
            else
            {
                this.gameWorld.Clear();
            }




            // Load particles.
            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(contentManager.Load<Texture2D>("shadow"));

            // Initializes a new instance of the particle emitter with textures.
            this.particleEngine = new ParticleEmitter(textures, new Vector2(400f, 240f));

            // Load tiles.
            this.tiles = new List<Texture2D>();

            this.tiles.Add(contentManager.Load<Texture2D>("sand"));
            this.tiles.Add(contentManager.Load<Texture2D>("sand"));
            this.tiles.Add(contentManager.Load<Texture2D>("sand"));
            this.tiles.Add(contentManager.Load<Texture2D>("sand"));
            this.tiles.Add(contentManager.Load<Texture2D>("sand"));

            // Load bodies.
            this.bodies = new List<Body>();

            this.bodies.Add(BodyFactory.CreateCircle(this.gameWorld, 0.6f, 1f));
            this.bodies[Constants.GameObjects.BlueShark].BodyType = BodyType.Dynamic;
            this.bodies[Constants.GameObjects.BlueShark].Position = this.centralVector;

            this.bodies.Add(BodyFactory.CreateRectangle(this.gameWorld, 0.9f, 0.9f, 1f));
            this.bodies[Constants.GameObjects.BlackShark].BodyType = BodyType.Dynamic;
            this.bodies[Constants.GameObjects.BlackShark].Position = new Vector2(5f, 3f);

            this.bodies.Add(BodyFactory.CreateCircle(this.gameWorld, 0.9f, 1f, 1f));
            this.bodies[Constants.GameObjects.Rocket].BodyType = BodyType.Static;
            this.bodies[Constants.GameObjects.Rocket].Position = new Vector2(3f, 2f);

            // Load textures.
            this.sprites = new List<Texture2D>();

            this.sprites.Add(contentManager.Load<Texture2D>("shark"));
            this.sprites.Add(contentManager.Load<Texture2D>("people"));
            this.sprites.Add(contentManager.Load<Texture2D>("pool"));

            this.runFrameWidth = this.sprites[Constants.GameObjects.BlueShark].Width / 16;
            this.runFrameHeight = this.sprites[Constants.GameObjects.BlueShark].Height;


            // TODO: use this.content to load your game content here

            // Start the timer
            timer.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {


            float elapsed = (float)e.ElapsedTime.Seconds;

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

            this.camera.Position = this.bodies[Constants.GameObjects.BlueShark].Position.ToRealVector();




        }



        /// <summary>
        /// Draws all game objects. This function is called every time
        /// drawing is supposed to happen, in a timely manner.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            SharedGraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Adjust origin vector according to the current blue shark's position.
            Vector2 origin = new Vector2(
                (this.sprites[Constants.GameObjects.BlueShark].Width / 16) / 2,
                this.sprites[Constants.GameObjects.BlueShark].Height / 2);

            // Draw the map.
            this.DrawMap();

            this.spriteBatch.Begin();

            // Draw any existing particles.
            this.particleEngine.Draw(this.spriteBatch);



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
                Color.White,
                this.bodies[Constants.GameObjects.BlackShark].Rotation,
                new Vector2(
                    this.sprites[Constants.GameObjects.BlackShark].Width / 2f,
                    this.sprites[Constants.GameObjects.BlackShark].Height / 2f),
                0.7f,
                SpriteEffects.None,
                0f);



            this.spriteBatch.End();


        }

        /// <summary>
        /// Draws the map of the game.
        /// </summary>
        private void DrawMap()
        {
            // Get the upper-left and lower-right points of the visible area.
            Microsoft.Xna.Framework.Point cameraPoint = this.camera.Position.ToCell();
            Microsoft.Xna.Framework.Point viewPoint = (this.camera.Position + this.ViewPortVector()).ToCell();

            // Get the upper-left and lower-right points of the visible part of the map.
            Microsoft.Xna.Framework.Point min = new Microsoft.Xna.Framework.Point(cameraPoint.X, cameraPoint.Y);
            Microsoft.Xna.Framework.Point max = new Microsoft.Xna.Framework.Point(
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
        /// Returns the viewport vector, representing the visible portion of the canvas (map).
        /// </summary>
        /// <returns>Viewport vector.</returns>
        private Vector2 ViewPortVector()
        {
            return new Vector2(
                screenWidth + Constants.Maps.TileWidth,
                screenHeight + Constants.Maps.TileHeight);
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
                blueSharkVelocity.X = -accelerometerReading.Acceleration.Y;
            }

            if (Math.Abs(accelerometerReading.Acceleration.X) > Constants.Speeds.BlueSharkSpeedMin)
            {
                blueSharkVelocity.Y = -accelerometerReading.Acceleration.X;
            }

            // Limit the velocity.
            blueSharkVelocity = Vector2.Clamp(blueSharkVelocity, -Constants.Speeds.BlueSharkVelocityMax, Constants.Speeds.BlueSharkVelocityMax);
        }
    }
}