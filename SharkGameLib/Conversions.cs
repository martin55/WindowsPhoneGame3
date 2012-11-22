namespace SharkGame
{
    using System;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Utility class for conversions and arithmetic.
    /// </summary>
    public static class Conversions
    {
        /// <summary>
        /// Returns an angle between two vectors.
        /// </summary>
        /// <param name="a">First vector.</param>
        /// <param name="b">Second vector.</param>
        /// <returns>Angle between the two vectors.</returns>
        public static float AngleBetween(this Vector2 a, Vector2 b)
        {
            float angle = (float)Math.Atan2((double)(a.Y - b.Y), (double)(a.X - b.X)) - MathHelper.ToRadians(90);
            if (angle < 0f)
            {
                angle += MathHelper.ToRadians(360);
            }

            return angle;
        }

        /// <summary>
        /// Returns a vector with display units as values coming from the vector with simulated units as values.
        /// </summary>
        /// <param name="virtualVector">Vector with simulated units as values.</param>
        /// <returns>Vector with display units as values.</returns>
        public static Vector2 ToRealVector(this Vector2 virtualVector)
        {
            return virtualVector * Constants.RealToVirtualRatio;
        }

        /// <summary>
        /// Returns a vector with simulated units as values coming from the vector with display units as values.
        /// </summary>
        /// <param name="realVector">Vector with display units as values.</param>
        /// <returns>Vector with simulated units as values.</returns>
        public static Vector2 ToSimVector(this Vector2 realVector)
        {
            return realVector / Constants.RealToVirtualRatio;
        }

        /// <summary>
        /// Returns vector's initial point. (?)
        /// </summary>
        /// <param name="vector">Two-dimensional vector.</param>
        /// <returns>Vector's initial point.</returns>
        public static Point ToCell(this Vector2 vector)
        {
            return new Point(
                (int)(vector.X / Constants.Maps.TileWidth),
                (int)(vector.Y / Constants.Maps.TileHeight));
        }
    }
}
