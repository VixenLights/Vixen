using Catel.MVVM;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Vixen.Sys;
using VixenModules.Property.IntelligentFixture;

namespace VixenModules.Preview.VixenPreview.SelectionDialog
{
	/// <summary>
	/// Maintains a view model for selecting fixture node from the Vixen fixture repository.
	/// </summary>
	public class SelectFixtureNodeViewModel : ViewModelBase                
    {
        #region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
        public SelectFixtureNodeViewModel()
        {
			// Create the collection of available nodes
            Nodes = new ObservableCollection<ElementNode>();

			// Add the nodes that contain a Intelligent Fixture property to the collection
			foreach (ElementNode node in VixenSystem.Nodes.Where(node => node.Properties.Any(prop => prop is IntelligentFixtureModule)))
			{
				// Add the node to the collection
				Nodes.Add(node);
			}
							
			// Create the OK button command 
			OkCommand = new Command(OK);

			// Create the Cancel button command
			CancelCommand = new Command(Cancel);
		}

        #endregion

        #region Public Properties

		/// <summary>
		/// Fixture nodes available for selection.
		/// </summary>
        public ObservableCollection<ElementNode> Nodes { get; set; }
		
		/// <summary>
		/// Selected fixture node.
		/// </summary>
		public ElementNode SelectedItem { get; set; }

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
