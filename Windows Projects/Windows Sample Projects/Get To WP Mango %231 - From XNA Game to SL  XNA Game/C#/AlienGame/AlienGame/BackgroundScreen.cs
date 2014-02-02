using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlienGameSample;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AlienGame
{
    class BackgroundScreen : GameScreen
    {
        Texture2D title;
        Texture2D background;

        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            title = ScreenManager.Game.Content.Load<Texture2D>("title");
            background = ScreenManager.Game.Content.Load<Texture2D>("background");
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                     bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            spriteBatch.Begin();

            // Draw Background
            spriteBatch.Draw(background, new Vector2(0, 0),
                 new Color(255, 255, 255, TransitionAlpha));

            // Draw Title
            spriteBatch.Draw(title, new Vector2(60, 55),
                 new Color(255, 255, 255, TransitionAlpha));

            spriteBatch.End();
        }
    }
}
