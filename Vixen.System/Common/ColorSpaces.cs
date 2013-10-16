using System;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Common.Controls.ColorManagement.ColorModels
{
	/// <summary>
	/// CIE XYZ color space
	/// </summary>
	[DataContract, TypeConverter(typeof (XYZTypeConverter))]
	public struct XYZ
	{
		public static readonly XYZ Empty = new XYZ();
		public static readonly XYZ White = new XYZ(95.047f, 100.000f, 108.883f);

		#region variables

	[DataMember] private float _x, _y, _z;

		#endregion

		#region ctor

		public XYZ(float x, float y, float z)
		{
			_x = ClipValue(x, 0.0f, 95.047f);
			_y = ClipValue(y, 0.0f, 100.000f);
			_z = ClipValue(z, 0.0f, 108.883f);
		}

		public static XYZ FromRGB(RGB value)
		{
			float
				r = GammaCorrection(value.R),
				g = GammaCorrection(value.G),
				b = GammaCorrection(value.B);
			//Observer. = 2°, Illuminant = D65
			return new XYZ(
				r*41.24f + g*35.76f + b*18.05f, //multiplicated by 100
				r*21.26f + g*71.52f + b*7.22f,
				r*1.93f + g*11.92f + b*95.05f);
		}

		#endregion

		#region static functions

		public static float ClipValue(float value, float min, float max)
		{
			if (float.IsNaN(value) ||
			    float.IsNegativeInfinity(value) ||
			    value < min)
				return min;
			else if (float.IsPositiveInfinity(value) ||
			         value > max)
				return max;
			else return value;
		}

		private static float GammaCorrection(float value)
		{
			if (value > 0.04045f)
				return (float)Math.Pow((value + 0.055f)/1.055f, 2.4f);
			else
				return value/12.92f;
		}

		private static float InvertGammaCorrection(float value)
		{
			if (value > 0.0031308)
				return 1.055f*(float)Math.Pow(value, 1.0f/2.4f) - 0.055f;
			else
				return 12.92f*value;
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
			if (obj is XYZ) {
				return ((XYZ) obj) == this;
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
			float
				r = InvertGammaCorrection(_x*+0.032406f + _y*-0.015372f + _z*-0.004986f),
				g = InvertGammaCorrection(_x*-0.009689f + _y*+0.018758f + _z*+0.000415f),
				b = InvertGammaCorrection(_x*+0.000557f + _y*-0.002040f + _z*+0.010570f);
			return new RGB(r, g, b);
		}

		public override string ToString()
		{
			return string.Format("CIE-XYZ[\nX={0};\tY={1};\tZ={2}\n]",
			                     _x, _y, _z);
		}

		#endregion

		#region properties

		public float X
		{
			get { return _x; }
			set { _x = ClipValue(value, 0.0f, 95.047f); }
		}

		public float Y
		{
			get { return _y; }
			set { _y = ClipValue(value, 0.0f, 100.000f); }
		}

		public float Z
		{
			get { return _z; }
			set { _z = ClipValue(value, 0.0f, 108.883f); }
		}

		#endregion
	}

	[DataContract]
	public struct RGB
	{
		#region variables

		[DataMember] private float _r, _g, _b;

		#endregion

		#region ctor

		public RGB(float r, float g, float b)
		{
			_r = XYZ.ClipValue(r, 0.0f, 1.0f);
			_g = XYZ.ClipValue(g, 0.0f, 1.0f);
			_b = XYZ.ClipValue(b, 0.0f, 1.0f);
		}

		public RGB(Color value) :
			this(
			(float) (value.R)/255.0f,
			(float) (value.G)/255.0f,
			(float) (value.B)/255.0f)
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
			if (obj is RGB) {
				return ((RGB) obj) == this;
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
				(int) Math.Round(255.0*_r),
				(int) Math.Round(255.0*_g),
				(int) Math.Round(255.0*_b));
		}

		#endregion

		#region properties

		public float R
		{
			get { return _r; }
			set { _r = XYZ.ClipValue(value, 0.0f, 1.0f); }
		}

		public float G
		{
			get { return _g; }
			set { _g = XYZ.ClipValue(value, 0.0f, 1.0f); }
		}

		public float B
		{
			get { return _b; }
			set { _b = XYZ.ClipValue(value, 0.0f, 1.0f); }
		}

		#endregion
	}

	[Serializable]
	public struct LAB
	{
		#region variables

		private float _l, _a, _b;

		#endregion

		#region ctor

		public LAB(float l, float a, float b)
		{
			_l = XYZ.ClipValue(l, 0.0f, 100.0f);
			_a = XYZ.ClipValue(a, -128.0f, 128.0f);
			_b = XYZ.ClipValue(b, -128.0f, 128.0f);
		}

		public static LAB FromXYZ(XYZ value)
		{
			//normalize values
			float x = DriveCurve(value.X/XYZ.White.X),
			       y = DriveCurve(value.Y/XYZ.White.Y),
			       z = DriveCurve(value.Z/XYZ.White.Z);
			//return value
			return new LAB(
				(116.0f*y) - 16.0f,
				500.0f*(x - y),
				200.0f*(y - z));
		}

		#endregion

		#region static functions

		private static float DriveCurve(float value)
		{
			if (value > 0.008856f) return (float)Math.Pow(value, 1.0f/3.0f);
			else return (7.787f*value) + (16.0f/116.0f);
		}

		private static float DriveInverseCurve(float value)
		{
			float cubic = value*value*value;
			if (cubic > 0.008856f) return cubic;
			else return (value - 16.0f/116.0f)/7.787f;
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
			if (obj is LAB) {
				return ((LAB) obj) == this;
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
			float y = (_l + 16.0f)/116.0f,
			       x = _a/500.0f + y,
			       z = y - _b/200.0f;
			return new XYZ(
				DriveInverseCurve(x)*XYZ.White.X,
				DriveInverseCurve(y)*XYZ.White.Y,
				DriveInverseCurve(z)*XYZ.White.Z);
		}

		public override string ToString()
		{
			return string.Format("CIE-Lab[\nL={0};\ta={1};\tb={2}\n]",
			                     _l, _a, _b);
		}

		#endregion

		#region properties

		public float L
		{
			get { return _l; }
			set { _l = XYZ.ClipValue(value, 0.0f, 100.0f); }
		}

		public float a
		{
			get { return _a; }
			set { _a = XYZ.ClipValue(value, -128.0f, 127.0f); }
		}

		public float b
		{
			get { return _b; }
			set { _b = XYZ.ClipValue(value, -128.0f, 127.0f); }
		}

		#endregion
	}

	[Serializable]
	public struct HSV
	{
		#region variables

		private float _h, _s, _v;

		#endregion

		#region ctor

		public HSV(float h, float s, float v)
		{
			_h = XYZ.ClipValue(h, 0.0f, 1.0f);
			_s = XYZ.ClipValue(s, 0.0f, 1.0f);
			_v = XYZ.ClipValue(v, 0.0f, 1.0f);
		}

		public static HSV FromRGB(RGB col)
		{
			float
				min = Math.Min(Math.Min(col.R, col.G), col.B),
				max = Math.Max(Math.Max(col.R, col.G), col.B),
				delta_max = max - min;

			HSV ret = new HSV(0, 0, 0);
			ret._v = max;

			if (delta_max == 0.0) {
				ret._h = 0.0f;
				ret._s = 0.0f;
			}
			else {
				ret._s = delta_max/max;

				float del_R = (((max - col.R)/6.0f) + (delta_max/2.0f))/delta_max;
				float del_G = (((max - col.G)/6.0f) + (delta_max/2.0f))/delta_max;
				float del_B = (((max - col.B)/6.0f) + (delta_max/2.0f))/delta_max;

				if (col.R == max) ret._h = del_B - del_G;
				else if (col.G == max) ret._h = (1.0f/3.0f) + del_R - del_B;
				else if (col.B == max) ret._h = (2.0f/3.0f) + del_G - del_R;

				if (ret._h < 0.0) ret._h += 1.0f;
				if (ret._h > 1.0) ret._h -= 1.0f;
			}
			return ret;
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
			if (obj is HSV) {
				return ((HSV) obj) == this;
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
			if (_s == 0.0f) {
				return new RGB(_v, _v, _v);
			}
			else {
				float h = _h*6.0f;
				if (h == 6.0) h = 0.0f;
				int h_i = (int) Math.Floor(h);
				float
					var_1 = _v*(1.0f - _s),
					var_2 = _v*(1.0f - _s*(h - h_i)),
					var_3 = _v*(1.0f - _s*(1.0f - (h - h_i)));

				float r, g, b;
				switch (h_i) {
					case 0:
						r = _v;
						g = var_3;
						b = var_1;
						break;
					case 1:
						r = var_2;
						g = _v;
						b = var_1;
						break;
					case 2:
						r = var_1;
						g = _v;
						b = var_3;
						break;
					case 3:
						r = var_1;
						g = var_2;
						b = _v;
						break;
					case 4:
						r = var_3;
						g = var_1;
						b = _v;
						break;
					default:
						r = _v;
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

		public float H
		{
			get { return _h; }
			set { _h = XYZ.ClipValue(value, 0.0f, 1.0f); }
		}

		public float S
		{
			get { return _s; }
			set { _s = XYZ.ClipValue(value, 0.0f, 1.0f); }
		}

		public float V
		{
			get { return _v; }
			set { _v = XYZ.ClipValue(value, 0.0f, 1.0f); }
		}

		#endregion
	}

	[Serializable]
	public struct CMYK
	{
		#region variables

		public float _c, _m, _y, _k;

		#endregion

		#region ctor

		public CMYK(float c, float m, float y, float k)
		{
			_c = XYZ.ClipValue(c, 0.0f, 1.0f);
			_m = XYZ.ClipValue(m, 0.0f, 1.0f);
			_y = XYZ.ClipValue(y, 0.0f, 1.0f);
			_k = XYZ.ClipValue(k, 0.0f, 1.0f);
		}

		public static CMYK FromRGB(RGB value)
		{
			float
				c = 1.0f - value.R,
				m = 1.0f - value.G,
				y = 1.0f - value.B,
				k = 1.0f;
			if (c < k) k = c;
			if (m < k) k = m;
			if (y < k) k = y;
			if (k == 1.0) //black
			{
				c = m = y = 0.0f;
			}
			else {
				c = (c - k)/(1.0f - k);
				m = (m - k)/(1.0f - k);
				y = (y - k)/(1.0f - k);
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
			if (obj is CMYK) {
				return ((CMYK) obj) == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return string.Format("{0}:{1}:{2}:{3}", _c,_m,_y,_k).GetHashCode();
	
		}

		#endregion

		#region conversion

		public RGB ToRGB()
		{
			float
				c = _c*(1.0f - _k) + _k,
				m = _m*(1.0f - _k) + _k,
				y = _y*(1.0f - _k) + _k;

			return new RGB(1.0f - c, 1.0f - m, 1.0f - y);
		}

		public override string ToString()
		{
			return string.Format("CMYK[\nC={0};\tM={1};\tY={2};\tK={3}\n]",
			                     _c, _m, _y, _k);
		}

		#endregion

		#region properties

		public float C
		{
			get { return _c; }
			set { _c = XYZ.ClipValue(value, 0.0f, 1.0f); }
		}

		public float M
		{
			get { return _m; }
			set { _m = XYZ.ClipValue(value, 0.0f, 1.0f); }
		}

		public float Y
		{
			get { return _y; }
			set { _y = XYZ.ClipValue(value, 0.0f, 1.0f); }
		}

		public float K
		{
			get { return _k; }
			set { _k = XYZ.ClipValue(value, 0.0f, 1.0f); }
		}

		#endregion
	}
}