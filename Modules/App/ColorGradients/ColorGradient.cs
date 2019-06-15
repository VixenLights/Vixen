using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Configuration;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Serialization;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ControlsEx;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Module;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module.App;

namespace VixenModules.App.ColorGradients
{
	/// <summary>
	/// ColorBlend object
	/// </summary>
	[DataContract]
	[Serializable]
	[TypeConverter(typeof(GradientTypeConverter))]
	public class ColorGradient : ICloneable
	{
		/// <summary>
		/// class for holding a gradient point
		/// </summary>
		[DataContract]
		[Serializable]
		public abstract class Point : IComparable<double>
		{
			[DataMember] private double _position;
			[DataMember] private double _focus;

			/// <summary>
			/// ctor
			/// </summary>
			public Point(double position)
				: this(position, 0.5)
			{
			}

			/// <summary>
			/// ctor
			/// </summary>
			public Point(double position, double focus)
			{
				if (!ColorGradient.isValid(position) ||
				    !ColorGradient.isValid(focus))
					throw new ArgumentException("position or focus");
				_position = position;
				_focus = focus;
			}

			/// <summary>
			/// gets the color
			/// </summary>
			public abstract Color GetColor(Color basecolor);

			#region properties

			/// <summary>
			/// gets or sets the position
			/// </summary>
			public double Position
			{
				get { return _position; }
				set
				{
					if (!ColorGradient.isValid(value))
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
					if (!ColorGradient.isValid(value))
						throw new ArgumentException("focus");
					if (value == _focus)
						return;
					_focus = value;
					RaiseChange();
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
				if (Changed != null)
					Changed(this, new ModifiedEventArgs(Action.Modified, this));
			}
			[field: NonSerialized]
			public event EventHandler<ModifiedEventArgs> Changed;
		}


		/// <summary>
		/// class for holding points and updating
		/// controls connected to this colorblend
		/// </summary>
		[CollectionDataContract]
		[Serializable]
		public class PointList<T> : CollectionBase<T> where T : Point, IComparable<T>
		{
			protected bool Equals(PointList<T> obj)
			{
				
				PointList<T> rhs = obj as PointList<T>;
				if (Count != rhs.Count)
					return false;

				for (int i = 0; i < this.Count; i++)
				{
					if (!this[i].Equals(rhs[i]))
						return false;
				}

				return true;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj is PointList<T>)
				{
					return Equals(obj as PointList<T>);
				}
				return base.Equals(obj);

			}

			#region change events

			[field: NonSerialized]
			public event EventHandler<ModifiedEventArgs> Changed;

			public void PointChangedHandler(object sender, ModifiedEventArgs e)
			{
				if (Changed != null)
					Changed(this, e);
			}

			//update owner property
			protected override void OnInsert(int index, T value)
			{
				value.Changed += PointChangedHandler;
			}

			protected override void OnSet(int index, T oldValue, T newValue)
			{
				oldValue.Changed -= PointChangedHandler;
				newValue.Changed += PointChangedHandler;
			}

			protected override void OnRemove(int index, T value)
			{
				value.Changed -= PointChangedHandler;
			}

			protected override void OnClear()
			{
				foreach (Point grd in this)
					grd.Changed -= PointChangedHandler;
			}

			//update edit controls
			protected override void OnValidate(T value)
			{
				if (value == null)
					throw new ArgumentNullException("value");
			}

			protected override void OnClearComplete()
			{
				PointChangedHandler(this, new ModifiedEventArgs(Action.Cleared, null));
			}

			protected override void OnSetComplete(int index, T oldValue, T newValue)
			{
				PointChangedHandler(this, new ModifiedEventArgs(Action.Removed, oldValue));
				PointChangedHandler(this, new ModifiedEventArgs(Action.Added, newValue));
			}

			protected override void OnRemoveComplete(int index, T value)
			{
				PointChangedHandler(this, new ModifiedEventArgs(Action.Removed, value));
			}

			protected override void OnInsertComplete(int index, T value)
			{
				PointChangedHandler(this, new ModifiedEventArgs(Action.Added, value));
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

		[DataMember] private PointList<ColorPoint> _colors;
		[DataMember] private PointList<AlphaPoint> _alphas;
		[DataMember] private bool _gammacorrected = false;
		[DataMember] private String _title = null;

		// doesn't get serialized; it's instantiated as needed.
		[NonSerialized]
		private ColorBlend _blend = null;
		[NonSerialized]
		private Color? _blendFilterColor = null;

		#endregion

		public ColorGradient()
		{
			_colors = new PointList<ColorPoint>();
			_alphas = new PointList<AlphaPoint>();
			SetEventHandlers();

			_alphas.Add(new AlphaPoint(255, 0));
			_alphas.Add(new AlphaPoint(255, 1));
			_colors.Add(new ColorPoint(Color.White, 0));
		}

		/// <summary>
		/// Deep copy constructor.
		/// </summary>
		/// <param name="other"></param>
		public ColorGradient(ColorGradient other)
		{
			CloneFrom(other);
		}

		public ColorGradient(Color staticColor)
			: this()
		{
			_colors.Clear();
			_colors.Add(new ColorPoint(staticColor, 0));
		}

		public ColorGradient(IEnumerable<Color> colors)
			: this()
		{
			_colors.Clear();
			foreach (Color c in colors) {
				_colors.Add(new ColorPoint(c, 0));
			}
		}

		public ColorGradient(IEnumerable<Tuple<Color, float>> colorsAndProportions)
			: this()
		{
			_colors.Clear();
			foreach (Tuple<Color, float> t in colorsAndProportions) {
				Color color = t.Item1;
				float proportion = t.Item2;
				color = Color.FromArgb((int) (color.R*proportion), (int) (color.G*proportion), (int) (color.B*proportion));
				_colors.Add(new ColorPoint(color, 0));
			}
		}

		[OnDeserialized]
		private void OnDeserialization(StreamingContext context)
		{
			SetEventHandlers();
		}

		private void SetEventHandlers()
		{
			_colors.Changed += ChangedPointHandler;
			_alphas.Changed += ChangedPointHandler;
		}

		#region algorithms

		/// <summary>
		/// clamps the value to [0; 1]
		/// </summary>
		public static float Clamp(float value)
		{
			if (value < 0f)
				return 0f;
			if (value > 1f)
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
		private ColorBlend CreateColorBlend(Color? filterColor = null)
		{
			//sort all points
			ColorPoint[] colpoints = Colors.SortedArray();
			AlphaPoint[] alphapoints = Alphas.SortedArray();
			//init out vars
			SortedList<float, Color> positions = new SortedList<float, Color>();

			// if we're filtering the colors, the iterate through the color list, setting all non-matching colors to black
			if (filterColor != null) {
				List<ColorPoint> newColorPoints = new List<ColorPoint>();
				foreach (ColorPoint colorPoint in colpoints) {
					ColorPoint newPoint = (ColorPoint) colorPoint.Clone();
					if (newPoint.Color.ToRGB().ToArgb() != filterColor) {
						// it's not the desired color. Make it black and add it, but only if there's
						// not an entry in the list with this position already
						if (!newColorPoints.Any(x => x.Position == newPoint.Position)) {
							newPoint.Color = XYZ.FromRGB(Color.Black);
							newColorPoints.Add(newPoint);
						}
					}
					else {
						// it's the desired color. Find any others in the list that are in this position
						// and black, remove them, and then replace it with this one
						newColorPoints.RemoveAll(
							x => x.Position == newPoint.Position && x.Color.ToRGB().ToArgb().ToArgb() == Color.Black.ToArgb());
						newColorPoints.Add(newPoint);
					}
				}
				colpoints = newColorPoints.ToArray();
			}
			//add color points
			for (int c = 0; c < colpoints.Length; c++) {
				// focus point; if filtered, non-standard focus points aren't supported.
				if (c > 0 && colpoints[c].Focus != .5 && filterColor == null) {
					AddPosition(colpoints, alphapoints, positions,
					            colpoints[c - 1].Position + (colpoints[c].Position - colpoints[c - 1].Position)*colpoints[c].Focus);
				}
				//color
				AddPosition(colpoints, alphapoints, positions, colpoints[c].Position);
			}

			// We aren't using alpha points, and first/last points get added below

			////add alpha points
			//for (int a = 0; a < alphapoints.Length; a++)
			//{
			//    if (a > 0 && alphapoints[a].Focus != .5)//focus
			//        AddPosition(colpoints, alphapoints, positions,
			//            alphapoints[a - 1].Position + (alphapoints[a].Position - alphapoints[a - 1].Position) * alphapoints[a].Focus);
			//    //alpha
			//    AddPosition(colpoints, alphapoints, positions, alphapoints[a].Position);
			//}

			//add first/last point
			if (positions.Count < 1) {
				positions.Add(0f, Color.Transparent);
			}
			if (!positions.ContainsKey(0f)) {
				if (filterColor != null) {
					float earliest = positions.Keys[0];
					Color c = Color.Black;
					for (int i = 0; i < positions.Count && positions.Keys[i] == earliest; i++) {
						if (positions.Values[i].ToArgb() != Color.Black.ToArgb()) {
							c = positions.Values[i];
							break;
						}
					}
					positions.Add(0f, c);
				}
				else {
					positions.Add(0f, positions.Values[0]);
				}
			}

			if (positions.Count < 2) {
				Color c = positions.Values[0];
				if (filterColor != null && c.ToArgb() != ((Color) filterColor).ToArgb())
					c = Color.Black;
				positions.Add(1f, c);
			}

			if (!positions.ContainsKey(1f)) {
				if (filterColor != null) {
					float latest = positions.Keys[positions.Count - 1];
					Color c = Color.Black;
					for (int i = positions.Count - 1; i >= 0 && positions.Keys[i] == latest; i--) {
						if (positions.Values[i].ToArgb() != Color.Black.ToArgb()) {
							c = positions.Values[i];
							break;
						}
					}
					positions.Add(1f, c);
				}
				else {
					positions.Add(1f, positions.Values[positions.Count - 1]);
				}
			}

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
			for (int i = 0; i < blend.Colors.Length; i++) {
				if (blend.Colors[i].A != 255)
					Alphas.Add(new AlphaPoint((byte) blend.Colors[i].A,
					                          (double) blend.Positions[i]));
				Colors.Add(new ColorPoint(blend.Colors[i],
				                          (double) blend.Positions[i]));
			}
		}

		#region interpolation helpers

		//compute balance out of focus point
		private double FocusToBalance(double a, double b, double focus, double pos)
		{
			if (a >= b || pos <= a) return .0;
			double ret = (pos - a)/(b - a);
			if (focus != .5) {
				//focus influences the breakpoint of
				//linear interpolations
				if (ret == focus)
					return .5;
				else if (ret < focus)
					//ret cannot be 0 when ret<focus
					return .5*ret/focus;
				else
					//ret cannot be 1 when ret>focus
					return .5*(1.0 + (ret - focus)/(1.0 - focus));
			}
			return ret;
		}

		//adds a new position to the list
		private void AddPosition(ColorPoint[] colpoints, AlphaPoint[] alphapoints,
		                         SortedList<float, Color> positions, double pos)
		{
			if (positions.ContainsKey((float) pos)) {
				if (positions[(float) pos].ToArgb() == Color.Black.ToArgb()) {
					positions.Remove((float) pos);
				}
				else {
					return;
				}
			}
			int alpha_a, alpha_b;
			int color_a, color_b;
			//evaluate positions
			SearchPos<AlphaPoint, double>(alphapoints, pos, out alpha_a, out alpha_b);
			SearchPos<ColorPoint, double>(colpoints, pos, out color_a, out color_b);
			//interpolate
			positions.Add((float) pos, Color.FromArgb(
				Interpolate(alphapoints, alpha_a, alpha_b, pos),
				Interpolate(colpoints, color_a, color_b, pos)));
		}

		// interpolates alpha list
		private byte Interpolate(AlphaPoint[] list, int a, int b, double pos)
		{
			if (b < a)
				return 0;
			if (a == b) return (byte) (list[a].Alpha*255.0);
			//compute involving focus position
			return (byte) XYZ.ClipValue(
				(list[a].Alpha + FocusToBalance(list[a].Position, list[b].Position, list[b].Focus, pos)
				 *(list[b].Alpha - list[a].Alpha))*255.0, 0.0, 255.0);
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
			return new RGB(col_a.R + bal*(col_b.R - col_a.R),
			               col_a.G + bal*(col_b.G - col_a.G),
			               col_a.B + bal*(col_b.B - col_a.B)).ToArgb();
		}

		// interpolate on colorblend
		private Color Interpolate(Color[] list, float[] positions, int a, int b, float pos)
		{
			if (b < a)
				return Color.Black;
			else if (a == b) return list[a];
			if (positions[a] >= positions[b]) return list[a];
			float bal = (pos - positions[a])/(positions[b] - positions[a]);
			//
			return Color.FromArgb(
				(int) (list[a].A + bal*(list[b].A - list[a].A)),
				(int) (list[a].R + bal*(list[b].R - list[a].R)),
				(int) (list[a].G + bal*(list[b].G - list[a].G)),
				(int) (list[a].B + bal*(list[b].B - list[a].B)));
		}

		// interpolate a fractional proportion of a given color at the given position
		private float InterpolateProportionOfColor(Color color, Color[] list, float[] positions, int a, int b, float pos)
		{
			if (b < a)
				return 0;

			if (a == b || positions[a] >= positions[b]) {
				return (list[a].ToArgb() == color.ToArgb() ? 1 : 0);
			}

			if (list[a].ToArgb() == color.ToArgb() && list[b].ToArgb() == color.ToArgb()) {
				return 1;
			}

			if (list[a].ToArgb() != color.ToArgb() && list[b].ToArgb() != color.ToArgb()) {
				return 0;
			}

			float bal = (pos - positions[a])/(positions[b] - positions[a]);

			if (list[a].ToArgb() == color.ToArgb()) {
				return (float) (1.0 - bal);
			}

			return (bal);
		}

		//generic interval searching in O(log(n))
		private void SearchPos<T, K>(T[] list, K pos, out int a, out int b) where T : IComparable<K>
		{
			int start = a = 0, end = b = list.Length - 1;
			while (end >= start) {
				int mid = start + (end - start)/2;
				switch (list[mid].CompareTo(pos)) {
					case 0: //found point
						a = b = mid;
						return;
					case 1:
						end = mid - 1;
						b = mid;
						break; //search left
					default:
						start = mid + 1;
						a = mid;
						break; //search right
				}
			}
			//found interval
		}

		#endregion

		#endregion

		public static implicit operator ColorBlend(ColorGradient blend)
		{
			return blend.GetColorBlend();
		}

		public static implicit operator ColorGradient(ColorBlend blend)
		{
			ColorGradient ret = new ColorGradient();
			ret.Colors.Clear();
			ret.AddColors(blend);
			return ret;
		}

		/// <summary>
		/// creates a System.Drawing.Drawing2D.ColorBlend
		/// out of this gradient
		/// </summary>
		public ColorBlend GetColorBlend(Color? filterColor = null)
		{
			CheckLibraryReference();
			if (_blend == null || _blendFilterColor != filterColor) {
				_blend = CreateColorBlend(filterColor);
				_blendFilterColor = filterColor;
			}
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
			SearchPos<float, float>(blend.Positions, (float) pos, out a, out b);
			return Interpolate(blend.Colors, blend.Positions, a, b, pos);
		}

		public List<Color> GetColorsExactlyAt(float pos)
		{
			List<Color> rv = new List<Color>();
			ColorBlend blend = GetColorBlend();
			pos = Clamp(pos);

			for (int i = 0; i < blend.Colors.Length && i < blend.Positions.Length; i++) {
				if (blend.Positions[i] == pos) {
					rv.Add(blend.Colors[i]);
				}
			}

			return rv;
		}

		public HashSet<Color> GetColorsInGradient()
		{
			HashSet<Color> result = new HashSet<Color>();
			foreach (ColorPoint colorPoint in Colors) {
				result.Add(colorPoint.Color.ToRGB());
			}
			return result;
		}


		public List<Tuple<Color, float>> GetDiscreteColorsAndProportionsAt(float pos)
		{
			List<Tuple<Color, float>> rv = new List<Tuple<Color, float>>();
			//Ensure we don't have a leftover bogus blend.
			_blend = null;
			ColorBlend blend = GetColorBlend();
			pos = Clamp(pos);

			// get the indices into the position list for the desired position
			int a, b;
			SearchPos<float, float>(blend.Positions, pos, out a, out b);
			
			HashSet<Color> colors = new HashSet<Color>();

			// if b < a, something went horribly wrong.
			if (b < a) {
				return rv;
			}

			// if they matched, the desired position was exactly on a point. Get all those matching it.
			if (a == b) {
				colors.AddRange(GetColorsExactlyAt(blend.Positions[a]));
			} else {
				// if the indices didn't match, the desired position was between 'a' and 'b'. Get all colors
				// at the position at index 'a', and all at position of index 'b'.
				colors.AddRange(GetColorsExactlyAt(blend.Positions[a]));
				colors.AddRange(GetColorsExactlyAt(blend.Positions[b]));
			}

			// now that we know what colors _might_ be involved, get the proportions for them all.
			foreach (Color color in colors) {
				rv.Add(new Tuple<Color, float>(color, GetProportionOfColorAt(pos, color)));
			}

			return rv;
		}

		public float GetProportionOfColorAt(float pos, Color color)
		{
			ColorBlend blend = GetColorBlend(color);
			pos = Clamp(pos);

			int a, b;
			SearchPos<float, float>(blend.Positions, pos, out a, out b);

			return InterpolateProportionOfColor(color, blend.Colors, blend.Positions, a, b, pos);
		}

		public Color GetColorAt(double pos)
		{
			return GetColorAt((float) pos);
		}

		public double GetProportionOfColorAt(double pos, Color color)
		{
			return GetProportionOfColorAt((float) pos, color);
		}

		public List<Tuple<Color, float>> GetDiscreteColorsAndProportionsAt(double pos)
		{
			return GetDiscreteColorsAndProportionsAt((float) pos);
		}

		public ColorGradient GetReverseColorGradient()
		{
			int colorPosition = 0;
			ColorGradient value = new ColorGradient();
			value.Colors.Clear();

			//We need to first sort the colors based on the position, reason is that when adding a color point it is added to the end of the Color list
			//and we need to have the colors in position order so we can use the appropiate Focus point.
			foreach (var colors in Colors)
			{
				for (int color = 0; color < value.Colors.Count; color++)
				{
					if (colors.Position > value.Colors[color].Position)
					{
						colorPosition = color + 1;
					}
				}
				value.Colors.Insert(colorPosition, colors);
			}

			//Now that its sorted by position we can use the color position and focus point of the next color. 
			ColorGradient newValue = new ColorGradient(value);
			int colorCount = value.Colors.Count - 1;
			for (int i = colorCount; 0 <= i; i--)
			{
				newValue.Colors[colorCount - i] = new ColorPoint(value.Colors[i])
				{
					Position = 1 - value.Colors[i].Position
				};

				if (i < colorCount)
					newValue.Colors[colorCount - i].Focus = 1 - value.Colors[i + 1].Focus;
			}

			return newValue;
		}

		#region properties

		/// <summary>
		/// gets or sets
		/// </summary>
		public bool Gammacorrected
		{
			get
			{
				CheckLibraryReference();
				return _gammacorrected;
			}
			set { _gammacorrected = value; }
		}

		public String Title
		{
			get
			{
				CheckLibraryReference();
				return _title;
			}
			set { _title = value; }
		}

		public PointList<AlphaPoint> Alphas
		{
			get
			{
				CheckLibraryReference();
				return _alphas;
			}
		}

		public PointList<ColorPoint> Colors
		{
			get
			{
				CheckLibraryReference();
				return _colors;
			}
		}

		#endregion

		#region event handling

		private void ChangedPointHandler(object sender, EventArgs e)
		{
			_blend = null;
			if (Changed != null)
				Changed(this, e);
		}

		[field: NonSerialized]
		public event EventHandler Changed;

		#endregion

		#region ICloneable Member

		/// <summary>
		/// returns a copy of all gradient points and parameters
		/// </summary>
		public object Clone()
		{
			ColorGradient ret = new ColorGradient();
			ret.Colors.Clear(); // Defaults to having a color
			ret.Alphas.Clear(); // Defaults to having a color
			if (_title != null)
				ret._title = (string) _title.Clone();
			ret._gammacorrected = _gammacorrected;

			foreach (ColorPoint cp in _colors)
				ret._colors.Add((ColorPoint) cp.Clone());
			foreach (AlphaPoint ap in _alphas)
				ret._alphas.Add((AlphaPoint) ap.Clone());

			// grab all the library-linking details as well
			ret.LibraryReferenceName = LibraryReferenceName;
			ret.IsCurrentLibraryGradient = IsCurrentLibraryGradient;

			return ret;
		}

		/// <summary>
		/// deep copy clone: clones this object from another given one.
		/// </summary>
		/// <param name="other"></param>
		public void CloneFrom(ColorGradient other)
		{
			CloneDataFrom(other);

			// grab all the library-linking details as well
			LibraryReferenceName = other.LibraryReferenceName;
			IsCurrentLibraryGradient = other.IsCurrentLibraryGradient;
		}

		/// <summary>
		/// deep copy clone: clones the data only from another given gradient.
		/// </summary>
		/// <param name="other"></param>
		public void CloneDataFrom(ColorGradient other)
		{
			_colors = new PointList<ColorPoint>();
			foreach (ColorPoint cp in other.Colors) {
				_colors.Add(new ColorPoint(cp));
			}
			_alphas = new PointList<AlphaPoint>();
			foreach (AlphaPoint ap in other.Alphas) {
				_alphas.Add(new AlphaPoint(ap));
			}
			_gammacorrected = other.Gammacorrected;
			if (other.Title != null) _title = other.Title;
			_blend = null;

			SetEventHandlers();
		}

		public ColorGradient GetSubGradient(double start, double end)
		{
			double range = end - start;
			if (range < 0)
			{
				throw new ArgumentException("end must be after start");
			}

			ColorGradient result = new ColorGradient();
			result.Colors.Clear();

			result.Colors.Add(new ColorPoint(GetColorAt(start), 0));

			foreach (ColorPoint cp in Colors)
			{
				if (cp.Position > start && cp.Position < end)
				{
					double scaledPos = (cp.Position - start) / range;
					if (scaledPos > 1.0 || scaledPos < 0.0)
					{
						throw new Exception("Error  calculating position: " + scaledPos + " out of range");
					}

					
					result.Colors.Add(new ColorPoint(cp.Color.ToRGB(), scaledPos));
				}
			}

			//Sample a few more colors out for more accuracy
			for (double d = start+.2d; d < end; d += .2d)
			{
				if (d < end)
				{
					var c = GetColorAt(d);
					double scaledPos = (d - start) / range;
					if (scaledPos > 1.0 || scaledPos < 0.0)
					{
						throw new Exception("Error  calculating position: " + scaledPos + " out of range");
					}
					result.Colors.Add(new ColorPoint(c, scaledPos));
				}
				
			}

			result.Colors.Add(new ColorPoint(GetColorAt(end), 1));

			return result;
		}

		// note: the start and end points returned will _not_ be scaled correctly, as the color gradients have
		// no concept of 'level', only color. Being discrete colors, they need to keep their 'full' intensity.
		public ColorGradient GetSubGradientWithDiscreteColors(double start, double end)
		{
			double range = end - start;
			if (range < 0) {
				throw new ArgumentException("end must be after start");
			}

			ColorGradient result = new ColorGradient();
			result.Colors.Clear();

			List<Tuple<Color, float>> startPoint = GetDiscreteColorsAndProportionsAt(start);
			List<Tuple<Color, float>> endPoint = GetDiscreteColorsAndProportionsAt(end);

			// hmm.. should these colors at the start and end be the discrete colors, exactly?
			// or should they be scaled based on the intensity? This whole thing is pretty dodgy,
			// since it's taking a bunch of assumed knowledge about how it needs to be working
			// elsewhere and applying it here (keeping the color exactly, since discrete filtering
			// matches by color).
			foreach (Tuple<Color, float> tuple in startPoint) {
				result.Colors.Add(new ColorPoint(tuple.Item1, 0.0));
			}

			foreach (ColorPoint cp in Colors) {
				if (cp.Position > start && cp.Position < end) {
					double scaledPos = (cp.Position - start) / range;
					if (scaledPos > 1.0 || scaledPos < 0.0) {
						throw new Exception("Error calculating position: " + scaledPos + " out of range");
					}
					result.Colors.Add(new ColorPoint(cp.Color.ToRGB(), scaledPos));
				}
			}

			foreach (Tuple<Color, float> tuple in endPoint) {
				result.Colors.Add(new ColorPoint(tuple.Item1, 1.0));
			}

			return result;
		}

		#endregion

		/// <summary>
		/// gets the absolute focus point of a point
		/// </summary>
		//protected double GetFocusPosition<T>(PointList<T> coll, T value) where T : Point, IComparable<T>
		public double GetFocusPosition(Point value)
		{
			if (value == null)
				throw new ArgumentException();
			if (value is AlphaPoint)
				return GetFocusPosition<AlphaPoint>(Alphas, value as AlphaPoint);
			else if (value is ColorPoint)
				return GetFocusPosition<ColorPoint>(Colors, value as ColorPoint);
			else
				throw new ArgumentException();
		}

		private double GetFocusPosition<T>(PointList<T> coll, T value) where T : Point, IComparable<T>
		{
			if (coll == null || value == null)
				throw new ArgumentNullException();
			//
			T[] sorted = coll.SortedArray();
			int i = Array.IndexOf<T>(sorted, value);
			if (i < 1) //first point or not found
				return double.NaN;
			return sorted[i - 1].Position + value.Focus*
			       (sorted[i].Position - sorted[i - 1].Position);
		}

		public void SetFocusPosition(Point value, double focuspos)
		{
			if (value == null || focuspos == double.NaN)
				throw new ArgumentException();
			if (value is AlphaPoint)
				SetFocusPosition<AlphaPoint>(Alphas, value as AlphaPoint, focuspos);
			else if (value is ColorPoint)
				SetFocusPosition<ColorPoint>(Colors, value as ColorPoint, focuspos);
			else
				throw new ArgumentException();
		}

		private void SetFocusPosition<T>(PointList<T> coll, T value, double focuspos) where T : Point, IComparable<T>
		{
			if (coll == null || value == null)
				throw new ArgumentNullException("coll or value");
			if (!ColorGradient.isValid(focuspos))
				throw new ArgumentException("focuspos");
			//
			T[] sorted = coll.SortedArray();
			int i = Array.IndexOf<T>(sorted, value);
			if (i > 0) {
				//calculate focus
				double w = sorted[i].Position - sorted[i - 1].Position;
				if (w <= 0)
					value.Focus = .5;
				else
					value.Focus = Math.Max(.0, Math.Min(1.0,
					                                    (focuspos - sorted[i - 1].Position)/w));
			}
		}

		public Bitmap GenerateColorGradientImage(Size size, bool discreteColors, bool drawLibraryLink=false)
		{
			Bitmap result = new Bitmap(size.Width, size.Height);

			if (discreteColors) {
				// count the number of unique colors in this gradient, and figure out how high to make each 'slice'
				Dictionary<int, XYZ> uniqueColors = new Dictionary<int, XYZ>();
				foreach (ColorPoint colorPoint in Colors) {
					int colorHash = colorPoint.Color.ToRGB().ToArgb().ToArgb();
					if (!uniqueColors.ContainsKey(colorHash))
						uniqueColors.Add(colorHash, colorPoint.Color);
				}
				float sliceHeight = size.Height/(float) uniqueColors.Count;

				int i = 0;
				foreach (XYZ color in uniqueColors.Values) {
					float startY = sliceHeight*i;
					float endY = sliceHeight*(i + 1);
					float midY = (startY + endY)/2;
					PointF point1 = new PointF(0, midY);
					PointF point2 = new PointF(size.Width, midY);

					using (LinearGradientBrush lnbrs = new LinearGradientBrush(point1, point2, Color.Transparent, Color.Transparent))
					using (Graphics subg = Graphics.FromImage(result)) {
						ColorBlend cb = GetColorBlend(color.ToRGB());
						lnbrs.InterpolationColors = cb;
						subg.FillRectangle(lnbrs, 0, startY, size.Width, sliceHeight);
					}

					i++;
				}
				
			}
			else {
				System.Drawing.Point point1 = new System.Drawing.Point(0, size.Height/2);
				System.Drawing.Point point2 = new System.Drawing.Point(size.Width, size.Height/2);

				using (LinearGradientBrush lnbrs = new LinearGradientBrush(point1, point2, Color.Transparent, Color.Transparent))
				using (Graphics g = Graphics.FromImage(result)) {
					lnbrs.InterpolationColors = GetColorBlend();
					g.FillRectangle(lnbrs, 0, 0, size.Width, size.Height);
				}
			}

			if (drawLibraryLink)
			{
				if (IsLibraryReference)
				{
					using (Graphics g = Graphics.FromImage(result))
					{
						var link = Resources.LibraryLink;
						link.MakeTransparent();
						var trianglePoints = new List<System.Drawing.Point>();
						trianglePoints.Add(new System.Drawing.Point(0,0));
						trianglePoints.Add(new System.Drawing.Point(0, (int)(size.Height*.75)));
						trianglePoints.Add(new System.Drawing.Point((int)(size.Height*.75), 0));
						g.FillPolygon(Brushes.Black, trianglePoints.ToArray());
						g.DrawImage(link, new Rectangle(0, 0, size.Height/2, size.Height/2));
					}
				}
			}
			
			
			return result;
		}

		public Bitmap GenerateFaderPointsStrip(Size size, bool discreteColors)
		{
			Bitmap result = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppPArgb);
			result.MakeTransparent();

			List<ColorPoint> sortedColors = new List<ColorPoint>(Colors.SortedArray());
			// we'll assume they're sorted, so any colorpoints at the same position are contiguous in the array
			using (Graphics g = Graphics.FromImage(result))
			{
				for (int i = 0; i < sortedColors.Count; i++)
				{
					ColorPoint currentPoint = sortedColors[i];
					double currentPos = currentPoint.Position;
					List<Color> currentColors = new List<Color>();
					currentColors.Add(currentPoint.GetColor(Color.Black));

					while (i + 1 < sortedColors.Count && sortedColors[i + 1].Position == currentPos)
					{
						currentColors.Add(sortedColors[i + 1].GetColor(Color.Black));
						i++;
					}

					DrawFader(g, currentPoint.Position, currentColors, size.Width, size.Height);
				}
			}

			return result;
		}

		/// <summary>
		/// gets the polygon of a fader in the selected rectangle
		/// </summary>
		private System.Drawing.Point[] GetFaderPolygon(Rectangle fader)
		{
			
			return new[]
					   {
							new System.Drawing.Point(fader.Left, fader.Bottom), 
						   new System.Drawing.Point(fader.Right, fader.Bottom), 
							new System.Drawing.Point(fader.Right, fader.Bottom - fader.Height /2), 
						    new System.Drawing.Point(fader.Left + fader.Width/2, fader.Top), //Top point
						    new System.Drawing.Point(fader.Left, fader.Bottom - fader.Height /2)
					   };
		}

		/// <summary>
		/// draws a fader
		/// </summary>
		private void DrawFader(Graphics gr, double pos, IEnumerable<Color> colors, int width, int height)
		{
			int faderHeight = 8;
			int faderWidth = 10;
			Rectangle fader = new Rectangle((int)(pos * (width-faderWidth)), 0, faderWidth, faderHeight);
			System.Drawing.Point[] pts = GetFaderPolygon(fader);
			//RectangleF field = new RectangleF(fader.Left, fader.Bottom -fader.Height/2, size / 2, size);
			//draw fader

			using (Brush faderBrush = new SolidBrush(ThemeColorTable.ForeColor))
			{
				gr.FillPolygon(faderBrush, pts);
			}

			using (Pen p = new Pen(ThemeColorTable.ButtonBorderColor))
			{
				gr.DrawPolygon(p, pts);
			}
			//draw background

			//if (colors.Any(x => x.A != 255))
			//	using (HatchBrush brs = new HatchBrush(HatchStyle.SmallCheckerBoard,
			//										   Color.Gray, Color.White))
			//	{
			//		gr.RenderingOrigin = System.Drawing.Point.Truncate(field.Location);
			//		gr.FillRectangle(brs, field);
			//	}
			////draw color
			//int count = colors.Count();
			//if (count == 1)
			//{
			//	using (SolidBrush brs = new SolidBrush(colors.First()))
			//	{
			//		gr.FillRectangle(brs, field);
			//	}
			//}
			//else
			//{
			//	using (LinearGradientBrush lgb = new LinearGradientBrush(field, Color.Black, Color.White, 0.0))
			//	{
			//		ColorBlend cb = new ColorBlend(count);
			//		double increment = 1.0 / (count - 1);
			//		cb.Positions = new float[count];
			//		for (int i = 0; i < count; i++)
			//		{
			//			cb.Positions[i] = (float)Math.Min((increment * i), 1.0);
			//		}
			//		cb.Colors = colors.ToArray();

			//		lgb.InterpolationColors = cb;
			//		gr.FillRectangle(lgb, field);
			//	}
			//}

			//frame
			//gr.DrawPolygon(Pens.Black, pts);
		}

		#region Equals

		public override bool Equals(object obj)
		{
			if (obj is ColorGradient)
			{
				ColorGradient color = (ColorGradient)obj;
				if (IsLibraryReference && color.IsLibraryReference && LibraryReferenceName.Equals(color.LibraryReferenceName))
				{
					return true;
				}
				
				if (IsLibraryReference || color.IsLibraryReference)
				{
					return false;
				}
				
				if (Colors.Equals(color.Colors) && Alphas.Equals(color.Alphas) )
				{
					return true;
				}
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Colors.GetHashCode() ^ Alphas.GetHashCode();
		}

		#endregion

		#region Library-linking gradients

		[NonSerialized]
		private ColorGradient _libraryReferencedGradient;

		[NonSerialized]
		private ColorGradientLibrary _library;

		private ColorGradientLibrary Library
		{
			get
			{
				if (_library == null)
					_library =
						ApplicationServices.Get<IAppModuleInstance>(ColorGradientLibraryDescriptor.ModuleID) as ColorGradientLibrary;

				return _library;
			}
		}

		[DataMember] private string _libraryReferenceName;

		public string LibraryReferenceName
		{
			get
			{
				if (_libraryReferenceName == null)
					return "";
				else
					return _libraryReferenceName;
			}
			set { _libraryReferenceName = value; }
		}

		public bool IsLibraryReference
		{
			get { return LibraryReferenceName.Length > 0; }
		}

		[DataMember]
		public bool IsCurrentLibraryGradient { get; set; }

		/// <summary>
		/// Checks that the library reference is still valid and current.
		/// </summary>
		/// <returns>true if the library reference is valid and current (data content hasn't changed), false if it has.</returns>
		public bool CheckLibraryReference()
		{
			// If we have a reference to a library item, try and use that to check if it's still valid.
			if (_libraryReferencedGradient != null) {
				if (!_libraryReferencedGradient.IsCurrentLibraryGradient) {
					return !UpdateLibraryReference();
				}
			}
			else {
				return !UpdateLibraryReference();
			}

			return true;
		}

		/// <summary>
		/// Tries to update the library referenced object.
		/// </summary>
		/// <returns>true if successfully updated the library reference, and the data has changed. False if
		/// not (no reference, library doesn't contain the item, etc.)</returns>
		public bool UpdateLibraryReference()
		{
			_libraryReferencedGradient = null;

			// if we have a name, try and find it in the library. Otherwise, remove the reference.
			if (IsLibraryReference) {
				if (Library.Contains(LibraryReferenceName)) {
					_libraryReferencedGradient = Library.GetColorGradient(LibraryReferenceName);
					CloneDataFrom(_libraryReferencedGradient);
					return true;
				}
				else {
					LibraryReferenceName = "";
				}
			}

			return false;
		}

		public void UnlinkFromLibrary()
		{
			LibraryReferenceName = "";
			_libraryReferencedGradient = null;
		}

		#endregion
	}


	/// <summary>
	/// encapsulates an alpha value with focus point
	/// </summary>
	[DataContract]
	[Serializable]
	public class AlphaPoint : ColorGradient.Point, IComparable<AlphaPoint>, ICloneable
	{
		//variables
		[DataMember] private double _alpha;

		/// <summary>
		/// ctor
		/// </summary>
		public AlphaPoint(byte alpha, double point)
			: this((double) alpha/255.0, .5, point)
		{
		}

		/// <summary>
		/// ctor
		/// </summary>
		public AlphaPoint(double alpha, double focus, double point)
			: base(point, focus)
		{
			if (!ColorGradient.isValid(alpha))
				throw new ArgumentException("alpha");
			_alpha = alpha;
		}

		public AlphaPoint(AlphaPoint other)
			: this(other.Alpha, other.Focus, other.Position)
		{
		}

		/// <summary>
		/// get color
		/// </summary>
		public override Color GetColor(Color basecolor)
		{
			return Color.FromArgb((int) (_alpha*255.0),
			                      basecolor);
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
			       ((AlphaPoint) obj)._alpha == this._alpha;
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
	[DataContract]
	[Serializable]
	public class ColorPoint : ColorGradient.Point, IComparable<ColorPoint>, ICloneable
	{
		//variables
		[DataMember] private XYZ _color;

		/// <summary>
		/// ctor
		/// </summary>
		public ColorPoint(Color color, double point)
			: this(XYZ.FromRGB(new RGB(color)), .5, point)
		{
		}

		/// <summary>
		/// ctor
		/// </summary>
		public ColorPoint(XYZ color, double focus, double point)
			: base(point, focus)
		{
			_color = color;
		}

		public ColorPoint(ColorPoint other)
			: this(other.Color, other.Focus, other.Position)
		{
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
			       ((ColorPoint) obj)._color == this._color;
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
	[Serializable]
	public class ModifiedEventArgs : EventArgs
	{
		private Action _action;
		private ColorGradient.Point _pt;

		public ModifiedEventArgs(Action action, ColorGradient.Point pt)
		{
			_action = action;
			_pt = pt;
		}

		public ColorGradient.Point Point
		{
			get { return _pt; }
		}

		public Action Action
		{
			get { return _action; }
		}
	}
}