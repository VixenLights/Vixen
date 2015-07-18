using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Resources.Properties;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module.App;

namespace VixenModules.App.ColorGradients
{
	public partial class ColorGradientLibrarySelector : Form
	{
		public ColorGradientLibrarySelector()
		{
			InitializeComponent();
			buttonCancel.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonDeleteColorGradient.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonEditColorGradient.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonNewColorGradient.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonOK.BackgroundImage = Resources.HeadingBackgroundImage;
			Icon = Common.Resources.Properties.Resources.Icon_Vixen3;
			DoubleClickMode = Mode.Ok;
		}

		/// <summary>
		/// Change the effect of double clicking on a curve. Ok invokes the Ok button, Edit invokes the Edit button.
		/// </summary>
		public Mode DoubleClickMode { get; set; }

		private void ColorGradientLibrarySelector_Load(object sender, EventArgs e)
		{
			PopulateListWithColorGradients();
			ColorGradientLibraryStaticData data;
			data =
				(ApplicationServices.Get<IAppModuleInstance>(ColorGradientLibraryDescriptor.ModuleID) as ColorGradientLibrary).
					StaticModuleData as ColorGradientLibraryStaticData;
			if (Screen.GetWorkingArea(this).Contains(data.SelectorWindowBounds) &&
			    data.SelectorWindowBounds.Width >= MinimumSize.Width) {
				Bounds = data.SelectorWindowBounds;
			}
		}

		private void ColorGradientLibrarySelector_FormClosing(object sender, FormClosingEventArgs e)
		{
			ColorGradientLibraryStaticData data;
			data =
				(ApplicationServices.Get<IAppModuleInstance>(ColorGradientLibraryDescriptor.ModuleID) as ColorGradientLibrary).
					StaticModuleData as ColorGradientLibraryStaticData;
			data.SelectorWindowBounds = Bounds;
		}

		private void PopulateListWithColorGradients()
		{
			listViewColorGradients.BeginUpdate();
			listViewColorGradients.Items.Clear();

			listViewColorGradients.LargeImageList = new ImageList();
			listViewColorGradients.LargeImageList.ColorDepth = ColorDepth.Depth32Bit;

			foreach (KeyValuePair<string, ColorGradient> kvp in Library) {
				ColorGradient gradient = kvp.Value;
				string name = kvp.Key;

				listViewColorGradients.LargeImageList.ImageSize = new Size(64, 64);
				listViewColorGradients.LargeImageList.Images.Add(name, gradient.GenerateColorGradientImage(new Size(64, 64), false));

				ListViewItem item = new ListViewItem();
				item.Text = name;
				item.Name = name;
				item.ImageKey = name;
				item.Tag = gradient;

				listViewColorGradients.Items.Add(item);
			}

			listViewColorGradients.EndUpdate();

			buttonEditColorGradient.Enabled = false;
			buttonDeleteColorGradient.Enabled = false;
		}

		private void listViewColorGradients_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonEditColorGradient.Enabled = (listViewColorGradients.SelectedIndices.Count == 1);
			buttonDeleteColorGradient.Enabled = (listViewColorGradients.SelectedIndices.Count == 1);
		}

		public Tuple<string, ColorGradient> SelectedItem
		{
			get
			{
				if (listViewColorGradients.SelectedItems.Count == 0)
					return null;

				return new Tuple<string, ColorGradient>(listViewColorGradients.SelectedItems[0].Name,
				                                        listViewColorGradients.SelectedItems[0].Tag as ColorGradient);
			}
		}

		private void buttonNewColorGradient_Click(object sender, EventArgs e)
		{
			Common.Controls.TextDialog dialog = new Common.Controls.TextDialog("Gradient name?");

			while (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				if (dialog.Response == string.Empty)
				{
					MessageBox.Show("Please enter a name.");
					continue;
				}

				if (Library.Contains(dialog.Response))
				{
					DialogResult result = MessageBox.Show("There is already a gradient with that name. Do you want to overwrite it?",
														  "Overwrite gradient?", MessageBoxButtons.YesNoCancel);
					if (result == System.Windows.Forms.DialogResult.Yes)
					{
						Library.AddColorGradient(dialog.Response, new ColorGradient());
						Library.EditLibraryItem(dialog.Response);
						PopulateListWithColorGradients();
						break;
					}
					else if (result == System.Windows.Forms.DialogResult.Cancel)
					{
						break;
					}
				}
				else
				{
					Library.AddColorGradient(dialog.Response, new ColorGradient());
					Library.EditLibraryItem(dialog.Response);
					PopulateListWithColorGradients();
					break;
				}
			}
		}

		private void buttonEditColorGradient_Click(object sender, EventArgs e)
		{
			if (listViewColorGradients.SelectedItems.Count != 1)
				return;

			Library.EditLibraryItem(listViewColorGradients.SelectedItems[0].Name);

			PopulateListWithColorGradients();
		}

		private void buttonDeleteColorGradient_Click(object sender, EventArgs e)
		{
			if (listViewColorGradients.SelectedItems.Count == 0)
				return;

			DialogResult result =
				MessageBox.Show("If you delete this library gradient, ALL places it is used will be unlinked and will" +
				                " become independent gradients. Are you sure you want to continue?", "Delete library gradient?",
				                MessageBoxButtons.YesNoCancel);

			if (result == System.Windows.Forms.DialogResult.Yes) {
				Library.RemoveColorGradient(listViewColorGradients.SelectedItems[0].Name);
				PopulateListWithColorGradients();
			}
		}

		private void listViewColorGradients_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (listViewColorGradients.SelectedItems.Count == 1)
			{
				if (DoubleClickMode.Equals(Mode.Ok))
				{
					DialogResult = DialogResult.OK;
				}

				buttonEditColorGradient.PerformClick();
			}
		}

		private ColorGradientLibrary _library;

		private ColorGradientLibrary Library
		{
			get
			{
				if (_library == null)
					_library =
						ApplicationServices.Get<IAppModuleInstance>(ColorGradientLibraryDescriptor.ModuleID) as ColorGradientLibrary;

				return _library;
			}
		}

		private void ColorGradientLibrarySelector_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				DialogResult = DialogResult.OK;
			if (e.KeyCode == Keys.Escape)
				DialogResult = DialogResult.Cancel;
		}

		public enum Mode
		{
			Ok,
			Edit
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.HeadingBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.HeadingBackgroundImage;
		}

	}
}