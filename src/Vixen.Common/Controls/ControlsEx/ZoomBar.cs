using Common.Controls.ControlsEx.ValueControls;
using System.ComponentModel;

namespace Common.Controls.ControlsEx
{
	/// <summary>
	/// scrollbar control for zoom levels in toolstrip or statusstrip
	/// </summary>
	[DefaultEvent("ZoomChanged")]
	public class ZoomBar : ToolStripControlHost
	{
		/// <summary>
		/// transparent fader
		/// </summary>
		private class ZoomTracker : HMiniTracker
		{
			public ZoomTracker()
			{
				SetStyle(ControlStyles.SupportsTransparentBackColor, true);
				BackColor = Color.Transparent;
			}
		}

		#region variables

		private enum MouseLoc
		{
			Out = 0,
			ZoomIn = 1,
			ZoomOut = 2
		}

		private Bitmap zoom_in, zoom_out;
		private ScaleFactor _zoom = ScaleFactor.Identity;

		#endregion

		public ZoomBar()
			: base(new ZoomTracker())
		{
			//load .resx
			ComponentResourceManager resources =
				new ComponentResourceManager(typeof (ZoomBar));
			zoom_in = (Bitmap) resources.GetObject("zoom_in");
			zoom_out = (Bitmap) resources.GetObject("zoom_out");
			//
			HMiniTracker tracker = Control as ZoomTracker;
			tracker.Width = 120;
			tracker.Assign(5, ScaleFactor.CommonZooms.Length - 1, 0);
			tracker.ValueChanged += new ValueChangedEH(tracker_ValueChanged);
			//make room for buttons
			Padding = new Padding(20, 0, 20, 0);
		}

		#region controller

		//handlers
		private void HandleMouseLeave(object sender, EventArgs e)
		{
			base.OnMouseLeave(e);
		}

		private void HandleMouseHover(object sender, EventArgs e)
		{
			base.OnMouseHover(e);
		}

		private void HandleMouseMove(object sender, MouseEventArgs e)
		{
			base.OnMouseMove(e);
		}

		private void tracker_ValueChanged(ValueControl sender, ValueChangedEventArgs e)
		{
			_zoom = ScaleFactor.CommonZooms[sender.Value];
			ToolTipText = _zoom.ToString();
			Invalidate();
			//
			if (ZoomChanged != null)
				ZoomChanged(this, new EventArgs());
		}

		//prevents delegating of the painting event
		protected override void OnSubscribeControlEvents(Control control)
		{
			if (control != null) {
				control.MouseHover += new EventHandler(HandleMouseHover);
				control.MouseLeave += new EventHandler(HandleMouseLeave);
				control.MouseMove += new MouseEventHandler(HandleMouseMove);
			}
		}

		//paint the buttons
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (zoom_in != null && zoom_out != null) {
				ZoomTracker trk = Control as ZoomTracker;
				//zoom out button
				if (trk.Value <= trk.Minimum || !Enabled)
					ControlPaint.DrawImageDisabled(e.Graphics,
					                               zoom_out, 0, 0, SystemColors.Control);
				else {
					if (loc == MouseLoc.ZoomOut && Control.MouseButtons != MouseButtons.None)
						ControlPaint.DrawBorder3D(e.Graphics,
						                          new Rectangle(0, 0, 20, Height),
						                          Border3DStyle.RaisedInner);
					e.Graphics.DrawImageUnscaled(zoom_out, Point.Empty);
				}
				//zoom in button
				if (trk.Value >= trk.Maximum || !Enabled)
					ControlPaint.DrawImageDisabled(e.Graphics,
					                               zoom_in, Width - zoom_in.Width, 0, SystemColors.Control);
				else {
					if (loc == MouseLoc.ZoomIn && Control.MouseButtons != MouseButtons.None)
						ControlPaint.DrawBorder3D(e.Graphics,
						                          new Rectangle(Width - 20, 0, 20, Height),
						                          Border3DStyle.RaisedInner);
					e.Graphics.DrawImageUnscaled(zoom_in, new Point(Width - zoom_in.Width, 0));
				}
			}
			//100% zoom line
			e.Graphics.DrawLine(Pens.Black, Width/2 - 1, 3*Height/4,
			                    Width/2 - 1, Height - 1);
			e.Graphics.DrawLine(Pens.White, Width/2, 3*Height/4 + 1,
			                    Width/2, Height);
		}

		#region buttons

		private MouseLoc loc = MouseLoc.Out;
		//calculate which button the mouse is above
		private MouseLoc GetLocation(Point pt)
		{
			if (pt.X < 20)
				return MouseLoc.ZoomOut;
			else if (pt.X > Width - 20)
				return MouseLoc.ZoomIn;
			return MouseLoc.Out;
		}

		//highlight pressed
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			loc = GetLocation(e.Location);
			if (loc != MouseLoc.Out)
				Invalidate();
		}

		//unhighlight, do clicking
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			//update and click
			ZoomTracker trk = Control as ZoomTracker;
			switch (loc) {
				case MouseLoc.ZoomIn:
					trk.Value++;
					break;
				case MouseLoc.ZoomOut:
					trk.Value--;
					break;
				default:
					return;
			}
			loc = MouseLoc.Out;
			tracker_ValueChanged(trk, new ValueChangedEventArgs());
		}

		#endregion

		#endregion

		#region properties

		//makes buttons change enabled state
		public override bool Enabled
		{
			get { return base.Enabled; }
			set
			{
				Invalidate();
				base.Enabled = value;
			}
		}

		/// <summary>
		/// gets or sets the zoom of the control
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ScaleFactor Zoom
		{
			get { return _zoom; }
			set
			{
				if (value == _zoom) return;
				_zoom = value;
				ToolTipText = _zoom.ToString();
				(Control as ZoomTracker).Value = ScaleFactor.GetNearestCommonZoom(value);
				Invalidate();
			}
		}

		/// <summary>
		/// occurs when fader is scrolled or up/down buttons are pressed
		/// </summary>
		public event EventHandler ZoomChanged;

		#endregion
	}
}