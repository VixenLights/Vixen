using System;
using System.Windows.Forms;
using System.Drawing;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.Timeline;
using Common.Resources;
using Common.Resources.Properties;
using WeifenLuo.WinFormsUI.Docking;
using VixenModules.Sequence.Timed;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_Marks : DockContent
	{
		public Form_Marks(TimelineControl timelineControl)
		{
			InitializeComponent();
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			toolStripButtonAddMarkCollection.Image = Tools.GetIcon(Resources.addItem, iconSize);
			toolStripButtonAddMarkCollection.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonDeleteMarkCollection.Image = Tools.GetIcon(Resources.delete_32, iconSize);
			toolStripButtonDeleteMarkCollection.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonEditMarkCollection.Image = Tools.GetIcon(Resources.configuration, iconSize);
			toolStripButtonEditMarkCollection.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStrip1.ImageScalingSize = new Size(iconSize, iconSize);
			TimelineControl = timelineControl;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			contextMenuStrip1.Renderer = new ThemeToolStripRenderer();
			toolStrip1.Renderer = new ThemeToolStripRenderer();
			toolStripMenuItemBold.Image = Tools.GetIcon(Resources.boldLine, iconSize);
			toolStripMenuItemDottedSolid.Image = Tools.GetIcon(Resources.dottedLine, iconSize);
			ToolStripMenuItemChangeColor.Image = Tools.GetIcon(Resources.colors, iconSize);
			listViewMarkCollections.BackColor = ThemeColorTable.BackgroundColor; //Over-rides the default Listview background
			toolStripButtonDeleteMarkCollection.Enabled = false;
		}

		private void Form_Marks_Load(object sender, EventArgs e)
		{
			var xml = new XMLProfileSettings();
			numericUpDownStandardNudge.Value = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/StandardNudge", Name), 10);
			numericUpDownSuperNudge.Value = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SuperNudge", Name), 20);
			toolStripMenuItemNudgeSettings.Checked = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/NudgeSettings", Name), false);
			//xml = null;

			panel1.Visible = toolStripMenuItemNudgeSettings.Checked;
			ResizeColumnHeaders();
		}

		public TimelineControl TimelineControl { get; set; }

		public void PopulateMarkCollectionsList(MarkCollection selectedCollection)
		{
			
			listViewMarkCollections.Items.Clear();
			foreach (MarkCollection mc in Sequence.MarkCollections)
			{
				ListViewItem item = new ListViewItem();
				item.Text = mc.Name;
				item.ForeColor = mc.MarkColor;
				item.Tag = mc;
				item.Checked = mc.Enabled;
				listViewMarkCollections.Items.Add(item);
				item.Selected = (mc == selectedCollection);
			}

			ResizeColumnHeaders();
		}

		private void ResizeColumnHeaders()
		{
			
			for (int i = 0; i < listViewMarkCollections.Columns.Count; i++)
			{
				listViewMarkCollections.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
			}
		}

		public MarkCollection SelectedMarkCollection
		{
			get
			{
				if (listViewMarkCollections.SelectedItems.Count > 0)
				{
					MarkCollection item = listViewMarkCollections.SelectedItems[0].Tag as MarkCollection;
					return item;
				}
				return null;
			}
		}

		public TimedSequence Sequence { get; set; }

		public event EventHandler<MarkCollectionArgs> MarkCollectionChecked;
		protected virtual void OnMarkCollectionChecked(MarkCollectionArgs e)
		{
			if (MarkCollectionChecked != null)
				MarkCollectionChecked(this, e);
		}

		public event EventHandler<EventArgs> EditMarkCollection;
		protected virtual void OnEditMarkCollection(EventArgs e)
		{
			if (EditMarkCollection != null)
				EditMarkCollection(this, e);
		}

		public event EventHandler<MarkCollectionArgs> ChangedMarkCollection;
		protected virtual void OnChangedMarkCollection(MarkCollectionArgs e)
		{
			if (ChangedMarkCollection != null)
				ChangedMarkCollection(this, e);
		}

		private void listViewMarkCollections_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			ListViewItem item = listViewMarkCollections.Items[e.Index];
			MarkCollection mc = item.Tag as MarkCollection;
			mc.Enabled = (e.NewValue == CheckState.Checked);
			OnMarkCollectionChecked(new MarkCollectionArgs(mc));			
		}

		private void toolStripButtonEditMarkCollection_Click(object sender, EventArgs e)
		{
			OnEditMarkCollection(EventArgs.Empty);
		}

		private void listViewMarkCollections_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			MarkCollection mc = (listViewMarkCollections.Items[e.Item].Tag as MarkCollection);
			mc.Name = e.Label ?? mc.Name;
			OnChangedMarkCollection(new MarkCollectionArgs(mc));
			PopulateMarkCollectionsList(mc);
		}

		private void toolStripButtonAddMarkCollection_Click(object sender, EventArgs e)
		{
			MarkCollection mc = new MarkCollection();
			mc.Name = "New Mark Collection";
			mc.MarkColor = Color.White;
			Sequence.MarkCollections.Add(mc);
			OnChangedMarkCollection(new MarkCollectionArgs(mc));
			PopulateMarkCollectionsList(mc);
		}

		private void toolStripButtonDeleteMarkCollection_Click(object sender, EventArgs e)
		{
			DeleteSelectedMarkCollections();
		}

		private void numericUpDownStandardNudge_ValueChanged(object sender, EventArgs e)
		{
			TimelineControl.ruler.StandardNudgeTime = Convert.ToInt32(numericUpDownStandardNudge.Value);
		}

		private void numericUpDownSuperNudge_ValueChanged(object sender, EventArgs e)
		{
			TimelineControl.ruler.SuperNudgeTime = Convert.ToInt32(numericUpDownSuperNudge.Value);
		}

		private void Form_Marks_Closing(object sender, FormClosingEventArgs e)
		{
			var xml = new XMLProfileSettings();
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/StandardNudge", Name), Convert.ToInt32(numericUpDownStandardNudge.Value));
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SuperNudge", Name), Convert.ToInt32(numericUpDownSuperNudge.Value));
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/NudgeSettings", Name), toolStripMenuItemNudgeSettings.Checked);
		}

		private void listViewMarkCollections_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && listViewMarkCollections.SelectedItems.Count > 0)
			{
				DeleteSelectedMarkCollections();
			}
		}

		private void DeleteSelectedMarkCollections()
		{
			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Question; //adds a system icon to the message form.
			var messageBox = new MessageBoxForm("Are you sure you want to delete the selected Marks in the Collection?", "Delete Mark Collection", true, false);
			messageBox.ShowDialog();
			if (messageBox.DialogResult == DialogResult.OK)
			{
				foreach (ListViewItem item in listViewMarkCollections.SelectedItems)
				{
					listViewMarkCollections.SelectedItems[0].Remove();
					MarkCollection mc = item.Tag as MarkCollection;
					Sequence.MarkCollections.Remove(mc);
				}
				OnChangedMarkCollection(new MarkCollectionArgs(null));
			}
		}

		private void boldToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var menuAction = sender;
			changeMarkCollection(menuAction);
		}

		private void dottedSolidToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var menuAction = sender;
			changeMarkCollection(menuAction);
		}

		private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var menuAction = sender;
			changeMarkCollection(menuAction);
		}

		private void changeMarkCollection(object menuAction)
		{
			DialogResult result = DialogResult.Cancel;
			Common.Controls.ColorManagement.ColorPicker.ColorPicker picker = null;
			switch (menuAction.ToString())
			{
				case "Change Color":
					picker = new Common.Controls.ColorManagement.ColorPicker.ColorPicker();
					result = picker.ShowDialog();
					break;
			}
			foreach (ListViewItem item in listViewMarkCollections.SelectedItems)
			{
				MarkCollection mc = item.Tag as MarkCollection;
				switch (menuAction.ToString())
				{
					case "Normal/Bold Line":
						mc.Bold = !mc.Bold;
						break;
					case "Dotted/Solid Line":
						mc.SolidLine = !mc.SolidLine;
						break;
					case "Change Color":
						if (result == DialogResult.OK)
						{
							mc.MarkColor = picker.Color.ToRGB().ToArgb();
							item.ForeColor = picker.Color.ToRGB().ToArgb();
						}
						break;
					case "Delete Selected Marks":
						DeleteSelectedMarkCollections();
						return;
				}
				OnMarkCollectionChecked(new MarkCollectionArgs(mc));
			}
		}

		private void listViewMarkCollections_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewMarkCollections.SelectedItems.Count > 0)
			{
				toolStripButtonDeleteMarkCollection.Enabled = true;
				toolStripMenuItemDottedSolid.Enabled = true;
				toolStripMenuItemBold.Enabled = true;
				ToolStripMenuItemChangeColor.Enabled = true;
			}
			else
			{
				toolStripButtonDeleteMarkCollection.Enabled = false;
				toolStripMenuItemDottedSolid.Enabled = false;
				toolStripMenuItemBold.Enabled = false;
				ToolStripMenuItemChangeColor.Enabled = false;
			}
		}

		private void toolStripMenuItemNudgeSettings_Click(object sender, EventArgs e)
		{
			toolStripMenuItemNudgeSettings.Checked = !toolStripMenuItemNudgeSettings.Checked;
			panel1.Visible = !panel1.Visible;
		}

	}

	public class MarkCollectionArgs: EventArgs
	{
		public MarkCollectionArgs(MarkCollection mc)
		{
			MarkCollection = mc;
		}

		public MarkCollection MarkCollection { get; private set; }
	}

}
