using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.App.Scheduler {
	partial class ScheduleItemVisual : UserControl {
		public ScheduleItemVisual(IScheduleItem item) {
			if(item == null) throw new ArgumentNullException("item");
			InitializeComponent();
			Item = item;
			BackColor = Color.Transparent;
		}

		public IScheduleItem Item { get; private set; }

		protected override void OnPaint(PaintEventArgs e) {
			_DrawRoundRect(e.Graphics, Pens.Black, Brushes.White, 0, 0, Width - 1, Height - 1, 3);
		}

		private void _DrawRoundRect(Graphics g, Pen borderPen, Brush fillBrush, float X, float Y, float width, float height, float radius) {
			g.SmoothingMode = SmoothingMode.AntiAlias;
			using(GraphicsPath gp = new GraphicsPath()) {
				gp.AddLine(X + radius, Y, X + width - (radius * 2), Y);
				gp.AddArc(X + width - (radius * 2), Y, radius * 2, radius * 2, 270, 90);
				gp.AddLine(X + width, Y + radius, X + width, Y + height - (radius * 2));
				gp.AddArc(X + width - (radius * 2), Y + height - (radius * 2), radius * 2, radius * 2, 0, 90);
				gp.AddLine(X + width - (radius * 2), Y + height, X + radius, Y + height);
				gp.AddArc(X, Y + height - (radius * 2), radius * 2, radius * 2, 90, 90);
				gp.AddLine(X, Y + height - (radius * 2), X, Y + radius);
				gp.AddArc(X, Y, radius * 2, radius * 2, 180, 90);
				gp.CloseFigure();

				g.FillPath(fillBrush, gp);
				g.DrawPath(borderPen, gp);
			}
		}

	}
}
