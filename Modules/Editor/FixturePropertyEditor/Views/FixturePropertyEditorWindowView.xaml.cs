using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using Catel.MVVM;
using VixenModules.App.Fixture;
using VixenModules.Editor.FixturePropertyEditor.ViewModels;

namespace VixenModules.Editor.FixturePropertyEditor.Views
{
    /// <summary>
    /// Maintains the fixture property window view.
    /// </summary>
    public partial class FixturePropertyEditorWindowView
    {
		#region Constructor
				
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="fixtureSpecification">Fixture specification being edited</param>
		public FixturePropertyEditorWindowView(FixtureSpecification fixtureSpecification)
		{
			InitializeComponent();

			// Save off the fixture specification 
			_fixtureSpecification = fixtureSpecification;

			// Initialize the width and height of the window
			Width = 650;
			Height = 500;						
		}

		#endregion

		#region Private Fields

		/// <summary>
		/// Fixture specification being edited.
		/// </summary>
		private FixtureSpecification _fixtureSpecification;

        #endregion

        #region Private Methods

		/// <summary>
		/// Event for when the window is loaded.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Cast the Catel view model to a FixturePropertyEditorWindowViewModel view model
			FixturePropertyEditorWindowViewModel vm = (FixturePropertyEditorWindowViewModel)ViewModel;
			
			// Give the view model the fixture being edited
			(ViewModel as FixturePropertyEditorWindowViewModel).FixtureSpecification =
				new Tuple<FixtureSpecification, Action, bool>(_fixtureSpecification,
					((Command)vm.OkCommand).RaiseCanExecuteChanged, true);
		}

		/// <summary>
		/// Displays the associated VixenLights help page.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			// Launch a browser with the URL
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the fixture specification being edited.
		/// </summary>
		/// <returns>Fixture specification being edited</returns>
		public FixtureSpecification GetFixtureSpecification()
		{
			// Get the fixture specification from the user control view model
			return (ViewModel as FixturePropertyEditorWindowViewModel).FixtureSpecification.Item1;
		}

		#endregion
	}
}
