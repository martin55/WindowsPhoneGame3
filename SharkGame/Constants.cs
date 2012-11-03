namespace SharkGame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Holds all game-related constants and static values.
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Single in-game tile width.
        /// </summary>
        public static int TileWidth { get { return 128; } }

        /// <summary>
        /// Single in-game tile height.
        /// </summary>
        public static int TileHeight { get { return 128; } }

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
        public class Maps
        {
            /// <summary>
            /// First predefined map.
            /// </summary>
            public static int[,] Map
            {
                get
                {
                    return new[,]
                    {
                        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 3, 0, 2, 0, 0, 0, 0, 0, 0, 0 },
                        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 3, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                    };
                }
            }
        }

        /// <summary>
        /// Holds all movement and speed-related constants.
        /// </summary>
        public class Speeds
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

            // TODO: check the things below.

            /// <summary>
            /// Speed. (?)
            /// </summary>
            public static float Speed { get { return 5f; } }

            /// <summary>
            /// Speed limit. (?)
            /// </summary>
            public static float BlueSharkSpeedMin { get { return 0.1f; } }

            /// <summary>
            /// Maximal ball velocity.
            /// </summary>
            public static Vector2 BallVelocityLimit { get { return new Vector2(3, 3); } }
        }
    }
}
