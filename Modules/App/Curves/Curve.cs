using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Vixen.Module.App;
using Vixen.Services;
using ZedGraph;

namespace VixenModules.App.Curves
{
	[DataContract]
	[Serializable]
	[TypeConverter(typeof(CurveTypeConverter))]
	public class Curve
	{
		

		public static Color ActiveCurveGridColor = Color.FromArgb(0,128,255);
		public static Color InactiveCurveGridColor = Color.DarkGray;

		public Curve(IPointList points)
		{
			Points = new PointPairList(points);
			LibraryReferenceName = string.Empty;
		}

		/// <summary>
		/// Deep copy constructor.
		/// </summary>
		/// <param name="curve"></param>
		public Curve(Curve curve)
		{
			Points = new PointPairList(curve.Points);
			LibraryReferenceName = curve.LibraryReferenceName;
			IsCurrentLibraryCurve = curve.IsCurrentLibraryCurve;
		}

		// default Curve constructor makes a ramp with x = y.
		public Curve()
			: this(CurveType.RampUp)
		{
		}

		public Curve(CurveType type)
		{
			switch (type)
			{
				case CurveType.RampUp:
					Points = new PointPairList(new[] { 0.0, 100.0 }, new[] { 0.0, 100.0 });
					break;
				case CurveType.RampDown:
					Points = new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 0.0 });
					break;
				case CurveType.Flat100:
					Points = new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 });
					break;
			}
		}

		/// <summary>
		/// Creates a flat curve of the given intensity
		/// </summary>
		/// <param name="intensity"></param>
		public Curve(double intensity)
		{
			Points = new PointPairList(new[] { 0.0, 100.0 }, new[] { intensity, intensity });	
		}


		private PointPairList _points;

		[DataMember]
		public PointPairList Points
		{
			get
			{
				CheckLibraryReference();
				return _points;
			}
			internal set { _points = value; }
		}

		protected Curve LibraryReferencedCurve { get; set; }

		[NonSerialized]
		private CurveLibrary _library;

		private CurveLibrary Library
		{
			get
			{
				if (_library == null)
					_library = ApplicationServices.Get<IAppModuleInstance>(CurveLibraryDescriptor.ModuleID) as CurveLibrary;

				return _library;
			}
		}


		[DataMember] protected string _libraryReferenceName;

		public string LibraryReferenceName
		{
			get
			{
				if (_libraryReferenceName == null)
					return string.Empty;
				else
					return _libraryReferenceName;
			}
			set { _libraryReferenceName = value; }
		}

		public bool IsLibraryReference
		{
			get { return LibraryReferenceName.Length > 0; }
		}

		/// <summary>
		/// If the curve is a library curve, and is currently valid in the library. When the curve changes,
		/// it should be removed from the library and be replaced with a new (updated) one.
		/// This should only ever be set for curves that are in the library.
		/// </summary>
		[DataMember]
		public bool IsCurrentLibraryCurve { get; set; }

		/// <summary>
		/// Checks that the library reference is still valid and current.
		/// </summary>
		/// <returns>true if the library reference is valid and current (data content hasn't changed), false if it has.</returns>
		public bool CheckLibraryReference()
		{
			// If we have a reference to a curve in the library, try and use that to check if the points are still valid.
			if (LibraryReferencedCurve != null) {
				if (!LibraryReferencedCurve.IsCurrentLibraryCurve) {
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
			LibraryReferencedCurve = null;

			// if we have a name, try and find it in the library. Otherwise, remove the reference.
			if (IsLibraryReference) {
				if (Library != null) {
					if (Library.Contains(LibraryReferenceName)) {
						LibraryReferencedCurve = Library.GetCurve(LibraryReferenceName);
						_points = new PointPairList(LibraryReferencedCurve.Points);
						return true;
					}
					else {
						LibraryReferenceName = string.Empty;
					}
				}
			}

			return false;
		}

		public virtual double GetValue(double x)
		{
			if (x > 100.0) x = 100.0;
			if (x < 0.0) x = 0.0;

			double returnValue = Points.InterpolateX(x);

			if (returnValue > 100.0) returnValue = 100.0;
			if (returnValue < 0.0) returnValue = 0.0;
			if (double.IsNaN(returnValue))
			{
				returnValue = 100;
			}

			return returnValue;
		}

		public double GetValue(int x)
		{
			return GetValue((double) x);
		}

		public int GetIntValue(double x)
		{
			return (int) Math.Round(GetValue(x), 0);
		}

		public int GetIntValue(int x)
		{
			return GetIntValue((double) x);
		}

		public void UnlinkFromLibraryCurve()
		{
			LibraryReferenceName = string.Empty;
			LibraryReferencedCurve = null;
		}

		public Bitmap GenerateGenericCurveImage(Size size, bool inactive=false, bool drawPoints=false, bool editable=false, Color? lineColor = null)
		{
			var adjust = editable ? 2 : 0;
			var penColor = editable ? Color.FromArgb(0, 128, 255) : Color.FromArgb(255, 136, 136, 136);
			if(lineColor.HasValue)
			{
				penColor = lineColor.Value;
			}

			Bitmap result = new Bitmap(size.Width+adjust, size.Height+adjust);

			using (Graphics g = Graphics.FromImage(result))
			{
				using (Brush b = new SolidBrush(inactive?Color.DimGray:Color.Black))
				{
					using (Pen p = new Pen(penColor, 2))
					{
						using (Brush pointBrush = new SolidBrush(Color.FromArgb(255, 136, 136, 136)))
						{
							g.FillRectangle(b, new Rectangle(0, 0, size.Width+adjust, size.Height+adjust));
					
							PointPair lastPoint = null;
							foreach (var point in Points.ToArray()) //get an array so if the points are modified we can still enumerate.
							{
								if (lastPoint == null)
								{
									lastPoint = point;
									continue;
								}

								var tPoint = TransformPoint(point, size);
								var tLastPoint = TransformPoint(lastPoint, size);
								g.DrawLine(p, tLastPoint, tPoint);
								if (drawPoints)
								{
									g.FillEllipse(pointBrush, tPoint.X - 3, tPoint.Y - 3, 6, 6);
									g.FillEllipse(pointBrush, tLastPoint.X - 3, tLastPoint.Y - 3, 6, 6);
								}
								lastPoint = point;
							}
						}
					}
				}
				
			}

			return result;
		}

		public Bitmap GenerateGenericCurveImage(
			Size size, 
			bool inactive,
			bool drawPoints,
			bool editable,
			Color? lineColor,
			List<Color> backgroundColors)
		{
			var adjust = editable ? 2 : 0;
			var penColor = editable ? Color.FromArgb(0, 128, 255) : Color.FromArgb(255, 136, 136, 136);
			if (lineColor.HasValue)
			{
				penColor = lineColor.Value;
			}

			Bitmap result = new Bitmap(size.Width + adjust, size.Height + adjust);

			using (Graphics g = Graphics.FromImage(result))
			{
				using (Brush b = new SolidBrush(inactive ? Color.DimGray : Color.Black))
				{					
					using (Pen p = new Pen(penColor, 2))
					{
						using (Brush pointBrush = new SolidBrush(Color.FromArgb(255, 136, 136, 136)))
						{
							int x = 0;
							int width = (size.Width + adjust) / backgroundColors.Count;

							foreach (Color color in backgroundColors)
							{								
								using (Brush background = new SolidBrush(color))
								{
									g.FillRectangle(background, new Rectangle(x, 0, width, size.Height + adjust));
									x += width;
								}
							}

							PointPair lastPoint = null;
							foreach (var point in Points.ToArray()) //get an array so if the points are modified we can still enumerate.
							{
								if (lastPoint == null)
								{
									lastPoint = point;
									continue;
								}

								var tPoint = TransformPoint(point, size);
								var tLastPoint = TransformPoint(lastPoint, size);
								g.DrawLine(p, tLastPoint, tPoint);
								if (drawPoints)
								{
									g.FillEllipse(pointBrush, tPoint.X - 3, tPoint.Y - 3, 6, 6);
									g.FillEllipse(pointBrush, tLastPoint.X - 3, tLastPoint.Y - 3, 6, 6);
								}
								lastPoint = point;
							}
						}
					}
				}

			}

			return result;
		}


		private Point TransformPoint(PointPair points, Size bounds)
		{
			int X = (int)points.X;
			int Y = Math.Abs((int)points.Y - 100);
			Y = (int)(Y*(bounds.Height/100f));
			X = (int)(X*((bounds.Width-1)/100f));
			return new Point(X,Y);
		}

		public Bitmap GenerateCurveImage(Size size)
		{
			GraphPane pane = new GraphPane(new RectangleF(0, 0, size.Width, size.Height), string.Empty, string.Empty, string.Empty);
			Bitmap result = new Bitmap(size.Width, size.Height);

			pane.AddCurve(string.Empty, Points, ActiveCurveGridColor);
			pane.XAxis.Scale.Min = 0;
			pane.XAxis.Scale.Max = 100;
			pane.YAxis.Scale.Min = 0;
			pane.YAxis.Scale.Max = 100;
			pane.XAxis.IsVisible = false;
			pane.YAxis.IsVisible = false;
			pane.Legend.IsVisible = false;
			pane.Title.IsVisible = false;

			pane.Chart.Fill = new Fill(SystemColors.Control);
			pane.Border = new Border(SystemColors.Control, 0);

			using (Graphics g = Graphics.FromImage(result)) {
				pane.AxisChange(g);
				result = pane.GetImage(true);
			}

			return result;
		}

		public override bool Equals(object obj)
		{
			if (obj is Curve)
			{
				return Equals((Curve) obj);
			}
			return base.Equals(obj);
		}

		protected bool Equals(Curve other)
		{
			return Equals(_points, other._points) && Equals(_library, other._library) && string.Equals(_libraryReferenceName, other._libraryReferenceName) && Equals(LibraryReferencedCurve, other.LibraryReferencedCurve) && IsCurrentLibraryCurve == other.IsCurrentLibraryCurve;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (_points != null ? _points.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (_library != null ? _library.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (_libraryReferenceName != null ? _libraryReferenceName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (LibraryReferencedCurve != null ? LibraryReferencedCurve.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ IsCurrentLibraryCurve.GetHashCode();
				return hashCode;
			}
		}

		#region Overrides of Object

		/// <inheritdoc />
		public override string ToString()
		{
			return $"Library Curve {IsLibraryReference}, Points: {Points}";
		}

		#endregion
	}
}