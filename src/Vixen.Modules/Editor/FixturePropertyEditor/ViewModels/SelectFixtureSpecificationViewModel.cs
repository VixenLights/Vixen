using Catel.MVVM;
using System.Collections.ObjectModel;
using System.Windows.Input;
using VixenModules.App.Fixture;

namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
	/// <summary>
	/// Maintains a view model for selecting fixture specfications from the Vixen fixture repository.
	/// </summary>
    public class SelectFixtureSpecificationViewModel : ViewModelBase                
    {
        #region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
        public SelectFixtureSpecificationViewModel()
        {
			// Create the collection of available fixtures
            Specifications = new ObservableCollection<FixtureSpecification>();

			// Create the OK button command 
			OkCommand = new Command(OK);

			// Create the Cancel button command
			CancelCommand = new Command(Cancel);
		}

        #endregion

        #region Public Properties

		/// <summary>
		/// Fixture specifications available for selection.
		/// </summary>
        public ObservableCollection<FixtureSpecification> Specifications { get; set; }

		/// <summary>
		/// Selected fixture specification.
		/// </summary>
        public FixtureSpecification SelectedItem { get; set; }

		/// <summary>
		/// Ok button command.
		/// </summary>
		public ICommand OkCommand { get; private set; }

		/// <summary>
		/// Cancel button command.
		/// </summary>
		public ICommand CancelCommand { get; private set; }

        #endregion

        #region Private Methods

		/// <summary>
		/// Method to invoke when the OK command is executed.
		/// </summary>
        private void OK()
		{
			// Call Catel processing
			this.SaveAndCloseViewModelAsync();
		}

		/// <summary>
		/// Method to invoke when the Cancel command is executed.
		/// </summary>
		private void Cancel()
		{
			// Call Catel processing
			this.CancelAndCloseViewModelAsync();
		}

		#endregion
	}
}
