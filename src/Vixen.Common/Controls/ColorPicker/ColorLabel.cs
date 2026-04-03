using System.ComponentModel;
using System.Resources;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.Theme;

namespace Common.Controls.ColorManagement.ColorPicker
{
	/// <summary>
	/// Zusammenfassung für ColorLabel.
	/// </summary>
	[ToolboxItem(false)]
	public class ColorLabel : Control
	{
		#region variables

		private Bitmap _tl_picker;

		private Color _color = Color.Black,
		              _oldcolor = Color.Black,
		              _mdowncolor;

		private bool _screen, _tracking;

		#endregion

		public ColorLabel()
		{
			ResourceManager man = new ResourceManager(typeof (ColorLabel));
			_tl_picker = (Bitmap) man.GetObject("tl_picker.png");

			SetStyle(ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.ResizeRedraw, true);
		}

		public static string ColorToHexString(Color col)
		{
			return string.Format("{0:X6}", col.ToArgb() & 0xFFFFFF, 16);
		}

		#region controller

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (Enabled) {
				//draw info panel
				int infowidth = 0,
				    y = 0;
				if (_screen && _tl_picker != null) {
					infowidth = Height;
					y = (Height - _tl_picker.Height)/2;
					e.Graphics.DrawImageUnscaled(_tl_picker, y, y);
				}
				using (SolidBrush sld = new SolidBrush(_color)) {
					y = Height/2;
					//draw colors
					e.Graphics.FillRectangle(sld, infowidth, 0, Width - infowidth, y);
					sld.Color = _oldcolor;
					e.Graphics.FillRectangle(sld, infowidth, y, Width - infowidth, Height - y);
					//draw texts
					using (StringFormat fmt = new StringFormat()) {
						fmt.Alignment = fmt.LineAlignment = StringAlignment.Center;
						//color
						sld.Color = ColorUtility.MaxContrastTo(_color);
						e.Graphics.DrawString(ColorToHexString(_color), Font, sld,
						                      new Rectangle(infowidth, 0, Width - infowidth, y), fmt);
						//previous color
						sld.Color = ColorUtility.MaxContrastTo(_oldcolor);
						e.Graphics.DrawString(ColorToHexString(_oldcolor), Font, sld,
						                      new Rectangle(infowidth, y, Width - infowidth, Height - y), fmt);
					}
				}
			}
			//draw broder
			ControlPaint.DrawBorder(e.Graphics,new Rectangle(0,0,Width, Height),ThemeColorTable.GroupBoxBorderColor,ButtonBorderStyle.Solid );
			//ControlPaint.DrawBorder3D(e.Graphics, 0, 0, this.Width, this.Height, Border3DStyle.SunkenOuter,
			//						  Border3DSide.All & ~Border3DSide.Middle);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			_mdowncolor = _color;
			_tracking = true;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (_tracking) {
				_screen = !ClientRectangle.Contains(
					PointToClient(MousePosition));
				if (_screen) //pick color from screen
				{
					_color = GDI32.GetScreenPixel(MousePosition.X,
					                              MousePosition.Y);
				}
				Cursor = _screen ? Cursors.Cross : Cursors.Default;
				Refresh();
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			Cursor = Cursors.Default;
			if (_screen) //update color
			{
				if (_color != _oldcolor)
					RaiseColorChanged();
			}
			//else use click-event
			_tracking = _screen = false;
			Refresh();
		}

		#endregion

		#region properties

		/// <summary>
		/// specifies the color without alpha
		/// </summary>
		[Description("specifies the color without alpha"),
		 DefaultValueAttribute(typeof (Color), "Black")]
		public Color Color
		{
			get { return _color; }
			set
			{
				value = Color.FromArgb(255, value);
				if (value == _color) return;
				_color = value;
				Refresh();
			}
		}

		/// <summary>
		/// specifies the previous color without alpha
		/// </summary>
		[Description("specifies the previous color without alpha"),
		 DefaultValueAttribute(typeof (Color), "Black")]
		public Color OldColor
		{
			get { return _oldcolor; }
			set
			{
				value = Color.FromArgb(255, value);
				if (value == _oldcolor) return;
				_oldcolor = value;
				Refresh();
			}
		}

		#endregion

		#region events

		//color changed
		public void RaiseColorChanged()
		{
			if (ColorChanged != null)
				ColorChanged(this, new EventArgs());
		}

		public event EventHandler ColorChanged;

		#endregion
	}
}