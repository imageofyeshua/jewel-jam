using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class ExtendedGame : Game
{
    // standard MonoGame objects for graphics and sprites
    protected GraphicsDeviceManager graphics;
    protected SpriteBatch spriteBatch;

    // object for handling keyboard and mouse input
    protected InputHelper inputHelper;

    /// <summary>
    /// The width and height of the game world, in game units.
    /// </summary>
    protected Point worldSize;

    /// <summary>
    /// The width and height of the window, in pixels.
    /// </summary>
    protected Point windowSize;

    /// <summary>
    /// A matrix used for scaling the game world so that it fits inside the window.
    /// </summary>
    protected Matrix spriteScale;

    /// <summary>
    /// An object for generating random numbers throughout the game.
    /// </summary>
    public static Random Random { get; private set; }

    /// <summary>
    /// An object for loading assets throughout the game.
    /// </summary>
    public static ContentManager ContentManager { get; private set; }

    protected ExtendedGame()
    {
        Content.RootDirectory = "Content";
        graphics = new GraphicsDeviceManager(this);

        inputHelper = new InputHelper();
        Random = new Random();

        // default window and world size
        windowSize = new Point(1024, 768);
        worldSize = new Point(1024, 768);
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // store a static reference to the ContentManager
        ContentManager = Content;

        // by default, we're not running in full-screen mode
        FullScreen = false;
    }

    protected override void Update(GameTime gameTime)
    {
        HandleInput();
    }

    protected virtual void HandleInput()
    {
        inputHelper.Update();

        // quit the game when the player presses ESC
        if (inputHelper.KeyPressed(Keys.Escape))
            Exit();

        // toggle full-screen mode when the player presses F5
        if (inputHelper.KeyPressed(Keys.F5))
            FullScreen = !FullScreen;
    }

    /// <summary>
    /// Scales the window to the desired size, and calculates how the game world should be scaled to fit inside that window.
    /// </summary>
    void ApplyResolutionSettings(bool fullScreen)
    {
        // make the game full-screen or not
        graphics.IsFullScreen = fullScreen;

        // get the size of the screen to use: either the window size or the full screen size
        Point screenSize;
        if (fullScreen)
            screenSize = new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
        else
            screenSize = windowSize;

        // scale the window to the desired size
        graphics.PreferredBackBufferWidth = screenSize.X;
        graphics.PreferredBackBufferHeight = screenSize.Y;

        graphics.ApplyChanges();

        // calculate and set the viewport to use
        GraphicsDevice.Viewport = CalculateViewport(screenSize);

        // calculate how the graphics should be scaled, so that the game world fits inside the window
        spriteScale = Matrix.CreateScale((float)GraphicsDevice.Viewport.Width / worldSize.X, (float)GraphicsDevice.Viewport.Height / worldSize.Y, 1);
    }

    /// <summary>
    /// Calculates and returns the viewport to use, so that the game world fits on the screen while preserving its aspect ratio.
    /// </summary>
    /// <param name="windowSize">The size of the screen on which the world should be drawn.</param>
    /// <returns>A Viewport object that will show the game world as large as possible while preserving its aspect ratio.</returns>
    Viewport CalculateViewport(Point windowSize)
    {
        // create a Viewport object
        Viewport viewport = new Viewport();

        // calculate the two aspect ratios
        float gameAspectRatio = (float)worldSize.X / worldSize.Y;
        float windowAspectRatio = (float)windowSize.X / windowSize.Y;

        // if the window is relatively wide, use the full window height
        if (windowAspectRatio > gameAspectRatio)
        {
            viewport.Width = (int)(windowSize.Y * gameAspectRatio);
            viewport.Height = windowSize.Y;
        }
        // if the window is relatively high, use the full window width
        else
        {
            viewport.Width = windowSize.X;
            viewport.Height = (int)(windowSize.X / gameAspectRatio);
        }

        // calculate and store the top-left corner of the viewport
        viewport.X = (windowSize.X - viewport.Width) / 2;
        viewport.Y = (windowSize.Y - viewport.Height) / 2;

        return viewport;
    }

    /// <summary>
    /// Gets or sets whether the game is running in full-screen mode.
    /// </summary>
    public bool FullScreen
    {
        get { return graphics.IsFullScreen; }
        protected set { ApplyResolutionSettings(value); }
    }

    /// <summary>
    /// Converts a position in screen coordinates to a position in world coordinates.
    /// </summary>
    /// <param name="screenPosition">A position in screen coordinates.</param>
    /// <returns>The corresponding position in world coordinates.</returns>
    Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        Vector2 viewportTopLeft = new Vector2(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y);
        float screenToWorldScale = worldSize.X / (float)GraphicsDevice.Viewport.Width;
        return (screenPosition - viewportTopLeft) * screenToWorldScale;
    }

}