using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.App.Scheduler {
	partial class ScheduleDayView : UserControl, ISchedulerView {
		//need?  just use panel's control collection?
		private ObservableList<IScheduleItem> _items;

		public event EventHandler<ScheduleEventArgs> TimeDoubleClick {
			add { dayPanel.TimeDoubleClick += value; }
			remove { dayPanel.TimeDoubleClick -= value; }
		}

		public ScheduleDayView() {
			InitializeComponent();

			_items = new ObservableList<IScheduleItem>();
			_items.CollectionChanged += _items_CollectionChanged;
		}

		private void _items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add:
					_AddVisualItem(e.NewItems[0] as IScheduleItem);
					break;
				case NotifyCollectionChangedAction.Remove:
					break;
				case NotifyCollectionChangedAction.Replace:
					break;
				case NotifyCollectionChangedAction.Reset:
					break;
			}
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			_UpdateScroll();
		}

		private void _UpdateScroll() {
			vScrollBar.Visible = dayPanel.Height / dayPanel.HalfHourHeight <= vScrollBar.Maximum;
			if(vScrollBar.Visible) {
				vScrollBar.LargeChange = dayPanel.Height / dayPanel.HalfHourHeight;

				if(vScrollBar.Value + vScrollBar.LargeChange > vScrollBar.Maximum) {
					vScrollBar.Value = vScrollBar.Maximum - vScrollBar.LargeChange + 1;
				}
			}
		}

		private void vScrollBar_ValueChanged(object sender, EventArgs e) {
			dayPanel.TopHalfHour = vScrollBar.Value;
		}

		private void _AddItem(IScheduleItem item) {
			_items.Add(item);
			_AddVisualItem(item);
		}

		private void _AddVisualItem(IScheduleItem item) {
			ScheduleItemVisual itemVisual = new ScheduleItemVisual(item);
			dayPanel.Controls.Add(itemVisual);
		}

		public IList<IScheduleItem> Items {
			get { return _items; }
			set {
				dayPanel.SuspendLayout();
				dayPanel.Controls.Clear();
				_items.Clear();
				foreach(IScheduleItem item in value) {
					_AddItem(item);
				}
				dayPanel.ResumeLayout();
			}
		}

	}
}
