using System.Collections.Generic;
using System.Collections.ObjectModel;
using VixenModules.App.Fixture;
using VixenModules.Editor.FixturePropertyEditor.ViewModels;

namespace VixenModules.Editor.FixturePropertyEditor.Views
{
    /// <summary>
    /// Select a fixture specification view.
    /// </summary>
    public partial class SelectFixtureSpecificationView
    {
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>		
		/// <param name="fixtures">Available fixture for selection</param>
		public SelectFixtureSpecificationView(IList<FixtureSpecification> fixtures)
		{
			// Initialize the window
			InitializeComponent();

			// Initialize the width and height of the window
			Width = 350;
			Height = 400;

			// Store off the available fixture specifications
			_fixtures = fixtures;							
		}

		#endregion

		#region Private Fields

		/// <summary>
		/// Collection of available fixture specifications.
		/// </summary>
		private IList<FixtureSpecification> _fixtures;

        #endregion

        #region Protected Methods

		/// <summary>
		/// Initializes the associated view model.
		/// </summary>
        protected override void Initialize()
		{
			// Call the base class implementation
			base.Initialize();

			// Give the view model the available fixture specifications
			(ViewModel as SelectFixtureSpecificationViewModel).Specifications = new ObservableCollection<FixtureSpecification>(_fixtures);

			// If there is at least one fixture specification then...
            if (_fixtures.Count > 0)
            {
				// Select the first fixture specification
				(ViewModel as SelectFixtureSpecificationViewModel).SelectedItem = _fixtures[0];
            }            
        }

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the selected fixture specification.
		/// </summary>
		/// <returns>Selected fixture specification</returns>
		public FixtureSpecification GetSelectedFixtureSpecification()
		{
			// Returns the selected fixture specification
			return (ViewModel as SelectFixtureSpecificationViewModel).SelectedItem;
		}

		#endregion
	}
}
