using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using VixenModules.App.Polygon;

namespace PolygonEditor
{
	/// <summary>
	/// Interaction logic for PolygonEditorView.xaml
	/// </summary>
	public partial class PolygonEditorView
    {
		public PolygonEditorView()
		{
			InitializeComponent();

			//This should not be managed in code behind. This can be done in the ViewModel with event to command pattern
			this.KeyDown += MainWindow_KeyDown;

			//Catel can create the VM when the window is created and inject it. The VM should have a parameterless constructor
			//that creates the empty polygon list. Catel will set the VM as the DataContext on the Window. 
			//Child controls can use that DataContext in XAML to pass the VM or the property on it to them without doing it in code behind.
			//End result this code behind should be empty
			PolygonEditorViewModel vm = new PolygonEditorViewModel(new ObservableCollection<Polygon>());
			PolygonUserControl.VM = vm;
			PolygonUserControl.DataContext = vm;

			DataContext = vm;
		}

		/// <summary>
		/// Move to command in the VM
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{			
			e.Handled = PolygonUserControl.TryDeletePoint();

		}
	}
}
