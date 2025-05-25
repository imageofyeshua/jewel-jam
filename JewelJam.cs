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

        // get the size of the screen to use
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

        // calculate how the graphics should be scaled, so that the game world fits inside the window
        spriteScale = Matrix.CreateScale((float)screenSize.X / worldSize.X, (float)screenSize.Y / worldSize.Y, 1);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // load the background sprite
        background = Content.Load<Texture2D>("spr_background");

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
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, spriteScale);

        _spriteBatch.Draw(background, Vector2.Zero, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    bool FullScreen
    {
        get { return _graphics.IsFullScreen; }
        set { ApplyResolutionSettings(value); }
    }
}
