using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Common.Controls;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.Shapes;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using Common.Resources.Properties;
using Vixen.Sys.Instrumentation;

namespace VixenModules.Preview.VixenPreview
{
	public partial class GDIPreviewForm : BaseForm, IDisplayForm
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private bool needsUpdate = true;
		private bool formLoading = true;
		private static MillisecondsValue _previewSetPixelsTime;
		private Stopwatch _sw = Stopwatch.StartNew();

		public GDIPreviewForm(VixenPreviewData data)
		{
			Icon = Resources.Icon_Vixen3;
			InitializeComponent();
			Data = data;
			_previewSetPixelsTime = new MillisecondsValue("Preview pixel set time");
			VixenSystem.Instrumentation.AddValue(_previewSetPixelsTime);
		}

		private const int CP_NOCLOSE_BUTTON = 0x200;
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams myCp = base.CreateParams;
				myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
				return myCp;
			}
		}

		private bool IsVisibleOnAnyScreen(Rectangle rect)
		{
			return Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(rect));
		}

		public VixenPreviewData Data { get; set; }

		public void UpdatePreview(/*Vixen.Preview.PreviewElementIntentStates elementStates*/)
		{
			if (!gdiControl.IsUpdating)
			{
				if (!VixenSystem.Elements.ElementsHaveState)
				{
					if (needsUpdate)
					{
						needsUpdate = false;
						gdiControl.BeginUpdate();
						gdiControl.EndUpdate();
						gdiControl.Invalidate();
					}

					toolStripStatusFPS.Text = "0 fps";
					return;
				}

				needsUpdate = true;

				try
				{
					gdiControl.BeginUpdate();

					_sw.Restart();
					Parallel.ForEach(VixenSystem.Elements, UpdateElementPixels);
					
					_previewSetPixelsTime.Set(_sw.ElapsedMilliseconds);
				}
				catch (Exception e)
				{
					Logging.Error(e.Message, e);
				}

				gdiControl.EndUpdate();
				gdiControl.Invalidate();

				toolStripStatusFPS.Text = string.Format("{0} fps", gdiControl.FrameRate.ToString());
			}
		}

		private void UpdateElementPixels(Element element)
		{
			if (element.State.Count > 0)
			{
				List<PreviewPixel> pixels;
				if (NodeToPixel.TryGetValue(element.Id, out pixels))
				{
					foreach (PreviewPixel pixel in pixels)
					{
						pixel.Draw(gdiControl.FastPixel, element.State);
					}
				}
			}
		}

		public ConcurrentDictionary<Guid, List<PreviewPixel>> NodeToPixel = new ConcurrentDictionary<Guid, List<PreviewPixel>>();
		public List<DisplayItem> DisplayItems
		{
			get
			{
				if (Data != null)
				{
					return Data.DisplayItems;
				}
				else
				{
					return null;
				}
			}
		}

		public void Reload()
		{
			if (NodeToPixel == null)
				throw new System.ArgumentException("PreviewBase.NodeToPixel == null");

			NodeToPixel.Clear();

			if (DisplayItems == null)
				throw new System.ArgumentException("DisplayItems == null");

			if (DisplayItems != null)
			{
				int pixelCount = 0;
				foreach (DisplayItem item in DisplayItems)
				{
					item.Shape.Layout();
					if (item.Shape.Pixels == null)
						throw new System.ArgumentException("item.Shape.Pixels == null");

					foreach (PreviewPixel pixel in item.Shape.Pixels)
					{
						if (pixel.Node != null)
						{
							if (pixel.Node.Element == null)
							{
								Logging.Warn("Null element for Node {0}", pixel.Node.Name);
								continue;
							}
							pixelCount++;
							List<PreviewPixel> pixels;
							if (NodeToPixel.TryGetValue(pixel.Node.Element.Id, out pixels))
							{
								if (!pixels.Contains(pixel))
								{
									pixels.Add(pixel);
								}
							}
							else
							{
								pixels = new List<PreviewPixel>();
								pixels.Add(pixel);
								NodeToPixel.TryAdd(pixel.Node.Element.Id, pixels);
							}
						}
					}
				}
				toolStripStatusPixels.Text = pixelCount.ToString();
			}

			gdiControl.BackgroundAlpha = Data.BackgroundAlpha;
			if (string.IsNullOrEmpty(Data.BackgroundFileName))
			{
				gdiControl.Background = null;
			}
			else
			{
				var file = Path.Combine(VixenPreviewDescriptor.ModulePath, Data.BackgroundFileName);
				gdiControl.Background = File.Exists(file) ? Image.FromFile(file) : null;
			}
			LayoutProps();
		}

		public void LayoutProps()
		{
			if (DisplayItems != null)
			{
				foreach (DisplayItem item in DisplayItems)
				{
					item.Shape.Layout();
				}
			}
		}

		public void Setup()
		{
			Reload();
			if (InvokeRequired)
			{
				BeginInvoke(new Action(RestoreWindowState));
			}
		}

		private void GDIPreviewForm_Move(object sender, EventArgs e)
		{
			if (Data == null)
			{
				Logging.Warn("VixenPreviewDisplay_Move: Data is null. abandoning move. (Thread ID: " +
											System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				return;
			}

			Data.Top = Top;
			Data.Left = Left;
			if(!formLoading) SaveWindowState();
		}

		private void GDIPreviewForm_Resize(object sender, EventArgs e)
		{
			if (Data == null)
			{
				Logging.Warn("VixenPreviewDisplay_Resize: Data is null. abandoning resize. (Thread ID: " +
											System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				return;
			}

			Data.Width = Width;
			Data.Height = Height;
			if (!formLoading) SaveWindowState();
		}

		private void GDIPreviewForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				var messageBox = new MessageBoxForm("The preview can only be closed from the Preview Configuration dialog.", "Close", MessageBoxButtons.OK, SystemIcons.Information);
				messageBox.ShowDialog();
				e.Cancel = true;
			}

			SaveWindowState();
		}

		private void SaveWindowState()
		{
			XMLProfileSettings xml = new XMLProfileSettings();
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowHeight", Name), Size.Height);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowWidth", Name), Size.Width);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationX", Name), Location.X);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationY", Name), Location.Y);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowState", Name),
				WindowState.ToString());
		}

		private void GDIPreviewForm_Load(object sender, EventArgs e)
		{
			formLoading = true;
			RestoreWindowState();
			formLoading = false;
		}

		private void RestoreWindowState()
		{
			WindowState = FormWindowState.Normal;
			StartPosition = FormStartPosition.WindowsDefaultBounds;
			XMLProfileSettings xml = new XMLProfileSettings();

			var desktopBounds =
				new Rectangle(
					new Point(
						xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationX", Name), Location.X),
						xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationY", Name), Location.Y)),
					new Size(
						xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowWidth", Name), Size.Width),
						xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowHeight", Name), Size.Height)));

			if (desktopBounds.Width < 300)
			{
				if (gdiControl.Background != null && gdiControl.Background.Width > 300)
					desktopBounds.Width = gdiControl.Background.Width;
				else
					desktopBounds.Width = 400;
			}

			if (desktopBounds.Height < 200)
			{
				if (gdiControl.Background != null && gdiControl.Background.Height > 200)
					desktopBounds.Height = gdiControl.Background.Height;
				else
					desktopBounds.Height = 300;
			}

			if (IsVisibleOnAnyScreen(desktopBounds))
			{
				StartPosition = FormStartPosition.Manual;
				DesktopBounds = desktopBounds;

				if (xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowState", Name), "Normal").Equals("Maximized"))
				{
					WindowState = FormWindowState.Maximized;
				}
			}
			else
			{
				// this resets the upper left corner of the window to windows standards
				StartPosition = FormStartPosition.WindowsDefaultLocation;

				// we can still apply the saved size
				Size = new Size(desktopBounds.Width,desktopBounds.Height);
			}

			
		}
	}
}
