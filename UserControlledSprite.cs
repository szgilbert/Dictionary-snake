using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DicSnake
{
    class UserControlledSprite : Sprite
    {
			int millisecondsPerFrame = 40;
			bool leftGate = true;
			bool rightGate = true;
        // Get direction of sprite based on player input and speed
        public override Vector2 Direction
        {
            set
            {
            }
            get
            {

                // If player pressed arrow keys, move the sprite
                if (Keyboard.GetState().IsKeyDown(Keys.Left) && direction.X != 1)
                {
                    direction.X = -1;
                    direction.Y = 0;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right) && direction.X != -1)
                {
                    direction.X = +1;
                    direction.Y = 0;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Up) && direction.Y != 1)
                {
                    direction.Y = -1;
                    direction.X = 0;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down) && direction.Y != -1)
                {
                    direction.Y = +1;
                    direction.X = 0;
                }

                // If player pressed the gamepad thumbstick, move the sprite
                GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
                if (gamepadState.ThumbSticks.Left.X != 0)
                    direction.X += gamepadState.ThumbSticks.Left.X;
                if (gamepadState.ThumbSticks.Left.Y != 0)
                    direction.Y -= gamepadState.ThumbSticks.Left.Y;

                return direction * speed;
            }
        }

        public UserControlledSprite(Texture2D textureImage, Vector2 position,
                Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
                Vector2 speed)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, null)
        {
        }

        public UserControlledSprite(Texture2D textureImage, Vector2 position,
                Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
                Vector2 speed, int millisecondsPerFrame)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, millisecondsPerFrame, null)
        {
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {

            // Move the sprite based on direction
            position += Direction;

						timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
						if (timeSinceLastFrame > millisecondsPerFrame)
							{
							// Increment to next frame
							timeSinceLastFrame = 0;
							++currentFrame.X;
							if (currentFrame.X >= sheetSize.X)
								currentFrame.X = 0;
							}
						if (Keyboard.GetState().IsKeyDown(Keys.Left) && direction.X != 1)
							{
								currentFrame.Y = 2;
								sheetSize.X = 6;
								if(leftGate)
								{
								currentFrame.X = 0;
								leftGate = false;
								}
							}
						if (Keyboard.GetState().IsKeyDown(Keys.Right) && direction.X != -1)
							{
								currentFrame.Y = 3;
								if(rightGate)
								{
								currentFrame.X = 0;
								rightGate = false;
								}
								sheetSize.X = 6;
							}
						if (Keyboard.GetState().IsKeyDown(Keys.Up) && direction.Y != 1)
							{
								currentFrame.Y = 1;
								sheetSize.X = 8;
								leftGate = true;
								rightGate = true;
							}
						if (Keyboard.GetState().IsKeyDown(Keys.Down) && direction.Y != -1)
							{
								currentFrame.Y = 0;
								sheetSize.X = 8;
								leftGate = true;
								rightGate = true;
							}

            //base.Update(gameTime, clientBounds);
        }
    }
}
