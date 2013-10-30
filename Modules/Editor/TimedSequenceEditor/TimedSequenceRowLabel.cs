using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	[System.ComponentModel.DesignerCategory("")] // Prevent this from showing up in designer.
	internal class TimedSequenceRowLabel : Common.Controls.Timeline.RowLabel
	{
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			if (ParentRow.ChildRows.Count > 0) {
				Bitmap icon;
				if (ParentRow.TreeOpen)
					icon = Resources.bullet_toggle_minus_16px;
				else
					icon = Resources.bullet_toggle_plus_16px;

				int x = (IconArea.Width - icon.Width)/2;
				int y = (IconArea.Height - icon.Height)/2;
				e.Graphics.DrawImage(icon, new Point(x, y));
			}
		}
	}
}