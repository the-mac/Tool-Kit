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
    using System.Windows.Input;

	/// RelayCommand is a very easy-to-use implementation of ICommand. You can use a RelayCommand to expose viewmodel functionality as a command, and
    /// supply the condition that determines the command's availability. A control in the view bound to a command can execute an available command will
    /// update its enabled state in response to the availability of the command.
	public sealed class RelayCommand : ICommand
	{
		#region fields
		readonly Predicate<object> canExecute;
		readonly Action<object> execute;
		#endregion fields

		#region constructors
		public RelayCommand(Action<object> execute) : this(execute, null) { }

		public RelayCommand(Action<object> execute, Predicate<object> canExecute)
		{
			this.execute = execute;
			this.canExecute = canExecute;
		}
		#endregion constructors

		#region methods
		internal void RaiseCanExecuteChanged()
		{
			if (this.CanExecuteChanged != null)
			{
				this.CanExecuteChanged(this, EventArgs.Empty);
			}
		}
		#endregion methods

		#region ICommand events
		public event EventHandler CanExecuteChanged;
		#endregion ICommand events

		#region ICommand methods
		public bool CanExecute(object parameter)
		{
			return this.canExecute != null ? this.canExecute(parameter) : true;
		}

		public void Execute(object parameter)
		{
			if (this.execute != null)
			{
				this.execute(parameter);
			}
		}
		#endregion ICommand methods
	}
}
