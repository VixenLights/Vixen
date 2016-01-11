using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Common.Controls;
using Common.Controls.Theme;
using Common.Controls.Timeline;
using Vixen.Module.Effect;
using Vixen.Services;
using VixenApplication;
using WeifenLuo.WinFormsUI.Docking;
using VixenModules.Sequence.Timed;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_Marks : DockContent
	{
		public Form_Marks(TimelineControl timelineControl)
		{
			InitializeComponent();
			TimelineControl = timelineControl;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			contextMenuStrip1.Renderer = new ThemeToolStripRenderer();
			listViewMarkCollections.BackColor = ThemeColorTable.BackgroundColor; //Over-rides the default Listview background
		}

		private void Form_Marks_Load(object sender, EventArgs e)
		{
			var xml = new XMLProfileSettings();
			numericUpDownStandardNudge.Value = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/StandardNudge", Name), 10);
			numericUpDownSuperNudge.Value = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/SuperNudge", Name), 20);
			//xml = null;
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
				item.ForeColor = mc.MarkColor;// : Color.FromArgb(221,221,221);
				//item.ForeColor = (mc.Enabled)
				//					? ((GetGrayValueForColor(mc.MarkColor) > 128) ? Color.Black : Color.White)
				//					: SystemColors.InactiveCaptionText;
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
				else
				{
					return null;
				}
			}
		}

		private double GetGrayValueForColor(Color c)
		{
			return c.R * 0.299 + c.G * 0.587 + c.B * 0.114;
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
			Point mousePoint = PointToClient(new Point(MousePosition.X, MousePosition.Y));
			ListViewItem mouseItem = listViewMarkCollections.GetItemAt(mousePoint.X, mousePoint.Y);
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
		}

		private void listViewMarkCollections_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				DeleteSelectedMarkCollections();
			}
		}

		private void DeleteSelectedMarkCollections()
		{
			if (listViewMarkCollections.SelectedItems.Count > 0)
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
			else
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //adds a system icon to the message form.
				var messageBox = new MessageBoxForm("Please select a Mark Collection to delete and press the delete button again.", "Delete Mark Collection", false, false);
				messageBox.ShowDialog();
			}
		}

		private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = (listViewMarkCollections.SelectedItems.Count == 0);
			boldToolStripMenuItem.Enabled = true;
			dottedSolidToolStripMenuItem.Enabled = true;
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
			ListViewItem item = listViewMarkCollections.SelectedItems[0];
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
					Common.Controls.ColorManagement.ColorPicker.ColorPicker picker = new Common.Controls.ColorManagement.ColorPicker.ColorPicker();

					DialogResult result = picker.ShowDialog();
					if (result == DialogResult.OK)
					{
						mc.MarkColor = picker.Color.ToRGB().ToArgb();
						item.ForeColor = picker.Color.ToRGB().ToArgb();
					}
					break;
			}
			OnMarkCollectionChecked(new MarkCollectionArgs(mc));
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
