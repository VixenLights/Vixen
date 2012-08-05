/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace Dataweb.NShape.WinFormsUI {

	/// <summary>
	/// A designer for the Display component for WinForms designer.
	/// </summary>
	public class DisplayDesigner : ScrollableControlDesigner {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.DisplayDesigner" />.
		/// </summary>
		public DisplayDesigner() {
		}


		/// <summary>
		/// Paints designtime adornments (dashed frame around the control)
		/// </summary>
		protected override void OnPaintAdornments(PaintEventArgs p) {
			Debug.Assert(base.Component is Display);
			Display component = base.Component as Display;
			if (component != null && component.BorderStyle == BorderStyle.None)
				this.DrawBorder(p.Graphics);
			base.OnPaintAdornments(p);
		}


		private void DrawBorder(Graphics graphics) {
			Color color;
			Control control = this.Control;
			Rectangle clientRectangle = control.ClientRectangle;
			if (control.BackColor.GetBrightness() < 0.5) {
				color = ControlPaint.Light(control.BackColor);
			} else {
				color = ControlPaint.Dark(control.BackColor);
			}
			Pen pen = new Pen(color);
			pen.DashStyle = DashStyle.Dash;
			clientRectangle.Width--;
			clientRectangle.Height--;
			graphics.DrawRectangle(pen, clientRectangle);
			pen.Dispose();
		}

	}

}
