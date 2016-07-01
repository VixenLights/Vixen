using System;
using System.Windows.Input;

namespace VixenModules.Editor.LayerEditor.Input
{
	public static class LayerEditorCommands
	{
		private static readonly Type ThisType = typeof(LayerEditorCommands);

		private static readonly RoutedUICommand _AddLayerCommand = new RoutedUICommand("Add Layer", "AddLayer", ThisType);
		private static readonly RoutedUICommand _RemoveLayerCommand = new RoutedUICommand("Remove Layer", "RemoveLayer", ThisType);


		/// <summary>
		/// Represents a command for the control to add a layer.
		/// </summary>
		public static RoutedUICommand AddLayer
		{
			get { return _AddLayerCommand; }
		}

		/// <summary>
		/// Represents a command for the control to remove a layer.
		/// </summary>
		public static RoutedUICommand RemoveLayer
		{
			get { return _RemoveLayerCommand; }
		}
	}
}
