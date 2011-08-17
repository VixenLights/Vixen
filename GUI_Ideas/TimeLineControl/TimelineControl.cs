using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Timeline
{
	public partial class TimelineControl : UserControl
	{
		public TimelineControl()
		{
			InitializeComponent();
		}

		public TimelineGrid Grid
		{
			get { return this.timelineGrid; }
		}

		public TimelineRowList RowList
		{
			get { return this.timelineRowList; }
		}

		public TimelineHeader Header
		{
			get { return this.timelineHeader; }
		}
	}
}
