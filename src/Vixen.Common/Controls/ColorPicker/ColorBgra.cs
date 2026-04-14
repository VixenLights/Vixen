using System.Runtime.InteropServices;

namespace Common.Controls.ColorManagement.ColorModels
{
	/// <summary>
	/// encapsulates a bgra color structure
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct ColorBgra
	{
		#region fields

		//components
		[FieldOffset(0)] public byte B;
		[FieldOffset(1)] public byte G;
		[FieldOffset(2)] public byte R;
		[FieldOffset(3)] public byte A;

		#endregion

		#region ctor

		public ColorBgra(uint argb)
		{
			A = (byte) (argb >> 24);
			R = (byte) (argb >> 16);
			G = (byte) (argb >> 8);
			B = (byte) (argb);
		}

		public ColorBgra(byte r, byte g, byte b) :
			this(255, r, g, b)
		{
		}

		public ColorBgra(byte a, byte r, byte g, byte b)
		{
			A = a;
			R = r;
			G = g;
			B = b;
		}

		#endregion

		#region constants

		public static ColorBgra Transparent
		{
			get { return new ColorBgra(0x00000000); }
		}

		public static ColorBgra Black
		{
			get { return new ColorBgra(0xff000000); }
		}

		public static ColorBgra White
		{
			get { return new ColorBgra(0xffffffff); }
		}

		#endregion

		#region properties

		/// <summary>
		/// gets or sets the alpha component of the color within range 0-255
		/// </summary>
		public int Alpha
		{
			get { return A; }
			set { A = (byte) Math.Min(255, Math.Max(0, value)); }
		}

		/// <summary>
		/// gets or sets the red component of the color within range 0-255
		/// </summary>
		public int Red
		{
			get { return R; }
			set { R = (byte) Math.Min(255, Math.Max(0, value)); }
		}

		/// <summary>
		/// gets or sets the green component of the color within range 0-255
		/// </summary>
		public int Green
		{
			get { return G; }
			set { G = (byte) Math.Min(255, Math.Max(0, value)); }
		}

		/// <summary>
		/// gets or sets the blue component of the color within range 0-255
		/// </summary>
		public int Blue
		{
			get { return B; }
			set { B = (byte) Math.Min(255, Math.Max(0, value)); }
		}

		#endregion

		#region conversion

		/// <summary>
		/// converts a system.drawing.color to colorbgra
		/// </summary>
		public static ColorBgra FromArgb(Color value)
		{
			return new ColorBgra(
				value.A,
				value.R,
				value.G,
				value.B);
		}

		/// <summary>
		/// converts a colorbgra to system.drawing.color
		/// </summary>
		public Color ToArgb()
		{
			return Color.FromArgb(
				A,
				R,
				G,
				B);
		}

		#endregion
	}
}