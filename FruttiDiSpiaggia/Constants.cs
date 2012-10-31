namespace FruttiDiSpiaggia
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
        /// Predefined map.
        /// </summary>
        public static int[,] Map { get { return new[,]{
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {3, 0, 2, 0, 0, 0, 0, 0, 0, 0 },
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            {3, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        }; } }

        /// <summary>
        /// Speed.
        /// </summary>
        public static float Speed { get { return 5f; } }

        /// <summary>
        /// Speed limit.
        /// </summary>
        public static float VelocityLimit { get { return 0.1f; } }

        /// <summary>
        /// Maximal ball velocity.
        /// </summary>
        public static Vector2 BallVelocityLimit { get { return new Vector2(3, 3); } }
    }
}
