using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Navigation;
using VixenModules.App.Fixture;
using VixenModules.Editor.FixturePropertyEditor.ViewModels;

namespace VixenModules.Editor.FixturePropertyEditor.Views
{
    /// <summary>
    /// Maintains a window that hosts the fixture function type editor.
    /// </summary>
    public partial class FunctionTypeWindowView
    {
		#region Constructor
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="functions">Fixture functions to edit</param>
		/// <param name="functionToSelect">Function to select initially</param>
		public FunctionTypeWindowView(List<FixtureFunction> functions, string functionToSelect)
		{
			InitializeComponent();

			// Initial size of the window
			Width = 1165;
			Height = 550;

			// Store off the fixture functions
			_functions = functions;

			// Store off the function to initially select
			_functionToSelect = functionToSelect;						
		}

		#endregion

		#region Private Fields

		/// <summary>
		/// Fixture function model data being edited.
		/// </summary>
		private List<FixtureFunction> _functions;

		/// <summary>
		/// Function to select initially.
		/// </summary>
		private string _functionToSelect;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initializes the window.
        /// </summary>
        protected override void Initialize()
		{
			// Call the base class implementation
			base.Initialize();
			
			// Initialize the view model with the functions and the function to select initially
			(ViewModel as FunctionTypeWindowViewModel).InitializeChildViewModels(_functions, _functionToSelect);				
		}
       		
		#endregion
		
		#region Public Methods

		/// <summary>
		/// Get the edited function model data.
		/// </summary>
		/// <returns></returns>
		public List<FixtureFunction> GetFunctionData()
		{
			// Retrieve the edited function data from the view model.
			return (ViewModel as FunctionTypeWindowViewModel).UpdatedFunctions;
		}

		#endregion

		#region Private Methods

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
	}
}
