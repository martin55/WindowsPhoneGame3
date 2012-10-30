namespace FishGame
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
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // bounding box
        Body topWall;
        Body rectangle3;
        Body _lander;
        World world;

        // scrolling map
        Camera camera = new Camera();

        List<Texture2D> tiles = new List<Texture2D>();

        static int tileWidth = 128;
        static int tileHeight = 128;

        int tileMapWidth;
        int tileMapHeight;

        static int screenWidth;
        static int screenHeight;

        static int mapWidthInPixels;
        static int mapHeightInPixels;

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
        /// Gets map width in pixels.
        /// </summary>
        public static int MapWidthInPixels
        {
            get { return mapWidthInPixels; }
        }

        /// <summary>
        /// Gets map height in pixels.
        /// </summary>
        public static int MapHeightInPixels
        {
            get { return mapHeightInPixels; }
        }

        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        ParticleEngine particleEngine;
        // Textures
        Texture2D _landerTexture;
        Texture2D fishRun;
        Texture2D wallTexture;
        Texture2D rectangleSprite;


        int[,] map = {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1,},
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0,},
            {3, 0, 2, 0, 0, 0, 0, 0, 0, 0 },
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0,},
        };

        int playerCurrentFrame = 0;
        int runFrameWidth = 0;
        int runFrameHeight = 0;
        double playerAnimationDelay = .1;
        double currentPlayerAnimationDelay = 0;
        private float RotationAngle = 0;

        // Entities
        Vector2 vel = new Vector2();
        Vector2 pos = new Vector2(screenHeight / 2f, screenWidth / 2f);
        Accelerometer _motion;
        Vector2 accelReading = new Vector2();
        private Vector2 _ballMaxVelocity = new Vector2(3, 3);
        private const float Threshold = 0.1f;
        private float speed = 5f;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game1" /> class.
        /// </summary>
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 480;
            _graphics.IsFullScreen = true;

            _graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
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

            if (world == null)
            {
                world = new World(new Vector2(0, 0));
            }
            else
            {
                world.Clear();
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

            mapWidthInPixels = tileMapWidth * tileWidth;
            mapHeightInPixels = tileMapHeight * tileHeight;

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


            _lander = BodyFactory.CreateCircle(world, 0.4f, 1.0f);
            _lander.BodyType = BodyType.Dynamic;
            _lander.Position = pos;

            wallTexture = Content.Load<Texture2D>("rocket");
            topWall = BodyFactory.CreateRectangle(world, 0.5f, 1f, 2.0f);
            topWall.BodyType = BodyType.Static;
            topWall.Position = new Vector2(3f, 2);

            rectangle3 = BodyFactory.CreateCircle(world, 0.4f, 1.0f);
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

        //Method for Changed Readings
        public void AccelerometerReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            accelReading.X = -(float)e.X;
            accelReading.Y = -(float)e.Y;

            // update the ball's velocity with the accelerometer values
            if (Math.Abs(accelReading.Y) > Threshold)
            {
                vel.X = accelReading.Y;
            }

            if (Math.Abs(accelReading.X) > Threshold)
            {

                vel.Y = accelReading.X;
            }


            // limit the velocity to the maximum
            vel = Vector2.Clamp(vel, -_ballMaxVelocity, _ballMaxVelocity);

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
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
                RotationAngle = fAngleBetween(pos, pos + vel);
                float circle = MathHelper.Pi * 2;
                RotationAngle = RotationAngle % circle;

                particleEngine.EmitterAng = 0;
                particleEngine.EmitterVel = new Vector2(-vel.X, -vel.Y);
                particleEngine.EmitterLocation = new Vector2(_lander.Position.X, _lander.Position.Y);
                particleEngine.Update();
            }

            pos.X += vel.X * speed;
            pos.Y += vel.Y * speed;
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
            int MaxX = _graphics.GraphicsDevice.Viewport.Width - _landerTexture.Width / 2;
            int MinX = _landerTexture.Width / 2;
            int MaxY = _graphics.GraphicsDevice.Viewport.Height - _landerTexture.Height / 2;
            int MinY = _landerTexture.Height / 2;

            _lander.ApplyForce(vel);
            _lander.Position = pos * elapsed;

            camera.Position = ToDisplayUnits(_lander.Position);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        private Vector2 origin;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SandyBrown);

            origin.X = (fishRun.Width / 16) / 2;
            origin.Y = fishRun.Height / 2;
            DrawMap();
            // Draw sprites
            _spriteBatch.Begin();

            particleEngine.Draw(_spriteBatch);

            _spriteBatch.Draw(wallTexture, ToDisplayUnits(topWall.Position),
                                         null,
                                            Color.White, topWall.Rotation, new Vector2(wallTexture.Width / 2.0f, wallTexture.Height / 2.0f), 1f,
                                            SpriteEffects.None, 0f);

            _spriteBatch.Draw(rectangleSprite, ToDisplayUnits(rectangle3.Position),

                                            null,

                                            Color.Red, rectangle3.Rotation, new Vector2(rectangleSprite.Width / 2.0f, rectangleSprite.Height / 2.0f), 1f,

                                            SpriteEffects.None, 0f);
            _spriteBatch.Draw(fishRun, ToDisplayUnits(_lander.Position),
                     new Microsoft.Xna.Framework.Rectangle(playerCurrentFrame * runFrameWidth, 0, runFrameWidth, runFrameHeight),
                      Color.White, RotationAngle, origin, 1.0f, SpriteEffects.None, 0f);


            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vPointA"></param>
        /// <param name="vPointB"></param>
        /// <returns></returns>
        private float fAngleBetween(Vector2 vPointA, Vector2 vPointB)
        {
            float fAngle = (float)Math.Atan2((double)(vPointA.Y - vPointB.Y), (double)(vPointA.X - vPointB.X)) - MathHelper.ToRadians(90);
            if (fAngle < 0) fAngle += MathHelper.ToRadians(360);
            return fAngle;
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawMap()
        {
            Point cameraPoint = VectorToCell(camera.Position);
            Point viewPoint = VectorToCell(camera.Position +
                                    ViewPortVector());
            Point min = new Point();
            Point max = new Point();
            min.X = cameraPoint.X;
            min.Y = cameraPoint.Y;
            max.X = (int)Math.Min(viewPoint.X, map.GetLength(1));
            max.Y = (int)Math.Min(viewPoint.Y, map.GetLength(0));
            Microsoft.Xna.Framework.Rectangle tileRectangle = 
                new Microsoft.Xna.Framework.Rectangle(
                    0,
                    0,
                    tileWidth,
                    tileHeight);

            _spriteBatch.Begin();
            for (int y = min.Y; y < max.Y; y++)
            {
                for (int x = min.X; x < max.X; x++)
                {
                    tileRectangle.X = x * tileWidth - (int)camera.Position.X;
                    tileRectangle.Y = y * tileHeight - (int)camera.Position.Y;
                    _spriteBatch.Draw(tiles[map[y, x]],
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
                        (int)(vector.X / tileWidth),
                        (int)(vector.Y / tileHeight));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Vector2 ViewPortVector()
        {
            return new Vector2(
                    screenWidth + tileWidth,
                    screenHeight + tileHeight);
        }

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