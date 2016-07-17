using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DicSnake
{
    class LetterSprite : Sprite
    {
        // Direction is same as speed
        int millisecondsPerFrame = 40;
				public char letter;
        public override Vector2 Direction
        {
            set { }
            get { return speed; }
        }

        public LetterSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, string collisionCueName, char a)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, collisionCueName)
        {
				this.letter = a;
        }

        public LetterSprite(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, int millisecondsPerFrame, string collisionCueName, char a)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, millisecondsPerFrame, collisionCueName)
        {
				this.letter = a;
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                // Increment to next frame
                timeSinceLastFrame = 0;
                ++currentFrame.X;
                if (currentFrame.X >= sheetSize.X)
                    currentFrame.X = 0;
            }
						

            //base.Update(gameTime, clientBounds);
        }
    }
}
