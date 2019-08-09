using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	[System.ComponentModel.DesignerCategory("")] // Prevent this from showing up in designer.
	internal class TimedSequenceRowLabel : Common.Controls.Timeline.RowLabel
	{
		private static readonly Bitmap IconOpen = Resources.bullet_toggle_minus_16px;
		private static readonly Bitmap IconClosed = Resources.bullet_toggle_plus_16px;
		private static readonly Bitmap IconActiveOpen = Resources.bullet_toggle_minus_active_16px;
		private static readonly Bitmap IconActiveClosed = Resources.bullet_toggle_plus_active_16px;
		private static readonly double ScaleFactor = ScalingTools.GetScaleFactor();

		static TimedSequenceRowLabel()
		{
			ToggleTreeButtonWidth = (int)(IconOpen.Width * ScaleFactor + 8);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (ParentRow.ChildRows.Count > 0)
			{

				Bitmap icon;
				var showChildActive = ShowActiveIndicators && ChildActiveIndicator();
				if (showChildActive)
				{
					icon = ParentRow.TreeOpen ? IconActiveOpen : IconActiveClosed;
				}
				else
				{
					icon = ParentRow.TreeOpen ? IconOpen : IconClosed;
				}
				

				int x = (int)(IconArea.Width - icon.Width*ScaleFactor)/2;
				int y = (int)(IconArea.Height - icon.Height*ScaleFactor)/2;
				if (showChildActive)
				{
					e.Graphics.FillRectangle(Brushes.Yellow, x, y, icon.Width, icon.Height);
				}
				e.Graphics.DrawImage(icon, x, y);
			}
		}
	}
}