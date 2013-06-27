using System.Windows;
using System.Windows.Controls;
using System;

namespace VixenModules.Preview.DisplayPreview.Views
{
	public partial class NodeEditorView
	{
		public NodeEditorView()
		{
			InitializeComponent();
		}

		private void CloseButtonClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void radioBtnWrite_Click(object sender, RoutedEventArgs e)
		{
			FrameworkElement fe = sender as FrameworkElement;
			InkCanvas canvas = fe.FindName("inkCanvas") as InkCanvas;
			canvas.EditingMode = InkCanvasEditingMode.Ink;
		}

		private void radioBtnErase_Click(object sender, RoutedEventArgs e)
		{
			FrameworkElement fe = sender as FrameworkElement;
			InkCanvas canvas = fe.FindName("inkCanvas") as InkCanvas;
			canvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
		}
	}
}