using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Common.Controls
{
	public partial class BulkRename : Form
	{
		public BulkRename()
		{
			InitializeComponent();
		}

		public BulkRename(IEnumerable<string> oldNames)
		{
			InitializeComponent();
		}

		public BulkRename(int fixedCount)
		{
			InitializeComponent();
		}

		private List<string> OldNames { get; set; }
		private int FixedCount { get; set; }



		private void ResizeListviewColumns()
		{
			int width = (listViewNames.Width - SystemInformation.VerticalScrollBarWidth) / 2;
			listViewNames.Columns[0].Width = width;
			listViewNames.Columns[1].Width = width;
		}

		private void listViewNames_Resize(object sender, EventArgs e)
		{
			ResizeListviewColumns();
		}
	}
}
