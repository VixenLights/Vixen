using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
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

		public event EventHandler<MarkCollectionCheckedArgs> MarkCollectionChecked;
		protected virtual void OnMarkCollectionChecked(MarkCollectionCheckedArgs e)
		{
			if (MarkCollectionChecked != null)
				MarkCollectionChecked(this, e);
		}

		private void listViewMarkCollections_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			Point mousePoint = PointToClient(new Point(MousePosition.X, MousePosition.Y));
			ListViewItem mouseItem = listViewMarkCollections.GetItemAt(mousePoint.X, mousePoint.Y);
			ListViewItem item = listViewMarkCollections.Items[e.Index];
			MarkCollection mc = item.Tag as MarkCollection;
			mc.Enabled = (e.NewValue == CheckState.Checked);
			OnMarkCollectionChecked(new MarkCollectionCheckedArgs(mc));			
		}

	}

	public class MarkCollectionCheckedArgs: EventArgs
	{
		public MarkCollectionCheckedArgs(MarkCollection mc)
		{
			MarkCollection = mc;
		}

		public MarkCollection MarkCollection { get; private set; }
	}

}
