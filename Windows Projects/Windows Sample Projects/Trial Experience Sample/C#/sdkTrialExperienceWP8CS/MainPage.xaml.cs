/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
namespace sdkTrialExperienceWP8CS
{
	using Microsoft.Phone.Controls;
	using SdkTrialExperienceCS;
	using sdkTrialExperienceWP8CS.Resources;
	using System;

	public partial class MainPage : PhoneApplicationPage
	{
		// Constructor
		public MainPage()
		{
			InitializeComponent();

			this.InitializeApplicationBar();
		}

		// The application bar controls are not bound to viewmodel properties, so imperatively set their text to localizable strings.
		private void InitializeApplicationBar()
		{
			this.MainViewModel.BuyCommand.CanExecuteChanged += MainViewModelBuyCommand_CanExecuteChanged;
			// Explicit initial call to the event handler to initialize the enabled state of the application bar controls.
			this.MainViewModelBuyCommand_CanExecuteChanged(this, EventArgs.Empty);

			if (this.ApplicationBar != null && this.ApplicationBar.Buttons.Count != 0 && this.ApplicationBar.MenuItems.Count != 0)
			{
				(this.ApplicationBar.Buttons[0] as Microsoft.Phone.Shell.ApplicationBarIconButton).Text = AppResources.BuyString;
				(this.ApplicationBar.MenuItems[0] as Microsoft.Phone.Shell.ApplicationBarMenuItem).Text = AppResources.BuyString;
			}
		}

		// Handle the viewmodel BuyCommand's CanExecuteChanged event to update the enabled state of the application bar controls, which are not bound to the command.
		void MainViewModelBuyCommand_CanExecuteChanged(object sender, EventArgs e)
		{
			if (this.ApplicationBar != null && this.ApplicationBar.Buttons.Count != 0 && this.ApplicationBar.MenuItems.Count != 0)
			{
				(this.ApplicationBar.Buttons[0] as Microsoft.Phone.Shell.ApplicationBarIconButton).IsEnabled = this.MainViewModel.BuyCommand.CanExecute(null);
				(this.ApplicationBar.MenuItems[0] as Microsoft.Phone.Shell.ApplicationBarMenuItem).IsEnabled = this.MainViewModel.BuyCommand.CanExecute(null);
			}
		}

		// Although the application bar controls are not bound to the viewmodel BuyCommand, we can still execute it via an event handler.
		private void ApplicationBar_Buy_Click(object sender, EventArgs e)
		{
			// A command takes a parameter and in this case we can pass null.
			this.MainViewModel.BuyCommand.Execute(null);
		}

		// A convenient helper property to access a typecast reference to the DataContext. The DataContext is set in markup to an instance
		// of MainViewModel, and this cast is correct as long as that markup remains intact.
		private MainViewModel MainViewModel
		{
			get
			{
				return this.DataContext as MainViewModel;
			}
		}
	}
}
