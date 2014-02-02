using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework.Input.Touch;

namespace AlienGame
{
    public partial class GamePage : PhoneApplicationPage
    {
        ContentManager contentManager;
        GameTimer timer;
        SpriteBatch spriteBatch;

        // Input Members
        SensorReadingEventArgs<AccelerometerReading> accelState;
        Accelerometer Accelerometer;

        // Create a helper instance for handling game logic
        GameplayHelper gameplayHelper;

        bool backPressed;

        public GamePage()
        {            
            InitializeComponent();

            Accelerometer = new Accelerometer();
            if (Accelerometer.State == SensorState.Ready)
            {
                Accelerometer.CurrentValueChanged += (s, e) =>
                {
                    accelState = e;
                };
                Accelerometer.Start();
            }

            // Get the content manager from the application
            contentManager = (Application.Current as App).Content;

            // Create a timer for this page
            timer = new GameTimer();
            timer.UpdateInterval = TimeSpan.FromTicks(333333);
            timer.Update += OnUpdate;
            timer.Draw += OnDraw;

            this.BackKeyPress += new EventHandler<System.ComponentModel.CancelEventArgs>(GamePage_BackKeyPress);
        }

        void GamePage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            backPressed = true;
            e.Cancel = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            backPressed = false;

            InitializeGame();

            // Start the timer
            timer.Start();

            base.OnNavigatedTo(e);
        }

        private void InitializeGame()
        {
            gameplayHelper = new GameplayHelper(contentManager, spriteBatch, 
                SharedGraphicsDeviceManager.Current.GraphicsDevice);

            gameplayHelper.LoadContent();

            gameplayHelper.Start();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            gameplayHelper.UnloadContent();

            // Stop the timer
            timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Input helper method provided by GameScreen.  Packages up the various input
        /// values for ease of use.  Here it checks for pausing and handles controlling
        /// the player's tank.
        /// </summary>
        /// <param name="input">The state of the gamepads</param>
        public void HandleInput(bool shouldPause)
        {
            Vector3 accelerationInfo = accelState == null ? Vector3.Zero : accelState.SensorReading.Acceleration;

            if (gameplayHelper.HandleInput(shouldPause, accelerationInfo, TouchPanel.GetState()))
            {
                FinishCurrentGame();
            }
        }

        private void FinishCurrentGame()
        {
            backPressed = false;
            NavigationService.GoBack();
        }

        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            HandleInput(backPressed);

            float elapsedSeconds = (float)e.ElapsedTime.TotalSeconds;

            gameplayHelper.Update(elapsedSeconds);
        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            float elapsedSeconds = (float)e.ElapsedTime.TotalSeconds;

            gameplayHelper.Draw(elapsedSeconds);
        }
    }
}