using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module.App;

namespace VixenModules.App.Curves
{
	public partial class CurveLibrarySelector : BaseForm
	{
		public CurveLibrarySelector()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			listViewCurves.BackColor = ThemeColorTable.BackgroundColor;
			Icon = Resources.Icon_Vixen3;
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
				item.ForeColor = ThemeColorTable.ForeColor;
				listViewCurves.Items.Add(item);
			}

			listViewCurves.EndUpdate();

			buttonEditCurve.Enabled = false;
			buttonDeleteCurve.Enabled = false;
			buttonEditCurve.ForeColor = ThemeColorTable.ForeColorDisabled;
			buttonDeleteCurve.ForeColor = ThemeColorTable.ForeColorDisabled;
		}

		private void listViewCurves_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonEditCurve.Enabled = (listViewCurves.SelectedIndices.Count == 1);
			buttonDeleteCurve.Enabled = (listViewCurves.SelectedIndices.Count == 1);
			buttonEditCurve.ForeColor = buttonEditCurve.Enabled ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			buttonDeleteCurve.ForeColor = buttonDeleteCurve.Enabled ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
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
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Please enter a name.", "Curve Name", false, false);
					messageBox.ShowDialog();
					continue;
				}

				if (Library.Contains(dialog.Response))
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("There is already a curve with that name. Do you want to overwrite it?", "Overwrite curve?", true, true);
					messageBox.ShowDialog();
					if (messageBox.DialogResult == DialogResult.OK)
					{
						Library.AddCurve(dialog.Response, new Curve());
						Library.EditLibraryCurve(dialog.Response);
						PopulateListWithCurves();
						break;
					}
					if (messageBox.DialogResult == DialogResult.Cancel)
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
			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("If you delete this library curve, ALL places it is used will be unlinked and will become independent curves. Are you sure you want to continue?", "Delete library curve?", true, false);
			messageBox.ShowDialog();

			if (messageBox.DialogResult == DialogResult.OK) {
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
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;

		}
	}
}