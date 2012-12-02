using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;



namespace SharkGame
{
    public class Camera
    {
        public Vector2 position;
        public Matrix _transform; // Matrix Transform

public Camera(Vector2 pos)
        {
            this.position = pos;
            
        }
// Auxiliary function to move the camera
public void Move(Vector2 amount)
{
    position += amount;
}
// Get set position
public Vector2 Pos
{
    get { return position; }
    set { position = value; }
}

public Matrix get_transformation()
{
    _transform =       // Thanks to o KB o for this solution
      Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
                                 Matrix.CreateTranslation(new Vector3(800 * 0.5f, 480 * 0.5f, 0));
    return _transform;
}


    }
}