using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace VixenModules.Effect.Nutcracker
{
    // hue, saturation and value are doubles in the range 0.0..1.0
    public class HSV
    {
        private float _hue, _saturation, _value;

        public HSV(float hue = 0f, float saturation = 0f, float value = 0f)
        {
            Hue = hue;
            Saturation = saturation;
            Value = value;
        }

        public float Hue 
        { 
            get { return _hue;} 
            set { _hue = value; }
        }

        public float Saturation
        {
            get { return _saturation; }
            set { _saturation = value; }
        }

        public float Value 
        {
            get { return _value; }
            set
            {
                _value = value; 
            }
        }

        public void SetToHSV(HSV hsv) 
        {
            Hue = hsv.Hue;
            Saturation = hsv.Saturation;
            Value = hsv.Value;
        }

        public static HSV ColorToHSV(Color color)
        {
            float max = Math.Max(color.R, Math.Max(color.G, color.B));
            float min = Math.Min(color.R, Math.Min(color.G, color.B));

            float hue = color.GetHue();
            float saturation = ((float)max == 0) ? 0 : 1f - (1f * (float)min / (float)max);
            float value = max / 255f;

            //Console.WriteLine("hue:" + hue);

            return new HSV(hue, saturation, value);
        }

        public static Color HSVtoColor(HSV inHsv)
        {
            HSV hsv = new HSV(inHsv.Hue, inHsv.Saturation, inHsv.Value);
            if (hsv.Hue > 0 && hsv.Hue < 1)
            {
                hsv.Hue *= 360;
            }
            int hi = Convert.ToInt32(Math.Floor(hsv.Hue / 60)) % 6;
            double f = hsv.Hue / 60f - Math.Floor(hsv.Hue / 60f);

            hsv.Value = hsv.Value * 255f;
            int v = Convert.ToInt32(hsv.Value);
            int p = Convert.ToInt32(hsv.Value * (1 - hsv.Saturation));
            int q = Convert.ToInt32(hsv.Value * (1 - f * hsv.Saturation));
            int t = Convert.ToInt32(hsv.Value * (1 - (1 - f) * hsv.Saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        ///// <summary>
        ///// Convert HSV to RGB
        ///// h is from 0-360
        ///// s,v values are 0-1
        ///// r,g,b values are 0-255
        ///// Based upon http://ilab.usc.edu/wiki/index.php/HSV_And_H2SV_Color_Space#HSV_Transformation_C_.2F_C.2B.2B_Code_2
        ///// </summary>
        //public static Color HsvToRgb(HSV hsv)
        //{
        //    // ######################################################################
        //    // T. Nathan Mundhenk
        //    // mundhenk@usc.edu
        //    // C/C++ Macro HSV to RGB

        //    double H = hsv.Hue;
        //    while (H < 0) { H += 360; };
        //    while (H >= 360) { H -= 360; };
        //    double R, G, B;
        //    if (hsv.Value <= 0)
        //    { R = G = B = 0; }
        //    else if (hsv.Saturation <= 0)
        //    {
        //        R = G = B = hsv.Value;
        //    }
        //    else
        //    {
        //        double hf = H / 60.0;
        //        int i = (int)Math.Floor(hf);
        //        double f = hf - i;
        //        double pv = hsv.Value * (1 - hsv.Saturation);
        //        double qv = hsv.Value * (1 - hsv.Saturation * f);
        //        double tv = hsv.Value * (1 - hsv.Saturation * (1 - f));
        //        switch (i)
        //        {

        //            // Red is the dominant color

        //            case 0:
        //                R = hsv.Value;
        //                G = tv;
        //                B = pv;
        //                break;

        //            // Green is the dominant color

        //            case 1:
        //                R = qv;
        //                G = hsv.Value;
        //                B = pv;
        //                break;
        //            case 2:
        //                R = pv;
        //                G = hsv.Value;
        //                B = tv;
        //                break;

        //            // Blue is the dominant color

        //            case 3:
        //                R = pv;
        //                G = qv;
        //                B = hsv.Value;
        //                break;
        //            case 4:
        //                R = tv;
        //                G = pv;
        //                B = hsv.Value;
        //                break;

        //            // Red is the dominant color

        //            case 5:
        //                R = hsv.Value;
        //                G = pv;
        //                B = qv;
        //                break;

        //            // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

        //            case 6:
        //                R = hsv.Value;
        //                G = tv;
        //                B = pv;
        //                break;
        //            case -1:
        //                R = hsv.Value;
        //                G = pv;
        //                B = qv;
        //                break;

        //            // The color is not defined, we should throw an error.

        //            default:
        //                //LFATAL("i Value error in Pixel conversion, Value is %d", i);
        //                R = G = B = hsv.Value; // Just pretend its black/white
        //                break;
        //        }
        //    }
        //    int r = Clamp((int)(R * 255.0));
        //    int g = Clamp((int)(G * 255.0));
        //    int b = Clamp((int)(B * 255.0));

        //    return Color.FromArgb(r, g, b);
        //}

        ///// <summary>
        ///// Clamp a value to 0-255
        ///// </summary>
        //static int Clamp(int i)
        //{
        //    if (i < 0) return 0;
        //    if (i > 255) return 255;
        //    return i;
        //}
    }
}
