using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;

public class TextureManager
{

    private int[] digitTextures = new int[10]; 
    public int LoadTexture(string path)
    {
        try
        {
            Bitmap bitmap = new Bitmap(path);
            int textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

           
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);

            bitmap.Dispose();
            return textureId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading texture {path}: {ex.Message}");
            return -1; 
        }
    }

    public void RenderTexture(int textureId, float width, float height)
    {
        GL.BindTexture(TextureTarget.Texture2D, textureId);
        GL.Begin(PrimitiveType.Quads);

        GL.TexCoord2(0.0, 1.0); GL.Vertex2(0, height);   
        GL.TexCoord2(1.0, 1.0); GL.Vertex2(width, height); 
        GL.TexCoord2(1.0, 0.0); GL.Vertex2(width, 0);      
        GL.TexCoord2(0.0, 0.0); GL.Vertex2(0, 0);         
        GL.End();      
    }


    public void LoadDigitTextures()
    {
        for (int i = 0; i < 10; i++)
        {
            digitTextures[i] = LoadTexture($"texture/image/digit/digit_{i}.png"); 
        }
    }
    public int GetDigitTexture(int digit)
    {
        if (digit < 0 || digit > 9)
            throw new ArgumentOutOfRangeException(nameof(digit), "Digit must be between 0 and 9.");
        return digitTextures[digit];
    }
}
