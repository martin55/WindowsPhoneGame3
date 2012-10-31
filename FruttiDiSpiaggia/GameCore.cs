namespace FruttiDiSpiaggia
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Devices.Sensors;
    using FarseerPhysics.Dynamics;
    using FarseerPhysics.Factories;

    /// <summary>
    /// This is the main type for the game.
    /// </summary>
    public class GameCore : Microsoft.Xna.Framework.Game
    {
        /* Fields */

        /// <summary>
        /// Device accelerometer state.
        /// </summary>
        Vector2 accelerometer;

        /// <summary>
        /// Textures for the particle generator.
        /// </summary>
        SpriteBatch _spriteBatch;

        /// <summary>
        /// Particle generator.
        /// </summary>
        ParticleEngine particleEngine;

        /// <summary>
        /// Game camera; at fixed height over the board, movable in different
        /// directions, always following the main character.
        /// </summary>
        private Camera camera;

        // Gameboard bounding box.
        /// <summary>
        /// 
        /// </summary>
        private Body topWall;

        /// <summary>
        /// 
        /// </summary>
        private Body rectangle3;

        /// <summary>
        /// 
        /// </summary>
        private Body _lander;

        /// <summary>
        /// Represents the game world, managing all physics.
        /// </summary>
        private World gameWorld;

        // Scrolling map.

        /// <summary>
        /// 
        /// </summary>
        private List<Texture2D> tiles = new List<Texture2D>();

        int tileMapWidth;
        int tileMapHeight;

        static int screenWidth;
        static int screenHeight;

        /// <summary>
        /// Gets screen width.
        /// </summary>
        public static int ScreenWidth
        {
            get { return screenWidth; }
        }

        /// <summary>
        /// Gets screen height.
        /// </summary>
        public static int ScreenHeight
        {
            get { return screenHeight; }
        }

        /// <summary>
        /// 
        /// </summary>
        GraphicsDeviceManager _graphics;
        
        // Textures

        /// <summary>
        /// 
        /// </summary>
        Texture2D _landerTexture;

        /// <summary>
        /// 
        /// </summary>
        Texture2D fishRun;

        /// <summary>
        /// 
        /// </summary>
        Texture2D wallTexture;

        /// <summary>
        /// 
        /// </summary>
        Texture2D rectangleSprite;

        /// <summary>
        /// 
        /// </summary>
        int playerCurrentFrame = 0;

        /// <summary>
        /// 
        /// </summary>
        int runFrameWidth = 0;

        /// <summary>
        /// 
        /// </summary>
        int runFrameHeight = 0;

        /// <summary>
        /// 
        /// </summary>
        double playerAnimationDelay = .1;

        /// <summary>
        /// 
        /// </summary>
        double currentPlayerAnimationDelay = 0;

        /// <summary>
        /// 
        /// </summary>
        private float RotationAngle = 0;

        // Entities

        /// <summary>
        /// 
        /// </summary>
        Vector2 vel = new Vector2();

        /// <summary>
        /// 
        /// </summary>
        Vector2 pos = new Vector2(screenHeight / 2f, screenWidth / 2f);

        /// <summary>
        /// 
        /// </summary>
        Accelerometer _motion;

        public static int MapWidthInPixels;
        public static int MapHeightInPixels;

        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="GameCore" /> class.
        /// </summary>
        public GameCore()
        {
            this.camera = new Camera();
            this.accelerometer = new Vector2();
            this._graphics = new GraphicsDeviceManager(this)
                {
                    PreferredBackBufferWidth = 800,
                    PreferredBackBufferHeight = 480,
                    IsFullScreen = true,
                    SupportedOrientations = DisplayOrientation.LandscapeLeft
                };

            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 
        protected override void Initialize()
        {
            // Setup entities
            _motion = new Accelerometer();
            _motion.ReadingChanged += new EventHandler<AccelerometerReadingEventArgs>(AccelerometerReadingChanged);
            _motion.Start();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Song song = Content.Load<Song>("game_s");  // Put the name of your song here instead of "song_title"
            //MediaPlayer.Play(song);
            //MediaPlayer.IsRepeating = true;

            if (gameWorld == null)
            {
                gameWorld = new World(new Vector2(0, 0));
            }
            else
            {
                gameWorld.Clear();
            }

            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //load particles
            List<Texture2D> textures = new List<Texture2D>();

            // textures.Add(Content.Load<Texture2D>("circle"));
            textures.Add(this.Content.Load<Texture2D>("a"));

            particleEngine = new ParticleEngine(textures, new Vector2(400, 240));

            tiles.Add(Content.Load<Texture2D>("sand"));
            tiles.Add(Content.Load<Texture2D>("sand2"));
            tiles.Add(Content.Load<Texture2D>("sand3"));
            tiles.Add(Content.Load<Texture2D>("sand4"));
            tiles.Add(Content.Load<Texture2D>("sand5"));

            screenWidth = GraphicsDevice.Viewport.Width;
            screenHeight = GraphicsDevice.Viewport.Height;

            // Load textures
            _landerTexture = Content.Load<Texture2D>("fish");
            rectangleSprite = Content.Load<Texture2D>("fish");
            fishRun = this.Content.Load<Texture2D>("runfish");

            runFrameWidth = fishRun.Width / 16;
            runFrameHeight = fishRun.Height;

            // bounding box
            //Initialize xTile map resources


            _lander = BodyFactory.CreateCircle(gameWorld, 0.4f, 1.0f);
            _lander.BodyType = BodyType.Dynamic;
            _lander.Position = pos;

            wallTexture = Content.Load<Texture2D>("rocket");
            topWall = BodyFactory.CreateRectangle(gameWorld, 0.5f, 1f, 2.0f);
            topWall.BodyType = BodyType.Static;
            topWall.Position = new Vector2(3f, 2);

            rectangle3 = BodyFactory.CreateCircle(gameWorld, 0.4f, 1.0f);
            rectangle3.BodyType = BodyType.Dynamic;
            rectangle3.Position = new Vector2(5.0f, 3);

            // _lander = new GameObject(_landerTexture, new Vector2(300, 200));
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
        /// Raised when the accelerometer readings are subject to change.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Detailed state connected with the event.</param>
        public void AccelerometerReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            this.accelerometer.X = -(float)e.X;
            this.accelerometer.Y = -(float)e.Y;

            // update the ball's velocity with the accelerometer values
            if (Math.Abs(this.accelerometer.Y) > Constants.VelocityLimit)
            {
                vel.X = this.accelerometer.Y;
            }

            if (Math.Abs(this.accelerometer.X) > Constants.VelocityLimit)
            {
                vel.Y = this.accelerometer.X;
            }

            // Limit the velocity.
            vel = Vector2.Clamp(vel, -Constants.BallVelocityLimit, Constants.BallVelocityLimit);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            gameWorld.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
            // TODO: Add your update logic here

            // TODO: Add your game logic here.

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            currentPlayerAnimationDelay += gameTime.ElapsedGameTime.TotalSeconds;

            if (Math.Abs(vel.X) > 0.07 || Math.Abs(vel.Y) > 0.07)
            {
                while (currentPlayerAnimationDelay > playerAnimationDelay)
                {
                    playerCurrentFrame++;
                    playerCurrentFrame = playerCurrentFrame % 16;
                    currentPlayerAnimationDelay -= playerAnimationDelay;
                }
                RotationAngle = AngleF(pos, pos + vel);
                float circle = MathHelper.Pi * 2;
                RotationAngle = RotationAngle % circle;

                particleEngine.EmitterAngle = 0;
                particleEngine.EmitterVelocity = new Vector2(-vel.X, -vel.Y);
                particleEngine.EmitterLocation = new Vector2(_lander.Position.X, _lander.Position.Y);
                particleEngine.Update();
            }

            pos.X += vel.X * Constants.Speed;
            pos.Y += vel.Y * Constants.Speed;
            vel *= elapsed;

            // Accept touch
            // TouchCollection touches = TouchPanel.GetState();
            // foreach (TouchLocation touch in touches)
            // {
            //     if (touch.State == TouchLocationState.Pressed)
            //     {
            //         _lander.Position = touch.Position;
            //         _lander.Velocity = new Vector2();
            //     }
            // }

            // Update particles
            int xMin = _landerTexture.Width / 2;
            int xMax = _graphics.GraphicsDevice.Viewport.Width - _landerTexture.Width / 2;
            int yMin = _landerTexture.Height / 2;
            int yMax = _graphics.GraphicsDevice.Viewport.Height - _landerTexture.Height / 2;

            _lander.ApplyForce(vel);
            _lander.Position = pos * elapsed;

            camera.Position = ToDisplayUnits(_lander.Position);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private Vector2 origin;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SandyBrown);

            origin.X = (fishRun.Width / 16) / 2;
            origin.Y = fishRun.Height / 2;

            DrawMap();

            // Draw sprites
            _spriteBatch.Begin();
            particleEngine.Draw(_spriteBatch);

            _spriteBatch.Draw(
                wallTexture,
                ToDisplayUnits(topWall.Position),
                null,
                Color.White,
                topWall.Rotation,
                new Vector2(wallTexture.Width / 2f, wallTexture.Height / 2f),
                1f,
                SpriteEffects.None,
                0f);

            _spriteBatch.Draw(
                rectangleSprite,
                ToDisplayUnits(rectangle3.Position),
                null,
                Color.Red,
                rectangle3.Rotation,
                new Vector2(rectangleSprite.Width / 2f, rectangleSprite.Height / 2f),
                1f,
                SpriteEffects.None,
                0f);

            _spriteBatch.Draw(
                fishRun,
                ToDisplayUnits(_lander.Position),
                new Microsoft.Xna.Framework.Rectangle(playerCurrentFrame * runFrameWidth, 0, runFrameWidth, runFrameHeight),
                Color.White,
                RotationAngle,
                origin,
                1f,
                SpriteEffects.None,
                0f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        private void DrawMap()
        {
            Point cameraPoint = VectorToCell(camera.Position);
            Point viewPoint = VectorToCell(camera.Position + ViewPortVector());
            Point min = new Point();
            Point max = new Point();
            min.X = cameraPoint.X;
            min.Y = cameraPoint.Y;
            max.X = (int)Math.Min(viewPoint.X, Constants.Map.GetLength(1));
            max.Y = (int)Math.Min(viewPoint.Y, Constants.Map.GetLength(0));
            Microsoft.Xna.Framework.Rectangle tileRectangle = 
                new Microsoft.Xna.Framework.Rectangle(
                    0,
                    0,
                    Constants.TileWidth,
                    Constants.TileHeight);

            _spriteBatch.Begin();
            for (int y = min.Y; y < max.Y; y++)
            {
                for (int x = min.X; x < max.X; x++)
                {
                    tileRectangle.X = x * Constants.TileWidth - (int)camera.Position.X;
                    tileRectangle.Y = y * Constants.TileHeight - (int)camera.Position.Y;
                    _spriteBatch.Draw(tiles[Constants.Map[y, x]],
                        tileRectangle,
                        Color.White);
                }
            }

            _spriteBatch.End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
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
                screenWidth + Constants.TileWidth,
                screenHeight + Constants.TileHeight);
        }

        /// <summary>
        /// 
        /// </summary>
        private static float _displayUnitsToSimUnitsRatio = 100f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="simUnits"></param>
        /// <returns></returns>
        public static Vector2 ToDisplayUnits(Vector2 simUnits)
        {
            return simUnits * _displayUnitsToSimUnitsRatio;
        }
    }
}