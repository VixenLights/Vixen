using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Common.Controls.Scaling;
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

				int x = (int)(IconArea.Width - icon.Width*ScalingTools.GetScaleFactor())/2;
				int y = (int)(IconArea.Height - icon.Height*ScalingTools.GetScaleFactor())/2;
				e.Graphics.DrawImage(icon, x, y);
			}
		}
	}
}