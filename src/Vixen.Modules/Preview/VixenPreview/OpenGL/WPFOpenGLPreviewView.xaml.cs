using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using Common.Broadcast;

using Common.Controls;

using OpenTK.Wpf;


namespace VixenModules.Preview.VixenPreview.OpenGL
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	/// 
	/// <remarks>
	/// The OpenGL view is refreshed by invalidating the GLWpfControl visual which fires the (GLWpfControl) OnRender event.
	/// The OnRender event is the single method or entry point to draw the view.
	/// 
	/// Most of this code originated from OpenGLPreviewForm.cs.  The OpenGLPreviewForm used MakeCurrent and SwapBuffers.
	/// 
	/// The GLWpfControl control has similar methods hanging off the Context.  Informal testing revealed that the Preview
	/// worked with and without these calls, so the calls have been omitted since they did not seem required.
	/// 
	/// The GLWpfControl has a flag named RenderContinuously.  This flag defaults to true but this resulted in refreshing
	/// view at a periodic rate.  This flag has been set to false to allow Vixen to control the refresh rate.
	/// 
	/// </remarks>
	public partial class WPFOpenGLPreviewView : Window, IDisplayForm, IDisposable
	{
		#region Fields
		
		/// <summary>
		/// Previous X position of the mouse.
		/// </summary>
		private int _prevMousePositionX;

		/// <summary>
		/// Previous Y position of the mouse.
		/// Used to short circuit the mouse move event.
		/// </summary>
		private int _prevMousePositionY;

		/// <summary>
		/// Flag that is true while the mouse is down.
		/// Used to short circuit the mouse move event.
		/// </summary>
		private bool _mouseDown;

		/// <summary>
		/// View model associated with the view.
		/// </summary>
		private WPFOpenGLPreviewViewModel _viewModel;

		#endregion

		#region Private Properties
		
		/// <summary>
		/// Flag indicating if state  has been restored.		
		/// </summary>
		/// <remarks>This flag is necessary to prevent the settings file from getting overwritten before it has been read in.</remarks>
		private bool StateRestored { get; set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">Vixen Preview Data</param>
		/// <param name="instanceId">Instance ID of the preview</param>
		/// <param name="viewModel">View model associated with the view</param>
		public WPFOpenGLPreviewView(
			VixenPreviewData data, 
			Guid instanceId, 
			WPFOpenGLPreviewViewModel viewModel)
		{
			// Store off the view model
			_viewModel = viewModel;

			// Set a delegate on the view model to refresh the frame
			viewModel.DrawingEngine.OnRenderFrameOnGUIThread = RenderFrameOnGUIThread;

			// Set the data context on the view
			DataContext = viewModel;
			
			// Store off the instance ID of the preview
			InstanceId = instanceId;

			// Register for property changes on the view model
			viewModel.PropertyChanged += ViewModel_PropertyChanged; 
									
			InitializeComponent();

			// Selects OpenGL version 3.3
			GLWpfControlSettings settings = new GLWpfControlSettings
			{
				MajorVersion = 3,
				MinorVersion = 3,
				RenderContinuously = false,
			};

			// Sets up the OpenGL context and prepares the control to render OpenGL content 
			OpenTkControl.Start(settings);			
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Event handler for when a view model property has changed.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			// Flag which indicates if the preview settings need to be updated
			bool saveWindowState = false;

			// Determine which property was modified
			switch(e.PropertyName)
			{
				// If the property that changed is the AlwayOnTop property then...
				case nameof(WPFOpenGLPreviewViewModel.AlwaysOnTop):
					{
						// Configure whether the window should always be on top
						Topmost = _viewModel.AlwaysOnTop;

						// If the AlwaysOnTop property is true then...
						if (_viewModel.AlwaysOnTop)
						{
							// Turn off the auto on top when playing property
							IsOnTopWhenPlaying = false;
						}

						// Set flag to update the settings
						saveWindowState = true;
					}
					break;
				case nameof(WPFOpenGLPreviewViewModel.AutoOnTop):
					{
						// Set the Auto On Top property that is part of IDisplayForm
						IsOnTopWhenPlaying = _viewModel.AutoOnTop;

						// Set flag to update the settings
						saveWindowState = true;
					}
					break;
				case nameof(WPFOpenGLPreviewViewModel.ClientSize):
					{
						// Resize the window based on the desired size from the view model
						Width = _viewModel.ClientSize.Width;
						Height = _viewModel.ClientSize.Height;
						
						// Fore OpenTK control to render
						OpenTkControl.InvalidateVisual();

						// Set flag to update the settings
						saveWindowState = true;
					}
					break;
				case nameof(WPFOpenGLPreviewViewModel.EnableLightScaling):
					{
						// Fore OpenTK control to render
						OpenTkControl.InvalidateVisual();

						// Set flag to update the settings
						saveWindowState = true;
					}
					break;
			}

			// If the window state needs to saved then...
			if (saveWindowState)
			{
				// Save the window state settings
				SaveWindowState();
			}
		}

		/// <summary>
		/// This is the main render method for the OpenTK WPF control.
		/// </summary>
		/// <param name="delta">Time between render calls</param>
		private void OpenTkControl_OnRender(TimeSpan delta)
		{									
			// If the window is minimized then short circuit the render
			if (WindowState != WindowState.Minimized)
			{
				// Have the drawing engine refresh the frame
				_viewModel.DrawingEngine.RenderPreview();
			}			
		}

		/// <summary>
		/// Invalidates the OpenTkControl to force the control fire the render event.
		/// </summary>
		private void RenderFrameOnGUIThread()
		{
			// Render the preview on the GUI thread
			OpenTkControl.Dispatcher.Invoke(() =>
			{
				// This should trigger the control to redraw
				OpenTkControl.InvalidateVisual();
			});
		}

		/// <summary>
		/// Window loaded event.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{			
			// Retrieve the camera position
			XMLProfileSettings xml = new XMLProfileSettings();
			var name = $"OpenGLPreview_{InstanceId}";
			float cameraX = 0.0f;
			float cameraY = 0.0f;
			float cameraZ = 0.0f;
			cameraX = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CameraPositionX", name), cameraX);
			cameraY = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CameraPositionY", name), cameraY);
			cameraZ = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CameraPositionZ", name), cameraZ);

			// Initialize the drawing engine with the camera position and size of the drawing area
			_viewModel.DrawingEngine.Initialize(				
				cameraX,
				cameraY,
				cameraZ);
			
			// Disable the X button on the title bar
			DisableCloseButtonInTitleBar();

			// Restore the Preview settings
			RestoreWindowState();
		}

		/// <summary>
		/// Event when the OpenTK control changes sizes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{			
			// Forward the call to the drawing engine
			_viewModel.DrawingEngine.OpenTKDrawingAreaChanged(e.NewSize.Width, e.NewSize.Height);
				
			// Save the window state
			SaveWindowState();			
		}

		/// <summary>
		/// Event when the Mouse wheel is moved over the OpenTK control.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControl_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			int factor = 20;
			if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
			{
				factor = 5;
			}
			int direction = -(e.Delta * SystemInformation.MouseWheelScrollLines / factor);

			// Update the zoom of the preview
			_viewModel.DrawingEngine.Zoom(direction);

			// This should trigger the control to redraw
			OpenTkControl.InvalidateVisual();

			// Save the window state
			SaveWindowState();
		}

		/// <summary>
		/// Event when the mouse down button is pressed over the OpenTK control.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControl_MouseDown(object sender, MouseButtonEventArgs e)
		{
			// If the left button has been pressed then...
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				// this method gets called whenever a new mouse button event happens
				_mouseDown = true;

				// if the mouse has just been clicked then we hide the cursor and store the position
				Cursor = System.Windows.Input.Cursors.Hand;

				// Store off the mouse position
				_prevMousePositionX = (int)e.GetPosition(this).X;
				_prevMousePositionY = (int)e.GetPosition(this).Y;
			}
		}

		/// <summary>
		/// Event when the mouse is moved over the OpenTK control.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			// Retrieve the position of the mouse as an integer
			int eX = (int)e.GetPosition(this).X;
			int eY = (int)e.GetPosition(this).Y;

			// If the mouse has not moved significantly then exit
			if (eX == _prevMousePositionX && eY == _prevMousePositionY) return;

			// If the Left mouse button is down then...			
			if (_mouseDown && e.LeftButton == MouseButtonState.Pressed)
			{
				// Move the view camera 
				_viewModel.DrawingEngine.MoveCamera(_prevMousePositionX, _prevMousePositionY, eX, eY);
				
				// Save off the mouse position
				_prevMousePositionX = eX;
				_prevMousePositionY = eY;

				// This should trigger the control to redraw
				OpenTkControl.InvalidateVisual();
			}
		}

		/// <summary>
		/// Event when mouse is released over the OpenTK control.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControl_MouseUp(object sender, MouseButtonEventArgs e)
		{
			// Restore the mouse cursor
			Cursor = System.Windows.Input.Cursors.Arrow;
			
			// Set flag to remember that the mouse button is no longer being pressed
			_mouseDown = false;

			// Save the window state
			SaveWindowState();
		}

		/// <summary>
		/// Event for when the window size changed.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			// Update the view model with the position of the upper left corner
			_viewModel.Left = Left.ToString();
			_viewModel.Top = Top.ToString();
				
			// Update the view model with the new size
			_viewModel.Width = $"{e.NewSize.Width}";
			_viewModel.Height = $"{e.NewSize.Height}";			
		}

		/// <summary>
		/// Event for when the window location changed.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void Window_LocationChanged(object sender, EventArgs e)
		{
			// Update the view model with the position of the upper left corner
			_viewModel.Left = Left.ToString();
			_viewModel.Top = Top.ToString();			
		}

		/// <summary>
		/// Event for when the user attempts to close the Preview window.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Display a message box indicating that the Preview can only be closed from the Configuration dialog
			MessageBoxForm messageBox = new MessageBoxForm("The preview can only be closed from the Preview Configuration dialog.", "Close", MessageBoxButtons.OK, SystemIcons.Information);
			messageBox.ShowDialog();
			e.Cancel = true;
		}

		/// <summary>
		/// Returns true if the specified rectangle is visible on a screen.
		/// </summary>
		/// <param name="rect">Rectangle to analize</param>
		/// <returns>True if the specified rectangle is visible on a screen</returns>
		private bool IsVisibleOnAnyScreen(Rectangle rect)
		{
			return Screen.AllScreens.Any(screen => screen.WorkingArea.Contains(rect.Location)) ||
				   Screen.AllScreens.Any(screen => screen.WorkingArea.Contains(new System.Drawing.Point(rect.Top, rect.Right)));
		}

		/// <summary>
		/// Finds the maximum size of the preview based on the position of the lights.
		/// </summary>
		/// <returns>Size of the preview based on position of the lights</returns>
		private System.Drawing.Size FindMaxPreviewSize()
		{
			int bottom = 0;
			int right = 0;
			foreach (var dataDisplayItem in Data.DisplayItems)
			{
				bottom = Math.Max(bottom, dataDisplayItem.Shape.Bottom);
				right = Math.Max(right, dataDisplayItem.Shape.Right);
			}

			return new System.Drawing.Size(right, bottom);
		}

		/// <summary>
		/// Disable the Close Button in the title bar.
		/// </summary>
		private void DisableCloseButtonInTitleBar()
		{
			var hwnd = new WindowInteropHelper(this).Handle;
			IntPtr hMenu = GetSystemMenu(hwnd, false);
			EnableMenuItem(hMenu, SC_CLOSE, MF_GRAYED);
		}

		/// <summary>
		/// Saves window state to the XML settings file.
		/// </summary>
		private void SaveWindowState()
		{
			// If the state has been restored then it is safe to update the saved state
			if (StateRestored)
			{
				XMLProfileSettings xml = new XMLProfileSettings();
				var name = $"OpenGLPreview_{InstanceId}";

				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ClientHeight", name), Height);
				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ClientWidth", name), Width);
				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationX", name), Left);
				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationY", name), Top);
				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CameraPositionX", name), _viewModel.DrawingEngine.Camera.Position.X);
				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CameraPositionY", name), _viewModel.DrawingEngine.Camera.Position.Y);
				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/CameraPositionZ", name), _viewModel.DrawingEngine.Camera.Position.Z);
				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ShowStatus", name), _viewModel.ShowStatusChecked);
				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/AlwaysOnTop", name), _viewModel.AlwaysOnTop);
				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/EnableLightScaling", name), _viewModel.DrawingEngine.EnableLightScaling);
				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/OnTopWhenActive", name), IsOnTopWhenPlaying);
				xml.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowState", name), (int)WindowState);
			}
		}

		/// <summary>
		/// Restores window state.
		/// </summary>
		private void RestoreWindowState()
		{						
			XMLProfileSettings xml = new XMLProfileSettings();
			string name = $"OpenGLPreview_{InstanceId}";

			_viewModel.ShowStatusChecked = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ShowStatus", name), true);
			_viewModel.AlwaysOnTop = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/AlwaysOnTop", name), false);
			_viewModel.EnableLightScaling = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/EnableLightScaling", name), true);
			_viewModel.AutoOnTop = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/OnTopWhenActive", name), false);
			
			// Default the Window position to the center of the screen
			WindowStartupLocation = WindowStartupLocation.CenterScreen;

			// Retrive the location of the Preview from the settings file
			Rectangle desktopBounds =
				new System.Drawing.Rectangle(
					new System.Drawing.Point(
						xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationX", name), (int)Left),
						xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowLocationY", name), (int)Top)),
					new System.Drawing.Size(100, 100));

			// If the saved off Window coordinates are visible on a screen then...
			if (IsVisibleOnAnyScreen(desktopBounds))
			{
				// Tell WPF we manually want to control the startup location
				this.WindowStartupLocation = WindowStartupLocation.Manual;

				// Set the position of the Window
				Left = desktopBounds.Left;
				Top = desktopBounds.Top;
			}
			else
			{
				// Otherwise just center the preview on the screen
				WindowStartupLocation = WindowStartupLocation.CenterScreen;
			}
				
			// Retrieve the Width and Height of the Window from the settings XML file
			int cHeight = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ClientHeight", name),
				_viewModel.DrawingEngine.HasBackgroundImage() ? _viewModel.DrawingEngine.Background.Height : 0);
			int cWidth = xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ClientWidth", name),
				_viewModel.DrawingEngine.HasBackgroundImage() ? _viewModel.DrawingEngine.Background.Width : 0);

			// If the Height and Width were not defined in the settings file then...
			if (cHeight == 0 && cWidth == 0)
			{
				// Find the maximum size of the preview based on the light positions
				System.Drawing.Size size = FindMaxPreviewSize();
				
				// If the light positions are greater than 50x50 then...
				if (size.Height > 50 && size.Width > 50)
				{
					// Set with window size based on pixel locations
					cHeight = size.Height;
					cWidth = size.Width;
				}
				else
				{
					// Otherwise just use the default from the drawing engine
					cHeight = 600;
					cWidth = 800;
				}
			}

			// Set the width and height of the window
			Width = cWidth;
			Height = cHeight;
	
			// Restore the window state
			WindowState = (WindowState)xml.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/WindowState", name), (int)WindowState.Normal);

			// Set a flag to indicate that the state has been restored
			StateRestored = true;			
		}

		/// <summary>
		/// Event for when a key down with respect to the window.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.I || e.Key == Key.O)
			{
				int factor = 20;

				if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
				{
					factor = 5;
				}

				int delta = 120;
				if (e.Key != Key.I)
				{
					delta = -delta;
				}

				int direction = -(delta * SystemInformation.MouseWheelScrollLines / factor);
				
				_viewModel.DrawingEngine.Zoom(direction);

				// This should trigger the control to redraw
				OpenTkControl.InvalidateVisual();

				SaveWindowState();
			}
			else
			{				
				Broadcast.Publish<System.Windows.Input.KeyEventArgs>("KeydownSWF", e);
			}
		}

		/// <summary>
		/// Update the title bar name.
		/// </summary>
		private void UpdateWindowTitle()
		{
			// Update the title in the Window Title Bar
			Title = _displayName;
		}

		#endregion

		#region IDisplayForm
		
		/// <inheritdoc/>
		public VixenPreviewData Data 
		{
			get
			{
				return _viewModel.DrawingEngine.Data;
			}
			set
			{
				_viewModel.DrawingEngine.Data = value;
			}				
		}

		private string _displayName = "Vixen Preview";

		/// <inheritdoc/>
		public string DisplayName 
		{ 
			get
			{
				return _displayName;
			}
			set
			{
				_displayName = value;
								
				Dispatcher.Invoke(UpdateWindowTitle);				
			}
		}

		/// <inheritdoc/>
		public Guid InstanceId { get; set; }

		/// <inheritdoc/>
		public bool IsOnTopWhenPlaying 
		{
			get;
			private set;			
		}

		/// <inheritdoc/>
		public void Setup()
		{
			// Setup the Preview by determining the number of lights or pixels displayed
			_viewModel.PixelCount = _viewModel.DrawingEngine.Setup();									 
		}

		/// <inheritdoc/>
		public void UpdatePreview()
		{
			// If the preview is stale then...
			if (_viewModel.DrawingEngine.IsPreviewStale() || _viewModel.DrawingEngine.NeedsUpdate)
			{
				// Indicate this is a standard frame vs responding to a (moving head) update event
				_viewModel.DrawingEngine.StandardFrame = true;

				// This should trigger the control to redraw
				OpenTkControl.InvalidateVisual();				
			}			
		}

		#endregion

		#region Win32 Bindings

		const uint MF_GRAYED = 0x00000001;
		const uint MF_ENABLED = 0x00000000;
		const uint SC_CLOSE = 0xF060;

		// Win32 Bindings needed to Disable the X button in the title bar			
		[DllImport("user32.dll")]
		static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
		[DllImport("user32.dll")]
		static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

		#endregion

		#region IDisposable

		/// <inheritdoc/>
		public void Dispose()
		{
			// If the view model is NOT null then...
			if (_viewModel != null)
			{
				// Dispose of the view model
				_viewModel.Dispose();
				_viewModel = null;

				// Dispose of the OpenTkControl
				OpenTkControl.Dispose();
				OpenTkControl = null;
			}
		}

		#endregion				
	}
}