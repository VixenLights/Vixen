using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Module.App;
using Vixen.Services;
using ZedGraph;

namespace VixenModules.App.Curves
{
	[DataContract]
	[TypeConverter(typeof(CurveTypeConverter))]
	public class Curve
	{
		public static Color ActiveCurveGridColor = Color.RoyalBlue;
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
			: this(new PointPairList(new[] {0.0, 100.0}, new[] {0.0, 100.0}))
		{
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
				Curve curve = (Curve)obj;
				if (IsLibraryReference && curve.IsLibraryReference && LibraryReferenceName.Equals(curve.LibraryReferenceName))
				{
					return true;
				}

				if (Points.Equals(curve.Points))
				{
					return true;
				}
			}
			return base.Equals(obj);
		}
	}
}