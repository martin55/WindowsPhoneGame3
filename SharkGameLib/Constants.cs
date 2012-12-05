namespace SharkGame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Holds all game-related constants and static values.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Number of meters represented by one pixel on the device screen
        /// (Farseer Physics use KMS (kilos, meters, seconds) system.
        /// </summary>
        public static float RealToVirtualRatio { get { return 100f; } }

        /// <summary>
        /// Width of the device screen in pixels. According to Microsoft documentation,
        /// devices supporting Windows Phone 7 need to have their screens at least 800 pixels tall.
        /// </summary>
        public static int ScreenWidth { get { return 800; } }

        /// <summary>
        /// Height of the device screen in pixels. According to Microsoft documentation,
        /// devices supporting Windows Phone 7 need to have their screens at least 480 pixels wide.
        /// </summary>
        public static int ScreenHeight { get { return 480; } }

        /// <summary>
        /// Game objects' indices for both bodies and textures collection
        /// </summary>
        public static class GameObjects
        {
            /// <summary>
            /// Shark index in collections.
            /// </summary>
            public static int Shark { get { return 0; } }

            /// <summary>
            /// Human index in collections.
            /// </summary>
            public static int Human { get { return 1; } }

            /// <summary>
            /// Pool index in collections.
            /// </summary>
            public static int Pool { get { return 2; } }

            /// <summary>
            /// Trap index in collections.
            /// </summary>
            public static int Trap { get { return 3; } }

            /// <summary>
            /// Wall index in collections.
            /// </summary>
            public static int Wall { get { return 4; } }
        }

        /// <summary>
        /// Holds all "hardcoded" game maps.
        /// </summary>
        public static class Maps
        {
            /// <summary>
            /// Gets given map's width in pixels.
            /// </summary>
            public static int MapWidth { get { return TileSize * Constants.Maps.TileWidth; } }

            /// <summary>
            /// Gets given maps' height in pixels.
            /// </summary>
            public static int MapHeight { get { return TileSize * Constants.Maps.TileHeight; } }

            /// <summary>
            /// Gets the size of a single tile (both a width and a height, as tiles are squares) in pixels.
            /// </summary>
            public static int TileSize { get { return 100; } }

            /// <summary>
            /// Gets the number of tiles in a row (tile width of the map).
            /// </summary>
            public static int TileWidth { get { return Constants.Maps.Map.GetLength(1); } }

            /// <summary>
            /// Gets the number of tiles in a column (tile height of the map).
            /// </summary>
            public static int TileHeight { get { return Constants.Maps.Map.GetLength(0); } }

            /// <summary>
            /// Tile map for use in game.
            /// </summary>
            /// <remarks>
            /// Legend:
            /// 0 - sand
            /// 1 - shallow water
            /// 2 - sea water
            /// </remarks>
            public static int[,] Map
            {
                get
                {
                    return new[,]
                    {
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },                      
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 },
                        { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 },
                        { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 },
                        { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 }
                    };
                }
            }

            /// <summary>
            /// Objects' map for the game's level.
            /// </summary>
            /// <remarks>
            /// Legend:
            /// 0 - nothing
            /// 1 - human
            /// 2 - pool of water
            /// 3 - trap
            /// 4 - crate
            /// </remarks>
            public static int[,] ObjectMap
            {
                get
                {
                    return new[,]
                    {
                        { 4, 0, 4, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 4, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                        { 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 },
                        { 4, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },                      
                        { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 3, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 1, 4, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
                        { 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0 },
                        { 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
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
            /// Shark's speed multiplier.
            /// </summary>
            public static float SharkSpeedMultiplier { get { return 5f; } }

            /// <summary>
            /// Minimal speed for the blue shark to be registered as movement
            /// (sensitivity setting for the accelerometer sensor).
            /// </summary>
            public static float SharkSpeedMin { get { return 0.1f; } }

            /// <summary>
            /// Maximal speed for the blue shark.
            /// </summary>
            public static Vector2 SharkVelocityMax { get { return new Vector2(3, 3); } }
        }
    }
}
