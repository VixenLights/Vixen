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
			//Observer. = 2�, Illuminant = D65
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
			//Observer. = 2�, Illuminant = D65
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

	public struct RGBA
	{
		private double _r, _g, _b, _a;

		#region ctor

		public RGBA(double r, double g, double b, double a)
		{
			_r = r;
			_g = g;
			_b = b;
			_a = a;
		}

		public RGBA(Color value) :
			this(
			(double)(value.R) / 255.0,
			(double)(value.G) / 255.0,
			(double)(value.B) / 255.0,
			(double)(value.A) / 255.0)
		{
		}

		#endregion

		#region operators

		public static bool operator ==(RGBA a, RGBA b)
		{
			return
				a._r == b._r &&
				a._g == b._g &&
				a._b == b._b &&
				a._a == b._a;
		}

		public static bool operator !=(RGBA a, RGBA b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj is RGBA)
			{
				return ((RGBA)obj) == this;
			}
			return false;
		}

		public override int GetHashCode()
		{

			return string.Format("{0}:{1}:{2}:{3}", _r, _g, _b, _a).GetHashCode();

		}

		#endregion

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

		public double A
		{
			get { return _a; }
			set { _a = XYZ.ClipValue(value, 0.0, 1.0); }
		}
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

		public static double VFromRgb(double r, double b, double g)
		{
			double max = Math.Max(Math.Max(r, g), b)/255d;
			return max;
		}

		public static double VFromRgb(Color col)
		{
			return Math.Max(Math.Max(col.R, col.G), col.B)/255d;
		}

		public static HSV FromRGB(Color c)
		{
			return FromRGB(c.R, c.G, c.B);
		}

		/// <summary>
		/// Convert from RGB values to HSV
		/// </summary>
		/// <param name="r">255 based red</param>
		/// <param name="g">255 based green</param>
		/// <param name="b">255 based blue</param>
		/// <returns></returns>
		public static HSV FromRGB(double r, double g, double b)
		{
			double k = 0.0;
			if (g < b)
			{
				//Swap g and b
				(g, b) = (b, g);
				k = -1.0;
			}
			double minGb = b;
			if (r < g)
			{
				//Swap r and g
				(r, g) = (g, r);
				k = -.3333333333333 - k;
				minGb = Math.Min(g, b);
			}
			double chroma = r - minGb;

			HSV hsv = new HSV {
				_v = r/255.0, 
				_h = Math.Abs(k + (g - b) / (6.0 * chroma + 1e-20)), 
				_s = chroma / (r + 1e-20)

			};

			return hsv;
		}

		public Color ToRGB()
		{
			return ToRGB(_h, _s, _v);
		}

		public static Color ToRGB(double hue, double saturation, double value)
		{
			if (saturation == 0.0)
			{
				return Color.FromArgb((int)(value*255), (int)(value*255), (int)(value*255));
			}
			
			double h = hue * 6.0;
			if (h == 6.0) h = 0.0;
			int i = (int)Math.Floor(h);
			double var1 = value * (1.0 - saturation);

			double r, g, b;
			switch (i)
			{
				case 0:
					r = value;
					g = value * (1.0 - saturation * (1.0 - (h - i)));
					b = var1;
					break;
				case 1:
					r = value * (1.0 - saturation * (h - i));
					g = value;
					b = var1;
					break;
				case 2:
					r = var1;
					g = value;
					b = value * (1.0 - saturation * (1.0 - (h - i)));
					break;
				case 3:
					r = var1;
					g = value * (1.0 - saturation * (h - i));
					b = value;
					break;
				case 4:
					r = value * (1.0 - saturation * (1.0 - (h - i)));
					g = var1;
					b = value;
					break;
				default:
					r = value;
					g = var1;
					b = value * (1.0 - saturation * (h - i));
					break;
			}

			return Color.FromArgb((int)(r*255.0f), (int)(g*255.0f), (int)(b*255.0f));
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
			set { _h = Clamp(value, 0.0, 1.0); }
		}

		public double S
		{
			get { return _s; }
			set { _s = Clamp(value, 0.0, 1.0); }
		}

		public double V
		{
			get { return _v; }
			set { _v = Clamp(value, 0.0, 1.0); }
		}

		public static double Clamp(double value, double min, double max)
		{
			return (value < min) ? min : (value > max) ? max : value;
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

	/// <summary>
	/// Maintains a color in the HSI color space.
	/// </summary>
	[Serializable]
	public struct HSI
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="h">Hue in radians</param>
		/// <param name="s">Saturation (0-1.0)</param>
		/// <param name="i">Intensity (0-1.0)</param>
		public HSI(double h, double s, double i)
		{
			_h = h;
			_s = s;
			_i = i;
		}

		#endregion

		#region Private Fields

		/// <summary>
		/// Hue in radians.
		/// </summary>
		private double _h;

		/// <summary>
		/// Saturation (0-1.0)
		/// </summary>
		private double _s;

		/// <summary>
		/// Intensity (0-1.0)
		/// </summary>
		private double _i;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the Hue in radians.
		/// </summary>
		public double H
		{
			get { return _h; }
			set { _h = Clamp(value, 0.0, 2 * Math.PI); }
		}

		/// <summary>
		/// Gets or sets the Saturation.  Allowable range is 0-1.0.
		/// </summary>
		public double S
		{
			get { return _s; }
			set { _s = Clamp(value, 0.0, 1.0); }
		}

		/// <summary>
		/// Gets or sets the intensity.  Allowable range is 0-1.0.
		/// </summary>
		public double I
		{
			get { return _i; }
			set { _i = Clamp(value, 0.0, 1.0); }
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Converts the color to RGBW format.
		/// </summary>
		/// <returns>RGBW structure representing the color</returns>
		public RGBW ToRGBW()
		{
			// Convert the color to RGBW format
			double red;
			double green;
			double blue;
			double white;
			HSI_To_RGBW(H, S, I, out red, out green, out blue, out white);

			// Two algorithms are included in this namespace for converting from RGB to RGBW.
			// To get the Saiko alorithm to match the stack exchange algorithm I needed to
			// round the r,g,b,w values to the nearest integer.
			int r1 = (int)Math.Round(red);
			int g1 = (int)Math.Round(green);
			int b1 = (int)Math.Round(blue);
			int w1 = (int)Math.Round(white);

			// Return RGBW struct converting from 0-255 to 0-1
			return new RGBW(r1 / 255.0, g1 / 255.0, b1 / 255.0, w1 / 255.0);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Constrains the value to the specified range.
		/// </summary>
		/// <param name="value">Value to constrain</param>
		/// <param name="min">Minimum of the allowable range</param>
		/// <param name="max">Maximum of the allowable range</param>
		/// <returns>Constrained value</returns>
		private double Clamp(double value, double min, double max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}

		/// <summary>
		/// Converts the HSI color to RGBW format.
		/// </summary>
		/// <param name="H">Hue in radians</param>
		/// <param name="S">Saturation (0-1.0)</param>
		/// <param name="I">Intensity (0-1.0)</param>
		/// <param name="red">Red component (0-255)</param>
		/// <param name="green">Green component (0-255)</param>
		/// <param name="blue">Blue component (0-255)</param>
		/// <param name="white">White component (0-255)</param>
		/// <remarks>
		/// This method was dervied from:
		/// https://blog.saikoled.com/post/44677718712/how-to-convert-from-hsi-to-rgb-white
		///
		/// The algorithm was further adjust based on this Python library.
		/// https://github.com/iamh2o/rgbw_colorspace_converter/
		/// </remarks>
		private void HSI_To_RGBW(double H, double S, double I, out double red, out double green, out double blue, out double white)
		{
			double r, g, b, w;
			double cos_h;
			double cos_1047_h;

			H = H % (2 * Math.PI); // cycle H around to 0-360 degrees
			
			S = S > 0 ? (S < 1 ? S : 1) : 0; // clamp S and I to interval [0,1]
			I = I > 0 ? (I < 1 ? I : 1) : 0;

			if (H < 2.09439)
			{
				cos_h = Math.Cos(H);
				cos_1047_h = Math.Cos(1.047196667 - H);
				r = (S * 255 * I / 3 * (1 + cos_h / cos_1047_h));
				g = (S * 255 * I / 3 * (1 + (1 - cos_h / cos_1047_h)));
				b = 0;
				w = (255 * (1 - S) * I);
			}
			else if (H < 4.188787)
			{
				H = H - 2.09439;
				cos_h = Math.Cos(H);
				cos_1047_h = Math.Cos(1.047196667 - H);
				g = (S * 255 * I / 3 * (1 + cos_h / cos_1047_h));
				b = (S * 255 * I / 3 * (1 + (1 - cos_h / cos_1047_h)));
				r = 0;
				w = (255 * (1 - S) * I);
			}
			else
			{
				H = H - 4.188787;
				cos_h = Math.Cos(H);
				cos_1047_h = Math.Cos(1.047196667 - H);
				b = (S * 255 * I / 3 * (1 + cos_h / cos_1047_h));
				r = (S * 255 * I / 3 * (1 + (1 - cos_h / cos_1047_h)));
				g = 0;
				w = (255 * (1 - S) * I);
			}

			// These checks were added because during testing I saw large negative numbers
			// instead of zero.
			if (Double.IsNaN(r))
			{
				r = 0.0;
			}

			if (Double.IsNaN(g))
			{
				g = 0.0;
			}

			if (Double.IsNaN(b))
			{
				b = 0.0;
			}

			if (Double.IsNaN(w))
			{
				w = 0.0;
			}

			red = Clamp(r * 3, 0, 255);
			green = Clamp(g * 3, 0, 255);
			blue = Clamp(b * 3, 0, 255);
			white = Clamp(w, 0, 255);
		}

		#endregion
	}

	/// <summary>
	/// Maintains a color in RGBW format.
	/// </summary>
	[Serializable]
	public struct RGBW
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="r">Red component (0-1.0)</param>
		/// <param name="g">Green component (0-1.0)</param>
		/// <param name="b">Blue component (0-1.0)</param>
		/// <param name="w">White component (0-1.0)</param>
		public RGBW(double r, double g, double b, double w)
		{
			_r = r;
			_g = g;
			_b = b;
			_w = w;
		}

		#endregion

		#region Private Fields

		/// <summary>
		/// Red component.
		/// </summary>
		private double _r;

		/// <summary>
		/// Green component.
		/// </summary>
		private double _g;

		/// <summary>
		/// Blue component.
		/// </summary>
		private double _b;

		/// <summary>
		/// White component.
		/// </summary>
		private double _w;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the red component of the color.
		/// </summary>
		public double R
		{
			get { return _r; }
			set { _r = Clamp(value, 0.0, 1.0); }
		}

		/// <summary>
		/// Gets or sets the green component of the color.
		/// </summary>
		public double G
		{
			get { return _g; }
			set { _g = Clamp(value, 0.0, 1.0); }
		}

		/// <summary>
		/// Gets or sets the blue component of the color.
		/// </summary>
		public double B
		{
			get { return _b; }
			set { _b = Clamp(value, 0.0, 1.0); }
		}

		/// <summary>
		/// Gets or sets the white component of the color.
		/// </summary>
		public double W
		{
			get { return _w; }
			set { _w = Clamp(value, 0.0, 1.0); }
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Constrains the value to the specified range.
		/// </summary>
		/// <param name="value">Value to constrain</param>
		/// <param name="min">Minimum of the allowable range</param>
		/// <param name="max">Maximum of the allowable range</param>
		/// <returns>Constrained value</returns>
		private double Clamp(double value, double min, double max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}

		#endregion
	}

	/// <summary>
	/// Extension methods for the Microsoft Color class.
	/// </summary>
	public static class ColorExtensions
	{
		/// <summary>
		/// Converts the color to RGBW format.
		/// </summary>
		/// <param name="color">Color to convert</param>
		/// <returns>RGBW struct representing the color</returns>
		/// <remarks>This method uses the Saiko algorithm</remarks>
		public static RGBW ToRGBW(this Color color)
		{
			// First step convert the color to HSI color space
			HSI hsi = color.ToHSI();

			// Second step convert to RGBW
			return hsi.ToRGBW();
		}

		/// <summary>
		/// Converts the color to RGBW format.
		/// </summary>
		/// <param name="color">Color to convert</param>
		/// <returns>RGBW struct representing the color</returns>
		/// <remarks>This method uses the stack exchange algorithm</remarks>
		public static RGBW ToRGBWFast(this Color color)
		{
			int r;
			int g;
			int b;
			int w;
			
			// Convert the color to RGBW
			RGBToRGBW(color, out r, out g, out b, out w);

			// Create an RGBW struct normalizing the component values to 0-1.0 range
			RGBW rgbw = new RGBW(r / 255.0, g / 255.0, b / 255.0, w / 255.0);
			
			return rgbw;
		}

		/// <summary>
		/// Converts a Color struct to an HSI struct.
		/// </summary>
		/// <param name="color">Color to convert</param>
		/// <returns>HSI struct representing the color</returns>
		public static HSI ToHSI(this Color color)
		{
			// Convert the RBG to HSI
			double h;
			double s;
			double i;
			RGBToHSI(color.R / 255.0, color.G / 255.0, color.B / 255.0, out h, out s, out i);

			// Create the HSI struct
			HSI hsi = new HSI(h, s, i);

			// Return the HSI struct
			return hsi;
		}

		/// <summary>
		/// Converts RGB values to HSI values.
		/// </summary>
		/// <param name="r">Red component (0-255)</param>
		/// <param name="g">Green component (0-255)</param>
		/// <param name="b">Blue component (0-255)</param>
		/// <param name="h">Hue Output In Radians</param>
		/// <param name="s">Saturation Output (0-1)</param>
		/// <param name="i">Intensity Output (0-1)</param>
		/// <remarks>
		/// This function was derived from:
		/// https://en.wikipedia.org/wiki/HSL_and_HSV
		/// </remarks>
		private static void RGBToHSI(double r, double g, double b, out double h, out double s, out double i)
		{
			i = (r + g + b) / 3.0;

			double rn = r / (r + g + b);
			double gn = g / (r + g + b);
			double bn = b / (r + g + b);

			h = Math.Acos((0.5 * ((rn - gn) + (rn - bn))) / (Math.Sqrt((rn - gn) * (rn - gn) + (rn - bn) * (gn - bn))));
			if (b > g)
			{
				h = 2 * Math.PI - h;
			}

			s = 1 - 3 * Math.Min(rn, Math.Min(gn, bn));
		}

		/// <summary>
		/// Converts an RGB value to RGBW value.
		/// </summary>
		/// <param name="color">Color to convert</param>
		/// <param name="r">Red component (0-255)</param>
		/// <param name="g">Green component (0-255)</param>
		/// <param name="b">Blue component (0-255)</param>
		/// <param name="w">White component (0-255)</param>
		/// <remarks>
		/// This method was found at the following article.
		/// https://stackoverflow.com/questions/40312216/converting-rgb-to-rgbw
		/// </remarks>
		static void RGBToRGBW(Color color, out int r, out int g, out int b, out int w)
		{
			float Ri = color.R;
			float Gi = color.G;
			float Bi = color.B;

			float tM = Math.Max(Ri, Math.Max(Gi, Bi));

			//If the maximum value is 0, immediately return pure black.
			if (tM == 0)
			{
				r = 0;
				g = 0;
				b = 0;
				w = 0;
				return;
			}

			//This section serves to figure out what the color with 100% hue is
			float multiplier = 255.0f / tM;
			float hR = Ri * multiplier;
			float hG = Gi * multiplier;
			float hB = Bi * multiplier;

			//This calculates the Whiteness (not strictly speaking Luminance) of the color
			float M = Math.Max(hR, Math.Max(hG, hB));
			float m = Math.Min(hR, Math.Min(hG, hB));
			float Luminance = ((M + m) / 2.0f - 127.5f) * (255.0f / 127.5f) / multiplier;

			//Calculate the output values
			int Wo = Convert.ToInt32(Luminance);
			int Bo = Convert.ToInt32(Bi - Luminance);
			int Ro = Convert.ToInt32(Ri - Luminance);
			int Go = Convert.ToInt32(Gi - Luminance);

			//Trim them so that they are all between 0 and 255
			if (Wo < 0) Wo = 0;
			if (Bo < 0) Bo = 0;
			if (Ro < 0) Ro = 0;
			if (Go < 0) Go = 0;
			if (Wo > 255) Wo = 255;
			if (Bo > 255) Bo = 255;
			if (Ro > 255) Ro = 255;
			if (Go > 255) Go = 255;

			r = Ro;
			g = Go;
			b = Bo;
			w = Wo;
		}
	}
}