using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Common.Controls.Timeline;
using Vixen.Module.Effect;
using Vixen.Services;
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
		}

		private void Form_Effects_Load(object sender, EventArgs e)
		{
			Common.Controls.XMLProfileSettings xml = new Common.Controls.XMLProfileSettings();
			numericUpDownStandardNudge.Value = xml.GetSetting("StandardNudge", 10);
			numericUpDownSuperNudge.Value = xml.GetSetting("SuperNudge", 20);
			xml = null;
		}

		public TimelineControl TimelineControl { get; set; }

		public void PopulateMarkCollectionsList(MarkCollection selectedCollection)
		{
			listViewMarkCollections.Items.Clear();
			foreach (MarkCollection mc in Sequence.MarkCollections)
			{
				ListViewItem item = new ListViewItem();
				item.Text = mc.Name;
				item.SubItems.Add(mc.Level.ToString());
				item.SubItems.Add(mc.MarkCount.ToString());
				item.BackColor = (mc.Enabled) ? mc.MarkColor : SystemColors.Window;
				item.ForeColor = (mc.Enabled)
									? ((GetGrayValueForColor(mc.MarkColor) > 128) ? Color.Black : Color.White)
									: SystemColors.InactiveCaptionText;
				item.Tag = mc;
				item.Checked = mc.Enabled;
				listViewMarkCollections.Items.Add(item);
				item.Selected = (mc == selectedCollection);
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
			mc.Name = e.Label;
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
			if (listViewMarkCollections.SelectedItems.Count > 0)
			{
				MarkCollection mc = (listViewMarkCollections.SelectedItems[0].Tag as MarkCollection);
				if (MessageBox.Show("Are you sure you want to delete the selected Mark Collection?", "Delete Mark Collection", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
				{
					listViewMarkCollections.SelectedItems[0].Remove();
					Sequence.MarkCollections.Remove(mc);
					OnChangedMarkCollection(new MarkCollectionArgs(null));
				}
			}
			else
			{
				MessageBox.Show("Please select a Mark Collection to delete and press the delete button again.", "Delete Mark Collection", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
			}
		}

		private void numericUpDownStandardNudge_ValueChanged(object sender, EventArgs e)
		{
			TimelineControl.ruler.StandardNudgeTime = Convert.ToInt32(numericUpDownStandardNudge.Value);
		}

		private void numericUpDownSuperNudge_ValueChanged(object sender, EventArgs e)
		{
			TimelineControl.ruler.SuperNudgeTime = Convert.ToInt32(numericUpDownSuperNudge.Value);
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
