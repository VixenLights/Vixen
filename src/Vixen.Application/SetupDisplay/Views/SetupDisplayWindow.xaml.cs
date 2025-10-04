using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Common.Controls;

using OpenTK.Graphics.OpenGL;
using OpenTK.Wpf;

using Orc.Theming;

using Vixen.Sys;
using Vixen.Sys.Managers;

using VixenApplication.SetupDisplay.ViewModels;

using VixenModules.App.Modeling;
using VixenModules.Editor.FixtureGraphics.WPF;

using WPFCommon.Extensions;

using Timer = System.Threading.Timer;


namespace VixenApplication.SetupDisplay.Views
{
	public partial class SetupDisplayWindow: Window
	{
		#region Fields

		private readonly Timer _nodeRefreshTimer;

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
		/// Moving head.
		/// </summary>
		private MovingHeadWPF _movingHead = new MovingHeadWPF();

		#endregion

		public SetupDisplayWindow()
		{
            ThemeManager.Current.SynchronizeTheme();
			InitializeComponent();
            Icon = Common.Resources.Properties.Resources.Icon_Vixen3.ToImageSource();
            DataContext = new SetupDisplayViewModel();
            _nodeRefreshTimer = new Timer(Timer_Elapsed, null, 2000, Timeout.Infinite);
            NodeManager.NodesChanged += NodeManager_NodesChanged;
            ElementTree.ExportDiagram = ExportWireDiagram;
			
			// Selects OpenGL version 3.3
			GLWpfControlSettings settings = new GLWpfControlSettings
			{
				MajorVersion = 3,
				MinorVersion = 3,
				RenderContinuously = true,
			};

			// Sets up the OpenGL context and prepares the control to render OpenGL content 
			OpenTkControl.Start(settings);
		}

        private void Timer_Elapsed(object? state)
        {
	        if (ElementTree.InvokeRequired)
	        {
		        ElementTree.Invoke(ElementTree.PopulateNodeTree);
	        }
			Debug.WriteLine("Refreshed");
		}

		private void NodeManager_NodesChanged(object? sender, EventArgs e)
		{
			_nodeRefreshTimer.Change(2000, Timeout.Infinite);
		}

		private void ExportWireDiagram(ElementNode node, bool flip = false)
		{
			ElementModeling.ElementsToSvg(node, flip);
		}

		#region Private Methods

		/// <summary>
		/// User control loaded event, used to draw background graphics.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e"><Event arguments/param>
		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			// Draw moving head 
			DrawMovingHead((int)Math.Min(MainViewport.ActualWidth, MainViewport.ActualHeight), System.Drawing.Color.Purple, MainViewport);

			// Position the camera at the origin
			float cameraX = 0.0f;
			float cameraY = 0.0f;
			float cameraZ = 0.0f;
			
			// Initialize the drawing engine with the camera position and size of the drawing area
			(DataContext as SetupDisplayViewModel).DrawingEngine.Initialize(			
				cameraX,
				cameraY,
				cameraZ);
		}

		#endregion

		/// <summary>
		/// Displays two moving head fixtures on the user control using the specified viewports.
		/// </summary>
		/// <param name="size">Width and height of the moving head</param>
		/// <param name="color">Color of the light beam</param>
		/// <param name="leftViewport">Left viewport to render in</param>
		/// <param name="rightViewport">Right viewport to render in</param>
		protected void DrawMovingHead(
			int size,
			System.Drawing.Color color,		
			Viewport3D viewport)
		{
			// Configure the left moving head
			_movingHead.MovingHead.IncludeLegend = false;
			_movingHead.MovingHead.TiltAngle = 45.0;
			_movingHead.MovingHead.PanAngle = 35.0;
			_movingHead.MovingHead.BeamColorLeft = color;
			_movingHead.MovingHead.BeamColorRight = color;
			_movingHead.MovingHead.BeamLength = 20;
			_movingHead.MovingHead.Focus = 20;

			// Draw the moving head
			_movingHead.DrawFixtureNoBitmap(size, size, 1.0, 0, 0, viewport);

			(DataContext as SetupDisplayViewModel).IsThreeD = false;			
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
				(DataContext as SetupDisplayViewModel).DrawingEngine.RenderPreview();				
			}
		}
		
		/// <summary>
		/// Event when the OpenTK control changes sizes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			// Forward the call to the drawing engine
			(DataContext as SetupDisplayViewModel).DrawingEngine.OpenTKDrawingAreaChanged(e.NewSize.Width, e.NewSize.Height);

			var dpiScale = VisualTreeHelper.GetDpi(OpenTkControl);
			int pixelWidth = (int)(OpenTkControl.ActualWidth * dpiScale.DpiScaleX);
			int pixelHeight = (int)(OpenTkControl.ActualHeight * dpiScale.DpiScaleY);

			GL.Viewport(0, 0, pixelWidth, pixelHeight);			
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
			(DataContext as SetupDisplayViewModel).DrawingEngine.Zoom(direction);

			// This should trigger the control to redraw
			OpenTkControl.InvalidateVisual();			
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
				(DataContext as SetupDisplayViewModel).DrawingEngine.MoveCamera(_prevMousePositionX, _prevMousePositionY, eX, eY);

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

	}
}
