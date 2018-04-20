using System;
using System.Collections.Generic;
using System.Linq;
using WorldViewer;

namespace WorldViewer
{
  public class Game1 : IDisposable
  {
    //GraphicsDeviceManager graphics;
    //SpriteBatch spriteBatch;

    public Game1()
    {
      //graphics = new GraphicsDeviceManager(this);
      //Content.RootDirectory = "Content";
    }

    protected void Initialize()
    {
      // TODO: Add your initialization logic here

      // base.Initialize();
    }

    protected void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      //spriteBatch = new SpriteBatch(GraphicsDevice);

      // TODO: use this.Content to load your game content here
      _testFile1 = new iDEM_ASCII();

      _testFile1.Load(@"G:\Users\iain\Downloads\UK_STRM\srtm_36_02.asc");

      _testFile2 = new iSRTS3_hgt();

      _testFile2.Load(@"G:\Users\iain\Downloads\N00E099.hgt.zip");
    }

    iDEM_ASCII _testFile1;
    iSRTS3_hgt _testFile2;

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// all content.
    /// </summary>
    protected void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected void Update(TimeSpan gameTime)
    {
      // Allows the game to exit
      //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
      //	this.Exit();

      //// TODO: Add your update logic here

      //base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected void Draw(TimeSpan gameTime)
    {
      //GraphicsDevice.Clear(Color.CornflowerBlue);

      //// TODO: Add your drawing code here

      //base.Draw(gameTime);
    }

    public void Run()
    {
    }

    public void Dispose()
    {
    }
  }
}
