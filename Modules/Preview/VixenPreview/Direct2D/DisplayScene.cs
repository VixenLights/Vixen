using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Intent;
using D2D = Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using DWrite = Microsoft.WindowsAPICodePack.DirectX.DirectWrite;
using WIC = Microsoft.WindowsAPICodePack.DirectX.WindowsImagingComponent;
using System.Threading.Tasks;
using System.Drawing;
using System.Timers;
using System.Diagnostics;
using System.Drawing.Imaging;
using Vixen.Sys;
using System.Threading;
using VixenModules.Preview.VixenPreview.Shapes;
using Vixen.Data.Value;
using Common.Controls.Direct2D;
using System.Drawing.Drawing2D;
using System.IO;

namespace VixenModules.Preview.VixenPreview.Direct2D {
	public sealed class DisplayScene : AnimatedScene {

		// we were requesting 100 fps before, but only got 20, so ask for 20 from now on...
		private static int desiredFps = 20;
		private static int timeQueueSeconds = 5;

		private DWrite.TextFormat textFormat;
		private DWrite.DWriteFactory writeFactory;
		private const int PIXEL_SIZE = 2;
		// These are used for tracking an accurate frames per second
		private DateTime time;
		private long frameCount;
		private long fps;
		private Guid DisplayID = Guid.Empty;

		public DisplayScene(System.Drawing.Image backgroundImage)
			: base(desiredFps) {
			this.writeFactory = DWrite.DWriteFactory.CreateFactory();
			BackgroundImage = backgroundImage;
		}
		private VixenPreviewData _data;
		public VixenPreviewData Data {
			set {
				_data = value;

			}
			get { return _data; }
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				this.writeFactory.Dispose();

			}

			base.Dispose(disposing);
		}

		protected override void OnCreateResources() {
			// We don't need to free any resources because the base class will
			// call OnFreeResources if necessary before calling this method.

			this.textFormat = this.writeFactory.CreateTextFormat("Arial", 10);

			base.OnCreateResources(); // Call this last to start the animation
		}

		protected override void OnFreeResources() {
			base.OnFreeResources(); // Call this first to stop the animation

		}
		System.Drawing.Image backgroundImage;
		int? imageHash;
		int? lastImageHash;

		public System.Drawing.Image BackgroundImage {
			get { return backgroundImage; }
			set {
				backgroundImage = value;
				if (value != null)
					imageHash = value.GetHashCode();
				else
					imageHash = null;

				if (imageHash != lastImageHash)
					isDirty = true;
			}
		}

		bool isDirty = true;
		D2D.D2DBitmap background = null;

		public D2D.D2DBitmap Background {
			get {
				TryCreateBackgroundBitmap();
				return background;
			}
		}


		public void ConvertImageToStreamAndAdjustBrightness(Image image, int value, MemoryStream stream) {

			if (value == 0) // No change, so just return
				return;

			using (Image img = image) {

				float sb = (float)value / 255F;

				float[][] colorMatrixElements =
				  {
                        new float[] {sb,  0,  0,  0, 0},
                        new float[] {0,  sb,  0,  0, 0},
                        new float[] {0,  0,  sb,  0, 0},
                        new float[] {0,  0,  0,  1, 0},
                        new float[] {0,  0,  0,  0, 1}

                  };

				ColorMatrix cm = new ColorMatrix(colorMatrixElements);

				using (ImageAttributes imgattr = new ImageAttributes()) {
					Rectangle rc = new Rectangle(0, 0, img.Width, img.Height);
					using (Graphics g = Graphics.FromImage(img)) {
						g.InterpolationMode = InterpolationMode.HighQualityBicubic;
						imgattr.SetColorMatrix(cm);
						g.DrawImage(img, rc, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgattr);
					}
				}

				img.Save(stream, img.RawFormat);
			}
		}

		private void TryCreateBackgroundBitmap() {
			if (RenderTarget != null && (imageHash != lastImageHash) && ((isDirty && BackgroundImage != null) || (background == null && BackgroundImage != null))) {
				 
				using (var ms = new System.IO.MemoryStream()) {

					ConvertImageToStreamAndAdjustBrightness(BackgroundImage, Data.BackgroundAlpha, ms);
					ms.Position = 0;

					using (var factory = WIC.ImagingFactory.Create()) {
						using (WIC.BitmapDecoder decoder = factory.CreateDecoderFromStream(ms, WIC.DecodeMetadataCacheOption.OnDemand)) {
							using (WIC.BitmapFrameDecode source = decoder.GetFrame(0)) {
								using (WIC.FormatConverter converter = factory.CreateFormatConverter()) {
									converter.Initialize(source.ToBitmapSource(), WIC.PixelFormats.Pbgra32Bpp, WIC.BitmapDitherType.None, WIC.BitmapPaletteType.MedianCut);
									background = RenderTarget.CreateBitmapFromWicBitmap(converter.ToBitmapSource());
									isDirty = false;
									lastImageHash = imageHash;
								}
							}
						}
					}
				}
			}
		}

		public static ConcurrentDictionary<Guid, ElementNode> ElementNodeCache = new ConcurrentDictionary<Guid, ElementNode>();

		protected override void OnRender() { }

	
		// we maintain a queue of the last N update times for stats reporting
		// we size it so that data ages out after timeQueueSeconds.
		Queue<long> updatTimes = new Queue<long>();

		public Queue<long> UpdateTimes {
			get { return updatTimes; }
			set {
				updatTimes = value;
				while (updatTimes.Count() > desiredFps*timeQueueSeconds) {
					var i = updatTimes.Dequeue();
				}
			}
		}

		// helper for maintaning the stats queue
		private void AddUpdateTime(long newval, out long maxval, out long avgval)
		{
			updatTimes.Enqueue(newval);
			while (updatTimes.Count() > desiredFps*timeQueueSeconds)
				updatTimes.Dequeue();
			maxval = updatTimes.Max();
			avgval = (long)updatTimes.Average();
		}

		public void Update() {

			Vixen.Sys.Managers.ElementManager elements = VixenSystem.Elements;
			Element[] elementArray = elements.Where(e => e.State.Where(i => (i as IIntentState<LightingValue>) != null).Where(i => (i as IIntentState<LightingValue>).GetValue().Intensity > 0).Any()).ToArray();

			try
			{
				Stopwatch w = Stopwatch.StartNew();

				// Calculate our actual frame rate
				this.frameCount++;

				if (DateTime.UtcNow.Subtract(this.time).TotalSeconds >= 1) {
					this.fps = this.frameCount;
					this.frameCount = 0;
					this.time = DateTime.UtcNow;
				}
				int iPixels = 0;

				// since we need to clear the display and re-draw it, it flickers if we do it live
				// so, create a temp, local, compatible bitmap render target for drawing
				// later we'll blt this to the real render target in one feel swoop
				using (var rt = this.RenderTarget.CreateCompatibleRenderTarget())
				{

					rt.BeginDraw();

					rt.Clear(new D2D.ColorF(0, 0, 0, 1f));
					//rt.Clear();

					if (Background != null) {
						rt.DrawBitmap(Background);
					}
					if (NodeToPixel.Count == 0)
						Reload();

					CancellationTokenSource tokenSource = new CancellationTokenSource();



					try
					{
						elementArray.AsParallel().WithCancellation(tokenSource.Token).ForAll(element =>
						{

							ElementNode node;

							if (!ElementNodeCache.TryGetValue(element.Id, out node))
							{
								if (element != null)
								{

									node = VixenSystem.Elements.GetElementNodeForElement(element);
									if (node != null)
										ElementNodeCache.TryAdd(element.Id, node);
								}
							}

							if (node != null)
							{
								Color pixColor;


								//TODO: Discrete Colors
								pixColor = IntentHelpers.GetAlphaRGBMaxColorForIntents(element.State);

								if (pixColor.A > 0)
									using (var brush = rt.CreateSolidColorBrush(pixColor.ToColorF())) {

										List<PreviewPixel> pixels;
										if (NodeToPixel.TryGetValue(node, out pixels))
										{
											iPixels += pixels.Count;
											pixels.ForEach(p => RenderPixel(rt, /*channelIntentState, */p, brush));

										}
									}
							}
						});
					}
					catch (Exception)
					{
						tokenSource.Cancel();
						//Console.WriteLine(e.Message);
					}


					// done w/local bitmap render target
					rt.EndDraw();

					// now get the bitmap and write it to the real target
					this.RenderTarget.BeginDraw();

					this.RenderTarget.DrawBitmap(rt.Bitmap);

					// Draw a little FPS in the top left corner
					w.Stop();
					var ms = w.ElapsedMilliseconds;
					long avgtime, maxtime;
					AddUpdateTime( ms, out maxtime, out avgtime);

					string text = string.Format("FPS {0} \nPoints {1} \nPixels {3} \nRender Time:{2} \nMax Render: {4} \nAvg Render: {5}", this.fps, elements.Count(), w.ElapsedMilliseconds, iPixels, avgtime.ToString("00"), avgtime.ToString("00"));

					using (var textBrush = this.RenderTarget.CreateSolidColorBrush(Color.White.ToColorF()))	{
						this.RenderTarget.DrawText(text, this.textFormat, new D2D.RectF(10, 10, 100, 20), textBrush);
					}

					// All done!
					this.RenderTarget.EndDraw();
				}

			}
			catch (Exception e) {

				Console.WriteLine(e.Message);
			}
		}

		private void RenderPixel( D2D.RenderTarget rt, PreviewPixel p, D2D.SolidColorBrush brush) {
			if (p.PixelSize <= 4)
				rt.DrawLine(new D2D.Point2F(p.X, p.Y), new D2D.Point2F(p.X + 1, p.Y + 1), brush, p.PixelSize);
			else
				rt.FillEllipse(new D2D.Ellipse() { Point = new D2D.Point2F(p.X, p.Y), RadiusX = p.PixelSize / 2, RadiusY = p.PixelSize / 2 }, brush);

		}


		#region Old Update Stuff

		public ConcurrentDictionary<ElementNode, List<PreviewPixel>> NodeToPixel = new ConcurrentDictionary<ElementNode, List<PreviewPixel>>();
		public List<DisplayItem> DisplayItems {
			get {
				if (Data != null) {
					return Data.DisplayItems;
				}
				else {
					return null;
				}
			}
		}

		public void Reload() {
			//lock (PreviewTools.renderLock)
			//{
			if (NodeToPixel == null)
				throw new System.ArgumentException("PreviewBase.NodeToPixel == null");

			NodeToPixel.Clear();

			if (DisplayItems == null)
				throw new System.ArgumentException("DisplayItems == null");

			if (DisplayItems != null)
				foreach (DisplayItem item in DisplayItems) {
					if (item.Shape.Pixels == null)
						throw new System.ArgumentException("item.Shape.Pixels == null");

					foreach (PreviewPixel pixel in item.Shape.Pixels) {
						if (pixel.Node != null) {
							List<PreviewPixel> pixels;
							if (NodeToPixel.TryGetValue(pixel.Node, out pixels)) {
								if (!pixels.Contains(pixel)) {
									pixels.Add(pixel);
								}
							}
							else {
								pixels = new List<PreviewPixel>();
								pixels.Add(pixel);
								NodeToPixel.TryAdd(pixel.Node, pixels);
							}
						}
					}
				}

			
			var file = Path.Combine(VixenPreviewDescriptor.ModulePath, Data.BackgroundFileName);
			BackgroundImage = File.Exists(file) ? Image.FromFile(file) : null;
			
		}


		#endregion


	}
}
