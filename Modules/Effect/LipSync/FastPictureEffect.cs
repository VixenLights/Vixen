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

		public Curve LevelCurve { get; set; }

		public int ScalePercent { get; set; }

		public bool ScaleToGrid { get; set; }

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
			var intervalPos = GetEffectTimeIntervalPosition(frame);
			var intervalPosFactor = intervalPos * 100;
			double level = LevelCurve.GetValue(intervalPosFactor) / 100;
			if (_fp != null)
			{
				int yoffset = (BufferHt + _fp.Height) / 2;
				int xoffset = (_fp.Width - BufferWi) / 2;

				for (int x = 0; x < _fp.Width; x++)
				{
					for (int y = 0; y < _fp.Height; y++)
					{
						var fpColor = _fp.GetPixel(x, y);
						
						var hsv = HSV.FromRGB(fpColor);
						hsv.V = hsv.V * level;
	
						frameBuffer.SetPixel(x - xoffset, yoffset - y, hsv);
					}
				}	
			}
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
