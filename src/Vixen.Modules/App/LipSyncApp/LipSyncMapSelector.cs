﻿using Common.Controls;
using Common.Controls.Theme;
using Vixen.Services;
using Vixen.Module.App;

namespace VixenModules.App.LipSyncApp
{
	public partial class LipSyncMapSelector : BaseForm
	{
		private LipSyncMapLibrary _library;

		public LipSyncMapSelector()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			mappingsListView.Sorting = SortOrder.Ascending;
			Icon = Common.Resources.Properties.Resources.Icon_Vixen3;
			Changed = false;
		}

		public bool Changed { get; set; }

		private string RenameOldLabel { get; set; }

		private void LipSyncMapSelector_Load(object sender, EventArgs e)
		{
			PopulateListWithMappings();
			mappingsListView.Activation = ItemActivation.Standard;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				mappingsListView.Dispose();
				components.Dispose();
			}

			base.Dispose(disposing);
		}

		private void LipSyncMapSelector_FormClosing(object sender, FormClosingEventArgs e)
		{
			var data = (ApplicationServices.Get<IAppModuleInstance>(LipSyncMapDescriptor.ModuleID) as LipSyncMapLibrary)?.StaticModuleData as
				LipSyncMapStaticData;
		}

		private void UpdateListViewCtrl()
		{
			mappingsListView.Columns.Clear();
			mappingsListView.Columns.Add("Name");
			//mappingsListView.Columns.Add("Type");
			mappingsListView.Columns.Add("Notes");
			
			mappingsListView.Items.Clear();
			foreach (KeyValuePair<string, LipSyncMapData> kvp in Library)
			{
				LipSyncMapData c = kvp.Value;

				ListViewItem lvi = new ListViewItem();
				lvi.Tag = kvp.Value;
				lvi.Text = kvp.Key;
				lvi.Name = kvp.Key;

				ListViewItem.ListViewSubItem subItemNotes = new ListViewItem.ListViewSubItem(lvi, "Notes");
				subItemNotes.Name = @"Notes";
				subItemNotes.Text = kvp.Value.Notes;

				//lvi.SubItems.Add(subItemType);
				lvi.SubItems.Add(subItemNotes);
				mappingsListView.Items.Add(lvi);
			}

			
			SetWidths();
			
		}

		private void PopulateListWithMappings()
		{
			mappingsListView.BeginUpdate();
			mappingsListView.Items.Clear();

			mappingsListView.LargeImageList = new ImageList();

			foreach (KeyValuePair<string, LipSyncMapData> kvp in Library) {
				LipSyncMapData c = kvp.Value;
				string name = kvp.Key;

				ListViewItem item = new ListViewItem();
				item.Text = name;
				item.Name = name;
				item.ImageKey = name;
				item.Tag = c;
				if (Library.DefaultMapping != null && Library.DefaultMappingName.Equals(name))
				{
					item.Font = new Font(item.Font, FontStyle.Bold);
				}
				
				mappingsListView.Items.Add(item);

			}

			SetWidths();
			
			mappingsListView.EndUpdate();

			UpdateListViewCtrl();
		}

		public Tuple<string, LipSyncMapItem> SelectedItem
		{
			get
			{
				if (mappingsListView.SelectedItems.Count == 0)
					return null;

				return new Tuple<string, LipSyncMapItem>(mappingsListView.SelectedItems[0].Name, mappingsListView.SelectedItems[0].Tag as LipSyncMapItem);
			}
		}

		private void EditMap()
		{
			if (mappingsListView.SelectedItems.Count != 1)
				return;

			Changed = Library.EditLibraryMapping(mappingsListView.SelectedItems[0].Name);

			PopulateListWithMappings();
		}
		
		private void DeleteSelectedMapping()
		{
			if ((mappingsListView.SelectedItems.Count == 0) || (mappingsListView.Items.Count <= 1))
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
				foreach (int j in mappingsListView.SelectedIndices)
				{
					Library.RemoveMapping(mappingsListView.Items[j].Name,true);
				}

				Changed = true;
				PopulateListWithMappings();
			}
		}

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

		private void buttonNewMap_Click(object sender, EventArgs e)
		{
			LipSyncMapData newMap = new LipSyncMapData();
			string mapName = Library.AddMapping(true, null, newMap, true);

			Changed = Library.EditLibraryMapping(mapName);
			if (!Changed)
			{
				Library.RemoveMapping(mapName);
			}
			this.PopulateListWithMappings();
		}

		private void buttonCloneMap_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem lvItem in mappingsListView.SelectedItems)
			{
				string mapName = Library.CloneLibraryMapping(lvItem.Name);
				if (mapName != "")
				{
					this.PopulateListWithMappings();
					Changed = true;
				}
			}

		}

		private void buttonEditMap_Click(object sender, EventArgs e)
		{
			EditMap();
			Refresh();
		}

		private void buttonDeleteMapping_Click(object sender, EventArgs e)
		{
			DeleteSelectedMapping();
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Common.Resources.Properties.Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Common.Resources.Properties.Resources.ButtonBackgroundImage;
		}

		private void LipSyncMapSelector_Resize(object sender, EventArgs e)
		{
			UpdateListViewCtrl();
		}

		private void mappingsListView_DoubleClick(object sender, EventArgs e)
		{
			EditMap();
		}

		private void mappingsListView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				DeleteSelectedMapping();
				e.Handled = true;
			}
		}

		private void mappingsListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonEditMap.Enabled = (mappingsListView.SelectedIndices.Count == 1);
			buttonCloneMap.Enabled = (mappingsListView.SelectedIndices.Count >= 1);
			buttonDeleteMap.Enabled = (mappingsListView.SelectedIndices.Count >= 1);
		}

		private void mappingsListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			buttonNewMap.Enabled = true;
			buttonEditMap.Enabled = false;
			buttonCloneMap.Enabled = false;
			buttonDeleteMap.Enabled = false;

			if (e.Label != null)
			{
				Changed = Library.RenameLibraryMapping(RenameOldLabel, e.Label.ToString());
			}
			PopulateListWithMappings();
			e.CancelEdit = true;
		}

		private void mappingsListView_BeforeLabelEdit(object sender, LabelEditEventArgs e)
		{
			buttonNewMap.Enabled = false;
			buttonEditMap.Enabled = false;
			buttonCloneMap.Enabled = false;
			buttonDeleteMap.Enabled = false; 

			ListViewItem lvItem = mappingsListView.SelectedItems[0];
			RenameOldLabel = lvItem.Name;
		}

		private void SetWidths()
		{
			mappingsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			mappingsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
		}
	}
}
