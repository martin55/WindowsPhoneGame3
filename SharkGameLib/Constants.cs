namespace SharkGame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Holds all game-related constants and static values.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Represents how many meters does one pixel represent.
        /// </summary>
        public static float RealToVirtualRatio { get { return 100f; } }

        /// <summary>
        /// Game objects' indices for both bodies and textures collection
        /// </summary>
        public static class GameObjects
        {
            /// <summary>
            /// Blue Shark index.
            /// </summary>
            public static int BlueShark { get { return 0; } }

            /// <summary>
            /// Black Shark index. 
            /// </summary>
            public static int BlackShark { get { return 1; } }

            /// <summary>
            /// Rocket index.
            /// </summary>
            public static int Rocket { get { return 2; } }
        }

        /// <summary>
        /// Holds all "hardcoded" game maps.
        /// </summary>
        /// <remarks>
        /// Legend:
        /// 0 - sand
        /// 1 - bushes facing bottom
        /// 2 - fish starting point
        /// 3 - bushes facing right
        /// </remarks>
        public static class Maps
        {
            /// <summary>
            /// Gets given map's width in pixels.
            /// </summary>
            public static int MapWidth { get { return 0; } }

            /// <summary>
            /// Gets given maps' height in pixels.
            /// </summary>
            public static int MapHeight { get { return 0; } }

            /// <summary>
            /// Single in-game tile width.
            /// </summary>
            public static int TileWidth { get { return 128; } }

            /// <summary>
            /// Single in-game tile height.
            /// </summary>
            public static int TileHeight { get { return 128; } }

            /// <summary>
            /// First predefined map.
            /// </summary>
            public static int[,] Map
            {
                get
                {
                    return new[,]
                    {
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 2, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                    };
                }
            }
        }

        /// <summary>
        /// Holds all movement and speed-related constants.
        /// </summary>
        public static class Speeds
        {
            /// <summary>
            /// Minimal value for the camera movement speed.
            /// </summary>
            public static float CameraMinSpeed { get { return 0.5f; } }

            /// <summary>
            /// Maximal value for the camera movement speed.
            /// </summary>
            public static float CameraMaxSpeed { get { return 50f; } }

            /// <summary>
            /// Camera movement speed.
            /// </summary>
            public static float CameraSpeed { get { return 4f; } }

            /// <summary>
            /// Shark's speed multiplier.
            /// </summary>
            public static float BlueSharkSpeedMultiplier { get { return 5f; } }

            /// <summary>
            /// Minimal speed for the blue shark to be registered as movement
            /// (sensitivity setting for the accelerometer sensor).
            /// </summary>
            public static float BlueSharkSpeedMin { get { return 0.1f; } }

            /// <summary>
            /// Maximal speed for the blue shark.
            /// </summary>
            public static Vector2 BlueSharkVelocityMax { get { return new Vector2(3, 3); } }
        }
    }
}
