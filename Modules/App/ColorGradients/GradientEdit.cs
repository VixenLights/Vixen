using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Common.Controls.ColorManagement.ColorModels;
using System.ComponentModel;
using System.Linq;
using Common.Controls.Scaling;

namespace VixenModules.App.ColorGradients
{
	/// <summary>
	/// edit control for a gradient
	/// </summary>
	[DefaultProperty("Gradient"),
	 DefaultEvent("SelectionChanged")]
	public class GradientEdit : Control
	{
		#region variables

		private readonly int _border;
		private ColorGradient _blend;
		private Orientation _orientation = Orientation.Horizontal;
		//selection
		private List<ColorGradient.Point> _selection = null;
		private bool _focussel = false;
		private Point _offset;
		private bool _discreteColors;
		private IEnumerable<Color> _validDiscreteColors;

		#endregion

		public GradientEdit()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
			         ControlStyles.OptimizedDoubleBuffer |
			         ControlStyles.UserPaint, true);
			_border = (int) (10*ScalingTools.GetScaleFactor());
		}

		#region helper

		/// <summary>
		/// converts a ui point into corresponding fader position
		/// </summary>
		private double PointToPos(Point pt)
		{
			if (_orientation == Orientation.Horizontal)
				return Math.Max(0.0, Math.Min(1.0,
				                              (double) (pt.X - _border)/
				                              (double) Math.Max(1.0, this.Width - _border*2 - 1)));
			//vertical
			return Math.Max(0.0, Math.Min(1.0,
			                              (double) (pt.Y - _border)/
			                              (double) Math.Max(1.0, this.Height - _border*2 - 1)));
		}

		/// <summary>
		/// converts a position to ui rectangle
		/// </summary>
		private Rectangle PosToRectangle(double pos)
		{
			if (_orientation == Orientation.Horizontal)
				return new Rectangle(
					(int) (pos*(double) (this.Width - _border*2 - 1)), 0,
					_border*2, this.Height - 1);
			//vertical
			return new Rectangle(
				0, (int) (pos*(double) (this.Height - _border*2 - 1)),
				this.Width - 1, _border*2);
		}

		/// <summary>
		/// gets the polygon of a fader in the selected rectangle
		/// </summary>
		private Point[] GetFaderPolygon(Rectangle fader, bool flip)
		{
			//todo: vertical
			if (flip)
				return new Point[]
				       	{
				       		new Point(fader.X + ScaleValue(1), fader.Y), new Point(fader.Right - ScaleValue(1), fader.Y),
				       		new Point(fader.Right, fader.Y + ScaleValue(1)), new Point(fader.Right, fader.Y + ScaleValue(16)),
				       		new Point(fader.X + fader.Width/2, fader.Y + ScaleValue(20)),
				       		new Point(fader.X, fader.Y + ScaleValue(16)), new Point(fader.X, fader.Y + ScaleValue(1))
				       	};
			return new Point[]
			       	{
			       		new Point(fader.X + 1, fader.Bottom), new Point(fader.Right - 1, fader.Bottom),
			       		new Point(fader.Right, fader.Bottom - ScaleValue(1)), new Point(fader.Right, fader.Bottom - ScaleValue(16)),
			       		new Point(fader.X + fader.Width/2, fader.Bottom - ScaleValue(20)),
			       		new Point(fader.X, fader.Bottom - ScaleValue(16)), new Point(fader.X, fader.Bottom - ScaleValue(1))
			       	};
		}

		private Point[] GetDiamondPolygon(Rectangle fader, bool flip)
		{
			int m = fader.X + fader.Width/2;
			//todo: vertical
			if (flip)
				return new Point[]
				       	{
				       		new Point(m - ScaleValue(5), fader.Y + ScaleValue(8)), new Point(m, fader.Y + ScaleValue(3)), new Point(m + ScaleValue(5), fader.Y + ScaleValue(8)),
				       		new Point(m, fader.Y + ScaleValue(13))
				       	};
			return new Point[]
			       	{
			       		new Point(m - ScaleValue(5), fader.Bottom - ScaleValue(8)), new Point(m, fader.Bottom - ScaleValue(3)), new Point(m + ScaleValue(5), fader.Bottom - ScaleValue(8)),
			       		new Point(m, fader.Bottom - ScaleValue(13))
			       	};
		}

		/// <summary>
		/// gets the color area of a fader
		/// </summary>
		private RectangleF GetFaderArea(Rectangle fader, bool flip)
		{
			if (flip)
				return new RectangleF(fader.X + ScaleValue(1f), fader.Y + ScaleValue(1.5f), fader.Width - ScaleValue(2), ScaleValue(14));
			return new RectangleF(fader.X + ScaleValue(1f), fader.Bottom - ScaleValue(14.5f), fader.Width - ScaleValue(2), ScaleValue(14));
		}

		private int ScaleValue(int value)
		{
			return (int) (value*ScalingTools.GetScaleFactor());
		}

		private float ScaleValue(float value)
		{
			return (float) (value*ScalingTools.GetScaleFactor());
		}

		/// <summary>
		/// draws a fader
		/// </summary>
		private void DrawFader(Graphics gr, double pos, IEnumerable<Color> colors, bool flip, bool selected)
		{
			Rectangle fader = PosToRectangle(pos);
			Point[] pts = GetFaderPolygon(fader, flip);
			RectangleF field = GetFaderArea(fader, flip);
			//draw fader
			gr.FillPolygon(selected ? SystemBrushes.Highlight : SystemBrushes.Control, pts);
			//draw background
			ControlPaint.DrawButton(gr, Rectangle.Inflate(
				Rectangle.Ceiling(field), 1, 1), ButtonState.Flat);
			if (colors.Any(x => x.A != 255))
				using (HatchBrush brs = new HatchBrush(HatchStyle.SmallCheckerBoard,
				                                       Color.Gray, Color.White)) {
					gr.RenderingOrigin = Point.Truncate(field.Location);
					gr.FillRectangle(brs, field);
				}
			//draw color
			int count = colors.Count();
			if (count == 1) {
				using (SolidBrush brs = new SolidBrush(colors.First())) {
					gr.FillRectangle(brs, field);
				}
			}
			else {
				using (LinearGradientBrush lgb = new LinearGradientBrush(field, Color.Black, Color.White, 0.0)) {
					ColorBlend cb = new ColorBlend(count);
					double increment = 1.0/(count - 1);
					cb.Positions = new float[count];
					for (int i = 0; i < count; i++) {
						cb.Positions[i] = (float) Math.Min((increment*i), 1.0);
					}
					cb.Colors = colors.ToArray();

					lgb.InterpolationColors = cb;
					gr.FillRectangle(lgb, field);
				}
			}
			//frame
			gr.DrawPolygon(Pens.Black, pts);
		}

		/// <summary>
		/// draws a diamond
		/// </summary>
		private void DrawDiamond(Graphics gr, double pos, bool flip, bool selected)
		{
			if (!double.IsNaN(pos)) {
				Rectangle fader = PosToRectangle(pos);
				Point[] pts = GetDiamondPolygon(fader, flip);
				gr.FillPolygon(selected ? SystemBrushes.Highlight : SystemBrushes.Control, pts);
				gr.DrawPolygon(Pens.Black, pts);
			}
		}

		/// <summary>
		/// gets if the fader or diamond at the given position would be hit by the given point
		/// </summary>
		private bool HitsSlider(Graphics gr, double pos, bool flip, bool diamond, Point pt, ref Point offset)
		{
			using (GraphicsPath path = new GraphicsPath()) {
				Rectangle fader = PosToRectangle(pos);
				offset = new Point(pt.X - (fader.X + fader.Width/2),
				                   pt.Y - (fader.Y + fader.Height/2));
				path.AddPolygon(diamond
				                	? GetDiamondPolygon(fader, flip)
				                	: GetFaderPolygon(fader, flip));
				return path.IsVisible(pt);
			}
		}

		/// <summary>
		/// gets the fader under the mouse and writes the mouseoffset
		/// to offset. if no fader is under the mouse, offset is (0,0)
		/// </summary>
		private List<ColorGradient.Point> GetFadersUnderMouse(Point location, ref Point offset, out bool focus)
		{
			focus = false;
			if (_blend != null) {
				using (Graphics gr = this.CreateGraphics()) {
					//check selection
					if (_selection != null && _selection.Count > 0) {
						//fader
						if (_selection.All(x => HitsSlider(gr, x.Position, x is AlphaPoint, false, location, ref _offset)))
							return _selection;
						//focus
						if (_selection.All(x => HitsSlider(gr, _blend.GetFocusPosition(x), x is AlphaPoint, true, location, ref _offset))) {
							focus = true;
							return _selection;
						}
					}
					List<ColorGradient.Point> rv = new List<ColorGradient.Point>();
					//check other color faders
					double targetPosition = -1;
					Point rvOffset = Point.Empty;
					foreach (ColorPoint pnt in _blend.Colors.SortedArray()) {
						if (HitsSlider(gr, pnt.Position, false, false, location, ref rvOffset)) {
							if (targetPosition < 0) {
								targetPosition = pnt.Position;
								offset = rvOffset;
							}
							if (pnt.Position == targetPosition) {
								rv.Add(pnt);
							}
						}
					}
					return rv;
					//check other alpha faders
					//foreach (AlphaPoint pnt in _blend.Alphas)
					//    if (HitsSlider(gr, pnt.Position, true, false, location, ref offset))
					//        rv.Add(pnt);
				}
			}
			offset = Point.Empty;
			return null;
		}

		/// <summary>
		/// move, drag out/in a fader
		/// </summary>
		private bool UpdateSelection(MouseEventArgs e)
		{
			if (_blend == null)
				return false;
			if (_selection != null && _selection.Count > 0) {
				if (_focussel) {
					//move focus point
					_blend.SetFocusPosition(_selection.First(), PointToPos(new Point(
					                                                       	e.X - _offset.X,
					                                                       	e.Y - _offset.Y)));
				}
				else if (!this.ClientRectangle.Contains(e.Location)) {
					// don't remove the point if dragged outside the area anymore,
					// it's bloody annoying and easy to do.
					////remove point if dragged out
					//_tmp = _selection;
					//if (_selection is ColorPoint)
					//    _blend.Colors.Remove(_selection as ColorPoint);
					//else
					//    _blend.Alphas.Remove(_selection as AlphaPoint);

					//_selection = null;
					//RaiseSelectionChanged();
				}
				else {
					//move selected point
					foreach (ColorGradient.Point point in _selection) {
						point.Position = PointToPos(new Point(e.X - _offset.X, e.Y - _offset.Y));
					}
				}
			}
			else
				return false;
			return true;
		}

		#endregion

		#region controller

		//draw gradient and faders
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (this.Width < _border*2 || this.Height < _border*2)
				return;
			e.Graphics.SmoothingMode =
				System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			//draw gradient
			Rectangle area = new Rectangle(_border, _border,
			                               this.Width - _border*2 - 1, this.Height - _border*3 - 1);
			if (_blend != null) {
				using (HatchBrush brs = new HatchBrush(HatchStyle.LargeCheckerBoard,
													   Color.Silver, Color.White)) {
					e.Graphics.FillRectangle(brs, area);
				}
				using (Bitmap bmp = _blend.GenerateColorGradientImage(area.Size, DiscreteColors)) {
					e.Graphics.DrawImage(bmp, area.X, area.Y);
				}
			}
			//border
			e.Graphics.DrawRectangle(Pens.Silver, area);
			if (_blend != null) {
				List<ColorPoint> sortedColors = new List<ColorPoint>(_blend.Colors.SortedArray());
				// we'll assume they're sorted, so any colorpoints at the same position are contiguous in the array
				for (int i = 0; i < sortedColors.Count; i++) {
					ColorPoint currentPoint = sortedColors[i];
					double currentPos = currentPoint.Position;
					List<Color> currentColors = new List<Color>();
					currentColors.Add(currentPoint.GetColor(Color.Black));

					while (i + 1 < sortedColors.Count && sortedColors[i + 1].Position == currentPos) {
						currentColors.Add(sortedColors[i + 1].GetColor(Color.Black));
						i++;
					}

					DrawFader(e.Graphics, currentPoint.Position, currentColors, false,
					          _selection != null && _selection.Contains(currentPoint) && !_focussel);
				}

				////draw color points
				//foreach (ColorPoint pnt in _blend.Colors)
				//    DrawFader(e.Graphics, pnt.Position,
				//        pnt.GetColor(Color.Black),
				//        false, pnt == _selection && !_focussel);
				// MS: 25/09/11: disable drawing alpha points, we don't want to use them (yet).
				// draw alpha points
				//foreach (AlphaPoint pnt in _blend.Alphas)
				//    DrawFader(e.Graphics, pnt.Position,
				//        pnt.GetColor(Color.Black),
				//        true, pnt == _selection && !_focussel);

				// draw selected focus
				if (_selection != null && _selection.Count > 0)
					DrawDiamond(e.Graphics, _blend.GetFocusPosition(_selection.First()),
					            _selection.First() is AlphaPoint, _focussel);
			}
		}

		//select or add faders
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (_blend != null && e.Button == MouseButtons.Left && !ReadOnly) {
				bool foc;
				Selection = GetFadersUnderMouse(e.Location, ref _offset, out foc);
				FocusSelection = foc;
				if (_selection == null || _selection.Count == 0) {
					//create new color or alpha point
					Rectangle area = Rectangle.Inflate(this.ClientRectangle, -_border, -_border);
					double pos = PointToPos(e.Location);
					_offset = Point.Empty;
					//
					if (_orientation == Orientation.Horizontal
					    	? e.Y > area.Bottom
					    	: e.X > area.Right) {
						List<ColorGradient.Point> newColorPoints;
						if (DiscreteColors) {
							List<Color> newColors = new List<Color>();
							double targetPos = -1;
							foreach (ColorPoint colorPoint in _blend.Colors.SortedArray()) {
								if (targetPos < 0 || (colorPoint.Position < pos && colorPoint.Position != targetPos)) {
									targetPos = colorPoint.Position;
									newColors = new List<Color>();
								}
								if (colorPoint.Position == targetPos)
									newColors.Add(colorPoint.Color.ToRGB());
								if (colorPoint.Position > targetPos)
									break;
							}
							newColorPoints = new List<ColorGradient.Point>();
							foreach (Color newColor in newColors) {
								ColorPoint point = new ColorPoint(newColor, targetPos);
								newColorPoints.Add(point);
							}
						}
						else {
							newColorPoints = new List<ColorGradient.Point> {new ColorPoint(_blend.GetColorAt((float) pos), pos)};
						}
						_selection = newColorPoints;
						foreach (ColorPoint newColorPoint in newColorPoints) {
							_blend.Colors.Add(newColorPoint);
						}
					}
					else if (_orientation == Orientation.Horizontal
					         	? e.Y < area.Y
					         	: e.X < area.X) {
						// MS: 25/09/11: disable drawing alpha points, we don't want to use them (yet).
						//AlphaPoint pnt = new AlphaPoint(_blend.GetColorAt((float)pos).A, pos);
						//_selection = pnt;
						//_blend.Alphas.Add(pnt);
					}
				}
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			//We need to make sure we double clicked on a fader and not just the control.
			Point pt = Point.Empty;
			bool foc;
			if (!GetFadersUnderMouse(e.Location, ref pt, out foc).Any())
			{
				return;
			}

			if (Selection != null && !ReadOnly)
				RaiseSelectionDoubleClicked();
			base.OnMouseDoubleClick(e);
		}

		//modify fader and set cursor
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				UpdateSelection(e);
			else if (!ReadOnly) {
				//set cursor
				Point pt = Point.Empty;
				Rectangle area = Rectangle.Inflate(
					this.ClientRectangle, -_border, -_border);
				bool foc;
				List<ColorGradient.Point> underMouse = GetFadersUnderMouse(e.Location, ref pt, out foc);
				if (underMouse != null && underMouse.Count > 0)
					//hit fader
					this.Cursor = Cursors.SizeWE;
				else if (_orientation == Orientation.Horizontal
				         	? // MS: 25/09/11: disable drawing alpha points, we don't want to use them (yet).
				         //(e.Y < area.Y || e.Y > area.Bottom) :
				         //(e.X < area.X || e.X > area.Right))
				         (e.Y > area.Bottom)
				         	: (e.X > area.Right))
					//create new point
					this.Cursor = Cursors.Hand;
				else
					//nothing
					this.Cursor = Cursors.Default;
				//
			}

			base.OnMouseMove(e);
		}

		//set cursor
		protected override void OnMouseLeave(EventArgs e)
		{
			this.Cursor = Cursors.Default;
			base.OnMouseLeave(e);
		}

		//mofify fader and reset
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (!UpdateSelection(e))
				base.OnMouseUp(e);
		}

		//general
		private void onChanged(object sender, EventArgs e)
		{
			this.Refresh();
			//update ui
			ModifiedEventArgs args = e as ModifiedEventArgs;
			if (args != null && (args.Action == Action.Cleared ||
			                     (args.Action == Action.Removed && _selection.Contains(args.Point))))
				Selection = null;
			else
				RaiseGradientChanged();
		}

		#endregion

		#region properties

		/// <summary>
		/// gets or sets the used color blend
		/// </summary>
		[Browsable(false),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ColorGradient Gradient
		{
			get { return _blend; }
			set
			{
				//if (value == _blend) return;
				//prepare
				if (_blend != null) {
					_blend.Changed -= new EventHandler(onChanged);
				}
				if (value != null) {
					value.Changed += new EventHandler(onChanged);
				}
				//set
				_selection = null;
				_blend = value;
				//update
				Refresh();
			}
		}

		/// <summary>
		/// gets or sets the selected alpha or color point
		/// </summary>
		[Browsable(false),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<ColorGradient.Point> Selection
		{
			get
			{
				if (_selection == null)
					_selection = new List<ColorGradient.Point>();
				return _selection;
			}
			set
			{
				if (value == _selection || _blend == null)
					return;

				if (value != null) {
					foreach (ColorGradient.Point point in value) {
						if (!_blend.Alphas.Contains(point as AlphaPoint) && !_blend.Colors.Contains(point as ColorPoint))
							return;
					}
				}

				if (_selection != null && _selection.Count > 0) {
					foreach (ColorGradient.Point point in _selection) {
						Invalidate(PosToRectangle(point.Position));
					}
					//update focus
					double foc = _blend.GetFocusPosition(_selection.First());
					if (!double.IsNaN(foc))
						this.Invalidate(PosToRectangle(foc));
				}
				if (value != null && value.Count > 0) {
					foreach (ColorGradient.Point point in value) {
						Invalidate(PosToRectangle(point.Position));
					}
					//update focus
					double foc = _blend.GetFocusPosition(value.First());
					if (!double.IsNaN(foc))
						this.Invalidate(PosToRectangle(foc));
				}
				_selection = value;
				this.Update();
				RaiseSelectionChanged();
			}
		}

		/// <summary>
		/// gets or sets if the focus point is selected
		/// </summary>
		[Browsable(false),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool FocusSelection
		{
			get { return _focussel; }
			set
			{
				if (value == _focussel)
					return;
				_focussel = value;
				if (_selection != null && _selection.Count > 0) {
					foreach (ColorGradient.Point point in _selection) {
						Invalidate(PosToRectangle(point.Position));
					}
					//update focus
					double foc = _blend.GetFocusPosition(_selection.First());
					if (!double.IsNaN(foc))
						this.Invalidate(PosToRectangle(foc));

					this.Update();
				}
				RaiseSelectionChanged();
			}
		}

		/// <summary>
		/// sets the selected color index in range -1 (none selected)
		/// to length-1
		/// </summary>
		[Browsable(false),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int[] SelectedColorIndex
		{
			get
			{
				if (_blend == null)
					return null;
				List<int> rv = new List<int>();
				foreach (ColorGradient.Point point in _selection) {
					rv.Add(_blend.Colors.IndexOf(point as ColorPoint));
				}
				return rv.ToArray();
			}
			set
			{
				if (_blend == null)
					return;
				//invalidate
				if (value == null)
					Selection = null;
				else {
					Selection = new List<ColorGradient.Point>();
					foreach (int i in value) {
						Selection.Add(_blend.Colors[i]);
					}
				}
			}
		}

		/// <summary>
		/// sets the selected alpha index in range -1 (none selected)
		/// to length-1
		/// </summary>
		[Browsable(false),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedAlphaIndex
		{
			get
			{
				if (_blend == null)
					return -1;
				return _blend.Alphas.IndexOf(_selection.FirstOrDefault() as AlphaPoint);
			}
			set
			{
				if (_blend == null)
					return;
				//invalidate
				if (value == -1)
					Selection = null;
				else
					Selection = new List<ColorGradient.Point> {_blend.Alphas[value]};
			}
		}

		public bool ReadOnly { get; set; }

		public bool DiscreteColors
		{
			get { return _discreteColors; }
			set
			{
				_discreteColors = value;
				Refresh();
			}
		}

		public IEnumerable<Color> ValidDiscreteColors
		{
			get { return _validDiscreteColors; }
			set
			{
				_validDiscreteColors = value;
				Refresh();
			}
		}

		#endregion

		#region events

		private void RaiseSelectionChanged()
		{
			if (SelectionChanged != null)
				SelectionChanged(this, EventArgs.Empty);
		}

		private void RaiseGradientChanged()
		{
			if (GradientChanged != null)
				GradientChanged(this, EventArgs.Empty);
		}

		private void RaiseSelectionDoubleClicked()
		{
			if (SelectionDoubleClicked != null)
				SelectionDoubleClicked(this, EventArgs.Empty);
		}

		/// <summary>
		/// raised when the selected color stop is changed
		/// </summary>
		public event EventHandler SelectionChanged;

		/// <summary>
		/// raised when the gradient is changed
		/// </summary>
		public event EventHandler GradientChanged;

		/// <summary>
		/// raised when the selected color stop is double clicked
		/// </summary>
		public event EventHandler SelectionDoubleClicked;

		#endregion
	}
}