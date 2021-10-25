using System;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Picture;

namespace VixenModules.Effect.LipSync
{
	/// <summary>
	/// A Light fast effect to generate pictures.
	/// </summary>
	public class FastPictureEffect:PixelEffectBase
	{
		private FastPixel.FastPixel _fp;

		public FastPictureEffect()
		{
			EffectModuleData = new PictureData();
		}

		public int Level { get; set; }

		public int ScalePercent { get; set; }

		public int TotalFrames { get; set; }

		public TimeSpan EffectEndTime { get; set; }

		public bool ScaleToGrid { get; set; }

		public Curve YOffsetCurve { get; set; }

		public Curve XOffsetCurve { get; set; }

		public Image Image { get; set; }

		#region Overrides of BaseEffect

		/// <inheritdoc />
		protected override EffectTypeModuleData EffectModuleData { get; }

		#endregion

		#region Overrides of PixelEffectBase

		/// <inheritdoc />
		public override StringOrientation StringOrientation { get; set; }

		/// <inheritdoc />
		protected override void SetupRender()
		{
			var scaledImage = ScaleToGrid
				? Picture.Picture.ScalePictureImage(Image, BufferWi, BufferHt)
				: Picture.Picture.ScaleImage(Image, (double)ScalePercent / 100);
			_fp = new FastPixel.FastPixel(scaledImage);
			_fp.Lock();
		}

		/// <inheritdoc />
		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			var intervalPosFactor = ((double)100 / EffectEndTime.TotalMilliseconds) * (StartTime.TotalMilliseconds + frame * FrameTime);
			var yOffsetAdjust = CalculateYOffset(intervalPosFactor);
			var xOffsetAdjust = CalculateXOffset(intervalPosFactor);
			if (_fp != null)
			{
				int yoffset = (int)(((double)(BufferHt + _fp.Height) / 2) + yOffsetAdjust - 1);
				int xoffset = (int)(xOffsetAdjust + ((double)BufferWi - _fp.Width) / 2);

				for (int x = 0; x < _fp.Width; x++)
				{
					for (int y = 0; y < _fp.Height; y++)
					{
						var fpColor = _fp.GetPixel(x, y);

						if (Level < 100)
						{
							var hsv = HSV.FromRGB(fpColor);
							hsv.V = hsv.V * ((double)Level / 100);
							fpColor = hsv.ToRGB();
						}
						frameBuffer.SetPixel(x + xoffset, yoffset - y, fpColor);
					}
				}
			}
		}

		private int CalculateYOffset(double intervalPos)
		{
			return (int)ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), (int)(BufferHt / 2 + _fp.Height / 2), -(int)(BufferHt / 2 + _fp.Height / 2));
		}

		private int CalculateXOffset(double intervalPos)
		{
			return (int)ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), (int)(BufferWi/2 + _fp.Width / 2), -(int)(BufferWi / 2 + _fp.Width / 2));
		}

		/// <inheritdoc />
		protected override void CleanUpRender()
		{
			_fp.Unlock(false);
			_fp.Dispose();
		}

		#endregion
	}
}
