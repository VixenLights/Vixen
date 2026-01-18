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
	/// <summary>
	/// Code Behind file for the SetupDisplayWindow.
	/// This class handles events for the Setup Display Window.
	/// </summary>
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

		/// <summary>
		/// This flag helps ensure the OpenGL drawing engine associated with the control
		/// is only initialized once.
		/// </summary>
		bool _openTKControlLoaded = false;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
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
			// Start the Prop Preview OpenTK control
			OpenTkControl.Start(settings);

			// Start the Display Preview OpenTK control						
			OpenTkControlPreview.Start(settings);
		}

		#endregion

		#region Private Methods

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
				
		/// <summary>
		/// Event handler for the preview setup.
		/// </summary>
		/// <param name="sender">Sender of the event</param>
		/// <param name="e">Event arguments</param>
		void OpenTkControlPreview_Loaded(object sender, System.EventArgs e)
		{
			// Remember that the preview setup OpenTK control has been loaded
			_openTKControlLoaded = true;			
		}

		/// <summary>
		/// User control loaded event, used to draw background graphics.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e"><Event arguments/param>
		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{			
			// Position the camera at the origin
			float cameraX = 0.0f;
			float cameraY = 0.0f;
			float cameraZ = 0.0f;
			
			// Initialize the drawing engine with the camera position and size of the drawing area
			(DataContext as SetupDisplayViewModel).PropPreviewDrawingEngine.Initialize(			
				cameraX,
				cameraY,
				cameraZ);
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
				// Draw the selected prop
				(DataContext as SetupDisplayViewModel).DrawProp();

				// Have the drawing engine refresh the frame
				(DataContext as SetupDisplayViewModel).PropPreviewDrawingEngine.RenderPreview();				
			}
		}

		/// <summary>
		/// Renders the OpenTK preview setup.
		/// </summary>
		/// <param name="delta">Time span between frames</param>
		private void OpenTkControlPreview_OnRender(TimeSpan delta)
		{			
			// If the OpenTK preview setup control has just been loaded then...
			if (_openTKControlLoaded)
			{
				// Position the camera at the origin
				float cameraX = 0.0f;
				float cameraY = 0.0f;
				float cameraZ = 0.0f;

				// Initialize the drawing engine
				(DataContext as SetupDisplayViewModel).DisplayPreviewDrawingEngine.Initialize(
					cameraX,
					cameraY,
					cameraZ);

				// Remember that we have intialized the drawing engine
				_openTKControlLoaded = false;
			}
					
			// If the window is minimized then short circuit the render
			if (WindowState != WindowState.Minimized)
			{												
				// Have the drawing engine refresh the frame
				(DataContext as SetupDisplayViewModel).DisplayPreviewDrawingEngine.RenderPreview();
			}			
		}

		/// <summary>
		/// Sets the OpenGL viewport on the specified OpenTK control.
		/// </summary>
		/// <param name="openTKControl">OpenTK control to set the viewport</param>
		private void SetViewport(GLWpfControl openTKControl)
		{
			// Get the dots per inch
			DpiScale dpiScale = VisualTreeHelper.GetDpi(OpenTkControl);

			// Calculate the size of the drawing area in pixels
			int pixelWidth = (int)(openTKControl.ActualWidth * dpiScale.DpiScaleX);
			int pixelHeight = (int)(openTKControl.ActualHeight * dpiScale.DpiScaleY);

			// Set the OpenTK viewport
			GL.Viewport(0, 0, pixelWidth, pixelHeight);
		}

		/// <summary>
		/// Event when the OpenTK control changes sizes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			// Set the view port of the prop preview			
			SetViewport(OpenTkControl);

			// Forward the call to the drawing engine
			(DataContext as SetupDisplayViewModel).PropPreviewDrawingEngine.OpenTKDrawingAreaChanged(e.NewSize.Width, e.NewSize.Height);			
		}

		/// <summary>
		/// Event when the OpenTK control changes sizes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControlPreview_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			// Set the view port of the preview setup
			SetViewport(OpenTkControlPreview);

			// Forward the call to the drawing engine
			(DataContext as SetupDisplayViewModel).DisplayPreviewDrawingEngine.OpenTKDrawingAreaChanged(e.NewSize.Width, e.NewSize.Height);			
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
			(DataContext as SetupDisplayViewModel).PropPreviewDrawingEngine.Zoom(direction);

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
				(DataContext as SetupDisplayViewModel).PropPreviewDrawingEngine.MoveCamera(_prevMousePositionX, _prevMousePositionY, eX, eY);

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

		/// <summary>
		/// Event when the mouse down button is pressed over the OpenTK Preview control.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControlPreview_MouseDown(object sender, MouseButtonEventArgs e)
		{
			// If the left button has been pressed then...
			if (e.LeftButton == MouseButtonState.Pressed)
			{				
				// If the mouse has just been clicked then we hide the cursor and store the position
				Cursor = System.Windows.Input.Cursors.Hand;

				// Store off the mouse position
				_prevMousePositionX = (int)e.GetPosition(OpenTkControlPreview).X;
				_prevMousePositionY = (int)e.GetPosition(OpenTkControlPreview).Y;

				// Forward the preview setup OpenTK control mouse down event to the drawing engine
				(DataContext as SetupDisplayViewModel).DisplayPreviewDrawingEngine.MouseDown(_prevMousePositionX, _prevMousePositionY);
			}
		}

		/// <summary>
		/// OpenTK control preview setup mouse up event handler.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OpenTkControlPreview_MouseUp(object sender, MouseButtonEventArgs e)
		{
			// Forward the mouse up event to the preview setup drawing engine
			(DataContext as SetupDisplayViewModel).DisplayPreviewDrawingEngine.MouseUp(new OpenTK.Mathematics.Vector2(_prevMousePositionX, _prevMousePositionY));
		}

		/// <summary>
		/// Event when the mouse is moved over the OpenTK control.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControlPreview_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			// Retrieve the position of the mouse as an integer
			int eX = (int)e.GetPosition(OpenTkControlPreview).X;
			int eY = (int)e.GetPosition(OpenTkControlPreview).Y;

			// If the mouse has not moved significantly then exit
			if (eX == _prevMousePositionX && eY == _prevMousePositionY) return;
			
			// Have the drawing engine handle the mouse move event
			bool overResizeBox = (DataContext as SetupDisplayViewModel).DisplayPreviewDrawingEngine.MouseMove(_prevMousePositionX, _prevMousePositionY, eX, eY);

			// Save off the mouse position
			_prevMousePositionX = eX;
			_prevMousePositionY = eY;

			// If the Left mouse button is down then...			
			if (e.LeftButton == MouseButtonState.Pressed)
			{				
				// This should trigger the control to redraw
				OpenTkControl.InvalidateVisual();
			}
			
			// If the mouse is over a resize handle then...
			if (overResizeBox)
			{
				// Set the cursor to the sizing cursor
				Cursor = System.Windows.Input.Cursors.SizeAll;
			}
			else
			{
				// Otherwise set it to the normal arrow cursor
				Cursor = System.Windows.Input.Cursors.Arrow;
			}
		}

		#endregion
	}
}
