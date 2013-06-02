using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;

namespace VixenModules.Effect.Nutcracker
{
    [DataContract]
    public class Palette
    {
        [DataMember]
        Color[] _colors;
        [DataMember]
        bool[] _colorsActive;
        Random random;

        public Palette()
        {
            _colors = new Color[6];
            _colorsActive = new bool[6];

            _colors[0] = Color.Red;
            _colors[1] = Color.Green;
            _colors[2] = Color.Blue;
            _colors[3] = Color.Yellow;
            _colors[4] = Color.Black;
            _colors[5] = Color.White;

            _colorsActive[0] = true;
            _colorsActive[1] = true;
        }

        public bool[] ColorsActive
        {
            get { return _colorsActive; }
            set { _colorsActive = value; }
        }

        public Color[] Colors
        {
            get { return _colors; }
            set
            {
                _colors = value;
            }
        }
        
        public int Count()
        {
            int activeColors = 0;
            foreach (bool isActive in ColorsActive)
            {
                if (isActive) activeColors++;
            }
            if (activeColors == 0)
            {
                return 1; // We're going to return 1 here because we'll return black or transparent if there are no colors
            }
            else
            {
                return activeColors;
            }
        }

        public void SetColor(int colorNum, Color color, bool setActive = true)
        {
            if (setActive)
            {
                ColorsActive[colorNum - 1] = true;
            }
            Colors[colorNum - 1] = color;
        }

        public Color GetColor(int index) 
        {
            if (index == 0 && Colors.Count() == 0)
            {
                return Color.Transparent;
            }

            Color color = Color.White;
            int currentColor = 0;
            int currentIndex = -1;
            foreach (bool isActive in ColorsActive)
            {
                if (isActive)
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        color = Colors[currentColor];
                        break;
                    }
                }
                currentColor++;
            }
            return color;
        }

        public HSV GetHSV(int index)
        {
            return HSV.ColorToHSV(GetColor(index));
            //return HSV.ColorToHSV(Colors[index]);
        }

        // generates a random number between num1 and num2 inclusive
        private float RandomRange(float num1, float num2)
        {
            double hi,lo;
            InitRandom();

            if (num1 < num2)
            {
                lo = num1;
                hi = num2;
            }
            else
            {
                lo = num2;
                hi = num1;
            }

            //return random.Next(lo, hi);
            return (float)(random.NextDouble()*(hi-lo)+ lo);
        }

        private void InitRandom() 
        {
            if (random == null)
                random = new Random();
        }

	    public void SetRangeColor(HSV hsv1, HSV hsv2, HSV newhsv)
	    {
		    newhsv.Hue = RandomRange(hsv1.Hue,hsv2.Hue);
		    newhsv.Saturation = RandomRange(hsv1.Saturation,hsv2.Saturation);
		    newhsv.Value = 1.0f;
	    }

    }
}
