/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
namespace SdkHelper
{
	using System;
	using System.ComponentModel;
	using System.Runtime.CompilerServices;
	using System.Windows;

	/// <summary>
	/// If you don't, or can't, derive from <see cref="PropertyChangedNotifier"/> or <see cref="PropertyChangedNotifierDO"/>,
	/// you can still use the PropertyChangedNotifierHelper class.
	/// </summary>
	public static class PropertyChangedNotifierHelper
	{
		#region methods
		public static bool SetProperty<T>(ref T target, T value)
		{
			if (Object.Equals(target, value)) return false;

			target = value;
			return true;
		}

		public static bool SetPropertyAndRaisePropertyChanged<T>(ref T target, T value, INotifyPropertyChanged thisReference, PropertyChangedEventHandler propertyChanged, [CallerMemberName] string propertyName = null)
		{
			if (Object.Equals(target, value)) return false;

			target = value;
			PropertyChangedNotifierHelper.RaisePropertyChanged(thisReference, propertyChanged, propertyName);
			return true;
		}

		public static void RaisePropertyChanged(INotifyPropertyChanged thisReference, PropertyChangedEventHandler propertyChanged, [CallerMemberName] string propertyName = null)
		{
			if (propertyChanged != null)
			{
				propertyChanged(thisReference, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion methods
	}

	/// <summary>
	/// Base class implementation of <see cref="INotifyPropertyChanged"/> for a type that doesn't also need to be a <see cref="DependencyObject"/>.
	/// </summary>
	public abstract class PropertyChangedNotifier : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged members
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion INotifyPropertyChanged members

		#region methods
		protected bool SetProperty<T>(ref T target, T value, [CallerMemberName] string propertyName = null)
		{
			return PropertyChangedNotifierHelper.SetProperty(ref target, value);
		}

		protected bool SetPropertyAndRaisePropertyChanged<T>(ref T target, T value, [CallerMemberName] string propertyName = null)
		{
			return PropertyChangedNotifierHelper.SetPropertyAndRaisePropertyChanged(ref target, value, this, this.PropertyChanged, propertyName);
		}

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedNotifierHelper.RaisePropertyChanged(this, this.PropertyChanged, propertyName);
		}
		#endregion methods
	}

	/// <summary>
	/// Base class implementation of <see cref="INotifyPropertyChanged"/> for a type that also needs to be a <see cref="DependencyObject"/>.
	/// </summary>
	public abstract class PropertyChangedNotifierDO : DependencyObject, INotifyPropertyChanged
	{
		#region INotifyPropertyChanged members
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion INotifyPropertyChanged members

		#region methods
		protected bool SetProperty<T>(ref T target, T value, [CallerMemberName] string propertyName = null)
		{
			return PropertyChangedNotifierHelper.SetProperty(ref target, value);
		}

		protected bool SetPropertyAndRaisePropertyChanged<T>(ref T target, T value, [CallerMemberName] string propertyName = null)
		{
			return PropertyChangedNotifierHelper.SetPropertyAndRaisePropertyChanged(ref target, value, this, this.PropertyChanged, propertyName);
		}

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedNotifierHelper.RaisePropertyChanged(this, this.PropertyChanged, propertyName);
		}
		#endregion methods
	}
}
