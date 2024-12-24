using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

public class Bird
{
    public float X { get; private set; } = 100;
    public float Y { get; private set; } = 300;
    private float velocity; 
    public float Size { get; private set; } = 30; 

    public void Jump(float force)
    {
        velocity = -force;
    }

    public void Update(float gravity)
    {
        velocity += gravity;
        Y += velocity;

        if (Y < 0)
            Y = 0;
        if (Y > 600)
            Y = 600; 
    }
    public bool IsDead(List<Obstacle> obstacles)
    {
        foreach (var obstacle in obstacles)
        {
            if (obstacle.CollidesWith(this))
            {
                return true;
            }
        }
        return false;
    }
  
    public void Reset()
    {
        X = 100; 
        Y = 300; 
        velocity = 0; 
    }

    public void Render(int birdTexture)
    {
        GL.BindTexture(TextureTarget.Texture2D, birdTexture);

        GL.Begin(PrimitiveType.Quads);
        GL.TexCoord2(0, 0);
        GL.Vertex2(X - Size, Y - Size);

        GL.TexCoord2(1, 0);
        GL.Vertex2(X + Size, Y - Size);

        GL.TexCoord2(1, 1);
        GL.Vertex2(X + Size, Y + Size);

        GL.TexCoord2(0, 1);
        GL.Vertex2(X - Size, Y + Size);

        GL.End();
    }
}
