using System.Diagnostics;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;

namespace Vixen.Data.Value
{
	public struct LightingValue : IIntentDataType
	{
		//Using HSV as doubles here makes for a larger size object than say RGB with would be 3 bytes.
		//Would it be beneficial to store everything as RGB and maybe seperate byte for intensity? 
		//Changing the doubles to floats introduces math errors in a just a blind conversion
		//We use a lot of these objects, so it makes sense to try and find an optimization here. 
		//Some effects use the RGBValue instead of this. Can we change things around a bit and always use it?
		//That would cause us to think about how we represent color in a more uniform manner. It is all over the board now.
		private readonly double _hue;
		private readonly double _saturation;
		private readonly double _value;
		private readonly double _intensity;
		
		/// <summary>
		/// Create a lighting value of the specified color with intensity of 1
		/// </summary>
		/// <param name="color"></param>
		public LightingValue(Color color):this(color, 1)
		{
		}

		/// <summary>
		/// Create a lighting value with the specified color and intensity
		/// </summary>
		/// <param name="color"></param>
		/// <param name="intensity">Percentage value in the range 0.0 -> 1.0 (from 0% to 100%).</param>
		public LightingValue(Color color, double intensity)
		{
			HSV.FromRGB(color, out _hue, out _saturation, out _value);
			_intensity = XYZ.ClipValue(intensity,0,1);
		}

		/// <summary>
		/// Create a lighting value of the specified color with intensity of 100%
		/// </summary>
		/// <param name="h"></param>
		/// <param name="s"></param>
		/// <param name="v"></param>
		public LightingValue(double h, double s, double v)
		{
			_hue = h;
			_saturation = s;
			_value = v;
			_intensity = 1;
		}

		public LightingValue(double h, double s, double v, double i)
		{
			_hue = h;
			_saturation = s;
			_value = v;
			_intensity = i;
		}

		public LightingValue(LightingValue lv, double i)
		{
			_hue = lv._hue;
			_saturation = lv._saturation;
			_value = lv._value;
			_intensity = i;
		}

		/// <summary>
		/// Percentage value in the range 0.0 -> 1.0 (from 0% to 100%).
		/// </summary>
		public double Hue
		{
			get { return _hue; }
		}

		/// <summary>
		/// Percentage value in the range 0.0 -> 1.0 (from 0% to 100%).
		/// </summary>
		public double Saturation
		{
			get { return _saturation; }
		}

		public double Value
		{
			get { return _value; }
		}

		/// <summary>
		/// The Intensity or brightness of this color in the range 0.0 -> 1.0 (from 0% to 100%).
		/// </summary>
		public double Intensity
		{
			get { return _intensity; }
		}

		public Color Color
		{
			get
			{
				return HSV.ToRGB(_hue, _saturation, _value);
			}
		}

		/// <summary>
		/// The lighting value as a intensity appplied color with a 100% alpha channel. Results in an opaque color ranging from black
		/// (0,0,0) when the intensity is 0 and the solid color when the intensity is 1 (ie. 100%).
		/// </summary>
		public Color FullColor
		{
			get { return HSV.ToRGB(_hue, _saturation, _value*Intensity); }
		}

		/// <summary>
		/// Gets the lighting value as a full brightness color with the intensity value applied to the alpha channel. 
		/// Results in an non opaque color ranging from transparent (0,0,0,0) when the intensity is 0 and the solid color when the intensity is 1 (ie. 100%).
		/// </summary>
		public Color FullColorWithAlpha
		{
			get
			{
				Color c = Color;
				return Color.FromArgb((int)(Intensity * byte.MaxValue), c);
			}
		}

	}
}