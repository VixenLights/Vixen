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
		private static readonly Bitmap IconOpen = Resources.bullet_toggle_minus_16px;
		private static readonly Bitmap IconClosed = Resources.bullet_toggle_plus_16px;
		private static readonly double ScaleFactor = ScalingTools.GetScaleFactor();

		static TimedSequenceRowLabel()
		{
			ToggleTreeButtonWidth = (int)(IconOpen.Width * ScaleFactor + 8);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (ParentRow.ChildRows.Count > 0) {
				var icon = ParentRow.TreeOpen ? IconOpen : IconClosed;

				int x = (int)(IconArea.Width - icon.Width*ScaleFactor)/2;
				int y = (int)(IconArea.Height - icon.Height*ScaleFactor)/2;
				e.Graphics.DrawImage(icon, x, y);
			}
		}
	}
}