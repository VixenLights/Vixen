using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using CommonElements.ColorManagement.ColorModels;
using CommonElements.ControlsEx;

namespace CommonElements.ColorManagement.Gradients
{
	/// <summary>
	/// ColorBlend object
	/// </summary>
	public class Gradient : ICloneable
	{
		/// <summary>
		/// class for holding a gradient point
		/// </summary>
		public abstract class Point : IComparable<double>
		{
			//variables
			private Gradient _owner;
			private double _position;
			private double _focus;
			/// <summary>
			/// ctor
			/// </summary>
			public Point(double position)
				: this(position, 0.5) { }
			/// <summary>
			/// ctor
			/// </summary>
			public Point(double position, double focus)
			{
				if (!Gradient.isValid(position) ||
					!Gradient.isValid(focus))
					throw new ArgumentException("position or focus");
				_position = position;
				_focus = focus;
			}
			/// <summary>
			/// gets the color
			/// </summary>
			public abstract Color GetColor(Color basecolor);
			#region properties
			internal void SetOwner(Gradient owner)
			{
				this._owner = owner;
			}
			public Gradient Owner
			{
				get { return _owner; }
			}
			/// <summary>
			/// gets or sets the position
			/// </summary>
			public double Position
			{
				get { return _position; }
				set
				{
					if (!Gradient.isValid(value))
						throw new ArgumentException("point");
					if (value == _position)
						return;
					_position = value;
					RaiseChange();
				}
			}
			/// <summary>
			/// gets or sets the focus
			/// </summary>
			public double Focus
			{
				get { return _focus; }
				set
				{
					if (!Gradient.isValid(value))
						throw new ArgumentException("focus");
					if (value == _focus)
						return;
					_focus = value;
					RaiseChange();
				}
			}
			//get focus position
			public abstract double GetFocusPosition();
			//set focus position
			public abstract void SetFocusPosition(double value);
			/// <summary>
			/// gets the absolute focus point
			/// </summary>
			protected double GetFocusPosition<T>(PointList<T> coll, T value) where T : Point, IComparable<T>
			{
				if (coll == null || value == null)
					throw new ArgumentNullException();
				//
				T[] sorted = coll.SortedArray();
				int i = Array.IndexOf<T>(sorted, value);
				if (i < 1)//first point or not found
					return double.NaN;
				return sorted[i - 1].Position + value.Focus *
					(sorted[i].Position - sorted[i - 1].Position);
			}
			protected void SetFocusPosition<T>(PointList<T> coll, T value, double focuspos) where T : Point, IComparable<T>
			{
				if (coll == null || value == null)
					throw new ArgumentNullException("coll or value");
				if (!Gradient.isValid(focuspos))
					throw new ArgumentException("focuspos");
				//
				T[] sorted = coll.SortedArray();
				int i = Array.IndexOf<T>(sorted, value);
				if (i > 0)
				{
					//calculate focus
					double w=sorted[i].Position - sorted[i - 1].Position;
					if (w <= 0)
						value.Focus = .5;
					else value.Focus = Math.Max(.0, Math.Min(1.0,
						(focuspos - sorted[i - 1].Position) / w));
				}
			}
			#endregion
			#region api
			public override bool Equals(object obj)
			{
				Point grd = obj as Point;
				if (grd == null)
					return false;
				return grd._focus == this._focus &&
					grd._position == this._position;
			}
			public override int GetHashCode()
			{
				return _focus.GetHashCode() << 16 ^
					_position.GetHashCode();
			}
			/// <summary>
			/// IComparable
			/// </summary>
			int IComparable<double>.CompareTo(double other)
			{
				return _position.CompareTo(other);
			}
			#endregion
			/// <summary>
			/// raises change event
			/// </summary>
			protected void RaiseChange()
			{
				if (_owner != null)
					_owner.RaiseChanged(new ModifiedEventArgs(Action.Modified, this));
			}
		}
		/// <summary>
		/// class for holding points and updating
		/// controls connected to this colorblend
		/// </summary>
		public class PointList<T> : CollectionBase<Gradient, T> where T : Point,IComparable<T>
		{
			public PointList(Gradient owner) : base(owner) { }
			#region change events
			//update owner property
			protected override void OnInsert(int index, T value)
			{
				if (value.Owner != null)
					throw new ArgumentException("can only be assigned to one owner");
				value.SetOwner(this.Owner);
			}
			protected override void OnSet(int index, T oldValue, T newValue)
			{
				if (newValue.Owner != null && newValue.Owner != this.Owner)
					throw new ArgumentException("can only be assigned to one owner");
				newValue.SetOwner(this.Owner);
				oldValue.SetOwner(null);
			}
			protected override void OnRemove(int index, T value)
			{
				((Point)value).SetOwner(null);
			}
			protected override void OnClear()
			{
				foreach (Point grd in this)
					grd.SetOwner(null);
			}
			//update edit controls
			protected override void OnValidate(T value)
			{
				if (value == null)
					throw new ArgumentNullException("value");
			}
			protected override void OnClearComplete()
			{
				this.Owner.RaiseChanged(new ModifiedEventArgs(Action.Cleared, null));
			}
			protected override void OnSetComplete(int index, T oldValue, T newValue)
			{
				this.Owner.RaiseChanged(new ModifiedEventArgs(Action.Removed, oldValue));
				this.Owner.RaiseChanged(new ModifiedEventArgs(Action.Added, newValue));
			}
			protected override void OnRemoveComplete(int index, T value)
			{
				this.Owner.RaiseChanged(new ModifiedEventArgs(Action.Removed, value));
			}
			protected override void OnInsertComplete(int index, T value)
			{
				this.Owner.RaiseChanged(new ModifiedEventArgs(Action.Added, value));
			}
			#endregion
			/// <summary>
			/// gets a sorted copy
			/// </summary>
			public T[] SortedArray()
			{
				T[] ret = new T[Count];
				CopyTo(ret, 0);
				Array.Sort(ret);
				return ret;
			}
		}
		#region variables
		private PointList<ColorPoint> _colors;
		private PointList<AlphaPoint> _alphas;
		private bool _gammacorrected = false;
		private ColorBlend _blend = null;
		private String _title = null;
		#endregion
		public Gradient()
		{
			_colors = new PointList<ColorPoint>(this);
			_alphas = new PointList<AlphaPoint>(this);
		}
		#region algorithms
		/// <summary>
		/// clamps the value to [0; 1]
		/// </summary>
		public static float Clamp(float value)
		{
			if (float.IsNaN(value) ||
				float.IsNegativeInfinity(value) || value < 0f)
				return 0f;
			if (float.IsPositiveInfinity(value) || value > 1f)
				return 1f;
			return value;
		}
		/// <summary>
		/// gets if double value is valid and between 0 and 1
		/// </summary>
		public static bool isValid(double value)
		{
			return !(double.IsNaN(value) ||
					double.IsInfinity(value) ||
					value < .0 || value > 1.0);
		}
		/// <summary>
		/// creates color blend out of color point data
		/// </summary>
		/// <returns></returns>
		private ColorBlend CreateColorBlend()
		{
			//sort all points
			ColorPoint[] colpoints = _colors.SortedArray();
			AlphaPoint[] alphapoints = _alphas.SortedArray();
			//init out vars
			SortedList<float, Color> positions = new SortedList<float, Color>();
			//add color points
			for (int c = 0; c < colpoints.Length; c++)
			{
				if (c > 0 && colpoints[c].Focus != .5)//focus
					AddPosition(colpoints, alphapoints, positions,
						colpoints[c - 1].Position + (colpoints[c].Position - colpoints[c - 1].Position) * colpoints[c].Focus);
				//color
				AddPosition(colpoints, alphapoints, positions, colpoints[c].Position);
			}
			//add alpha points
			for (int a = 0; a < alphapoints.Length; a++)
			{
				if (a > 0 && alphapoints[a].Focus != .5)//focus
					AddPosition(colpoints, alphapoints, positions,
						alphapoints[a - 1].Position + (alphapoints[a].Position - alphapoints[a - 1].Position) * alphapoints[a].Focus);
				//alpha
				AddPosition(colpoints, alphapoints, positions, alphapoints[a].Position);
			}

			//add first/last point
			if (positions.Count < 1 || !positions.ContainsKey(0f))
				positions.Add(0f, positions.Count < 1 ?
					Color.Transparent : positions.Values[0]);
			if (positions.Count < 2 || !positions.ContainsKey(1f))
				positions.Add(1f, positions.Count < 2 ?
					Color.Transparent : positions.Values[positions.Count - 1]);
			//
			ColorBlend ret = new ColorBlend();
			Color[] col = new Color[positions.Count];
			positions.Values.CopyTo(col, 0);
			ret.Colors = col;
			float[] pos = new float[positions.Count];
			positions.Keys.CopyTo(pos, 0);
			ret.Positions = pos;
			return ret;
		}
		/// <summary>
		/// adds all colors of the given blend
		/// </summary>
		private void AddColors(ColorBlend blend)
		{
			if (blend == null ||
				blend.Colors == null || blend.Positions == null ||
				blend.Colors.Length != blend.Positions.Length)
				throw new ArgumentException("blend is invalid");
			//
			for (int i = 0; i < blend.Colors.Length; i++)
			{
				if (blend.Colors[i].A != 255)
					_alphas.Add(new AlphaPoint((byte)blend.Colors[i].A,
						(double)blend.Positions[i]));
				_colors.Add(new ColorPoint(blend.Colors[i],
					(double)blend.Positions[i]));
			}
		}
		#region interpolation helpers
		//compute balance out of focus point
		private double FocusToBalance(double a, double b, double focus, double pos)
		{
			if (a >= b || pos<=a) return .0;
			double ret = (pos - a) / (b - a);
			if (focus != .5)
			{
				//focus influences the breakpoint of
				//linear interpolations
				if (ret == focus)
					return .5;
				else if (ret < focus)
					//ret cannot be 0 when ret<focus
					return .5 * ret / focus;
				else
					//ret cannot be 1 when ret>focus
					return .5 * (1.0 + (ret - focus) / (1.0 - focus));
			}
			return ret;
		}
		//adds a new position to the list
		private void AddPosition(ColorPoint[] colpoints, AlphaPoint[] alphapoints,
			SortedList<float, Color> positions, double pos)
		{
			if (positions.ContainsKey((float)pos))
				return;
			int alpha_a, alpha_b;
			int color_a, color_b;
			//evaluate positions
			SearchPos<AlphaPoint,double>(alphapoints, pos, out alpha_a, out alpha_b);
			SearchPos<ColorPoint,double>(colpoints, pos, out color_a, out color_b);
			//interpolate
			positions.Add((float)pos, Color.FromArgb(
				Interpolate(alphapoints, alpha_a, alpha_b, pos),
				Interpolate(colpoints, color_a, color_b, pos)));
		}
		// interpolates alpha list
		private byte Interpolate(AlphaPoint[] list, int a, int b, double pos)
		{
			if (b < a)
				return 0;
			if (a == b) return (byte)(list[a].Alpha * 255.0);
			//compute involving focus position
			return (byte)XYZ.ClipValue(
				(list[a].Alpha + FocusToBalance(list[a].Position, list[b].Position, list[b].Focus, pos)
				* (list[b].Alpha - list[a].Alpha)) * 255.0, 0.0, 255.0);
		}
		// interpolate on color list.
		private Color Interpolate(ColorPoint[] list, int a, int b, double pos)
		{
			if (b < a)
				return Color.Black;
			else if (a == b) return list[a].Color.ToRGB().ToArgb();
			double bal = FocusToBalance(list[a].Position, list[b].Position, list[b].Focus, pos);
			//compute
			RGB col_a = list[a].Color.ToRGB(),
				col_b = list[b].Color.ToRGB();
			return new RGB(col_a.R + bal * (col_b.R - col_a.R),
				col_a.G + bal * (col_b.G - col_a.G),
				col_a.B + bal * (col_b.B - col_a.B)).ToArgb();
		}
		// interpolate on colorblend
		private Color Interpolate(Color[] list, float[] positions, int a, int b, float pos)
		{
			if (b < a)
				return Color.Black;
			else if (a == b) return list[a];
			if (positions[a] >= positions[b]) return list[a];
			float bal = (pos - positions[a]) / (positions[b] - positions[a]);
			//
			return Color.FromArgb(
				(int)((float)list[a].A + bal * (float)(list[b].A - list[a].A)),
				(int)((float)list[a].R + bal * (float)(list[b].R - list[a].R)),
				(int)((float)list[a].G + bal * (float)(list[b].G - list[a].G)),
				(int)((float)list[a].B + bal * (float)(list[b].B - list[a].B)));
		}
		//generic interval searching in O(log(n))
		private void SearchPos<T,K>(T[] list, K pos, out int a, out int b) where T : IComparable<K>
		{
			int start = a = 0, end = b = list.Length - 1;
			while (end >= start)
			{
				int mid = start + (end - start) / 2;
				switch (list[mid].CompareTo(pos))
				{
					case 0://found point
						a = b = mid;
						return;
					case 1: end = mid - 1; b = mid; break;//search left
					default: start = mid + 1; a = mid; break;//search right
				}
			}
			//found interval
		}
		#endregion
		#endregion
		public static implicit operator ColorBlend(Gradient blend)
		{
			return blend.GetColorBlend();
		}
		public static implicit operator Gradient(ColorBlend blend)
		{
			Gradient ret = new Gradient();
			ret.AddColors(blend);
			return ret;
		}
		/// <summary>
		/// creates a System.Drawing.Drawing2D.ColorBlend
		/// out of this gradient
		/// </summary>
		public ColorBlend GetColorBlend()
		{
			if (_blend == null)
				_blend = CreateColorBlend();
			return _blend;
		}
		/// <summary>
		/// gets the color at the specified position
		/// </summary>
		public Color GetColorAt(float pos)
		{
			ColorBlend blend = GetColorBlend();
			pos = Clamp(pos);
			//
			int a, b;
			SearchPos<float,float>(blend.Positions, (float)pos, out a, out b);
			return Interpolate(blend.Colors, blend.Positions, a, b, pos);
		}
		#region properties
		/// <summary>
		/// gets or sets
		/// </summary>
		public bool Gammacorrected
		{
			get { return _gammacorrected; }
			set { _gammacorrected = value; }
		}
		public String Title
		{
			get { return _title; }
			set { _title = value; }
		}
		public PointList<AlphaPoint> Alphas
		{
			get { return _alphas; }
		}

		public PointList<ColorPoint> Colors
		{
			get { return _colors; }
		}
		#endregion
		#region event handling
		private void RaiseChanged(EventArgs args)
		{
			_blend = null;
			if (Changed != null)
				Changed(this, args);
		}
		public event EventHandler Changed;
		#endregion
		#region ICloneable Member
		/// <summary>
		/// returns a copy of all gradient points and parameters
		/// </summary>
		public object Clone()
		{
			Gradient ret = new Gradient();
			//
			if(_title!=null)
				ret._title = (string)_title.Clone();
			ret._gammacorrected = _gammacorrected;
			//
			foreach (ColorPoint cp in _colors)
				ret._colors.Add((ColorPoint)cp.Clone());
			foreach (AlphaPoint ap in _alphas)
				ret._alphas.Add((AlphaPoint)ap.Clone());
			//
			return ret;
		}
		#endregion
	}
	/// <summary>
	/// encapsulates an alpha value with focus point
	/// </summary>
	public class AlphaPoint : Gradient.Point, IComparable<AlphaPoint>, ICloneable
	{
		//variables
		private double _alpha;
		/// <summary>
		/// ctor
		/// </summary>
		public AlphaPoint(byte alpha, double point)
			: this((double)alpha / 255.0, .5, point)
		{ }
		/// <summary>
		/// ctor
		/// </summary>
		public AlphaPoint(double alpha, double focus, double point)
			: base(point, focus)
		{
			if (!Gradient.isValid(alpha))
				throw new ArgumentException("alpha");
			_alpha = alpha;
		}
		/// <summary>
		/// get color
		/// </summary>
		public override Color GetColor(Color basecolor)
		{
			return Color.FromArgb((int)(_alpha * 255.0),
				basecolor);
		}
		/// <summary>
		/// gets the position of the focus point based
		/// on the owning gradient and the focus value.
		/// this operation requires O(n) runtime.
		/// returns double.NaN if not valid
		/// </summary>
		public override double GetFocusPosition()
		{
			if (Owner == null)
				return double.NaN;
			return GetFocusPosition<AlphaPoint>(Owner.Alphas, this);
		}
		/// <summary>
		/// sets the position of the focus point, if possible.
		/// </summary>
		public override void SetFocusPosition(double value)
		{
			if (Owner != null)
				SetFocusPosition<AlphaPoint>(Owner.Alphas, this, value);
		}
		/// <summary>
		/// gets or sets the alpha
		/// </summary>
		public double Alpha
		{
			get { return _alpha; }
			set
			{
				if (value == _alpha)
					return;
				_alpha = value;
				RaiseChange();
			}
		}
		#region api
		public override bool Equals(object obj)
		{
			return base.Equals(obj) &&
				((AlphaPoint)obj)._alpha == this._alpha;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode() ^
				_alpha.GetHashCode();
		}
		/// <summary>
		/// IComparable
		/// </summary>
		int IComparable<AlphaPoint>.CompareTo(AlphaPoint other)
		{
			return Position.CompareTo(other.Position);
		}
		/// <summary>
		/// ICloneable
		/// </summary>
		public object Clone()
		{
			return new AlphaPoint(Alpha, Focus, Position);
		}
		#endregion
	}
	/// <summary>
	/// encapsulates a color value with focus point
	/// </summary>
	public class ColorPoint : Gradient.Point, IComparable<ColorPoint>, ICloneable
	{
		//variables
		private XYZ _color;
		/// <summary>
		/// ctor
		/// </summary>
		public ColorPoint(Color color, double point)
			: this(XYZ.FromRGB(new RGB(color)), .5, point)
		{ }
		/// <summary>
		/// ctor
		/// </summary>
		public ColorPoint(XYZ color, double focus, double point)
			: base(point, focus)
		{
			_color = color;
		}
		/// <summary>
		/// get color
		/// </summary>
		public override Color GetColor(Color basecolor)
		{
			return System.Drawing.Color.FromArgb(basecolor.A,
				_color.ToRGB().ToArgb());
		}
		/// <summary>
		/// gets the position of the focus point based
		/// on the owning gradient and the focus value.
		/// this operation requires O(n) runtime.
		/// returns double.NaN if not valid
		/// </summary>
		public override double GetFocusPosition()
		{
			if (Owner == null)
				return double.NaN;
			return GetFocusPosition<ColorPoint>(Owner.Colors, this);
		}
		/// <summary>
		/// sets the position of the focus point, if possible.
		/// </summary>
		public override void SetFocusPosition(double value)
		{
			if (Owner != null)
				SetFocusPosition<ColorPoint>(Owner.Colors, this, value);
		}
		/// <summary>
		/// gets or sets the color
		/// </summary>
		public XYZ Color
		{
			get { return _color; }
			set
			{
				if (value == _color)
					return;
				_color = value;
				RaiseChange();
			}
		}
		#region api
		public override bool Equals(object obj)
		{
			return base.Equals(obj) &&
				((ColorPoint)obj)._color == this._color;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode() ^
				_color.GetHashCode();
		}
		/// <summary>
		/// IComparable
		/// </summary>
		int IComparable<ColorPoint>.CompareTo(ColorPoint other)
		{
			return Position.CompareTo(other.Position);
		}
		/// <summary>
		/// ICloneable
		/// </summary>
		public object Clone()
		{
			return new ColorPoint(Color, Focus, Position);
		}
		#endregion
	}
	/// <summary>
	/// events for collection modification
	/// </summary>
	public enum Action
	{
		Added,
		Removed,
		Cleared,
		Modified
	}
	/// <summary>
	/// event handling class for insert, delete and clear
	/// </summary>
	public class ModifiedEventArgs : EventArgs
	{

		private Action _action;
		private Gradient.Point _pt;
		public ModifiedEventArgs(Action action, Gradient.Point pt)
		{
			_action = action;
			_pt = pt;
		}
		public Gradient.Point Point
		{
			get { return _pt; }
		}
		public Action Action
		{
			get { return _action; }
		}
	}

}
