using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ControlsEx
{
	/// <summary>
	/// a visio-like ruler
	/// </summary>
	public class Ruler : Control
	{
		#region variables
		private static readonly float[] multipliers =
			new float[] { 2f, 5f };
		private int _offset = 0;
		private float _value = float.NaN, _length = 1f;
		private ScaleFactor _zoom = ScaleFactor.Identity;
		private Orientation _orientation = Orientation.Horizontal;
		#endregion
		/// <summary>
		/// ctor
		/// </summary>
		public Ruler()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw, true);
			base.Font = new Font("Microsoft Sans Serif", 7f);
		}
		#region controller
		protected override void OnPaint(PaintEventArgs e)
		{
			using (LinearGradientBrush lnbrs = new LinearGradientBrush(
				//adjust orientation
					  new Point(0, 0),
					  (_orientation == Orientation.Horizontal) ?
					  new Point(0, this.Size.Height) :
					  new Point(this.Size.Width, 0),
				//white to gray
					  Color.FromArgb(251, 251, 251),
					  Color.FromArgb(199, 199, 199)))
			{
				//draw background
				e.Graphics.FillRectangle(lnbrs, new Rectangle(Point.Empty, this.Size));
				if (_orientation == Orientation.Horizontal)
					e.Graphics.DrawLine(Pens.Black, 0, this.Height - 1, this.Width, this.Height - 1);
				else
					e.Graphics.DrawLine(Pens.Black, this.Width - 1, 0, this.Width - 1, this.Height);
			}
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			DrawRulerUnits(e);
		}
		/// <summary>
		/// draws the ruler
		/// </summary>
		private void DrawRulerUnits(PaintEventArgs e)
		{
			int unit = 1,//starting unit count
				index = 0;//subdivision count
			float unitwidth = _zoom.Scale((float)unit);
			if (unitwidth == 0f) return;//error
			for (; unitwidth <= 60f && index < 1000; index++)
			{
				unitwidth *= multipliers[index % multipliers.Length];
				unit = (int)(unit * multipliers[index % multipliers.Length]);
			}
			//start one unit off the control
			int startunit = -(_zoom.Unscale(_offset) / unit) * unit - unit;
			using (StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap))
			{
				fmt.LineAlignment = StringAlignment.Center;
				fmt.Alignment = StringAlignment.Near;
				//draw horizontal
				if (_orientation == Orientation.Horizontal)
				{

					for (float x = _zoom.Scale((float)startunit) + (float)_offset;
						x < this.Width; x += unitwidth, startunit += unit)
					{
						DrawXLines(e.Graphics, x, unitwidth, this.Height - 1, index + 1);
						e.Graphics.DrawString(startunit.ToString(), this.Font, Brushes.Black,
							new RectangleF(x, 0f, 60f, this.Height / 2), fmt);
					}
				}
				//draw vertical
				else
				{
					fmt.FormatFlags |= StringFormatFlags.DirectionVertical;
					for (float y = _zoom.Scale((float)startunit) + (float)_offset;
						y < this.Height; y += unitwidth, startunit += unit)
					{
						DrawYLines(e.Graphics, y, unitwidth, this.Width - 1, index + 1);
						e.Graphics.DrawString((-startunit).ToString(), this.Font, Brushes.Black,
							new RectangleF(0f, y, this.Width / 2, 60f), fmt);
					}
				}
			}
			using (SolidBrush brs = new SolidBrush(Color.FromArgb(127, 0, 0, 0)))
			{
				Rectangle rct = GetMarkerBounds(_value, _length);
				e.Graphics.FillRectangle(brs, rct);
				rct.Width--; rct.Height--;
				if (rct.Width > 1 && rct.Height > 1)
					e.Graphics.DrawRectangle(Pens.White, rct);
			}
		}
		/// <summary>
		/// draws the subdivisions on a horizontal ruler
		/// </summary>
		private void DrawXLines(Graphics gr, float x, float width, float height, int depth)
		{
			if (depth < 1) return;
			//draw divider line
			gr.DrawLine(Pens.Gray,
				x, (float)this.Height - 2f,
				x, (float)this.Height - 2f - height);
			//draw subdivisions
			float nwidth = width / multipliers[depth % multipliers.Length];
			if (nwidth < 3f) return;
			height *= 0.66f;
			for (float nx = x; nx < x + width; nx += nwidth)
			{
				DrawXLines(gr, nx, nwidth, height, depth - 1);
			}
		}
		/// <summary>
		/// draws the subdivisions on a vertical ruler
		/// </summary>
		private void DrawYLines(Graphics gr, float y, float height, float width, int depth)
		{
			if (depth < 1) return;
			//draw divider line
			gr.DrawLine(Pens.Gray,
				(float)this.Width - 2f, y,
				(float)this.Width - 2f - width, y);
			//draw subdivisions
			float nheight = height / multipliers[depth % multipliers.Length];
			if (nheight < 3f) return;
			width *= 0.66f;
			for (float ny = y; ny < y + height; ny += nheight)
			{
				DrawYLines(gr, ny, nheight, width, depth - 1);
			}
		}
		/// <summary>
		/// gets the bounding rectangle of a marker at the specified position
		/// </summary>
		private Rectangle GetMarkerBounds(float value, float length)
		{
			if (float.IsInfinity(value) ||
				float.IsNaN(value) ||
				float.IsInfinity(length) ||
				float.IsNaN(length))
				return Rectangle.Empty;
			if (_orientation == Orientation.Horizontal)
				return new Rectangle(
					(int)_zoom.Scale(value) + _offset + 1, 0,
					Math.Max(1, (int)_zoom.Scale(length)), this.Height - 1);
			else//vertical
				return new Rectangle(
					 0, (int)_zoom.Scale(value) + _offset + 1,
					 this.Width - 1, Math.Max(1, (int)_zoom.Scale(length)));
		}
		#endregion
		/// <summary>
		/// use this to display a marker, use float.NaN to hide it
		/// </summary>
		public void SetMarker(float value, float length)
		{
			if (value == _value && length == _length) return;
			this.Invalidate(
				Rectangle.Inflate(GetMarkerBounds(_value, _length), 1, 1));
			_value = value;
			_length = length;
			this.Invalidate(
				Rectangle.Inflate(GetMarkerBounds(value, length), 1, 1));
			this.Update();
		}
		#region properties
		/// <summary>
		/// gets or sets the orientation of the ruler
		/// </summary>
		[DefaultValue(typeof(Orientation), "Horizontal")]
		public Orientation Orientation
		{
			get { return _orientation; }
			set
			{
				if (value == _orientation) return;
				_orientation = value;
				this.Refresh();
			}
		}
		/// <summary>
		/// gets or sets the offset of the ruler measured
		/// </summary>
		[DefaultValue(0)]
		public int Offset
		{
			get { return _offset; }
			set
			{
				if (value == _offset) return;
				_offset = value;
				this.Refresh();
			}
		}
		/// <summary>
		/// gets or sets the zoom of the ruler
		/// </summary>
		[Browsable(false)]
		public ScaleFactor Zoom
		{
			get { return _zoom; }
			set
			{
				if (value == _zoom) return;
				_zoom = value;
				this.Refresh();
			}
		}
		/// <summary>
		/// gets or sets the value of the marker in units,
		/// to hide the marker, specify float.NaN
		/// to specifiy both value and length in one setp,
		/// use <see cref="SetMarker"/>
		/// </summary>
		[Browsable(false)]
		public float Value
		{
			get { return _value; }
			set { SetMarker(value, _length); }
		}
		/// <summary>
		/// gets or sets the length of the marker in units,
		/// to hide the marker, specify float.NaN
		/// to specifiy both value and length in one setp,
		/// use <see cref="SetMarker"/>
		/// </summary>
		public float Length
		{
			get { return _length; }
			set { SetMarker(_value, value); }
		}
		#endregion
	}
}
