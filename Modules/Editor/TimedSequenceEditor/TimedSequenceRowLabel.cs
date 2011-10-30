using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace VixenModules.Editor.TimedSequenceEditor
{
    [System.ComponentModel.DesignerCategory("")]    // Prevent this from showing up in designer.
	class TimedSequenceRowLabel : CommonElements.Timeline.RowLabel
	{
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);
			if (ParentRow.ChildRows.Count > 0) {
				Bitmap icon;
				if (ParentRow.TreeOpen)
					icon = TimedSequenceEditorResources.minus_button;
				else
					icon = TimedSequenceEditorResources.plus_button;

				int x = (IconArea.Width - icon.Width) / 2;
				int y = (IconArea.Height - icon.Height) / 2;
				e.Graphics.DrawImage(icon, new Point(x, y));
			}
		}
	}
}
