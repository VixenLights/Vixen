using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module.App;

namespace VixenModules.App.LipSyncApp
{
    public partial class LipSyncMapSelector : Form
    {
        private Bitmap _iconBitmap;

        public LipSyncMapSelector()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
            listViewMappings.Sorting = SortOrder.Ascending;
			Icon = Resources.Icon_Vixen3;
            Changed = false;
		}

        public bool Changed { get; set; }

		private void LipSyncMapSelector_Load(object sender, EventArgs e)
		{
            Assembly assembly = Assembly.Load("LipSyncApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            if (assembly != null)
            {
                ResourceManager lipSyncRM = new ResourceManager("VixenModules.App.LipSyncApp.LipSyncResources", assembly);
                _iconBitmap = new Bitmap((Image)lipSyncRM.GetObject("AI"), new Size(64, 64));
            }

			PopulateListWithMappings();
			LipSyncMapStaticData data;
			data =
				(ApplicationServices.Get<IAppModuleInstance>(LipSyncMapDescriptor.ModuleID) as LipSyncMapLibrary).StaticModuleData as
				LipSyncMapStaticData;
			if (Screen.GetWorkingArea(this).Contains(data.SelectorWindowBounds) &&
			    data.SelectorWindowBounds.Width >= MinimumSize.Width) {
				Bounds = data.SelectorWindowBounds;
			}
            listViewMappings.Activation = ItemActivation.Standard;
        }

		private void LipSyncMapSelector_FormClosing(object sender, FormClosingEventArgs e)
		{
			LipSyncMapStaticData data;
			data =
				(ApplicationServices.Get<IAppModuleInstance>(LipSyncMapDescriptor.ModuleID) as LipSyncMapLibrary).StaticModuleData as
				LipSyncMapStaticData;
			data.SelectorWindowBounds = Bounds;
		}

		private void PopulateListWithMappings()
		{
			listViewMappings.BeginUpdate();
			listViewMappings.Items.Clear();

			listViewMappings.LargeImageList = new ImageList();

			foreach (KeyValuePair<string, LipSyncMapData> kvp in Library) {
                LipSyncMapData c = kvp.Value;
				string name = kvp.Key;

				listViewMappings.LargeImageList.ImageSize = new Size(64, 64);
				listViewMappings.LargeImageList.Images.Add(name, _iconBitmap);

				ListViewItem item = new ListViewItem();
				item.Text = name;
				item.Name = name;
				item.ImageKey = name;
				item.Tag = c;
                if (_library.DefaultMappingName.Equals(name))
                {
                    item.Font = new Font(item.Font, FontStyle.Bold);
                }
                
				listViewMappings.Items.Add(item);

			}
			listViewMappings.EndUpdate();

			//listViewMappings.LargeImageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit, ImageSize = new Size(68, 68) };

			//foreach (KeyValuePair<string, LipSyncMapData> kvp in Library)
			//{
			//	LipSyncMapData c = kvp.Value;
			//	string name = kvp.Key;

			//	var image = c.cGenerateGenericCurveImage(new Size(68, 68));
			//	Graphics gfx = Graphics.FromImage(image);
			//	gfx.VixenModules.App.Curves.DrawRectangle(new Pen(Color.FromArgb(136, 136, 136), 2), 0, 0, 68, 68);

			//	listViewMappings.LargeImageList.Images.Add(name, image);

			//	ListViewItem item = new ListViewItem { Text = name, Name = name, ImageKey = name, Tag = c };
			//	item.ForeColor = DarkThemeColorTable.ForeColor;
			//	listViewMappings.Items.Add(item);
			//}

			//listViewMappings.EndUpdate();

            buttonNewMap.Enabled = true;
            buttonEditMap.Enabled = false;
			buttonDeleteMap.Enabled = false;
            buttonCloneMap.Enabled = false;

		}

		private void listViewMappings_SelectedIndexChanged(object sender, EventArgs e)
		{
            buttonEditMap.Enabled = (listViewMappings.SelectedIndices.Count == 1);
            buttonCloneMap.Enabled = (listViewMappings.SelectedIndices.Count >= 1);
            buttonDeleteMap.Enabled = (listViewMappings.SelectedIndices.Count >= 1);
		}

		public Tuple<string, LipSyncMapItem> SelectedItem
		{
			get
			{
				if (listViewMappings.SelectedItems.Count == 0)
					return null;

                return new Tuple<string, LipSyncMapItem>(listViewMappings.SelectedItems[0].Name, listViewMappings.SelectedItems[0].Tag as LipSyncMapItem);
			}
		}

        private void EditMap()
        {
            if (listViewMappings.SelectedItems.Count != 1)
                return;

            Changed = Library.EditLibraryMapping(listViewMappings.SelectedItems[0].Name);

            PopulateListWithMappings();
        }
		
        private void buttonEditMap_Click(object sender, EventArgs e)
		{
            EditMap();
			Refresh();
		}

        private void DeleteSelectedMapping()
        {
            if ((listViewMappings.SelectedItems.Count == 0) || (listViewMappings.Items.Count <= 1))
            {
                return;
            }

			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Exclamation; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("If you delete this mapping, ALL places it is used will be unlinked and will" +
								" revert to the default Mapping. Are you sure you want to continue?", "Delete the mapping?", true, false);
			messageBox.ShowDialog();

			if (messageBox.DialogResult == DialogResult.OK)
            {
                foreach (int j in listViewMappings.SelectedIndices)
                {
                    Library.RemoveMapping(listViewMappings.Items[j].Name);
                }

                Changed = true;
                PopulateListWithMappings();
            }
        }

		private void buttonDeleteMapping_Click(object sender, EventArgs e)
		{
            DeleteSelectedMapping();
		}

		private void listViewMappings_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (listViewMappings.SelectedItems.Count == 1)
				DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private LipSyncMapLibrary _library;

		private LipSyncMapLibrary Library
		{
			get
			{
				if (_library == null)
					_library = ApplicationServices.Get<IAppModuleInstance>(LipSyncMapDescriptor.ModuleID) as LipSyncMapLibrary;

				return _library;
			}
		}

		private void LipSyncMapSelector_KeyDown(object sender, KeyEventArgs e)
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
				listViewMappings.Dispose();
				components.Dispose();
			}

			base.Dispose(disposing);
		}

        private void listViewMappings_ItemActivate(object sender, EventArgs e)
        {
            EditMap();
        }

        private void buttonNewMap_Click(object sender, EventArgs e)
        {
            LipSyncNewMapType newMapTypeselector = new LipSyncNewMapType();
            DialogResult dr = newMapTypeselector.ShowDialog();
            if (dr == DialogResult.OK)
            {
                LipSyncMapData newMap = new LipSyncMapData();
                newMap.MatrixStringCount = newMapTypeselector.StringCount;
                newMap.MatrixPixelsPerString = newMapTypeselector.PixelsPerString;
                string mapName = _library.AddMapping(
                    true, 
                    null, 
                    newMap,
                    newMapTypeselector.matrixMappingRadio.Checked);
                
                Changed = Library.EditLibraryMapping(mapName);
                this.PopulateListWithMappings();
            }

			Refresh();
        }

        private void listViewMappings_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedMapping();
                e.Handled = true;
            }
        }

        private void buttonCloneMap_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvItem in listViewMappings.SelectedItems)
            {
                LipSyncMapData tempItem = new LipSyncMapData(_library.GetMapping(lvItem.Name));
                string mapName = _library.AddMapping(true, lvItem.Name, tempItem, tempItem.IsMatrix);
                this.PopulateListWithMappings();
                Changed = true;
            }

        }

		private void button_Paint(object sender, PaintEventArgs e)
		{
			ThemeButtonRenderer.OnPaint(sender, e, null);
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			ThemeButtonRenderer.ButtonHover = true;
			var btn = sender as Button;
			btn.Invalidate();
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			ThemeButtonRenderer.ButtonHover = false;
			var btn = sender as Button;
			btn.Invalidate();
		}
    }
}
