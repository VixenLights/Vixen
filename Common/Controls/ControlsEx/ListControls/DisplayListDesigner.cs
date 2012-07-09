using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Collections.Generic;

namespace Common.Controls.ControlsEx.ListControls
{
	/// <summary>
	/// Zusammenfassung für DisplayListDesigner.
	/// </summary>
	public class DisplayListDesigner : ControlDesigner
	{
		public override void Initialize(System.ComponentModel.IComponent component)
		{
			base.Initialize(component as DisplayList);
			((DisplayList)component).Items.Add(new DesignerDisplayItem());
		}
	}
	/// <summary>
	/// dummy element for designer
	/// </summary>
	public class DesignerDisplayItem : DisplayItem
	{
		protected override void OnDraw(Graphics gr, Rectangle rct)
		{
			gr.SmoothingMode = SmoothingMode.AntiAlias;
			gr.DrawEllipse(Pens.Black, rct.X, rct.Y,
				rct.Width - 1, rct.Height - 1);
		}
		protected override void OnDrawUnscaled(Graphics gr, int x, int y)
		{
			gr.DrawEllipse(Pens.Black, 0, 0, 56, 56);
		}

		public override string Text
		{
			get { return "Sample Item"; }
			set { }
		}
		public override Size Size
		{
			get { return new Size(56, 56); }
		}

	}
}
