using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;
using Vixen;
using Vixen.Sys;
using Vixen.Sys.Instrumentation;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Preview.VixenPreview.GDIPreview
{
	public partial class GDIPreviewForm : BaseForm, IDisplayForm
	{
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private bool _needsUpdate = true;
		private bool _formLoading = true;
		private static MillisecondsValue _previewSetPixelsTime;
		private Stopwatch _sw = Stopwatch.StartNew();
		private Size? _mouseGrabOffset;
		private readonly ContextMenuStrip _contextMenuStrip = new ContextMenuStrip();
		private string _displayName = "Vixen Preview";
		private bool _showBorders;
		private bool _showStatus;
		private bool _alwaysOnTop;
		private bool _lockPosition;

		public GDIPreviewForm(VixenPreviewData data, Guid instanceId)
		{
			Icon = Resources.Icon_Vixen3;
			InitializeComponent();
			Data = data;
			InstanceId = instanceId;
			gdiControl.Margin = Padding.Empty;
			gdiControl.Padding = Padding.Empty;
			gdiControl.MouseMove += GdiControl_MouseMove;
			gdiControl.MouseUp += GdiControl_MouseUp;
			gdiControl.MouseDown += GdiControl_MouseDown;
			gdiControl.MouseWheel += GdiControl_MouseWheel;
			gdiControl.KeyDown += GdiControl_KeyDown;
			double scaleFactor = ScalingTools.GetScaleFactor();
			_contextMenuStrip.Renderer = new ThemeToolStripRenderer();
			int imageSize = (int)(16 * scaleFactor);
			_contextMenuStrip.ImageScalingSize = new Size(imageSize, imageSize);
			UpdateDisplayName();
			_previewSetPixelsTime = new MillisecondsValue("Preview pixel set time");
			VixenSystem.Instrumentation.AddValue(_previewSetPixelsTime);
		}

		private void ConfigureAlwaysOnTop()
		{
			if (_alwaysOnTop)
			{
				TopMost = true;
			}
			else
			{
				TopMost = false;
			}
		}

		private void ConfigureBorders()
		{
			if (!_showBorders)
			{
				FormBorderStyle = FormBorderStyle.None;
			}
			else
			{
				FormBorderStyle = FormBorderStyle.Sizable;
			}
		}

		private void ConfigureStatusBar()
		{
			if (!_showStatus)
			{
				statusStrip.Visible = false;
			}
			else
			{
				statusStrip.Visible = true;
			}
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

		private static double DistanceFromPoint(Point origin, Size point)
		{
			return Math.Sqrt(Math.Pow((point.Width - origin.X), 2) + Math.Pow((point.Height - origin.Y), 2));
		}

		private Size BorderOffset()
		{
			var screenRectangle = RectangleToScreen(ClientRectangle);
			return new Size(screenRectangle.Left - Left, screenRectangle.Top - Top);
		}

		private void GdiControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (_mouseGrabOffset.HasValue && DistanceFromPoint(e.Location, _mouseGrabOffset.Value) > 2)
			{
				if (_showBorders)
				{
					
					this.Location = Cursor.Position - _mouseGrabOffset.Value - BorderOffset();
				}
				else
				{
					this.Location = Cursor.Position - _mouseGrabOffset.Value;
				}
				
			}
		}

		private void GdiControl_MouseUp(object sender, MouseEventArgs e)
		{
			_mouseGrabOffset = null;
		}

		private void GdiControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (!_lockPosition && e.Button == MouseButtons.Left)
			{
				_mouseGrabOffset = new Size(e.Location);
			}
			else if(e.Button == MouseButtons.Right)
			{
				HandleContextMenu();
			}
		}

		private void GdiControl_MouseWheel(object sender, MouseEventArgs e)
		{
			double delta = e.Delta / 1500d;

			// Zoom to the pointer location
			//zoomTo = MousePointToZoomPoint(e.Location);

			ZoomLevel += delta;
		}

		private void GdiControl_KeyDown(object sender, KeyEventArgs e)
		{

			if (e.KeyCode == Keys.I || e.KeyCode == Keys.O)
			{
				double factor = 1;
				if ((ModifierKeys & Keys.Shift) == Keys.Shift)
				{
					factor = 2;
				}

				double delta = .05;
				if (e.KeyCode != Keys.I)
				{
					delta = -delta;
				}

				ZoomLevel = ZoomLevel + delta * factor;

			}

		}

		private void HandleContextMenu()
		{
			_contextMenuStrip.Items.Clear();

			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			
			var item = new ToolStripMenuItem("Show Status");
			item.ToolTipText = @"Enable/Disable the preview status bar.";

			if (_showStatus)
			{
				item.Image = Tools.GetIcon(Resources.check_mark, iconSize); ;
			}

			item.Click += (sender, args) =>
			{
				_showStatus = !_showStatus;
				ConfigureStatusBar();
				SaveWindowState();
			};

			_contextMenuStrip.Items.Add(item);

			item = new ToolStripMenuItem("Show Borders");
			item.ToolTipText = @"Enable/Disable the preview borders.";

			if (_showBorders)
			{
				item.Image = Tools.GetIcon(Resources.check_mark, iconSize); ;
			}

			item.Click += (sender, args) =>
			{
				_showBorders = !_showBorders;
				ConfigureBorders();
				SaveWindowState();
			};

			_contextMenuStrip.Items.Add(item);

			item = new ToolStripMenuItem("Lock Position");
			item.ToolTipText = @"Enable/Disable the window position lock.";

			if (_lockPosition)
			{
				item.Image = Tools.GetIcon(Resources.check_mark, iconSize); ;
			}

			item.Click += (sender, args) =>
			{
				_lockPosition = !_lockPosition;
				SaveWindowState();
			};

			_contextMenuStrip.Items.Add(item);

			item = new ToolStripMenuItem("Always On Top");
			item.ToolTipText = @"Enable/Disable the window always on top.";

			if (_alwaysOnTop)
			{
				item.Image = Tools.GetIcon(Resources.check_mark, iconSize); ;
			}

			item.Click += (sender, args) =>
			{
				_alwaysOnTop = !_alwaysOnTop;
				ConfigureAlwaysOnTop();
				SaveWindowState();
			};

			_contextMenuStrip.Items.Add(item);

			item = new ToolStripMenuItem("Auto On Top");
			item.ToolTipText = @"Enable/Disable bringing this preview on top and back automatically.";

			if (IsOnTopWhenPlaying)
			{
				item.Image = Tools.GetIcon(Resources.check_mark, iconSize); ;
			}

			item.Click += (sender, args) =>
			{
				IsOnTopWhenPlaying = !IsOnTopWhenPlaying;
				if (IsOnTopWhenPlaying)
				{
					_alwaysOnTop = false;
					ConfigureAlwaysOnTop();
				}
				else if (TopMost)
				{
					TopMost = false;
				}

				SaveWindowState();
			};

			_contextMenuStrip.Items.Add(item);


			item = new ToolStripMenuItem("Reset Size");
			item.ToolTipText = @"Resets the viewable size to match the background size.";

			item.Click += (sender, args) =>
			{
				Size s = gdiControl.BackgroundSize;
				ClientSize = new Size(s.Width, s.Height+ (statusStrip.Visible?statusStrip.Height:0));
				SaveWindowState();
			};

			_contextMenuStrip.Items.Add(item);

			item = new ToolStripMenuItem("Reset Zoom");
			item.ToolTipText = @"Resets the Zoom to 100%.";

			item.Click += (sender, args) =>
			{
				ZoomLevel = 1;
				SaveWindowState();
			};

			_contextMenuStrip.Items.Add(item);

			var seperator = new ToolStripSeparator();
			_contextMenuStrip.Items.Add(seperator);

			var locationLabel = new ToolStripLabel(string.Format("Location: {0},{1}", DesktopLocation.X, DesktopLocation.Y));
			_contextMenuStrip.Items.Add(locationLabel);

			var sizeLabel = new ToolStripLabel(string.Format("Size: {0} X {1}", ClientSize.Width, ClientSize.Height));
			_contextMenuStrip.Items.Add(sizeLabel);

			_contextMenuStrip.Show(MousePosition);
		}

		private bool AreCornersVisibleOnAnyScreen(Rectangle rect)
		{
			return Screen.AllScreens.Any(screen => screen.WorkingArea.Contains(rect.Location)) ||
			       Screen.AllScreens.Any(screen => screen.WorkingArea.Contains(new Point(rect.Top, rect.Right)));
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
					if (_needsUpdate)
					{
						_needsUpdate = false;
						gdiControl.BeginUpdate();
						gdiControl.EndUpdate();
						gdiControl.Invalidate();
					}

					toolStripStatusFPS.Text = "0 fps";
					return;
				}

				_needsUpdate = true;

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
						pixel.Draw(gdiControl.FastPixel, element.State, ZoomLevel);
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

			//Data.Top = Top;
			//Data.Left = Left;
			if(!_formLoading) SaveWindowState();
		}

		private void GDIPreviewForm_Resize(object sender, EventArgs e)
		{
			if (Data == null)
			{
				Logging.Warn("VixenPreviewDisplay_Resize: Data is null. abandoning resize. (Thread ID: " +
											System.Threading.Thread.CurrentThread.ManagedThreadId + ")");
				return;
			}

			//Data.Width = Width;
			//Data.Height = Height;
			if (!_formLoading) SaveWindowState();
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
			gdiControl.MouseMove -= GdiControl_MouseMove;
			gdiControl.MouseUp -= GdiControl_MouseUp;
			gdiControl.MouseDown -= GdiControl_MouseDown;

			SaveWindowState();
		}

		private void SaveWindowState()
		{
			XMLProfileSettings xml = new XMLProfileSettings();
			var name = $"Preview_{InstanceId}";

			if (WindowState != FormWindowState.Minimized)
			{
				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowHeight", name), Size.Height);
				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowWidth", name), Size.Width);
				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationX", name), Location.X);
				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationY", name), Location.Y);
			}
			
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowState", name),
				WindowState.ToString());
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ShowStatus", name), _showStatus);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ShowBorders", name), _showBorders);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/AlwaysOnTop", name), _alwaysOnTop);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/LockPosition", name), _lockPosition);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ZoomLevel", name), ZoomLevel);
			xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/OnTopWhenActive", name), IsOnTopWhenPlaying);
		}

		private void GDIPreviewForm_Load(object sender, EventArgs e)
		{
			_formLoading = true;
			RestoreWindowState();
			_formLoading = false;
		}

		private void RestoreWindowState()
		{
			WindowState = FormWindowState.Normal;
			StartPosition = FormStartPosition.WindowsDefaultBounds;
			XMLProfileSettings xml = new XMLProfileSettings();
			var name = $"Preview_{InstanceId}";

			_showStatus = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ShowStatus", name), true);
			_showBorders = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ShowBorders", name), true);
			_alwaysOnTop = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/AlwaysOnTop", name), false);
			_lockPosition = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/LockPosition", name), false);
			ZoomLevel = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ZoomLevel", name), 1d);
			IsOnTopWhenPlaying = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/OnTopWhenActive", name), false);

			ConfigureStatusBar();
			ConfigureBorders();
			ConfigureAlwaysOnTop();

			var desktopBounds =
				new Rectangle(
					new Point(
						xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationX", name), Location.X),
						xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationY", name), Location.Y)),
					new Size(
						xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowWidth", name), 0),
						xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowHeight", name), 0)));

			if (desktopBounds.Width < 20)
			{
				if (gdiControl.Background != null && gdiControl.Background.Width > 20)
					desktopBounds.Width = gdiControl.Background.Width + Width-ClientSize.Width;
				else
					desktopBounds.Width = 400;
			}

			if (desktopBounds.Height < 10)
			{
				if (gdiControl.Background != null && gdiControl.Background.Height > 10)
					desktopBounds.Height = gdiControl.Background.Height + Height-ClientSize.Height + (statusStrip.Visible?statusStrip.Height:0);
				else
					desktopBounds.Height = 300;
			}

			var windowState = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowState", name), "Normal");
			
			if (windowState.Equals("Maximized") && IsVisibleOnAnyScreen(desktopBounds))
			{
				StartPosition = FormStartPosition.Manual;
				DesktopLocation = desktopBounds.Location;
				WindowState = FormWindowState.Maximized;
			}
			else if (AreCornersVisibleOnAnyScreen(desktopBounds))
			{
				StartPosition = FormStartPosition.Manual;
				DesktopBounds = desktopBounds;
			}
			else
			{
				// this resets the upper left corner of the window to windows standards
				StartPosition = FormStartPosition.WindowsDefaultLocation;

				// we can still apply the saved size
				Size = new Size(desktopBounds.Width,desktopBounds.Height);
			}

			
		}

		public string DisplayName
		{
			get { return _displayName; }
			set
			{
				_displayName = value;
				if (InvokeRequired)
				{
					Invoke(new Delegates.GenericDelegate(UpdateDisplayName));
				}
				else
				{
					UpdateDisplayName();
				}
				
			}
		}

		/// <inheritdoc />
		public Guid InstanceId { get; set; }

		/// <inheritdoc />
		public bool IsOnTopWhenPlaying { get; private set; }

		public void UpdateDisplayName()
		{
			Text = _displayName;
		}

		private double _zoomLevel = 1;

		public double ZoomLevel
		{
			get { return _zoomLevel; }
			set
			{
				double ZoomMax = 2;

				const double ZoomMin = .25;

				if (value >= ZoomMin && value <= ZoomMax)
					_zoomLevel = value;
				else if (value < ZoomMin)
					_zoomLevel = ZoomMin;
				else if (value > ZoomMax)
					_zoomLevel = ZoomMax;

				//if (DisplayItems != null)
				//{
				//	foreach (DisplayItem item in DisplayItems)
				//	{
				//		item.ZoomLevel = _zoomLevel;
				//	}
				//}

				gdiControl.ZoomLevel = _zoomLevel;
				gdiControl.CreateAlphaBackground();
			}
		}

	}
}
