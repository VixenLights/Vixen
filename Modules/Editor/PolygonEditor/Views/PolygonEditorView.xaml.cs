using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using VixenModules.App.Polygon;

namespace PolygonEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class PolygonEditorView
    {
		public PolygonEditorView()
		{
			InitializeComponent();
			this.KeyDown += MainWindow_KeyDown;

			PolygonEditorViewModel vm = new PolygonEditorViewModel(new ObservableCollection<Polygon>());
			PolygonUserControl.VM = vm;
			PolygonUserControl.DataContext = vm;

			DataContext = vm;
		}

		private void MainWindow_KeyDown(object sender, KeyEventArgs e)
		{			
			e.Handled = PolygonUserControl.TryDeletePoint();

		}
	}
}
