namespace SharkGame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a two-dimensional camera for the Shark Game.
    /// </summary>
    public class Camera
    {
        /* Fields */

        /// <summary>
        /// Camera's position.
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// Camera's transformation matrix.
        /// </summary>
        private Matrix transformationMatrix;

        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera" /> class
        /// and places it at the given position.
        /// </summary>
        /// <param name="position">Initial camera position.</param>
        public Camera(Vector2 position)
        {
            this.position = position;
        }

        /* Properties */

        /// <summary>
        /// Gets or sets the camera's position.
        /// Clamping is performed when calling the setter.
        /// </summary>
        public Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /* Methods */

        /// <summary>
        /// Auxiliary function to move the camera.
        /// </summary>
        /// <param name="delta">Delta for the movement.</param>
        public void Move(Vector2 delta)
        {
            this.position += delta;
        }

        /// <summary>
        /// Gets the camera's transformation matrix.
        /// </summary>
        /// <returns>Transformation matrix for this camera.</returns>
        public Matrix GetTransformationMatrix()
        {
            // Thanks to o KB o for this solution.
            this.transformationMatrix = Matrix.CreateTranslation(
                new Vector3(-this.position.X, -this.position.Y, 0f))
                * Matrix.CreateTranslation(new Vector3(800f * 0.5f, 480f * 0.5f, 0f));
            return this.transformationMatrix;
        }
    }
}