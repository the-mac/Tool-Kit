/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
namespace SdkTrialExperienceCS
{
	using SdkHelper;
	using sdkTrialExperienceWP8CS.Resources;

	public class MainViewModel : PropertyChangedNotifier
	{
		#region fields
		private RelayCommand buyCommand;
		#endregion fields

		#region constructors
		public MainViewModel()
		{
			// Subscribe to the helper class's static LicenseChanged event so that we can re-query its LicenseMode property when it changes.
			TrialExperienceHelper.LicenseChanged += TrialExperienceHelper_LicenseChanged;
		}
		#endregion constructors

		#region properties
		public string AppName
		{
			get
			{
				return AppResources.ApplicationTitle;
			}
		}

		/// <summary>
		/// You can bind the Command property of a Button to BuyCommand. When the Button is clicked, BuyCommand will be
		/// invoked. The Button will be enabled as long as BuyCommand can execute.
		/// </summary>
		public RelayCommand BuyCommand
		{
			get
			{
				if (this.buyCommand == null)
				{
					// The RelayCommand is constructed with two parameters - the action to perform on invocation,
					// and the condition under which the command can execute. It's important to call RaiseCanExecuteChanged
					// on a command whenever its can-execute condition might have changed. Here, we do that in the TrialExperienceHelper_LicenseChanged
					// event handler.
					this.buyCommand = new RelayCommand(
						param => TrialExperienceHelper.Buy(),
						param => TrialExperienceHelper.LicenseMode == TrialExperienceHelper.LicenseModes.Trial);
				}
				return this.buyCommand;
			}
		}

		public string BehaviorMessage
		{
			get
			{
#if DEBUG
				return string.Format(AppResources.BehaviorMessageDebug, TrialExperienceHelper.simulatedLicMode);
#else // DEBUG
				return AppResources.BehaviorMessageRelease;
#endif // DEBUG
			}
		}

		public string BuyAffordancesMessage
		{
			get
			{
				return AppResources.BuyAffordancesMessage;
			}
		}

		public string ConfigurationMessage
		{
			get
			{
#if DEBUG
				return AppResources.ConfigurationMessageDebug;
#else // DEBUG
				return AppResources.ConfigurationMessageRelease;
#endif // DEBUG
			}
		}

		public string LicenseModeString
		{
			get
			{
				return TrialExperienceHelper.LicenseMode.ToString() + ' ' + AppResources.ModeString;
			}
		}

		public string ToPurchaseMessage
		{
			get
			{
#if DEBUG
				return AppResources.ToPurchaseMessageDebug;
#else // DEBUG
				return AppResources.ToPurchaseMessageRelease;
#endif // DEBUG
			}
		}
		#endregion properties

		#region event handlers
		// Handle TrialExperienceHelper's LicenseChanged event by raising property changed notifications on the
		// properties and commands that 
		internal void TrialExperienceHelper_LicenseChanged()
		{
			this.RaisePropertyChanged("ExpirationDateString");
			this.RaisePropertyChanged("LicenseModeString");
			this.BuyCommand.RaiseCanExecuteChanged();
		}
		#endregion event handlers
	}
}
