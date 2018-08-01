using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Curtain
{
	public class Curtain : PixelEffectBase
	{
		private CurtainData _data;
		private int _lastCurtainDir;
		private int _lastCurtainLimit;

		public Curtain()
		{
			_data = new CurtainData();
			EnableTargetPositioning(true, true);
			InitAllAttributes();
		}

		#region Setup

		[Value]
		public override StringOrientation StringOrientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Config properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyOrder(0)]
		public CurtainDirection Direction
		{
			get { return _data.Direction; }
			set
			{
				_data.Direction = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Iterations")]
		[ProviderDescription(@"Iterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(1)]
		public int Speed
		{
			get { return _data.Speed; }
			set
			{
				_data.Speed = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Edge")]
		[ProviderDescription(@"Edge")]
		[PropertyOrder(3)]
		public CurtainEdge Edge
		{
			get { return _data.Edge; }
			set
			{
				_data.Edge = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Swag")]
		[ProviderDescription(@"Swag")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 10, 1)]
		[PropertyOrder(4)]
		public int Swag
		{
			get { return _data.Swag; }
			set
			{
				_data.Swag = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"ColorGradient")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(2)]
		public ColorGradient Color
		{
			get { return _data.Gradient; }
			set
			{
				_data.Gradient = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Level properties

		[Value]
		[ProviderCategory(@"Brightness", 3)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		public Curve LevelCurve
		{
			get { return _data.LevelCurve; }
			set
			{
				_data.LevelCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/curtain/"; }
		}

		#endregion

		private void InitAllAttributes()
		{
			UpdateStringOrientationAttributes(true);
			TypeDescriptor.Refresh(this);
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as CurtainData;
				InitAllAttributes();
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		protected override void SetupRender()
		{
			if (Direction == CurtainDirection.CurtainOpen || Direction == CurtainDirection.CurtainOpenClose)
			{
				_lastCurtainDir = 0;
			}
			else
			{
				_lastCurtainDir = 1;
			}

			_lastCurtainLimit = 0;
		}

		protected override void CleanUpRender()
		{
			//Nothing to clean up
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			var swagArray = new List<int>();
			int curtainDir, xlimit, middle, ylimit;
			int swaglen = BufferHt > 1 ? Swag*BufferWi/40 : 0;
			double position = (GetEffectTimeIntervalPosition(frame)*Speed)%1;
			double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;

			if (swaglen > 0)
			{
				double a = (double) (BufferHt - 1)/(swaglen*swaglen);
				for (int x = 0; x < swaglen; x++)
				{
					swagArray.Add((int) (a*x*x));
				}
			}
			if (Direction < CurtainDirection.CurtainOpenClose)
			{
				if (Direction == CurtainDirection.CurtainOpen)
				{
					xlimit = (int)((position * BufferWi) + (swaglen * position * 2));
					ylimit = (int)((position * BufferHt) + (swaglen * position * 2));
				}
				else
				{
					xlimit = (int)((position * BufferWi) - (swaglen * (1 - position) * 2));
					ylimit = (int)((position * BufferHt) - (swaglen * (1 - position) * 2));
				}
			}
			else
			{
				if (Direction == CurtainDirection.CurtainOpenClose)
				{
					xlimit = (int)(position <= .5 ? (position * 2 * BufferWi) + (swaglen * position * 4) : ((position - .5) * 2 * BufferWi) - (swaglen * (1 - position) * 4));
					ylimit = (int)(position <= .5 ? (position * 2 * BufferHt) + (swaglen * position * 4) : ((position - .5) * 2 * BufferHt) - (swaglen * (1 - position) * 4));
				}
				else
				{
					xlimit = (int)(position <= .5 ? (position * 2 * BufferWi) - (swaglen * (0.5 - position) * 4) : ((position - .5) * 2 * BufferWi) + (swaglen * position * 2));
					ylimit = (int)(position <= .5 ? (position * 2 * BufferHt) - (swaglen * (0.5 - position) * 4) : ((position - .5) * 2 * BufferHt) + (swaglen * position * 2));
				}
			}
			if (Direction < CurtainDirection.CurtainOpenClose)
			{
				curtainDir = (int)Direction % 2;
			}
			else if (xlimit < _lastCurtainLimit - swaglen * 2)
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
			switch (Edge)
			{
				case CurtainEdge.Left:
					// left
					DrawCurtain(true, xlimit, swagArray, frameBuffer, level, (BufferWi- 1));
					break;
				case CurtainEdge.Center:
					// center
					middle = (xlimit + 1)/2;
					DrawCurtain(true, middle, swagArray, frameBuffer, level, (BufferWi / 2 - 1));
					DrawCurtain(false, middle, swagArray, frameBuffer, level, (BufferWi / 2 - 1));
					break;
				case CurtainEdge.Right:
					// right
					DrawCurtain(false, xlimit, swagArray, frameBuffer, level, (BufferWi - 1));
					break;
				case CurtainEdge.Bottom:

					// bottom
					DrawCurtainVertical(true, ylimit, swagArray, frameBuffer, level, (BufferHt - 1));
					break;
				case CurtainEdge.Middle:
					// middle
					middle = (ylimit + 1)/2;
					DrawCurtainVertical(true, middle, swagArray, frameBuffer, level, (BufferHt / 2 - 1));
					DrawCurtainVertical(false, middle, swagArray, frameBuffer, level, (BufferHt / 2 - 1));
					break;
				case CurtainEdge.Top:
					// top
					DrawCurtainVertical(false, ylimit, swagArray, frameBuffer, level, (BufferHt - 1));
					break;
			}
		}


		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var swagArray = new List<int>();
			int swaglen = BufferHt > 1 ? Swag * BufferWi / 40 : 0;

			var swagBufferHt = BufferHt;
			switch (Edge)
			{
					case CurtainEdge.Middle:
					case CurtainEdge.Top:
					case CurtainEdge.Bottom:
					swagBufferHt = BufferWi;
					break;

			}

			for (int effectFrame = 0; effectFrame < numFrames; effectFrame++)
			{
				frameBuffer.CurrentFrame = effectFrame;
				var timeIntervalPosition = GetEffectTimeIntervalPosition(effectFrame);
				double position = (timeIntervalPosition * Speed) % 1;
				double level = LevelCurve.GetValue(timeIntervalPosition * 100) / 100;

				if (swaglen > 0)
				{
					swagArray.Clear();
					double a = (double)(swagBufferHt) / (swaglen * swaglen);
					for (int x = 0; x < swaglen; x++)
					{
						swagArray.Add((int)(a * x * x));
					}
				}
				int xlimit;
				int ylimit;
				if (Direction < CurtainDirection.CurtainOpenClose)
				{
					if (Direction == CurtainDirection.CurtainOpen)
					{
						xlimit = (int)((position * BufferWi) + (swaglen * position * 2));
						ylimit = (int)((position * BufferHt) + (swaglen * position * 2));
					}
					else
					{
						xlimit = (int)((position * BufferWi) - (swaglen * (1 - position) * 2));
						ylimit = (int)((position * BufferHt) - (swaglen * (1 - position) * 2));
					}
				}
				else
				{
					if (Direction == CurtainDirection.CurtainOpenClose)
					{
						xlimit = (int)(position <= .5 ? (position * 2 * BufferWi) + (swaglen * position * 4) : ((position - .5) * 2 * BufferWi) - (swaglen * (1 - position) * 4));
						ylimit = (int)(position <= .5 ? (position * 2 * BufferHt) + (swaglen * position * 4) : ((position - .5) * 2 * BufferHt) - (swaglen * (1 - position) * 4));
					}
					else
					{
						xlimit = (int)(position <= .5 ? (position * 2 * BufferWi) - (swaglen * (0.5 - position) * 4) : ((position - .5) * 2 * BufferWi) + (swaglen * position * 2));
						ylimit = (int)(position <= .5 ? (position * 2 * BufferHt) - (swaglen * (0.5 - position) * 4) : ((position - .5) * 2 * BufferHt) + (swaglen * position * 2));
					}
				}
				int curtainDir;
				if (Direction < CurtainDirection.CurtainOpenClose)
				{
					curtainDir = (int)Direction % 2;
				}
				else if (xlimit < _lastCurtainLimit - swaglen * 2)
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
					xlimit = BufferWi - xlimit;
					ylimit = BufferHt - ylimit;
				}

				int middle;
				switch (Edge)
				{
					case CurtainEdge.Left:
						// left
						DrawCurtainLocation(true, xlimit, swagArray, frameBuffer, level, BufferWi);
						break;
					case CurtainEdge.Center:
						// center
						middle = xlimit / 2;
						DrawCurtainLocation(true, middle, swagArray, frameBuffer, level, BufferWi / 2);
						DrawCurtainLocation(false, middle, swagArray, frameBuffer, level, BufferWi / 2);
						break;
					case CurtainEdge.Right:
						// right
						DrawCurtainLocation(false, xlimit, swagArray, frameBuffer, level, BufferWi);
						break;
					case CurtainEdge.Bottom:

						// bottom
						DrawCurtainVerticalLocation(false, ylimit, swagArray, frameBuffer, level, BufferHt);
						break;
					case CurtainEdge.Middle:
						// middle
						middle = ylimit / 2;
						DrawCurtainVerticalLocation(true, middle, swagArray, frameBuffer, level, BufferHt / 2);
						DrawCurtainVerticalLocation(false, middle, swagArray, frameBuffer, level, BufferHt / 2);
						break;
					case CurtainEdge.Top:
						// top
						DrawCurtainVerticalLocation(true, ylimit, swagArray, frameBuffer, level, BufferHt);
						break;
				}
			}
		}

		private void DrawCurtainLocation(bool leftEdge, int xlimit, List<int> swagArray, PixelLocationFrameBuffer frameBuffer,
			double level, int width)
		{

			var rightBufferLimit = BufferWi + BufferWiOffset;

			var elements = leftEdge
				? frameBuffer.ElementLocations.Where(e => e.X >= (rightBufferLimit) - xlimit)
				: frameBuffer.ElementLocations.Where(e => e.X <= xlimit + BufferWiOffset);

			foreach (var elementLocation in elements)
			{
				var col = GetLocationColor(leftEdge, width, elementLocation);
				if (level < 1)
				{
					HSV hsv = HSV.FromRGB(col);
					hsv.V *= level;
					col = hsv.ToRGB();
				}
				frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, col);
			}

			// swag
			if (swagArray.Count > 0)
			{
				var swagElements = leftEdge ? frameBuffer.ElementLocations.Where(e => e.X <= (BufferWi + BufferWiOffset) - xlimit && e.X >= (BufferWi + BufferWiOffset) - (xlimit + swagArray.Count)).ToLookup(e => e.X) : frameBuffer.ElementLocations.Where(e => e.X >= xlimit + BufferWiOffset && e.X <= xlimit + swagArray.Count + BufferWiOffset).ToLookup(e => e.X);
				
				for (int i = 0; i < swagArray.Count; i++)
				{
					int x = leftEdge? rightBufferLimit - (xlimit + i):xlimit+i+BufferWiOffset;

					var limit = BufferHt - swagArray[i] + BufferHtOffset;
					foreach (var elementLocation in swagElements[x])
					{
						if (elementLocation.Y < limit)
						{
							var col = GetLocationColor(leftEdge, width, elementLocation);
							if (level < 1)
							{
								HSV hsv = HSV.FromRGB(col);
								hsv.V *= level;
								col = hsv.ToRGB();
							}
							frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, col);
						}
					}

				}
			}
		}

		private Color GetLocationColor(bool leftEdge, int width, ElementLocation elementLocation)
		{
			Color col;
			if (!leftEdge || BufferWi == width)
			{
				var percent = (double) (elementLocation.X - BufferWiOffset) / width;
				col = Color.GetColorAt(leftEdge ? 1 - percent : percent);
			}
			else
			{
				var percent = (double) (elementLocation.X - BufferWiOffset - (BufferWi - width)) / width;
				col = Color.GetColorAt(1 - percent);
			}
			return col;
		}

		private void DrawCurtainVerticalLocation(bool topEdge, int ylimit, List<int> swagArray, PixelLocationFrameBuffer frameBuffer,
			double level, int width)
		{
			var topBufferLimit = BufferHt + BufferHtOffset;
			var elements = topEdge ? frameBuffer.ElementLocations.Where(e => e.Y > topBufferLimit - ylimit) : frameBuffer.ElementLocations.Where(e => e.Y < ylimit + BufferHtOffset);

			foreach (var elementLocation in elements)
			{
				var col = GetVerticalLocationColor(topEdge, width, elementLocation);
				if (level < 1)
				{
					HSV hsv = HSV.FromRGB(col);
					hsv.V *= level;
					col = hsv.ToRGB();
				}
				frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, col);
			}

			//Swag
			if (swagArray.Count > 0)
			{
				var swagElements = topEdge ? frameBuffer.ElementLocations.Where(e => e.Y <= topBufferLimit - ylimit && e.Y >= topBufferLimit - (ylimit + swagArray.Count)).ToLookup(e => e.Y) : frameBuffer.ElementLocations.Where(e => e.Y >= ylimit + BufferHtOffset && e.Y <= ylimit + swagArray.Count + BufferHtOffset).ToLookup(e => e.Y);

				for (int i = 0; i < swagArray.Count; i++)
				{
					int x = topEdge ? topBufferLimit - (ylimit + i) : ylimit + i + BufferHtOffset;

					var limit = BufferWiOffset + swagArray[i];
					foreach (var elementLocation in swagElements[x])
					{
						if (elementLocation.X > limit)
						{
							var col = GetVerticalLocationColor(topEdge, width, elementLocation);
							if (level < 1)
							{
								HSV hsv = HSV.FromRGB(col);
								hsv.V *= level;
								col = hsv.ToRGB();
							}
							frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, col);
						}
					}
				}
			}
		}

		private Color GetVerticalLocationColor(bool topEdge, int width, ElementLocation elementLocation)
		{
			Color col;
			if (!topEdge || BufferHt == width)
			{
				var percent = (double) (elementLocation.Y - BufferHtOffset) / width;
				col = Color.GetColorAt(topEdge ? 1 - percent : percent);
			}
			else
			{
				var percent = (double) (elementLocation.Y - BufferHtOffset - (BufferHt - width)) / width;
				col = Color.GetColorAt(1 - percent);
			}
			return col;
		}

		private void DrawCurtain(bool leftEdge, int xlimit, List<int> swagArray, IPixelFrameBuffer frameBuffer, double level, int width)
		{
			Color col;
			for (int i = 0; i < xlimit; i++)
			{
				col = Color.GetColorAt((double)i / width);
				if (level < 1)
				{
					HSV hsv = HSV.FromRGB(col);
					hsv.V *= level;
					col = hsv.ToRGB();
				}
				
				int x = leftEdge ? BufferWi - i - 1 : i;
				for (int y = BufferHt - 1; y >= 0; y--)
				{
					frameBuffer.SetPixel(x, y, col);
				}
			}

			// swag
			for (int i = 0; i < swagArray.Count; i++)
			{
				int x = xlimit + i;
				col = Color.GetColorAt((double)x / width);
				if (level < 1)
				{
					HSV hsv = HSV.FromRGB(col);
					hsv.V *= level;
					col = hsv.ToRGB();
				}

				if (leftEdge) x = BufferWi - x - 1;
				for (int y = BufferHt - 1; y > swagArray[i]; y--)
				{
					frameBuffer.SetPixel(x, y, col);
				}
			}
		}

		private void DrawCurtainVertical(bool topEdge, int ylimit, List<int> swagArray, IPixelFrameBuffer frameBuffer,
			double level, int width)
		{
			int i, x, y;
			Color col;
			for (i = 0; i < ylimit; i++)
			{
				col = Color.GetColorAt((double)i / width);
				if (level < 1)
				{
					HSV hsv = HSV.FromRGB(col);
					hsv.V *= level;
					col = hsv.ToRGB();
				}
				y = topEdge ? BufferHt - i - 1 : i;
				for (x = BufferWi - 1; x >= 0; x--)
				{
					frameBuffer.SetPixel(x, y, col);
				}
			}

			// swag
			for (i = 0; i < swagArray.Count(); i++)
			{
				y = ylimit + i;
				col = Color.GetColorAt((double)y / width);
				if (level < 1)
				{
					HSV hsv = HSV.FromRGB(col);
					hsv.V *= level;
					col = hsv.ToRGB();
				}
				if (topEdge) y = BufferHt - y - 1;
				for (x = BufferWi - 1; x > swagArray[i]; x--)
				{
					frameBuffer.SetPixel(x, y, col);
				}
			}
		}
	}
}