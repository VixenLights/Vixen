using System;
using System.Drawing;

namespace ControlsEx
{
	/// <summary>
	/// scalefactor is an encapsulation of real fractions
	/// </summary>
	public struct ScaleFactor
	{
		#region static
		public static readonly ScaleFactor[] CommonZooms = new ScaleFactor[]{
			new ScaleFactor(1,10),new ScaleFactor(1,5),new ScaleFactor(1,3),
			new ScaleFactor(1,2),new ScaleFactor(3,4),new ScaleFactor(1,1),
			new ScaleFactor(2,1),new ScaleFactor(3,1),new ScaleFactor(5,1),
			new ScaleFactor(10,1),new ScaleFactor(20,1)
		};
		public static readonly ScaleFactor Identity = new ScaleFactor(1, 1);
		/// <summary>
		/// gets the nearestcommon zoom index
		/// </summary>
		/// <returns></returns>
		public static int GetNearestCommonZoom(ScaleFactor value)
		{
			//search nearest tracker value
			int win = 5; double dist = double.MaxValue;
			for (int i = 0; i < ScaleFactor.CommonZooms.Length; i++)
			{
				//squared distance
				double d = CommonZooms[i] - value;
				d = d * d;
				if (d < dist) { dist = d; win = i; }
			}
			return win;
		}
		#endregion
		#region variables
		private int _numerator, _denominator;
		#endregion
		/// <summary>
		/// constructs a new scalefactor
		/// </summary>
		/// <param name="numerator">must be greater than 0</param>
		/// <param name="denominator">must be greater than 0</param>
		public ScaleFactor(int numerator, int denominator)
		{
			if (numerator <= 0)
				throw new ArgumentException("numerator must be greater than 0");
			if (denominator <= 0)
				throw new ArgumentException("denomitator must be greater than 0");
			this._numerator = numerator;
			this._denominator = denominator;
		}
		/// <summary>
		/// constructs a new scalefactor from a double value
		/// </summary>
		public static ScaleFactor FromDouble(double value)
		{
			if (double.IsInfinity(value) ||
				double.IsNaN(value))
				throw new ArgumentException("value is invalid");
			return Reduce((int)Math.Floor(value * 1000.0), 1000);
		}
		/// <summary>
		/// reduces the fraction to lowest numbers and returns a scalefactor
		/// </summary>
		private static ScaleFactor Reduce(int numerator, int denominator)
		{
			int i = 2;
			while (i <= numerator && i <= denominator)
			{
				if ((numerator % i) == 0 &&
					(denominator % i) == 0)
				{
					numerator /= i;
					denominator /= i;
				}
				else i++;
			}
			return new ScaleFactor(numerator, denominator);
		}
		#region operators
		public static bool operator ==(ScaleFactor a, ScaleFactor b)
		{
			return (a._numerator * b._denominator) == (a._denominator * b._numerator);
		}
		public static bool operator !=(ScaleFactor a, ScaleFactor b)
		{
			return (a._numerator * b._denominator) != (a._denominator * b._numerator);
		}
		public static bool operator <(ScaleFactor a, ScaleFactor b)
		{
			return (a._numerator * b._denominator) < (a._denominator * b._numerator);
		}
		public static bool operator >(ScaleFactor a, ScaleFactor b)
		{
			return (a._numerator * b._denominator) > (a._denominator * b._numerator);
		}
		public static bool operator <=(ScaleFactor a, ScaleFactor b)
		{
			return (a._numerator * b._denominator) <= (a._denominator * b._numerator);
		}
		public static bool operator >=(ScaleFactor a, ScaleFactor b)
		{
			return (a._numerator * b._denominator) >= (a._denominator * b._numerator);
		}
		/// <summary>
		/// evaluates if this instance equals to the given object
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is ScaleFactor)
				return ((ScaleFactor)obj) == this;
			return false;
		}
		/// <summary>
		/// returns the hashcode for this instance
		/// </summary>
		public override int GetHashCode()
		{
			return _numerator ^ _denominator;
		}
		#endregion
		#region public members
		/// <summary>
		/// returns the ratio of scaling
		/// </summary>
		public double ToDouble()
		{
			return ((double)_numerator) / ((double)_denominator);
		}
		public float ToFloat()
		{
			return (float)_numerator / (float)_denominator;
		}
		public static implicit operator double(ScaleFactor value)
		{
			return value.ToDouble();
		}
		public static implicit operator float(ScaleFactor value)
		{
			return value.ToFloat();
		}
		/// <summary>
		/// returns the percent format of this instance
		/// </summary>
		public override string ToString()
		{
			return this.ToDouble().ToString("0%");
		}
		#region scale
		/// <summary>
		/// scales a double scalar
		/// </summary>
		public double Scale(double value)
		{
			return (value * (double)_numerator) / (double)_denominator;
		}
		/// <summary>
		/// scales a float scalar
		/// </summary>
		public float Scale(float value)
		{
			return (value * (float)_numerator) / (float)_denominator;
		}
		/// <summary>
		/// scales an int scalar
		/// </summary>
		public int Scale(int value)
		{
			return (value * _numerator) / _denominator;
		}
		/// <summary>
		/// scales a point
		/// </summary>
		public PointF Scale(PointF value)
		{
			return new PointF(Scale(value.X), Scale(value.Y));
		}
		/// <summary>
		/// scales a point
		/// </summary>
		public Point Scale(Point value)
		{
			return new Point(Scale(value.X), Scale(value.Y));
		}
		/// <summary>
		/// scales a size
		/// </summary>
		public SizeF Scale(SizeF value)
		{
			return new SizeF(Scale(value.Width), Scale(value.Height));
		}
		/// <summary>
		/// scales a size
		/// </summary>
		public Size Scale(Size value)
		{
			return new Size(Scale(value.Width), Scale(value.Height));
		}
		#endregion
		#region unscale
		/// <summary>
		/// unscales a double scalar
		/// </summary>
		public double Unscale(double value)
		{
			return (value * (double)_denominator) / (double)_numerator;
		}
		/// <summary>
		/// unscales a float scalar
		/// </summary>
		public float Unscale(float value)
		{
			return (value * (float)_denominator) / (float)_numerator;
		}
		/// <summary>
		/// unscales an int scalar
		/// </summary>
		public int Unscale(int value)
		{
			return (value * _denominator) / _numerator;
		}
		/// <summary>
		/// unscales a point
		/// </summary>
		public PointF Unscale(PointF value)
		{
			return new PointF(Unscale(value.X), Unscale(value.Y));
		}
		/// <summary>
		/// unscales a point
		/// </summary>
		public Point Unscale(Point value)
		{
			return new Point(Unscale(value.X), Unscale(value.Y));
		}
		/// <summary>
		/// unscales a size
		/// </summary>
		public SizeF Unscale(SizeF value)
		{
			return new SizeF(Unscale(value.Width), Unscale(value.Height));
		}
		/// <summary>
		/// unscales a size
		/// </summary>
		public Size Unscale(Size value)
		{
			return new Size(Unscale(value.Width), Unscale(value.Height));
		}
		#endregion
		#endregion
		/// <summary>
		/// gets the numerator
		/// </summary>
		public int Numerator
		{
			get { return _numerator; }
		}
		/// <summary>
		/// gets the denominator
		/// </summary>
		public int Denominator
		{
			get { return _denominator; }
		}
	}
}
