namespace SharkGame
{
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a two-dimensional camera for the Shark Game.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// Camera's position.
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// Camera's transformation matrix.
        /// </summary>
        public Matrix _transform;

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera" /> class.
        /// </summary>
        /// <param name="pos"></param>
        public Camera(Vector2 pos)
        {
            this.position = pos;
        }

        /// <summary>
        /// Auxiliary function to move the camera.
        /// </summary>
        /// <param name="amount"></param>
        public void Move(Vector2 amount)
        {
            position += amount;
        }

        /// <summary>
        /// Gets the camera's position.
        /// </summary>
        public Vector2 Pos
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <summary>
        /// Gets the camera's transformation matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix get_transformation()
        {
            // Thanks to o KB o for this solution.
            this._transform = Matrix.CreateTranslation(
                new Vector3(-this.position.X, -this.position.Y, 0f))
                * Matrix.CreateTranslation(new Vector3(800f * 0.5f, 480f * 0.5f, 0f));
            return this._transform;
        }
    }
}