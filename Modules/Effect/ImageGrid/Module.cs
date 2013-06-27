using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using Common.ValueTypes;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace VixenModules.Effect.ImageGrid
{
	public class Module : EffectModuleInstanceBase
	{
		private Data _data;
		private EffectIntents _effectIntents;

		protected override void _PreRender()
		{
			_effectIntents = new EffectIntents();

			Image image;
			try {
				image = Image.FromFile(_data.FilePath);
			}
			catch {
				return;
			}

			foreach (ElementNode targetNode in TargetNodes) {
				// Each element represents a single pixel in the grid display.
				// Therefore, the intent for the element will represent the state of that
				// pixel over the lifetime of the effect.

				// Get the grid dimensions from the node.
				VixenModules.Property.Grid.Module gridProperty =
					(VixenModules.Property.Grid.Module) targetNode.Properties.Get(((Descriptor) Descriptor)._gridPropertyId);
				VixenModules.Property.Grid.Data gridData = (VixenModules.Property.Grid.Data) gridProperty.ModuleData;

				// For now, just scale it to the dimensions of the grid.
				Element[] elements = targetNode.ToArray();
				byte[] pixelBuffer = new byte[] {0, 0, 0, byte.MaxValue};
				using (Bitmap bitmap = new Bitmap(gridData.Width, gridData.Height, image.PixelFormat)) {
					using (Graphics g = Graphics.FromImage(bitmap)) {
						g.InterpolationMode = InterpolationMode.HighQualityBicubic;
						g.DrawImage(image, 0, 0, bitmap.Width, bitmap.Height);

						BitmapData bitmapData = bitmap.LockBits(Rectangle.FromLTRB(0, 0, bitmap.Width, bitmap.Height),
						                                        ImageLockMode.ReadOnly, image.PixelFormat);

						byte[] rgbValues = new byte[Math.Abs(bitmapData.Stride)*bitmap.Height];
						System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, rgbValues, 0, rgbValues.Length);

						int bytesPerPixel = bitmapData.Stride/bitmapData.Width;
						for (int y = 0, pixelIndex = 0; y < bitmapData.Height; y++) {
							int sourceDataIndex = y*bitmapData.Stride;
							for (int x = 0; x < bitmapData.Width; x++, pixelIndex++, sourceDataIndex += bytesPerPixel) {
								Array.Copy(rgbValues, sourceDataIndex, pixelBuffer, 0, bytesPerPixel);
								int argbValue = BitConverter.ToInt32(pixelBuffer, 0);
								Color pixelColor = Color.FromArgb(argbValue);
								LightingValue startValue = new LightingValue(pixelColor, 1);
								LightingValue endValue = new LightingValue(pixelColor, 1);
								IIntent intent = new LightingIntent(startValue, endValue, TimeSpan);
								_effectIntents.AddIntentForElement(elements[pixelIndex].Id, intent, TimeSpan.Zero);
							}
						}

						bitmap.UnlockBits(bitmapData);
					}
				}
			}
		}

		protected override EffectIntents _Render()
		{
			return _effectIntents;
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set { _data = (Data) value; }
		}

		[Value]
		public FilePath FilePath
		{
			get { return new FilePath(_data.FilePath); }
			set { _data.FilePath = value.Value; }
		}
	}
}