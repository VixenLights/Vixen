using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace VixenModules.Effect.Nutcracker
{
	partial class NutcrackerEffects
	{
		public enum CurtainType
		{
			//curtainType: 0=open, 1=close, 2=open then close, 3=close then open
			CurtainOpen = 0,
			CurtainClose,
			CurtainOpenClose,
			CurtainCloseOpen
		}

		private int _lastCurtainDir;
	    private int _lastCurtainLimit;

		public void RenderCurtain(int edge, CurtainType curtainType, int swag, bool repeat)
		{
			var swagArray = new List<int>();
			int curtainDir, xlimit, middle, ylimit;
			int swaglen = BufferHt > 1 ? swag*BufferWi/40 : 0;
			double position = GetEffectTimeIntervalPosition();

			if (swaglen > 0)
			{
				double a = (double) (BufferHt - 1)/(swaglen*swaglen);
				for (int x = 0; x < swaglen; x++)
				{
					swagArray.Add((int) (a*x*x));
				}
			}
			if (curtainType < CurtainType.CurtainOpenClose)
			{
				if (FitToTime)
				{
					xlimit = (int) (position*BufferWi);
					ylimit = (int) (position*BufferHt);
				}
				else
				{
					xlimit = (int) (repeat || State < 200 ? (State%200)*BufferWi/199 : BufferWi);
					ylimit = (int) (repeat || State < 200 ? (State%200)*BufferHt/199 : BufferHt);
				}
			}
			else
			{
				if (FitToTime)
				{
					xlimit = (int) (position <= .5 ? position*2*BufferWi : (position - .5)*2*BufferWi);
					ylimit = (int) (position <= .5 ? position*2*BufferHt : (position - .5)*2*BufferHt);
				}
				else
				{
					xlimit = (int) (repeat || State < 400 ? (State%200)*BufferWi/199 : 0);
					ylimit = (int) (repeat || State < 400 ? (State%200)*BufferHt/199 : 0);
				}
			}
			if (State == 0 || curtainType < CurtainType.CurtainOpenClose)
			{
				curtainDir = (int)curtainType%2;
			}
			else if (xlimit < _lastCurtainLimit)
			{
				curtainDir = 1 - _lastCurtainDir;
			}
			else
			{
				curtainDir = _lastCurtainDir;
			}
			_lastCurtainDir = curtainDir;
			_lastCurtainLimit = xlimit;
			if (curtainDir == 0)
			{
				xlimit = BufferWi - xlimit - 1;
				ylimit = BufferHt - ylimit - 1;
			}
			switch (edge)
			{
				case 0:
					// left
					DrawCurtain(true, xlimit, swagArray);
					break;
				case 1:
					// center
					middle = (xlimit + 1)/2;
					DrawCurtain(true, middle, swagArray);
					DrawCurtain(false, middle, swagArray);
					break;
				case 2:
					// right
					DrawCurtain(false, xlimit, swagArray);
					break;
				case 3:
					DrawCurtainVertical(true, ylimit, swagArray);
					break;
				case 4:
					middle = (ylimit + 1)/2;
					DrawCurtainVertical(true, middle, swagArray);
					DrawCurtainVertical(false, middle, swagArray);
					break;
				case 5:
					DrawCurtainVertical(false, ylimit, swagArray);
					break;
			}
		}

		private void DrawCurtain(bool leftEdge, int xlimit, List<int> swagArray)
		{
			int i, x, y;
			Color color;
			for (i = 0; i < xlimit; i++)
			{
				color = GetMultiColorBlend((double) i/BufferWi, true);
				x = leftEdge ? BufferWi - i - 1 : i;
				for (y = BufferHt - 1; y >= 0; y--)
				{
					SetPixel(x, y, color);
				}
			}

			// swag
			for (i = 0; i < swagArray.Count; i++)
			{
				x = xlimit + i;
				color = GetMultiColorBlend((double) x/BufferWi, true);
				if (leftEdge) x = BufferWi - x - 1;
				for (y = BufferHt - 1; y > swagArray[i]; y--)
				{
					SetPixel(x, y, color);
				}
			}
		}

		private void DrawCurtainVertical(bool topEdge, int ylimit, List<int> swagArray)
		{
			int i, x, y;
			Color color;
			for (i = 0; i < ylimit; i++)
			{
				color = GetMultiColorBlend(((double) i/BufferHt), true);
				y = topEdge ? BufferHt - i - 1 : i;
				for (x = BufferWi - 1; x >= 0; x--)
				{
					SetPixel(x, y, color);
				}
			}

			// swag
			for (i = 0; i < swagArray.Count(); i++)
			{
				y = ylimit + i;
				color = GetMultiColorBlend(((double) y/BufferHt), true);
				if (topEdge) y = BufferHt - y - 1;
				for (x = BufferWi - 1; x > swagArray[i]; x--)
				{
					SetPixel(x, y, color);
				}
			}
		}
	}
}