using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;


namespace DicSnake
	{
	class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
		{
		// SpriteBatch for drawing
		SpriteBatch spriteBatch;

		// A sprite for the player and a list of automated sprites
		UserControlledSprite player;
		List<LetterSprite> spriteList = new List<LetterSprite>();
		List<LetterSprite> tailList = new List<LetterSprite>();
		List<Vector2> positionList = new List<Vector2>();

		// Variables for spawning new letters
		int letterSpawnTimer = 0;
		int spawnSpeedMod = 0;
		int nextSpawnTime = 0;
		int tailPositionOffset = 6;
		float tailRaitoOffset = 1.5f;
		int rndRow = 0;
		bool wordFound = false;
		Vector2 midscreen = new Vector2(450, 300);
		int easySpawn = 10;

		//player variables
		const float MOVESPEED_MOD = 1.1f;
		int playerSpeedJump = 0;
		int JUMP_INTERVAL = 5000;
		int easySpeed = 10;
		int hardSpeed = 20;
		int hardSpawn = 50;
		int maxSpeed;


		//variables for loading the dictionary
		string inputWord;
		List<string> wordList = new List<string>();

		public SpriteManager(Game game)
			: base(game)
			{
			// TODO: Construct any child components here
			}

		/// <summary>
		/// Allows the game component to perform any initialization it needs to before starting
		/// to run.  This is where it can query for any required services and load content.
		/// </summary>
		public override void Initialize()
			{
			// Initialize spawn time
			ResetSpawnTime();
			LoadDictionary();

			base.Initialize();
			}

		protected override void LoadContent()
			{
			spriteBatch = new SpriteBatch(Game.GraphicsDevice);

			player = new UserControlledSprite(
							Game.Content.Load<Texture2D>(@"Images/linkSprite"),
							midscreen, new Point(32, 48), 10, new Point(0, 0),
							new Point(8, 4), new Vector2(4, 4));

			for (int i = 0; i < 7; i++)
				{
				SpawnLetter();
				}

			playerSpeedJump = JUMP_INTERVAL;
			if (((Game1)Game).easyMode)
				{
				maxSpeed = easySpeed;
				letterSpawnTimer = 5000;
				spawnSpeedMod = easySpawn;
				}
			else
				{
				maxSpeed = hardSpeed;
				letterSpawnTimer = 2500;
				spawnSpeedMod = hardSpawn;
				}
			base.LoadContent();
			}

		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
			{

			// Decrement next spawn time
			nextSpawnTime -= gameTime.ElapsedGameTime.Milliseconds;
			if (nextSpawnTime < 0)
				{
				// Time to spawn a new letter
				SpawnLetter();

				// Reset spawn timer
				ResetSpawnTime();
				}

			playerSpeedJump -= gameTime.ElapsedGameTime.Milliseconds;
			if (playerSpeedJump < 0 && player.speed.X < maxSpeed)
				{
				player.speed.X *= MOVESPEED_MOD;
				player.speed.Y *= MOVESPEED_MOD;
				playerSpeedJump = JUMP_INTERVAL;
				if (!((Game1)Game).easyMode)
					playerSpeedJump = JUMP_INTERVAL / 2;
				}



			if (Keyboard.GetState().IsKeyDown(Keys.Space))
				{
				CheckForWord(gameTime);
				}

			// Update player
			player.Update(gameTime, Game.Window.ClientBounds);

			if (player.IsOutOfBounds(Game.Window.ClientBounds))
				((Game1)Game).loseGame();

			//fill position list
			positionList.Add(player.position);

			//erase unused positions
			if (positionList.Count > (tailList.Count + 5) * tailPositionOffset)
				{
				positionList.RemoveAt(0);
				}
			// Update all sprites
			for (int i = 0; i < spriteList.Count; ++i)
				{
				LetterSprite s = spriteList[i];

				s.Update(gameTime, Game.Window.ClientBounds);

				// Check for collisions
				if (s.collisionRect.Intersects(player.collisionRect))
					{
					// Play collision sound
					if (s.collisionCueName != null)
						((Game1)Game).PlayCue(s.collisionCueName);

					// Remove collided sprite from the game
					tailList.Add(s);
					spriteList.RemoveAt(i);
					--i;
					}
				}
			//update sprites in tail
			UpdateTail(gameTime);

			base.Update(gameTime);
			}

		public override void Draw(GameTime gameTime)
			{
			spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

			// Draw the player
			player.Draw(gameTime, spriteBatch);

			// Draw all sprites
			foreach (Sprite s in spriteList)
				s.Draw(gameTime, spriteBatch);

			//draw tail
			foreach (Sprite s in tailList)
				s.Draw(gameTime, spriteBatch);

			spriteBatch.End();
			base.Draw(gameTime);
			}

		private void ResetSpawnTime()
			{
			// Set the next spawn time for an enemy
			if (letterSpawnTimer > 600)
				spawnSpeedMod++;
			nextSpawnTime = letterSpawnTimer - spawnSpeedMod;
			}

		private void SpawnLetter()
			{
			Vector2 position = Vector2.Zero;
			Vector2 speed = Vector2.Zero;
			bool spawn = true;
			// Default frame size
			Point frameSize = new Point(32, 32);

			// Randomly place letter,
			// then randomly create a position along that side of the screen
			// and randomly choose a speed for the letter
			position = new
											Vector2(
											((Game1)Game).rnd.Next(0,
											Game.GraphicsDevice.PresentationParameters.BackBufferWidth
											- frameSize.X),
											((Game1)Game).rnd.Next(0,
											Game.GraphicsDevice.PresentationParameters.BackBufferHeight
											- frameSize.Y));

			// Create the sprite
			//choose random rows for different letters
			rndRow = ((Game1)Game).rnd.Next(0, 25);
			LetterSprite letter = new LetterSprite(Game.Content.Load<Texture2D>(@"images\Letters"),
			position, frameSize, 0, new Point(0, rndRow),
			new Point(20, 26), speed, "collision", (char)((int)'A' + rndRow));

			for (int i = 0; i < spriteList.Count; i++)
				{
				LetterSprite s = spriteList[i];
				if (letter.collisionRect.Intersects(s.collisionRect) || letter.collisionRect.Intersects(player.collisionRect))
					spawn = false;
				}

			if (spawn)
				spriteList.Add(letter);
			}

		// Return current position of the player sprite
		public Vector2 GetPlayerPosition()
			{
			return player.GetPosition;
			}

		//update tail position
		public void UpdateTail(GameTime gameTime)
			{
			for (int i = 0; i < tailList.Count; i++)
				{
				LetterSprite s = tailList[i];

				s.Update(gameTime, Game.Window.ClientBounds);
				s.position = positionList[Convert.ToInt32(positionList.Count - ((i + tailRaitoOffset)) * tailPositionOffset)];
				if (s.collisionRect.Intersects(player.collisionRect) && i > 1)
					{
					((Game1)Game).loseGame();
					}
				}
			}

		public void LoadDictionary()
			{
			FileStream inFile = new FileStream("dictionary.txt", FileMode.Open, FileAccess.Read);
			StreamReader sReader = new StreamReader(inFile);
			inputWord = sReader.ReadLine();
			while (inputWord != null)
				{
				wordList.Add(inputWord);
				inputWord = sReader.ReadLine();
				}
			}

		public void CheckForWord(GameTime gameTime)
			{
			int wordCount;
			int currentCount;
			string inputWord;
			//loop to go through all letters in taillist
			for (int i = 0; i < tailList.Count; i++)
				{
				wordCount = (tailList.Count - i) + 1;

				if (wordCount > 15)
					wordCount = 15;

				if (wordCount != 0)
					{
					currentCount = wordCount - 1;
					}
				else
					{
					currentCount = wordCount;
					}

				//loop through to create a word
				for (int j = 0; j < wordCount; j++)
					{
					inputWord = "";
					if (!wordFound)
						{
						for (int f = 0; f < currentCount; f++)
							{
							LetterSprite l = tailList[i + f];
							inputWord = inputWord + l.letter;
							inputWord = inputWord.ToUpper();
							}
						}
					//check input word against dictionary array using existing code in dictionary program
					if (currentCount > 2)
						{
						if (0 <= wordList.BinarySearch(inputWord))
							{
							// Play word sound
							((Game1)Game).PlayCue("word");

							//adjust last word and score
							((Game1)Game).lastWord = inputWord;
							((Game1)Game).currentScore += (currentCount * currentCount) * tailList.Count;
							wordFound = true;
							//delete word
							for (int k = 0; k < currentCount; k++)
								{
								if (player.speed.X > (4 * MOVESPEED_MOD))
									{
									player.speed.X /= MOVESPEED_MOD;
									player.speed.Y /= MOVESPEED_MOD;
									}
								tailList.RemoveAt(i);
								}
							}
						else
							{
							currentCount -= 1;//(only if a word wasn't found)
							}
						}
					}
				wordFound = false;
				}
			}
		}
	}
