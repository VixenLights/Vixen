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

namespace VixenModules.App.Curves
{
	public partial class CurveLibrarySelector : Form
	{
		public CurveLibrarySelector()
		{
			InitializeComponent();
			Icon = Common.Resources.Properties.Resources.Icon_Vixen3;
		}

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

			listViewCurves.LargeImageList = new ImageList();

			foreach (KeyValuePair<string, Curve> kvp in Library) {
				Curve c = kvp.Value;
				string name = kvp.Key;

				listViewCurves.LargeImageList.ImageSize = new Size(64, 64);
				listViewCurves.LargeImageList.Images.Add(name, c.GenerateCurveImage(new Size(64, 64)));

				ListViewItem item = new ListViewItem();
				item.Text = name;
				item.Name = name;
				item.ImageKey = name;
				item.Tag = c;

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
				DialogResult = System.Windows.Forms.DialogResult.OK;
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
	}
}