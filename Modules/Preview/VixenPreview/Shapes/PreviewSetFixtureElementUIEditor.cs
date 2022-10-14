using System;
using System.ComponentModel;
using System.Drawing.Design;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.SelectionDialog;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	/// <summary>
	/// UI Editor for associating an element node with a fixture graphic.
	/// </summary>
	internal class PreviewSetFixtureElementUIEditor : UITypeEditor
	{
		#region Public Methods

		/// <inheritdoc/>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			// Configure the property editor to be modal
			return UITypeEditorEditStyle.Modal;
		}

		/// <summary>
		/// Edits the Linked Element associated with the fixture shape (graphic).
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value">Element node being associated</param>
		/// <returns>Updated element node value</returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			// Cast the current value to an ElementNode
			ElementNode node = value as ElementNode;

			// Create the view to select the fixture to associate with the shape
			SelectFixtureNodeView nodeSelectionView = new SelectFixtureNodeView(node);
			
			// Display the selection dialog
			bool? selectedOK = nodeSelectionView.ShowDialog();

			// If the user has selected OK button then...
			if (selectedOK.HasValue && selectedOK.Value)
			{
				// Update the selected element node
				value = nodeSelectionView.GetSelectedFixtureNode();
			}

			// Return the updated value
			return value;
		}

		#endregion
	}
}