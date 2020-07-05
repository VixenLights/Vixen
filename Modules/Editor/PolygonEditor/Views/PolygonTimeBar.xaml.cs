using System.Windows;
using System.Windows.Controls;
using VixenModules.Editor.PolygonEditor.ViewModels;

namespace VixenModules.Editor.PolygonEditor.Views
{
    /// <summary>
    /// Interaction logic for PolygonTimeBar.xaml
    /// </summary>
    public partial class PolygonTimeBar : UserControl
	{
		#region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
		public PolygonTimeBar()
		{
			InitializeComponent();
            Loaded += PolygonTimeBar_Loaded;            
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// View model for the polygon editor.
        /// </summary>
        public PolygonEditorViewModel VM { get; set; }

		#endregion

		#region Private Methods

        /// <summary>
        /// Control loaded event handler.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
		private void PolygonTimeBar_Loaded(object sender, RoutedEventArgs e)
        {
            //Hack this to fit the existing property but this is not ideal. This control should have its own view model and then
            //the control can inherit from Catel:UserControl and Catel can inject that VM behind this control.
            //That VM will have a parent of the VM being used here and then you can walk the ladder if needed.
            //However VM should be mostly self contained and not need to rely on their parents. They can expose functions that the
            //parents can interact with when needed.
            if (DataContext is PolygonEditorViewModel vm)
            {
                VM = vm;                
            }            
        }

        #endregion
    }
}
