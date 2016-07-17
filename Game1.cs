/* Sam Gilbert
 * program (dic(tionary)snake AKA Zelda snake
 * extra tid bits include an introduction song for the start screen
 * a main looping song for the main game
 * an appropriate sound for letter pick-up and word finding
 * a death sound for the gameOver screen */







using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace DicSnake
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteManager spriteManager;

        //XACT stuff
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue trackCue;
				enum GameState { Start, InGame, GameOver };
				GameState currentGameState = GameState.Start;

				// Score stuff
				public int currentScore = 0;
				SpriteFont scoreFont;
				public string lastWord;

				//mode stuff
				public bool easyMode;

				// Background
				Texture2D backgroundTexture;

        // Random number generator
        public Random rnd { get; private set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            rnd = new Random();

            // Adjust game window size
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1024;
        }

        protected override void Initialize()
        {
            spriteManager = new SpriteManager(this);
            Components.Add(spriteManager);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

						// Load fonts
						scoreFont = Content.Load<SpriteFont>(@"fonts\score");

            // Load the XACT data
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");

            // Start the soundtrack audio
            //trackCue = soundBank.GetCue("track");
            //trackCue.Play();

            // Play the start sound
            //soundBank.PlayCue("start");
						trackCue = soundBank.GetCue("Intro");
						trackCue.Play();
						// Load the background
						backgroundTexture = Content.Load<Texture2D>(@"Images\background");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed)
                this.Exit();

            // Update audio engine 
            audioEngine.Update();

						switch (currentGameState)
							{
							case GameState.Start:


							spriteManager.Enabled = false;
							spriteManager.Visible = false;
								if (Keyboard.GetState().IsKeyDown(Keys.Left))
									{
									trackCue.Stop(AudioStopOptions.Immediate);
									trackCue = soundBank.GetCue("track");
									trackCue.Play();
									easyMode = true;
									currentGameState = GameState.InGame;
									spriteManager.Enabled = true;
									spriteManager.Visible = true;
									}
								if (Keyboard.GetState().IsKeyDown(Keys.Right))
								{
								easyMode = false;
								currentGameState = GameState.InGame;
								spriteManager.Enabled = true;
								spriteManager.Visible = true;
								}
								break;
							case GameState.InGame:
								break;
							case GameState.GameOver:
									spriteManager.Enabled = false;
									spriteManager.Visible = false;
								if (Keyboard.GetState().IsKeyDown(Keys.Enter))
									Exit();
								break;
							}

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

						switch (currentGameState)
							{
							case GameState.Start:
								GraphicsDevice.Clear(Color.AliceBlue);

								// Draw text for intro splash screen
								spriteBatch.Begin();
								string text = "HEY LISTEN!\nMake words Link!";
								spriteBatch.DrawString(scoreFont, text,
										new Vector2((Window.ClientBounds.Width / 2)
										- (scoreFont.MeasureString(text).X / 2),
										(Window.ClientBounds.Height / 2)
										- (scoreFont.MeasureString(text).Y / 2)),
										Color.SaddleBrown);

								text = "(Press left for easy and right for hard)";
								spriteBatch.DrawString(scoreFont, text,
										new Vector2((Window.ClientBounds.Width / 2)
										- (scoreFont.MeasureString(text).X / 2),
										(Window.ClientBounds.Height / 2)
										- (scoreFont.MeasureString(text).Y / 2) + 30),
										Color.SaddleBrown);

								spriteBatch.End();
								break;

							case GameState.InGame:
								GraphicsDevice.Clear(Color.White);
								spriteBatch.Begin();

								// Draw background image
								spriteBatch.Draw(backgroundTexture,
										new Rectangle(0, 0, Window.ClientBounds.Width,
										Window.ClientBounds.Height), null,
										Color.White, 0, Vector2.Zero,
										SpriteEffects.None, 0);

								// Draw fonts
								spriteBatch.DrawString(scoreFont,
										"Score: " + currentScore,
										new Vector2(10, 10), Color.DarkBlue,
										0, Vector2.Zero,
										1, SpriteEffects.None, 1);

								spriteBatch.DrawString(scoreFont, "Last Word: " + lastWord,
										new Vector2(10, 30), Color.DarkBlue, 0, Vector2.Zero,
										1, SpriteEffects.None, 1);

								spriteBatch.End();
								break;

							case GameState.GameOver:
								GraphicsDevice.Clear(Color.AliceBlue);

								spriteBatch.Begin();
								string gameover = "Game Over\nYou Died.";
								spriteBatch.DrawString(scoreFont, gameover,
										new Vector2((Window.ClientBounds.Width / 2)
										- (scoreFont.MeasureString(gameover).X / 2),
										(Window.ClientBounds.Height / 2)
										- (scoreFont.MeasureString(gameover).Y / 2)),
										Color.SaddleBrown);

								gameover = "Your score: " + currentScore;
								spriteBatch.DrawString(scoreFont, gameover,
										new Vector2((Window.ClientBounds.Width / 2)
										- (scoreFont.MeasureString(gameover).X / 2),
										(Window.ClientBounds.Height / 2)
										- (scoreFont.MeasureString(gameover).Y / 2) + 30),
										Color.SaddleBrown);

								gameover = "(Press ENTER to exit)";
								spriteBatch.DrawString(scoreFont, gameover,
										new Vector2((Window.ClientBounds.Width / 2)
										- (scoreFont.MeasureString(gameover).X / 2),
										(Window.ClientBounds.Height / 2)
										- (scoreFont.MeasureString(gameover).Y / 2) + 60),
										Color.SaddleBrown);

								spriteBatch.End();
								break;
							}

            base.Draw(gameTime);
        }


        public void PlayCue(string cueName)
        {
            soundBank.PlayCue(cueName);
        }

				public void loseGame()
					{
					trackCue.Stop(AudioStopOptions.Immediate);
					//play death sound
					trackCue = soundBank.GetCue("gameOver");
					trackCue.Play();
					currentGameState = GameState.GameOver;

					}
    }
}
