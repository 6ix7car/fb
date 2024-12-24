using fb.game;
using NAudio.Wave;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;

public class Game : GameWindow
{
    private Bird bird;
    private List<Obstacle> obstacles;
    private float gravity = 0.4f;
    private float jumpForce = 5f;
    private Random random;
    private bool isGameOver;
    private bool isGameStarted;

    private TextureManager textureManager;
    private int startTexture;
    private int gameOverTexture;
    private int backgroundTexture;
    public int birdTexture;
    private int upperPipeTexture;
    private int lowerPipeTexture;

    private WaveOutEvent StartsoundPlayer = new WaveOutEvent();
    private AudioFileReader startMusic;

    private WaveOutEvent GamesoundPlayer = new WaveOutEvent();
    private AudioFileReader gameMusic;

    private WaveOutEvent OversoundPlayer = new WaveOutEvent();
    private AudioFileReader OverMusic;

    private bool isStartSoundPlaying = false;
    private bool isGameSoundPlaying = false;
    private bool isOverSoundPlaying = false;

    private Score score;
    private bool isSoundEnabled = true;
    private float lastToggleTime = 0;
    private float toggleDelay = 1f;
    public Game() : base(800, 600)
    {
        Icon = new Icon("texture/ico/icon.ico");
        bird = new Bird();
        obstacles = new List<Obstacle>();
        random = new Random();
        score = new Score();
        textureManager = new TextureManager();
        LoadTextures();
        isGameOver = false;
        isGameStarted = false;
        try
        {
            startMusic = new AudioFileReader("texture/sound/start.mp3");
            gameMusic = new AudioFileReader("texture/sound/back.mp3");
            OverMusic = new AudioFileReader("texture/sound/over.mp3");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading music: {ex.Message}");
        }

    }

    private void LoadTextures()
    {
        startTexture = textureManager.LoadTexture("texture/image/start.jpg");
        gameOverTexture = textureManager.LoadTexture("texture/image/gameover.png");
        backgroundTexture = textureManager.LoadTexture(@"texture/image/background.png");
        birdTexture = textureManager.LoadTexture(@"texture/image/bird.png");
        upperPipeTexture = textureManager.LoadTexture("texture/image/upper_pipe.png");
        lowerPipeTexture = textureManager.LoadTexture("texture/image/lower_pipe.png");
        if (startTexture == -1) Console.WriteLine("Ошибка загрузки стартовой текстуры");
        if (gameOverTexture == -1) Console.WriteLine("Ошибка загрузки текстуры проигрыша");
        if (backgroundTexture == -1) Console.WriteLine("Ошибка загрузки фоновой текстуры");
        textureManager.LoadDigitTextures();

    }


    protected override void OnUnload(EventArgs e)
    {
        base.OnUnload(e);
        GL.Enable(EnableCap.Texture2D);
        GL.DeleteTexture(startTexture);
        GL.DeleteTexture(gameOverTexture);
        GL.DeleteTexture(backgroundTexture);
        GL.DeleteTexture(backgroundTexture);
        GL.DeleteTexture(birdTexture);
        GL.DeleteTexture(upperPipeTexture);
        GL.DeleteTexture(lowerPipeTexture);
        StartsoundPlayer.Dispose();
        GamesoundPlayer.Dispose();
        OversoundPlayer.Dispose();
    }
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        GL.ClearColor(1.0f, 1.0f, 1.0f, 0.0f);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        GL.Ortho(0, Width, Height, 0, -1, 1);
        GL.MatrixMode(MatrixMode.Modelview);
        GL.Enable(EnableCap.Texture2D);
        var error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine($"OpenGL Error: {error}");
        }
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        StartsoundPlayer.Init(startMusic);
        GamesoundPlayer.Init(gameMusic);
        OversoundPlayer.Init(OverMusic);

    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        HandleInput();

        lastToggleTime += (float)e.Time;


        if (Keyboard.GetState().IsKeyDown(Key.M) && lastToggleTime > toggleDelay)
        {
            ToggleSound();
            lastToggleTime = 0f;
        }

        if (isGameStarted && !isGameOver)
        {
            bird.Update(gravity);
           
            if (bird.Y <= 0 || bird.Y >= 600) 
            {
                isGameOver = true; 
            }
            foreach (var obstacle in obstacles)
            {
                obstacle.Update();
                if (obstacle.X < bird.X && !obstacle.Passed)
                {
                    obstacle.Passed = true;
                    score.Scorepassed();
                }
                if (obstacle.X < -50)
                {
                    obstacle.Reset();
                    obstacle.Passed = false;
                }
            }

            if (bird.IsDead(obstacles))
            {
                isGameOver = true;

            }
        }
        ErrorCode error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine($"OpenGL Error: {error}");
        }

    }

    private void UpdateSound()
    {

        if (!isGameStarted)
        {
            if (!isStartSoundPlaying)
            {

                StartsoundPlayer.Play();
                isStartSoundPlaying = true;
            }
        }
        else
        {
            isStartSoundPlaying = false;
            StartsoundPlayer.Stop();
        }

        if (isGameOver)
        {
            if (!isOverSoundPlaying)
            {

                OversoundPlayer.Play();
                isOverSoundPlaying = true;
            }
        }
        else
        {
            isOverSoundPlaying = false;
            OversoundPlayer.Stop();
        }

        if (!isGameOver && isGameStarted)
        {
            if (!isGameSoundPlaying)
            {

                GamesoundPlayer.Play();
                isGameSoundPlaying = true;
            }
        }
        else
        {
            isGameSoundPlaying = false;
            GamesoundPlayer.Stop();
        }
    }
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        UpdateSound();



        if (!isGameStarted)
        {
            RenderStartScreen();
        }
        else if (isGameOver)
        {
            RenderGameOverScreen();
        }
        else
        {
            RenderBackground();
            score.Draw(textureManager, Width, Height);
            bird.Render(birdTexture);
            foreach (var obstacle in obstacles)
            {
                obstacle.Render();
            }
        }

        ErrorCode error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            Console.WriteLine($"OpenGL Error: {error}");
        }

        SwapBuffers();

    }

    private void RenderStartScreen()
    {
        textureManager.RenderTexture(startTexture, Width, Height);
    }

    private void RenderGameOverScreen()
    {
        textureManager.RenderTexture(gameOverTexture, Width, Height);
    }
    private void RenderBackground()
    {
        textureManager.RenderTexture(backgroundTexture, Width, Height);
    }

    private void HandleInput()
    {
        var keyboardState = Keyboard.GetState();

        if (!isGameStarted && keyboardState.IsKeyDown(Key.Enter))
        {
            isGameStarted = true;
            ResetGame();
            return;
        }

        if (isGameOver)
        {
            if (keyboardState.IsKeyDown(Key.R))
            {
                RestartGame();
            }
            else if (keyboardState.IsKeyDown(Key.Escape))
            {
                Exit();
            }
            return;
        }

        if (keyboardState.IsKeyDown(Key.Space))
        {
            bird.Jump(jumpForce);
        }

        if (keyboardState.IsKeyDown(Key.Escape))
        {
            Exit();
        }

    }

    private void ToggleSound()
    {
        isSoundEnabled = !isSoundEnabled;

        if (isSoundEnabled)
        {
            StartsoundPlayer.Volume = 1.0f;
            GamesoundPlayer.Volume = 1.0f;
            OversoundPlayer.Volume = 1.0f;
            Console.WriteLine("Sound Enabled");
        }
        else
        {
            StartsoundPlayer.Volume = 0.0f;
            GamesoundPlayer.Volume = 0.0f;
            OversoundPlayer.Volume = 0.0f;
            Console.WriteLine("Sound Disabled");
        }
    }

    private void ResetGame()
    {
        bird.Reset();
        obstacles.Clear();
        LoadObstacles();
        isGameOver = false;
        score.Reset();

    }

    private void RestartGame()
    {
        ResetGame();
        isGameStarted = true;
    }

    private void LoadObstacles()
    {
        for (int i = 0; i < 3; i++)
        {
            obstacles.Add(new Obstacle(i * 300 + 800, upperPipeTexture, lowerPipeTexture));
        }
    }

}
