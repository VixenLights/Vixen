using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module.App;

namespace VixenModules.App.Curves
{
	public partial class CurveLibrarySelector : Form
	{
		public CurveLibrarySelector()
		{
			InitializeComponent();
			buttonCancel.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonDeleteCurve.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonEditCurve.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonNewCurve.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonOK.BackgroundImage = Resources.HeadingBackgroundImage;
			Icon = Common.Resources.Properties.Resources.Icon_Vixen3;
			DoubleClickMode = Mode.Ok;
		}

		/// <summary>
		/// Change the effect of double clicking on a curve. Ok invokes the Ok button, Edit invokes the Edit button.
		/// </summary>
		public Mode DoubleClickMode { get; set; }

		private void CurveLibrarySelector_Load(object sender, EventArgs e)
		{
			PopulateListWithCurves();
			CurveLibraryStaticData data;
			data =
				(ApplicationServices.Get<IAppModuleInstance>(CurveLibraryDescriptor.ModuleID) as CurveLibrary).StaticModuleData as
				CurveLibraryStaticData;
			if (Screen.GetWorkingArea(this).Contains(data.SelectorWindowBounds) &&
			    data.SelectorWindowBounds.Width >= MinimumSize.Width) {
				Bounds = data.SelectorWindowBounds;
			}
		}

		private void CurveLibrarySelector_FormClosing(object sender, FormClosingEventArgs e)
		{
			CurveLibraryStaticData data;
			data =
				(ApplicationServices.Get<IAppModuleInstance>(CurveLibraryDescriptor.ModuleID) as CurveLibrary).StaticModuleData as
				CurveLibraryStaticData;
			data.SelectorWindowBounds = Bounds;
		}

		private void PopulateListWithCurves()
		{
			listViewCurves.BeginUpdate();
			listViewCurves.Items.Clear();

			listViewCurves.LargeImageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit, ImageSize = new Size(68, 68) };

			foreach (KeyValuePair<string, Curve> kvp in Library)
			{
				Curve c = kvp.Value;
				string name = kvp.Key;

				var image = c.GenerateGenericCurveImage(new Size(68, 68));
				Graphics gfx = Graphics.FromImage(image);
				gfx.DrawRectangle(new Pen(Color.FromArgb(136, 136, 136), 2), 0, 0, 68, 68);

				listViewCurves.LargeImageList.Images.Add(name, image);

				ListViewItem item = new ListViewItem { Text = name, Name = name, ImageKey = name, Tag = c };
				item.ForeColor = DarkThemeColorTable.ForeColor;
				listViewCurves.Items.Add(item);
			}

			listViewCurves.EndUpdate();

			buttonEditCurve.Enabled = false;
			buttonDeleteCurve.Enabled = false;
		}

		private void listViewCurves_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonEditCurve.Enabled = (listViewCurves.SelectedIndices.Count == 1);
			buttonDeleteCurve.Enabled = (listViewCurves.SelectedIndices.Count == 1);
		}

		public Tuple<string, Curve> SelectedItem
		{
			get
			{
				if (listViewCurves.SelectedItems.Count == 0)
					return null;

				return new Tuple<string, Curve>(listViewCurves.SelectedItems[0].Name, listViewCurves.SelectedItems[0].Tag as Curve);
			}
		}

		private void buttonNewCurve_Click(object sender, EventArgs e)
		{
			Common.Controls.TextDialog dialog = new Common.Controls.TextDialog("Curve name?");

			while (dialog.ShowDialog() == DialogResult.OK)
			{
				if (dialog.Response == string.Empty)
				{
					MessageBox.Show("Please enter a name.");
					continue;
				}

				if (Library.Contains(dialog.Response))
				{
					DialogResult result = MessageBox.Show("There is already a curve with that name. Do you want to overwrite it?",
														  "Overwrite curve?", MessageBoxButtons.YesNoCancel);
					if (result == DialogResult.Yes)
					{
						Library.AddCurve(dialog.Response, new Curve());
						Library.EditLibraryCurve(dialog.Response);
						PopulateListWithCurves();
						break;
					}
					else if (result == DialogResult.Cancel)
					{
						break;
					}
				}
				else
				{
					Library.AddCurve(dialog.Response, new Curve());
					Library.EditLibraryCurve(dialog.Response);
					PopulateListWithCurves();
					break;
				}
			}
		}

		private void buttonEditCurve_Click(object sender, EventArgs e)
		{
			if (listViewCurves.SelectedItems.Count != 1)
				return;

			Library.EditLibraryCurve(listViewCurves.SelectedItems[0].Name);

			PopulateListWithCurves();
		}

		private void buttonDeleteCurve_Click(object sender, EventArgs e)
		{
			if (listViewCurves.SelectedItems.Count == 0)
				return;

			DialogResult result =
				MessageBox.Show("If you delete this library curve, ALL places it is used will be unlinked and will" +
				                " become independent curves. Are you sure you want to continue?", "Delete library curve?",
				                MessageBoxButtons.YesNoCancel);

			if (result == System.Windows.Forms.DialogResult.Yes) {
				Library.RemoveCurve(listViewCurves.SelectedItems[0].Name);
				PopulateListWithCurves();
			}
		}

		private void listViewCurves_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (listViewCurves.SelectedItems.Count == 1)
			{
				if (DoubleClickMode.Equals(Mode.Ok))
				{
					DialogResult = DialogResult.OK;
				}
				
				buttonEditCurve.PerformClick();
			}
				
		}

		private CurveLibrary _library;

		private CurveLibrary Library
		{
			get
			{
				if (_library == null)
					_library = ApplicationServices.Get<IAppModuleInstance>(CurveLibraryDescriptor.ModuleID) as CurveLibrary;

				return _library;
			}
		}

		private void CurveLibrarySelector_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
				DialogResult = DialogResult.OK;
			if (e.KeyCode == Keys.Escape)
				DialogResult = DialogResult.Cancel;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				listViewCurves.Dispose();
				components.Dispose();
			}

			base.Dispose(disposing);
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

		private void buttonTextColorChange(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.ForeColor = btn.Enabled ? DarkThemeColorTable.ForeColor : DarkThemeColorTable.ForeColorDisabled;
		}

	}
}