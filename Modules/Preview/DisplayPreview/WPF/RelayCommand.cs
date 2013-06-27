namespace VixenModules.Preview.DisplayPreview.WPF
{
	using System;
	using System.Diagnostics;
	using System.Windows.Input;

	/// <summary>
	///   A command whose sole purpose is to 
	///   relay its functionality to other
	///   objects by invoking delegates. The
	///   default return value for the CanExecute
	///   method is 'true'.
	/// </summary>
	public class RelayCommand : ICommand
	{
		private readonly Predicate<object> _canExecute;
		private readonly Action<object> _execute;

		/// <summary>
		///   Initializes a new instance of the <see cref = "RelayCommand" /> class. 
		///   Creates a new command.
		/// </summary>
		/// <param name = "execute">
		///   The execution logic.
		/// </param>
		/// <param name = "canExecute">
		///   The execution status logic.
		/// </param>
		public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
		{
			if (execute == null) {
				throw new ArgumentNullException("execute");
			}

			_execute = execute;
			_canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }

			remove { CommandManager.RequerySuggested -= value; }
		}

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _canExecute == null ? true : _canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}
	}
}