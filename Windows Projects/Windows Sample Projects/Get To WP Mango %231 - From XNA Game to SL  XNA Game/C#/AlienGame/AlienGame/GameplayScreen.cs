using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlienGameSample;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Devices.Sensors;

namespace AlienGame
{
    class GameplayScreen : GameScreen
    {
        // Input Members
        AccelerometerReadingEventArgs accelState;
        Accelerometer Accelerometer;

        // Create a helper instance for handling game logic
        GameplayHelper gameplayHelper;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);

            Accelerometer = new Accelerometer();
            if (Accelerometer.State == SensorState.Ready)
            {
                Accelerometer.ReadingChanged += (s, e) =>
                {
                    accelState = e;
                };
                Accelerometer.Start();
            }            
        }

        public override void LoadContent()
        {
            gameplayHelper = new GameplayHelper(ScreenManager.Game.Content, ScreenManager.SpriteBatch,
                ScreenManager.Game.GraphicsDevice);

            gameplayHelper.LoadContent();

            gameplayHelper.Start();

            base.LoadContent();
        }

        public override void UnloadContent()
        {
            gameplayHelper.UnloadContent();

            base.UnloadContent();
        }

        /// <summary>
        /// Runs one frame of update for the game.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsActive)
            {
                gameplayHelper.Update(elapsedSeconds);
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Draw the game world, effects, and HUD
        /// </summary>
        /// <param name="gameTime">The elapsed time since last Draw</param>
        public override void Draw(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            gameplayHelper.Draw(elapsedSeconds);
        }        

        /// <summary>
        /// Input helper method provided by GameScreen.  Packages up the various input
        /// values for ease of use.  Here it checks for pausing and handles controlling
        /// the player's tank.
        /// </summary>
        /// <param name="input">The state of the gamepads</param>
        public override void HandleInput(InputState input)
        {
            Vector3 accelerationInfo = accelState == null ? Vector3.Zero : 
                new Vector3((float)accelState.X, (float)accelState.Y, (float)accelState.Z);

            if (gameplayHelper.HandleInput(input.PauseGame, accelerationInfo, TouchPanel.GetState()))
            {
                FinishCurrentGame();
            }
        }

        private void FinishCurrentGame()
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            ScreenManager.AddScreen(new BackgroundScreen());
            ScreenManager.AddScreen(new MainMenuScreen());
        }
    }    
}
