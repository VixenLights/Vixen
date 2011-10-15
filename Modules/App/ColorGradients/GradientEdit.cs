using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using CommonElements.ColorManagement.ColorModels;
using System.ComponentModel;

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

		private const int BORDER = 6;
		private ColorGradient _blend;
		private Orientation _orientation = Orientation.Horizontal;
		//selection
		private ColorGradient.Point _selection = null, _tmp = null;
		private bool _focussel = false;
		private Point _offset;

		#endregion


		public GradientEdit()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.OptimizedDoubleBuffer |
				ControlStyles.UserPaint, true);
		}


		#region helper

		/// <summary>
		/// converts a ui point into corresponding fader position
		/// </summary>
		private double PointToPos(Point pt)
		{
			if (_orientation == Orientation.Horizontal)
				return Math.Max(0.0, Math.Min(1.0,
					(double)(pt.X - BORDER) /
					(double)Math.Max(1.0, this.Width - BORDER * 2 - 1)));
			//vertical
			return Math.Max(0.0, Math.Min(1.0,
				(double)(pt.Y - BORDER) /
				(double)Math.Max(1.0, this.Height - BORDER * 2 - 1)));
		}

		/// <summary>
		/// converts a position to ui rectangle
		/// </summary>
		private Rectangle PosToRectangle(double pos)
		{
			if (_orientation == Orientation.Horizontal)
				return new Rectangle(
					(int)(pos * (double)(this.Width - BORDER * 2 - 1)), 0,
					BORDER * 2, this.Height - 1);
			//vertical
			return new Rectangle(
				0, (int)(pos * (double)(this.Height - BORDER * 2 - 1)),
				this.Width - 1, BORDER * 2);
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
							new Point(fader.X+1,fader.Y),new Point(fader.Right-1,fader.Y),
							new Point(fader.Right,fader.Y+1),new Point(fader.Right,fader.Y+11),
							new Point(fader.X+fader.Width/2,fader.Y+15),
							new Point(fader.X,fader.Y+11),new Point(fader.X,fader.Y+1)
						};
			return new Point[]
						{
							new Point(fader.X+1,fader.Bottom),new Point(fader.Right-1,fader.Bottom),
							new Point(fader.Right,fader.Bottom-1),new Point(fader.Right,fader.Bottom-11),
							new Point(fader.X+fader.Width/2,fader.Bottom-15),
							new Point(fader.X,fader.Bottom-11),new Point(fader.X,fader.Bottom-1)
						};
		}

		private Point[] GetDiamondPolygon(Rectangle fader, bool flip)
		{
			int m = fader.X + fader.Width / 2;
			//todo: vertical
			if (flip)
				return new Point[] {
					new Point(m-3,fader.Y+6),new Point(m,fader.Y+3),new Point(m+3,fader.Y+6),new Point(m,fader.Y+9)
				};
			return new Point[]{
					new Point(m-3,fader.Bottom-6),new Point(m,fader.Bottom-3),new Point(m+3,fader.Bottom-6),new Point(m,fader.Bottom-9)
				};
		}

		/// <summary>
		/// gets the color area of a fader
		/// </summary>
		private RectangleF GetFaderArea(Rectangle fader, bool flip)
		{
			if (flip)
				return new RectangleF(fader.X + 1.5f, fader.Y + 1.5f, fader.Width - 3, 8);
			return new RectangleF(fader.X + 1.5f, fader.Bottom - 9.5f, fader.Width - 3, 8);
		}

		/// <summary>
		/// draws a fader
		/// </summary>
		private void DrawFader(Graphics gr, double pos, Color col, bool flip, bool selected)
		{
			Rectangle fader = PosToRectangle(pos);
			Point[] pts = GetFaderPolygon(fader, flip);
			RectangleF field = GetFaderArea(fader, flip);
			//draw fader
			gr.FillPolygon(selected ? SystemBrushes.Highlight : SystemBrushes.Control, pts);
			//draw background
			ControlPaint.DrawButton(gr, Rectangle.Inflate(
				Rectangle.Ceiling(field), 1, 1), ButtonState.Normal);
			if (col.A != 255)
				using (HatchBrush brs = new HatchBrush(HatchStyle.SmallCheckerBoard,
						Color.Gray, Color.White))
				{
					gr.RenderingOrigin = Point.Truncate(field.Location);
					gr.FillRectangle(brs, field);
				}
			//draw color
			using (SolidBrush brs = new SolidBrush(col))
			{
				gr.FillRectangle(brs, field);
			}
			//frame
			gr.DrawPolygon(Pens.Black, pts);
		}

		/// <summary>
		/// draws a diamond
		/// </summary>
		private void DrawDiamond(Graphics gr, double pos, bool flip, bool selected)
		{
			if (!double.IsNaN(pos))
			{
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
			using (GraphicsPath path = new GraphicsPath())
			{
				Rectangle fader = PosToRectangle(pos);
				offset = new Point(pt.X - (fader.X + fader.Width / 2),
					pt.Y - (fader.Y + fader.Height / 2));
				path.AddPolygon(diamond ?
					GetDiamondPolygon(fader, flip) :
					GetFaderPolygon(fader, flip));
				return path.IsVisible(pt);
			}
		}

		/// <summary>
		/// gets the fader under the mouse and writes the mouseoffset
		/// to offset. if no fader is under the mouse, offset is (0,0)
		/// </summary>
		private ColorGradient.Point GetFaderUnderMouse(Point location, ref Point offset, out bool focus)
		{
			focus = false;
			if (_blend != null)
			{
				using (Graphics gr = this.CreateGraphics())
				{
					//check selection
					if (_selection != null)
					{
						//fader
						if (HitsSlider(gr, _selection.Position, _selection is AlphaPoint,
							false, location, ref _offset)) return _selection;
						//focus
						if (HitsSlider(gr, _blend.GetFocusPosition(_selection), _selection is AlphaPoint,
							true, location, ref _offset)) { focus = true; return _selection; }
					}
					//check other color faders
					foreach (ColorPoint pnt in _blend.Colors)
						if (HitsSlider(gr, pnt.Position, false, false, location, ref offset))
							return pnt;
					//check other alpha faders
					foreach (AlphaPoint pnt in _blend.Alphas)
						if (HitsSlider(gr, pnt.Position, true, false, location, ref offset))
							return pnt;
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
			if (_selection != null)
			{
				if (_focussel)
				{
					//move focus point
					_blend.SetFocusPosition(_selection, PointToPos(new Point(
							e.X - _offset.X,
							e.Y - _offset.Y)));
				}
				else if (!this.ClientRectangle.Contains(e.Location))
				{
					//remove point if dragged out
					_tmp = _selection;
					if (_selection is ColorPoint)
						_blend.Colors.Remove(_selection as ColorPoint);
					else
						_blend.Alphas.Remove(_selection as AlphaPoint);

					_selection = null;
					RaiseSelectionChanged();
				}
				else
				{
					//move selected point
					_selection.Position = PointToPos(new Point(
							e.X - _offset.X,
							e.Y - _offset.Y));
				}
			}
			else if (_tmp != null && this.ClientRectangle.Contains(e.Location))
			{
				_tmp.Position = PointToPos(new Point(
							e.X - _offset.X,
							e.Y - _offset.Y));
				//re-add point
				if (_tmp is ColorPoint)
					_blend.Colors.Add((ColorPoint)_tmp);
				else
					_blend.Alphas.Add((AlphaPoint)_tmp);
				Selection = _tmp;
				_tmp = null;
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
			if (this.Width < BORDER * 2 || this.Height < BORDER * 2)
				return;
			e.Graphics.SmoothingMode =
				System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			//draw gradient
			Rectangle area = new Rectangle(BORDER, BORDER,
				this.Width - BORDER * 2 - 1, this.Height - BORDER * 2 - 1);
			if (_blend != null)
			{
				using (HatchBrush brs = new HatchBrush(HatchStyle.LargeCheckerBoard,
					Color.Silver, Color.White))
				{
					e.Graphics.FillRectangle(brs, area);
				}
				using (LinearGradientBrush brs = new LinearGradientBrush(area.Location,
					new Point(area.Right, area.Y), Color.Black, Color.Black))
				{
					brs.InterpolationColors = _blend.GetColorBlend();
					e.Graphics.FillRectangle(brs, area);
				}
			}
			//border
			e.Graphics.DrawRectangle(Pens.Silver, area);
			if (_blend != null)
			{
				//draw color points
				foreach (ColorPoint pnt in _blend.Colors)
					DrawFader(e.Graphics, pnt.Position,
						pnt.GetColor(Color.Black),
						false, pnt == _selection && !_focussel);
				// MS: 25/09/11: disable drawing alpha points, we don't want to use them (yet).
				// draw alpha points
				//foreach (AlphaPoint pnt in _blend.Alphas)
				//    DrawFader(e.Graphics, pnt.Position,
				//        pnt.GetColor(Color.Black),
				//        true, pnt == _selection && !_focussel);
				// draw selected focus
				if (_selection != null)
					DrawDiamond(e.Graphics, _blend.GetFocusPosition(_selection),
						_selection is AlphaPoint, _focussel);
			}
		}

		//select or add faders
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (_blend != null && e.Button == MouseButtons.Left)
			{
				bool foc;
				Selection = GetFaderUnderMouse(e.Location, ref _offset, out foc);
				FocusSelection = foc;
				if (_selection == null)
				{
					//create new color or alpha point
					Rectangle area = Rectangle.Inflate(this.ClientRectangle, -BORDER, -BORDER);
					double pos = PointToPos(e.Location);
					_offset = Point.Empty;
					//
					if (_orientation == Orientation.Horizontal ?
						e.Y > area.Bottom : e.X > area.Right)
					{
						ColorPoint pnt = new ColorPoint(_blend.GetColorAt((float)pos), pos);
						_selection = pnt;
						_blend.Colors.Add(pnt);
					}
					else if (_orientation == Orientation.Horizontal ?
						e.Y < area.Y : e.X < area.X)
					{
						// MS: 25/09/11: disable drawing alpha points, we don't want to use them (yet).
						//AlphaPoint pnt = new AlphaPoint(_blend.GetColorAt((float)pos).A, pos);
						//_selection = pnt;
						//_blend.Alphas.Add(pnt);
					}
				}
			}
			base.OnMouseDown(e);
		}

		//modify fader and set cursor
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				UpdateSelection(e);
			else
			{
				//set cursor
				Point pt = Point.Empty;
				Rectangle area = Rectangle.Inflate(
					this.ClientRectangle, -BORDER, -BORDER);
				bool foc;
				if (GetFaderUnderMouse(e.Location, ref pt, out foc) != null)
					//hit fader
					this.Cursor = Cursors.SizeWE;
				else if (_orientation == Orientation.Horizontal ?
						// MS: 25/09/11: disable drawing alpha points, we don't want to use them (yet).
						//(e.Y < area.Y || e.Y > area.Bottom) :
						//(e.X < area.X || e.X > area.Right))
						(e.Y > area.Bottom) :
						(e.X > area.Right))
					//create new point
					this.Cursor = Cursors.Hand;
				else
					//nothing
					this.Cursor = Cursors.Default;
				//
				base.OnMouseMove(e);
			}
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
			_tmp = null;
		}

		//general
		private void onChanged(object sender, EventArgs e)
		{
			this.Refresh();
			//update ui
			ModifiedEventArgs args = e as ModifiedEventArgs;
			if (args != null && (args.Action == Action.Cleared ||
				(args.Action == Action.Removed && args.Point == _selection)))
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
				if (value == _blend) return;
				//prepare
				if (_blend != null)
				{
					_blend.Changed -= new EventHandler(onChanged);
				}
				if (value != null)
				{
					value.Changed += new EventHandler(onChanged);
				}
				//set
				_selection = null;
				_tmp = null;
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
		public ColorGradient.Point Selection
		{
			get { return _selection; }
			set
			{
				if (value == _selection || _blend == null ||
					(value != null && !_blend.Alphas.Contains(value as AlphaPoint) &&
					!_blend.Colors.Contains(value as ColorPoint)))
					return;
				if (_selection != null)
				{
					this.Invalidate(PosToRectangle(_selection.Position));
					//update focus
					double foc = _blend.GetFocusPosition(_selection);
					if (!double.IsNaN(foc))
						this.Invalidate(PosToRectangle(foc));
				}
				if (value != null)
				{
					this.Invalidate(PosToRectangle(value.Position));
					//update focus
					double foc = _blend.GetFocusPosition(value);
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
				if (_selection != null)
				{
					this.Invalidate(PosToRectangle(_selection.Position));
					//update focus
					double foc = _blend.GetFocusPosition(_selection);
					if (!double.IsNaN(foc))
						this.Invalidate(PosToRectangle(foc));
					//
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
		public int SelectedColorIndex
		{
			get
			{
				if (_blend == null)
					return -1;
				return _blend.Colors.IndexOf(_selection as ColorPoint);
			}
			set
			{
				if (_blend == null)
					return;
				//invalidate
				if (value == -1)
					Selection = null;
				else
					Selection = _blend.Colors[value];
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
				return _blend.Alphas.IndexOf(_selection as AlphaPoint);
			}
			set
			{
				if (_blend == null)
					return;
				//invalidate
				if (value == -1)
					Selection = null;
				else
					Selection = _blend.Alphas[value];
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

		/// <summary>
		/// raised when the selected color stop is changed
		/// </summary>
		public event EventHandler SelectionChanged;

		/// <summary>
		/// raised when the gradient is changed
		/// </summary>
		public event EventHandler GradientChanged;

		#endregion
	}
}
