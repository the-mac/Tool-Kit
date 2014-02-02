using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlienGameSample;
using System.Threading;
using Microsoft.Xna.Framework;

namespace AlienGame
{
    class LoadingScreen : GameScreen
    {
        private Thread backgroundThread;

        public LoadingScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);
        }

        void BackgroundLoadContent()
        {
            ScreenManager.Game.Content.Load<object>("alien_hit");
            ScreenManager.Game.Content.Load<object>("alien1");
            ScreenManager.Game.Content.Load<object>("background");
            ScreenManager.Game.Content.Load<object>("badguy_blue");
            ScreenManager.Game.Content.Load<object>("badguy_green");
            ScreenManager.Game.Content.Load<object>("badguy_orange");
            ScreenManager.Game.Content.Load<object>("badguy_red");
            ScreenManager.Game.Content.Load<object>("bullet");
            ScreenManager.Game.Content.Load<object>("cloud1");
            ScreenManager.Game.Content.Load<object>("cloud2");
            ScreenManager.Game.Content.Load<object>("fire");
            ScreenManager.Game.Content.Load<object>("gamefont");
            ScreenManager.Game.Content.Load<object>("ground");
            ScreenManager.Game.Content.Load<object>("hills");
            ScreenManager.Game.Content.Load<object>("laser");
            ScreenManager.Game.Content.Load<object>("menufont");
            ScreenManager.Game.Content.Load<object>("moon");
            ScreenManager.Game.Content.Load<object>("mountains_blurred");
            ScreenManager.Game.Content.Load<object>("player_hit");
            ScreenManager.Game.Content.Load<object>("scorefont");
            ScreenManager.Game.Content.Load<object>("smoke");
            ScreenManager.Game.Content.Load<object>("sun");
            ScreenManager.Game.Content.Load<object>("tank");
            ScreenManager.Game.Content.Load<object>("tank_fire");
            ScreenManager.Game.Content.Load<object>("tank_tire");
            ScreenManager.Game.Content.Load<object>("tank_top");
            ScreenManager.Game.Content.Load<object>("title");
            ScreenManager.Game.Content.Load<object>("titlefont");
        }

        public override void LoadContent()
        {
            if (backgroundThread == null)
            {
                backgroundThread = new Thread(BackgroundLoadContent);
                backgroundThread.Start();
            }

            base.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (backgroundThread != null && backgroundThread.Join(10))
            {
                backgroundThread = null;
                this.ExitScreen();
                ScreenManager.AddScreen(new MainMenuScreen());
                ScreenManager.Game.ResetElapsedTime();
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
    }
}
