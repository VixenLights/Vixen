using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module.App;

namespace VixenModules.App.ColorGradients
{
	public partial class ColorGradientEditor : Form
	{
		public ColorGradientEditor(ColorGradient gradient)
		{
			InitializeComponent();
			gradientEditPanel.GradientChanged += GradientChangedHandler;
			Gradient = gradient;
		}

		private ColorGradient _gradient;
		public ColorGradient Gradient
		{
			get
			{
				return _gradient;
			}
			set
			{
				_gradient = new ColorGradient(value);
				PopulateFormWithGradient(_gradient);
			}
		}

		private string _libraryItemName;
		public string LibraryItemName
		{
			get { return _libraryItemName; }
			set
			{
				_libraryItemName = value;
				PopulateFormWithGradient(Gradient);
			}
		}

		private ColorGradientLibrary _library;
		private ColorGradientLibrary Library
		{
			get
			{
				if (_library == null)
					_library = ApplicationServices.Get<IAppModuleInstance>(ColorGradientLibraryDescriptor.ModuleID) as ColorGradientLibrary;

				return _library;
			}
		}


		public void GradientChangedHandler(object sender, EventArgs e)
		{
			// the editor panel should be operating on a reference to our gradient, so we don't need to do anything.
		}


		private void PopulateFormWithGradient(ColorGradient item)
		{
			gradientEditPanel.Gradient = item;

			// if we're editing one from the library, treat it special
			if (item.IsCurrentLibraryGradient) {
				if (LibraryItemName == null) {
					labelCurve.Text = "This gradient is a library gradient.";
					Text = "Color Gradient Editor: Library Gradient";
				} else {
					labelCurve.Text = "This gradient is the library gradient: " + LibraryItemName;
					Text = "Color Gradient Editor: Library Gradient " + LibraryItemName;
				}

				gradientEditPanel.ReadOnly = false;
				buttonSaveToLibrary.Enabled = false;
				buttonLoadFromLibrary.Enabled = false;
				buttonUnlink.Enabled = false;
				buttonEditLibraryItem.Enabled = false;

			} else {
				if (item.IsLibraryReference) {
					labelCurve.Text = "This gradient is linked to the library: " + item.LibraryReferenceName;
				} else {
					labelCurve.Text = "This gradient is not linked to any in the library.";
				}

				gradientEditPanel.ReadOnly = item.IsLibraryReference;
				buttonSaveToLibrary.Enabled = !item.IsLibraryReference;
				buttonLoadFromLibrary.Enabled = true;
				buttonUnlink.Enabled = item.IsLibraryReference;
				buttonEditLibraryItem.Enabled = item.IsLibraryReference;

				Text = "Curve Editor";
			}

			gradientEditPanel.Invalidate();
		}



		private void buttonLoadFromLibrary_Click(object sender, EventArgs e)
		{
			ColorGradientLibrarySelector selector = new ColorGradientLibrarySelector();
			if (selector.ShowDialog() == System.Windows.Forms.DialogResult.OK && selector.SelectedItem != null) {
				// make a new curve that references the selected library curve, and set it to the current Curve
				ColorGradient newGradient = new ColorGradient(selector.SelectedItem.Item2);
				newGradient.LibraryReferenceName = selector.SelectedItem.Item1;
				newGradient.IsCurrentLibraryGradient = false;
				Gradient = newGradient;
			}

		}

		private void buttonSaveToLibrary_Click(object sender, EventArgs e)
		{
			Common.Controls.TextDialog dialog = new Common.Controls.TextDialog("Gradient name?");

			while (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				if (dialog.Response == "") {
					MessageBox.Show("Please enter a name.");
					continue;
				}

				if (Library.Contains(dialog.Response)) {
					DialogResult result = MessageBox.Show("There is already a gradient with that name. Do you want to overwrite it?",
						"Overwrite gradient?", MessageBoxButtons.YesNoCancel);
					if (result == System.Windows.Forms.DialogResult.Yes) {
						Library.AddColorGradient(dialog.Response, new ColorGradient(Gradient));
						break;
					} else if (result == System.Windows.Forms.DialogResult.Cancel) {
						break;
					}
				} else {
					Library.AddColorGradient(dialog.Response, new ColorGradient(Gradient));
					break;
				}
			}
		}

		private void buttonUnlink_Click(object sender, EventArgs e)
		{
			Gradient.UnlinkFromLibrary();
			PopulateFormWithGradient(Gradient);

		}

		private void buttonEditLibraryItem_Click(object sender, EventArgs e)
		{
			string libraryName = Gradient.LibraryReferenceName;
			Library.EditLibraryItem(libraryName);
			PopulateFormWithGradient(Gradient);
		}
	}
}
