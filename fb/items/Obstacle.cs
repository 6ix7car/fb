using OpenTK.Graphics.OpenGL;
using System;

public class Obstacle
{
    public float X { get; private set; }
    public float TopY { get; private set; }
    public float BottomY { get; private set; }
    public bool Passed { get; set; }

    private const float GAP = 200; 
    private int upperPipeTexture;
    private int lowerPipeTexture;

    public Obstacle(float x, int upperPipeTex, int lowerPipeTex)
    {
        X = x;
        
        TopY = new Random().Next(100, 400);
        BottomY = new Random().Next(100, 400) + GAP; 
        Passed = false;
        upperPipeTexture = upperPipeTex;
        lowerPipeTexture = lowerPipeTex;

    }

    public void Update()
    {
        X -= 5; 
    }

    public void Reset()
    {
        X = 800; 
        float height = new Random().Next(100, 400);
        TopY = height;
        BottomY = height + GAP;
    }

    public void Render()
    {
        GL.BindTexture(TextureTarget.Texture2D, upperPipeTexture);
        GL.Begin(PrimitiveType.Quads);

        
        GL.TexCoord2(0.0, 1.0); GL.Vertex2(X - 20, 0); 
        GL.TexCoord2(1.0, 1.0); GL.Vertex2(X + 20, 0); 
        GL.TexCoord2(1.0, 0.0); GL.Vertex2(X + 20, TopY); 
        GL.TexCoord2(0.0, 0.0); GL.Vertex2(X - 20, TopY); 
        GL.End();

        GL.BindTexture(TextureTarget.Texture2D, lowerPipeTexture);
        GL.Begin(PrimitiveType.Quads);

        
        GL.TexCoord2(0.0, 1.0); GL.Vertex2(X - 20, BottomY); 
        GL.TexCoord2(1.0, 1.0); GL.Vertex2(X + 20, BottomY); 
        GL.TexCoord2(1.0, 0.0); GL.Vertex2(X + 20, 600); 
        GL.TexCoord2(0.0, 0.0); GL.Vertex2(X - 20, 600); 
        GL.End();
    }

    public bool CollidesWith(Bird bird)
    {
        return (bird.X + bird.Size > X - 20 && bird.X - bird.Size < X + 20 &&
                (bird.Y - bird.Size < TopY || bird.Y + bird.Size > BottomY));
    }
}
