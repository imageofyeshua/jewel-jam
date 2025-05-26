using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class JewelJam : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // a matrix used for scaling the gameworld so that it fits inside the window
    Matrix spriteScale;

    // the background sprite
    protected Texture2D background;

    // a sprite to draw at the mouse position, as an example of using ScreenToWorld
    protected Texture2D cursorSprite;

    // gameword, and window size structs storing two integers
    Point worldSize, windowSize;

    // user input helper
    InputHelper inputHelper;

    // full screen or not
    bool fullScreen;

    public JewelJam()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        inputHelper = new InputHelper();
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    void ApplyResolutionSettings(bool fullScreen)
    {
        // make the game full screen or not
        _graphics.IsFullScreen = fullScreen;

        // get the size of the screen to use: either the window size or the full screen size
        Point screenSize;
        if (fullScreen)
            screenSize = new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
        else
            screenSize = windowSize;

        // scale the window to the desired size
        _graphics.PreferredBackBufferWidth = screenSize.X;
        _graphics.PreferredBackBufferHeight = screenSize.Y;

        _graphics.ApplyChanges();

        // calculate and set the viewport to use
        GraphicsDevice.Viewport = CalculateViewport(screenSize);

        // calculate how the graphics should be scaled, so that the game world fits inside the window
        spriteScale = Matrix.CreateScale((float)GraphicsDevice.Viewport.Width / worldSize.X,
            (float)GraphicsDevice.Viewport.Height / worldSize.Y, 1);
    }

    Viewport CalculateViewport(Point windowSize)
    {
        // create a viewport object
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

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // load the background sprite
        background = Content.Load<Texture2D>("spr_background");

        // load the cursor sprite
        cursorSprite = Content.Load<Texture2D>("spr_single_jewel1");

        // set the world size to the width and height of that sprite
        worldSize = new Point(background.Width, background.Height);

        // set the window size
        windowSize = new Point(1024, 768);
        FullScreen = false;
    }

    protected override void Update(GameTime gameTime)
    {
        inputHelper.Update();

        if (inputHelper.KeyPressed(Keys.Escape))
            Exit();

        if (inputHelper.KeyPressed(Keys.F5))
            FullScreen = !FullScreen;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // start drawing sprites, applying the scaling matrix
        _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, spriteScale);

        // draw the background sprite
        _spriteBatch.Draw(background, Vector2.Zero, Color.White);

        _spriteBatch.Draw(cursorSprite, ScreenToWorld(inputHelper.MousePosition), Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    bool FullScreen
    {
        get { return _graphics.IsFullScreen; }
        set { ApplyResolutionSettings(value); }
    }

    Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        Vector2 viewportTopLeft = new Vector2(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y);
        float screenToWorldScale = worldSize.X / (float)GraphicsDevice.Viewport.Width;
        return (screenPosition - viewportTopLeft) * screenToWorldScale;
    }
}
