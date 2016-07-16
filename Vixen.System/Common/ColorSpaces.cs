using System;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Common.Controls.ColorManagement.ColorModels
{
	/// <summary>
	/// CIE XYZ color space
	/// </summary>
	[DataContract, TypeConverter(typeof(XYZTypeConverter))]
	[Serializable]
	public struct XYZ
	{
		public static readonly XYZ Empty = new XYZ();
		public static readonly XYZ White = new XYZ(95.047, 100.000, 108.883);

		#region variables

		[DataMember]
		private double _x, _y, _z;

		#endregion

		#region ctor

		public XYZ(double x, double y, double z)
		{
			_x = ClipValue(x, 0.0, 95.047);
			_y = ClipValue(y, 0.0, 100.000);
			_z = ClipValue(z, 0.0, 108.883);
		}

		public static XYZ FromRGB(RGB value)
		{
			double
				r = GammaCorrection(value.R),
				g = GammaCorrection(value.G),
				b = GammaCorrection(value.B);
			//Observer. = 2°, Illuminant = D65
			return new XYZ(
				r * 41.24 + g * 35.76 + b * 18.05, //multiplicated by 100
				r * 21.26 + g * 71.52 + b * 7.22,
				r * 1.93 + g * 11.92 + b * 95.05);
		}

		#endregion

		#region static functions

		public static double ClipValue(double value, double min, double max)
		{
			if (double.IsNaN(value) ||
				double.IsNegativeInfinity(value) ||
				value < min)
				return min;
			else if (double.IsPositiveInfinity(value) ||
					 value > max)
				return max;
			else return value;
		}

		private static double GammaCorrection(double value)
		{
			if (value > 0.04045)
				return Math.Pow((value + 0.055) / 1.055, 2.4);
			else
				return value / 12.92;
		}

		private static double InvertGammaCorrection(double value)
		{
			if (value > 0.0031308)
				return 1.055 * Math.Pow(value, 1.0 / 2.4) - 0.055;
			else
				return 12.92 * value;
		}

		#endregion

		#region operators

		public static bool operator ==(XYZ a, XYZ b)
		{
			return
				a._x == b._x &&
				a._y == b._y &&
				a._z == b._z;
		}

		public static bool operator !=(XYZ a, XYZ b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj is XYZ)
			{
				return ((XYZ)obj) == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return string.Format("{0}:{1}:{2}", _x, _y, _z).GetHashCode();

		}

		#endregion

		#region conversion

		public RGB ToRGB()
		{
			//Observer. = 2°, Illuminant = D65
			double
				r = InvertGammaCorrection(_x * +0.032406 + _y * -0.015372 + _z * -0.004986),
				g = InvertGammaCorrection(_x * -0.009689 + _y * +0.018758 + _z * +0.000415),
				b = InvertGammaCorrection(_x * +0.000557 + _y * -0.002040 + _z * +0.010570);
			return new RGB(r, g, b);
		}

		public override string ToString()
		{
			return string.Format("CIE-XYZ[\nX={0};\tY={1};\tZ={2}\n]",
								 _x, _y, _z);
		}

		#endregion

		#region properties

		public double X
		{
			get { return _x; }
			set { _x = ClipValue(value, 0.0, 95.047); }
		}

		public double Y
		{
			get { return _y; }
			set { _y = ClipValue(value, 0.0, 100.000); }
		}

		public double Z
		{
			get { return _z; }
			set { _z = ClipValue(value, 0.0, 108.883); }
		}

		#endregion
	}

	[DataContract]
	public struct RGB
	{
		#region variables

		[DataMember]
		private double _r, _g, _b;

		#endregion

		#region ctor

		public RGB(double r, double g, double b)
		{
			_r = XYZ.ClipValue(r, 0.0, 1.0);
			_g = XYZ.ClipValue(g, 0.0, 1.0);
			_b = XYZ.ClipValue(b, 0.0, 1.0);
		}

		public RGB(Color value) :
			this(
			(double)(value.R) / 255.0,
			(double)(value.G) / 255.0,
			(double)(value.B) / 255.0)
		{
		}

		#endregion

		#region operators

		public static bool operator ==(RGB a, RGB b)
		{
			return
				a._r == b._r &&
				a._g == b._g &&
				a._b == b._b;
		}

		public static bool operator !=(RGB a, RGB b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj is RGB)
			{
				return ((RGB)obj) == this;
			}
			return false;
		}

		public override int GetHashCode()
		{

			return string.Format("{0}:{1}:{2}", _r, _g, _b).GetHashCode();

		}

		#endregion

		#region conversion

		public static implicit operator Color(RGB value)
		{
			return value.ToArgb();
		}

		public static implicit operator RGB(Color value)
		{
			return new RGB(value);
		}

		public Color ToArgb()
		{
			return Color.FromArgb(
				(int)Math.Round(255.0 * _r),
				(int)Math.Round(255.0 * _g),
				(int)Math.Round(255.0 * _b));
		}

		#endregion

		#region properties

		public double R
		{
			get { return _r; }
			set { _r = XYZ.ClipValue(value, 0.0, 1.0); }
		}

		public double G
		{
			get { return _g; }
			set { _g = XYZ.ClipValue(value, 0.0, 1.0); }
		}

		public double B
		{
			get { return _b; }
			set { _b = XYZ.ClipValue(value, 0.0, 1.0); }
		}

		#endregion
	}

	[Serializable]
	public struct LAB
	{
		#region variables

		private double _l, _a, _b;

		#endregion

		#region ctor

		public LAB(double l, double a, double b)
		{
			_l = XYZ.ClipValue(l, 0.0, 100.0);
			_a = XYZ.ClipValue(a, -128.0, 128.0);
			_b = XYZ.ClipValue(b, -128.0, 128.0);
		}

		public static LAB FromXYZ(XYZ value)
		{
			//normalize values
			double x = DriveCurve(value.X / XYZ.White.X),
				   y = DriveCurve(value.Y / XYZ.White.Y),
				   z = DriveCurve(value.Z / XYZ.White.Z);
			//return value
			return new LAB(
				(116.0 * y) - 16.0,
				500.0 * (x - y),
				200.0 * (y - z));
		}

		#endregion

		#region static functions

		private static double DriveCurve(double value)
		{
			if (value > 0.008856) return Math.Pow(value, 1.0 / 3.0);
			else return (7.787 * value) + (16.0 / 116.0);
		}

		private static double DriveInverseCurve(double value)
		{
			double cubic = value * value * value;
			if (cubic > 0.008856) return cubic;
			else return (value - 16.0 / 116.0) / 7.787;
		}

		#endregion

		#region operators

		public static bool operator ==(LAB a, LAB b)
		{
			return
				a._l == b._l &&
				a._a == b._a &&
				a._b == b._b;
		}

		public static bool operator !=(LAB a, LAB b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj is LAB)
			{
				return ((LAB)obj) == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return string.Format("{0}:{1}:{2}", _l, _a, _b).GetHashCode();


		}

		#endregion

		#region conversion

		public XYZ ToXYZ()
		{
			double y = (_l + 16.0) / 116.0,
				   x = _a / 500.0 + y,
				   z = y - _b / 200.0;
			return new XYZ(
				DriveInverseCurve(x) * XYZ.White.X,
				DriveInverseCurve(y) * XYZ.White.Y,
				DriveInverseCurve(z) * XYZ.White.Z);
		}

		public override string ToString()
		{
			return string.Format("CIE-Lab[\nL={0};\ta={1};\tb={2}\n]",
								 _l, _a, _b);
		}

		#endregion

		#region properties

		public double L
		{
			get { return _l; }
			set { _l = XYZ.ClipValue(value, 0.0, 100.0); }
		}

		public double a
		{
			get { return _a; }
			set { _a = XYZ.ClipValue(value, -128.0, 127.0); }
		}

		public double b
		{
			get { return _b; }
			set { _b = XYZ.ClipValue(value, -128.0, 127.0); }
		}

		#endregion
	}

	[Serializable]
	public struct HSV
	{
		#region variables

		private double _h, _s, _v;

		#endregion

		#region ctor

		public HSV(double h, double s, double v)
		{
			_h = XYZ.ClipValue(h, 0.0, 1.0);
			_s = XYZ.ClipValue(s, 0.0, 1.0);
			_v = XYZ.ClipValue(v, 0.0, 1.0);
		}

		public static double VFromRgb(RGB col)
		{
			double max = Math.Max(Math.Max(col.R, col.G), col.B);
			return max;
		}

		public static double VFromRgb(Color col)
		{
			return Math.Max(Math.Max(col.R, col.G), col.B)/255d;
		}

		public static HSV FromRGB(RGB col)
		{
			return FromRGB(col.R, col.G, col.B);	
		}

		public static HSV FromRGB(double r, double g, double b)
		{
			double
				min = Math.Min(Math.Min(r, g), b),
				max = Math.Max(Math.Max(r, g), b),
				delta_max = max - min;

			HSV ret = new HSV(0, 0, 0);
			ret._v = max;

			if (delta_max == 0.0)
			{
				ret._h = 0.0;
				ret._s = 0.0;
			}
			else
			{
				ret._s = delta_max / max;

				double del_R = (((max - r) / 6.0) + (delta_max / 2.0)) / delta_max;
				double del_G = (((max - g) / 6.0) + (delta_max / 2.0)) / delta_max;
				double del_B = (((max - b) / 6.0) + (delta_max / 2.0)) / delta_max;

				if (r == max) ret._h = del_B - del_G;
				else if (g == max) ret._h = (1.0 / 3.0) + del_R - del_B;
				else if (b == max) ret._h = (2.0 / 3.0) + del_G - del_R;

				if (ret._h < 0.0) ret._h += 1.0;
				if (ret._h > 1.0) ret._h -= 1.0;
			}
			return ret;
		}
		public static void FromRGB(RGB col, out double hue, out double saturation, out double value)
		{
			double
				min = Math.Min(Math.Min(col.R, col.G), col.B),
				max = Math.Max(Math.Max(col.R, col.G), col.B),
				delta_max = max - min;

			var s = 0.0d;
			var h = 0.0d;

			if (delta_max != 0.0)
			{
				s = delta_max / max;

				double del_R = (((max - col.R) / 6.0) + (delta_max / 2.0)) / delta_max;
				double del_G = (((max - col.G) / 6.0) + (delta_max / 2.0)) / delta_max;
				double del_B = (((max - col.B) / 6.0) + (delta_max / 2.0)) / delta_max;

				if (col.R == max) h = del_B - del_G;
				else if (col.G == max) h = (1.0 / 3.0) + del_R - del_B;
				else if (col.B == max) h = (2.0 / 3.0) + del_G - del_R;

				if (h < 0.0) h += 1.0;
				if (h > 1.0) h -= 1.0;

			}

			hue = h;
			saturation = s;
			value = max;
		}


		#endregion

		#region operators

		public static bool operator ==(HSV a, HSV b)
		{
			return
				a._h == b._h &&
				a._s == b._s &&
				a._v == b._v;
		}

		public static bool operator !=(HSV a, HSV b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj is HSV)
			{
				return ((HSV)obj) == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return string.Format("{0}:{1}:{2}", _h, _s, _v).GetHashCode();

		}

		#endregion

		#region conversion

		public RGB ToRGB()
		{
			return ToRGB(_h, _s, _v);
		}

		public static RGB ToRGB(double hue, double saturation, double value)
		{
			if (saturation == 0.0)
			{
				return new RGB(value, value, value);
			}
			else
			{
				double h = hue * 6.0;
				if (h == 6.0) h = 0.0;
				int h_i = (int)Math.Floor(h);
				double
					var_1 = value * (1.0 - saturation),
					var_2 = value * (1.0 - saturation * (h - h_i)),
					var_3 = value * (1.0 - saturation * (1.0 - (h - h_i)));

				double r, g, b;
				switch (h_i)
				{
					case 0:
						r = value;
						g = var_3;
						b = var_1;
						break;
					case 1:
						r = var_2;
						g = value;
						b = var_1;
						break;
					case 2:
						r = var_1;
						g = value;
						b = var_3;
						break;
					case 3:
						r = var_1;
						g = var_2;
						b = value;
						break;
					case 4:
						r = var_3;
						g = var_1;
						b = value;
						break;
					default:
						r = value;
						g = var_1;
						b = var_2;
						break;
				}
				return new RGB(r, g, b);
			}
		}

		public override string ToString()
		{
			return string.Format("HSV[\nH={0};\tS={1};\tV={2}\n]",
								 _h, _s, _v);
		}

		#endregion

		#region properties

		public double H
		{
			get { return _h; }
			set { _h = XYZ.ClipValue(value, 0.0, 1.0); }
		}

		public double S
		{
			get { return _s; }
			set { _s = XYZ.ClipValue(value, 0.0, 1.0); }
		}

		public double V
		{
			get { return _v; }
			set { _v = XYZ.ClipValue(value, 0.0, 1.0); }
		}

		#endregion
	}

	[Serializable]
	public struct CMYK
	{
		#region variables

		public double _c, _m, _y, _k;

		#endregion

		#region ctor

		public CMYK(double c, double m, double y, double k)
		{
			_c = XYZ.ClipValue(c, 0.0, 1.0);
			_m = XYZ.ClipValue(m, 0.0, 1.0);
			_y = XYZ.ClipValue(y, 0.0, 1.0);
			_k = XYZ.ClipValue(k, 0.0, 1.0);
		}

		public static CMYK FromRGB(RGB value)
		{
			double
				c = 1.0 - value.R,
				m = 1.0 - value.G,
				y = 1.0 - value.B,
				k = 1.0;
			if (c < k) k = c;
			if (m < k) k = m;
			if (y < k) k = y;
			if (k == 1.0) //black
			{
				c = m = y = 0.0;
			}
			else {
				c = (c - k) / (1.0 - k);
				m = (m - k) / (1.0 - k);
				y = (y - k) / (1.0 - k);
			}
			return new CMYK(c, m, y, k);
		}

		#endregion

		#region operators

		public static bool operator ==(CMYK a, CMYK b)
		{
			return
				a._c == b._c &&
				a._k == b._k &&
				a._m == b._m &&
				a._y == b._y;
		}

		public static bool operator !=(CMYK a, CMYK b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj is CMYK)
			{
				return ((CMYK)obj) == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return string.Format("{0}:{1}:{2}:{3}", _c, _m, _y, _k).GetHashCode();

		}

		#endregion

		#region conversion

		public RGB ToRGB()
		{
			double
				c = _c * (1.0 - _k) + _k,
				m = _m * (1.0 - _k) + _k,
				y = _y * (1.0 - _k) + _k;

			return new RGB(1.0 - c, 1.0 - m, 1.0 - y);
		}

		public override string ToString()
		{
			return string.Format("CMYK[\nC={0};\tM={1};\tY={2};\tK={3}\n]",
								 _c, _m, _y, _k);
		}

		#endregion

		#region properties

		public double C
		{
			get { return _c; }
			set { _c = XYZ.ClipValue(value, 0.0, 1.0); }
		}

		public double M
		{
			get { return _m; }
			set { _m = XYZ.ClipValue(value, 0.0, 1.0); }
		}

		public double Y
		{
			get { return _y; }
			set { _y = XYZ.ClipValue(value, 0.0, 1.0); }
		}

		public double K
		{
			get { return _k; }
			set { _k = XYZ.ClipValue(value, 0.0, 1.0); }
		}

		#endregion
	}
}