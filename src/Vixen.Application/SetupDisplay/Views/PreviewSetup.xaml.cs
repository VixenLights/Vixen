using System.Windows;
using System.Windows.Input;

using VixenApplication.SetupDisplay.ViewModels;

namespace VixenApplication.SetupDisplay.Views
{
	/// <summary>
	/// Interaction logic for PreviewSetup.xaml
	/// </summary>
	public partial class PreviewSetup : OpenTkPreviewUserControl
	{
		#region Constructor 

		/// <summary>
		/// Constructor
		/// </summary>
		public PreviewSetup()
		{
			InitializeComponent();

			// Initialize the OpenTK Display Preview
			InitializeOpenTk(OpenTkControlPreview);			
		}

		#endregion

		#region Fields

		/// <summary>
		/// This flag helps ensure the OpenGL drawing engine associated with the control
		/// is only initialized once.
		/// </summary>
		bool _openTKControlLoaded = false;
		
		#endregion

		#region Private Methods
		
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
			
			// Have the drawing engine refresh the frame
			(DataContext as SetupDisplayViewModel).DisplayPreviewDrawingEngine.RenderPreview();			
		}

		/// <summary>
		/// Event handler for the preview setup.
		/// </summary>
		/// <param name="sender">Sender of the event</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControlPreview_Loaded(object sender, System.EventArgs e)
		{
			// Remember that the preview setup OpenTK control has been loaded
			_openTKControlLoaded = true;

			// Refresh the CanExecute delegates
			(DataContext as SetupDisplayViewModel).RefreshCanExecuteCommands();
		}
		
		/// <summary>
		/// Event when the OpenTK control changes sizes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControlPreview_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			// Call base class implementation
			OpenTkControl_SizeChanged(sender, e, OpenTkControlPreview, (DataContext as SetupDisplayViewModel).DisplayPreviewDrawingEngine);			
		}

		/// <summary>
		/// Event when a key is pressed in the OpenTK Preview control.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControlPreview_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			// If either of the zoom keys (I or O) are pressed then...
			if (e.Key == Key.I || e.Key == Key.O)
			{
				// Determine if either of the shift keys are down 
				bool shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

				int factor = 20;
				if (shift)
				{
					factor = 5;
				}

				int delta = 120;
				if (e.Key != Key.I)
				{
					delta = -delta;
				}

				// Determine the zoom factor
				int direction = -(delta * SystemInformation.MouseWheelScrollLines / factor);

				// Update the zoom of the preview
				(DataContext as SetupDisplayViewModel).DisplayPreviewDrawingEngine.Zoom(direction);

				// This should trigger the control to redraw
				OpenTkControlPreview.InvalidateVisual();
			}
		}

		/// <summary>
		/// Event when the Mouse wheel is moved over the OpenTK control.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControlPreview_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			// Call helper method to zoom the OpenTK viewport
			OpenTkControl_MouseWheel(sender, e, OpenTkControlPreview, (DataContext as SetupDisplayViewModel).DisplayPreviewDrawingEngine);
		}

		/// <summary>
		/// Event when the mouse down button is pressed over the OpenTK Preview control.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControlPreview_MouseDown(object sender, MouseButtonEventArgs e)
		{
			// Ensure that the OpenTKControlPreview control receives focus
			OpenTkControlPreview.Focus();

			// If the left button has been pressed then...
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				// If the mouse has just been clicked then change the cursor and store off the position
				Cursor = System.Windows.Input.Cursors.Hand;

				// Store off the mouse position
				PrevMousePositionX = (int)e.GetPosition(OpenTkControlPreview).X;
				PrevMousePositionY = (int)e.GetPosition(OpenTkControlPreview).Y;

				// Forward the preview setup OpenTK control mouse down event to the drawing engine
				(DataContext as SetupDisplayViewModel).DisplayPreviewDrawingEngine.MouseDown(PrevMousePositionX, PrevMousePositionY);
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
			(DataContext as SetupDisplayViewModel).DisplayPreviewDrawingEngine.MouseUp(new OpenTK.Mathematics.Vector2(PrevMousePositionX, PrevMousePositionY));
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
			if (eX == PrevMousePositionX && eY == PrevMousePositionY) return;

			// Have the drawing engine handle the mouse move event
			bool overResizeBox = (DataContext as SetupDisplayViewModel).DisplayPreviewDrawingEngine.MouseMove(PrevMousePositionX, PrevMousePositionY, eX, eY);

			// Save off the mouse position
			PrevMousePositionX = eX;
			PrevMousePositionY = eY;

			// If the Left mouse button is down then...			
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				// This should trigger the control to redraw
				OpenTkControlPreview.InvalidateVisual();
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
