using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		private DWrite.TextFormat textFormat;
		private DWrite.DWriteFactory writeFactory;
		private const int PIXEL_SIZE = 2;
		// These are used for tracking an accurate frames per second
		private DateTime time;
		private int frameCount;
		private int fps;
		private Guid DisplayID = Guid.Empty;
		public DisplayScene(System.Drawing.Image backgroundImage)
			: base(100) // Will probably only be about 67 fps due to the limitations of the timer
		{
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

		public bool Enabled { get { return IsAnimating; } set { IsAnimating = value; } }

		public void ConvertImageToStreamAndAdjustBrightness(Image image, int value, MemoryStream stream) {

			if (value == 0) // No change, so just return
				return;

			using (Image img = image) {

				float sb = (((float)value - 255) / 255F) * .7f;

				float[][] colorMatrixElements =
				  {
                        new float[] {1,  0,  0,  0, 0},
                        new float[] {0,  1,  0,  0, 0},
                        new float[] {0,  0,  1,  0, 0},
                        new float[] {0,  0,  0,  1, 0},
                        new float[] {sb, sb, sb, 1, 1}

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
				Console.WriteLine("TryCreateBackgroundBitmap");
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



		protected override void OnRender() {

			try {
				Stopwatch w = Stopwatch.StartNew();

				// Calculate our actual frame rate
				this.frameCount++;

				if (DateTime.UtcNow.Subtract(this.time).TotalSeconds >= 1) {
					this.fps = this.frameCount;
					this.frameCount = 0;
					this.time = DateTime.UtcNow;
				}


				this.RenderTarget.BeginDraw();

				this.RenderTarget.Clear(new D2D.ColorF(0, 0, 0, 1f));
				//this.RenderTarget.Clear();

				if (Background != null) {
					//if (isDirty || currentState == null) {
					//	currentState = this.Factory.CreateDrawingStateBlock();
					this.RenderTarget.DrawBitmap(Background);
					//	this.RenderTarget.SaveDrawingState(currentState);
					//	isDirty = false;
					//} else
					//	this.RenderTarget.RestoreDrawingState(currentState);
				}
				if (NodeToPixel.Count == 0)
					Reload();

				CancellationTokenSource tokenSource = new CancellationTokenSource();

				if (IsAnimating) {

					try {

						//ElementStates.AsParallel().WithCancellation(tokenSource.Token).ForAll(channelIntentState => {

						foreach (var channelIntentState in ElementStates) {

							var elementId = channelIntentState.Key;
							Element element = VixenSystem.Elements.GetElement(elementId);

							if (element != null) {

								ElementNode node = VixenSystem.Elements.GetElementNodeForElement(element);
								if (node != null) {

									List<PreviewPixel> pixels;
									if (NodeToPixel.TryGetValue(node, out pixels)) {


										foreach (PreviewPixel p in pixels) {

											Color pixColor;


											//TODO: Discrete Colors
											pixColor = Vixen.Intent.ColorIntent.GetAlphaColorForIntents(channelIntentState.Value);

											using (var brush = this.RenderTarget.CreateSolidColorBrush(pixColor.ToColorF())) {
												//this.RenderTarget.DrawRectangle(rect, brush, .5f);
												//this.RenderTarget.FillRectangle(rect, brush);
												this.RenderTarget.FillEllipse(new D2D.Ellipse() { Point = new D2D.Point2F(p.X, p.Y), RadiusX = p.PixelSize / 2, RadiusY = p.PixelSize / 2 }, brush);
											}

										}
									}
								}
							}
						}
						//});

					}
					catch (Exception e) {
						tokenSource.Cancel();
						//Console.WriteLine(e.Message);
					}
				}





				w.Stop();
				// Draw a little FPS in the top left corner
				string text = string.Format("FPS {0} Points {1}", this.fps, ElementStates.Count());

				using (var textBrush = this.RenderTarget.CreateSolidColorBrush(Color.White.ToColorF())) {
					this.RenderTarget.DrawText(text, this.textFormat, new D2D.RectF(10, 10, 100, 20), textBrush);
				}

				// All done!

				this.RenderTarget.EndDraw();




			}
			catch (Exception e) {

				Console.WriteLine(e.Message);
			}
		}


		//private void GenerateDemoPoints()
		//{
		//	if (Points == null) Points = new ConcurrentBag< DisplayPoint>();


		//	Stopwatch w = Stopwatch.StartNew();


		//	var size = this.RenderTarget.Size;

		//	if (size.Width > 1 && size.Height > 1) {

		//		Random rnd = new Random((int)((double)DateTime.Now.Millisecond * Math.PI));

		//		for (int i = 0; i < size.Width; i++) {
		//			for (int j = 1; j < size.Height + 1; j++) {
		//				if (rnd.Next(1, 100) <= 5) {
		//					DisplayPoint p = new DisplayPoint();

		//					p = new DisplayPoint() {
		//						Shape = new Rectangle() {
		//							Height = PIXEL_SIZE,
		//							Width = PIXEL_SIZE,
		//							X = (int)i * PIXEL_SIZE,
		//							Y = (int)j * PIXEL_SIZE,
		//						},
		//						Identifier = (float)i + ((float)j / 100),
		//						Color = System.Drawing.Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255))
		//					};

		//					Points[p.Identifier] = p;
		//				}
		//			}
		//		}
		//		Console.WriteLine("Generated {1} Points In {0}", w.Elapsed, Points.Count());

		//	}

		//}

		public struct DisplayPoint {
			public Rectangle Shape { get; set; }
			public System.Drawing.Color Color { get; set; }
			public string Identifier { get { return string.Format("{0}-{1}-{2}-{3}", Shape.X, Shape.Y, Shape.Width, Shape.Height); } }
			public int PixelSize { get; set; }

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

			if (System.IO.File.Exists(Data.BackgroundFileName))
				BackgroundImage = Image.FromFile(Data.BackgroundFileName);
			else
				BackgroundImage = null;

			//LoadBackground();
			//}

		}

		public ElementIntentStates ElementStates { get; set; }


		#endregion


	}
}
