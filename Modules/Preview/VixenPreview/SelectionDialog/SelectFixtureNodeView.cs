using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.SelectionDialog
{
	/// <summary>
	/// Select a fixture node view.
	/// </summary>
	public partial class SelectFixtureNodeView
    {
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="node">Currently associated node</param>
		public SelectFixtureNodeView(ElementNode node)
		{
			// Initialize the window
			InitializeComponent();

			// Initialize the width and height of the window
			Width = 350;
			Height = 400;

			// Store off the node associated with the fixture shape
			_node = node;
		}

		#endregion

		#region Private Fields

		/// <summary>
		/// Element node associated with the fixture shape.
		/// This node is the default selected item in the selection dialog.
		/// </summary>
		private ElementNode _node;

		#endregion

		#region Protected Methods

		/// <summary>
		/// Initializes the associated view model.
		/// </summary>
		protected override void Initialize()
		{
			// Call the base class implementation
			base.Initialize();

			// Gets the view model from the Catel base class
			SelectFixtureNodeViewModel vm = (SelectFixtureNodeViewModel)ViewModel; 

			// Select the specified fixture node
			vm.SelectedItem = _node;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Gets the selected fixture node.
		/// </summary>
		/// <returns>Selected fixture node</returns>
		public ElementNode GetSelectedFixtureNode()
		{
			// Returns the selected fixture node
			return ((SelectFixtureNodeViewModel)ViewModel).SelectedItem;
		}

		#endregion
	}
}
