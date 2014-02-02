using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlienGameSample;

namespace AlienGame
{
    class MainMenuScreen : MenuScreen
    {
        public MainMenuScreen()
            : base("Main")
        {
            // Create our menu entries.
            MenuEntry startGameMenuEntry = new MenuEntry("START GAME");
            MenuEntry exitMenuEntry = new MenuEntry("QUIT");

            // Hook up menu event handlers.
            startGameMenuEntry.Selected += StartGameMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(startGameMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        void StartGameMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new GameplayScreen());
        }

        protected override void OnCancel()
        {
            ScreenManager.Game.Exit();
        }
    }
}
