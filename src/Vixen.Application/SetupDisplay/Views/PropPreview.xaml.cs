using System.Windows;
using System.Windows.Input;

using VixenApplication.SetupDisplay.ViewModels;

namespace VixenApplication.SetupDisplay.Views
{
	/// <summary>
	/// Interaction logic for PropPreview.xaml
	/// </summary>
	public partial class PropPreview : OpenTkPreviewUserControl
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public PropPreview()
        {
            InitializeComponent();

			// Initialize the prop preview OpenTK control
			InitializeOpenTk(OpenTkControl);			
		}

		#endregion

		#region Fields

		/// <summary>
		/// Flag that is true while the mouse is down.
		/// Used to short circuit the mouse move event.
		/// </summary>
		private bool _mouseDown;
		
		#endregion

		#region Private Methods

		/// <summary>
		/// This is the main render method for the OpenTK WPF control.
		/// </summary>
		/// <param name="delta">Time between render calls</param>
		private void OpenTkControl_OnRender(TimeSpan delta)
		{						
			// Draw the selected prop
			(DataContext as SetupDisplayViewModel).DrawProp();

			// Have the drawing engine refresh the frame
			(DataContext as SetupDisplayViewModel).PropPreviewDrawingEngine.RenderPreview();			
		}

		/// <summary>
		/// Event when the OpenTK control changes sizes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			// Call base class implementation
			OpenTkControl_SizeChanged(sender, e, OpenTkControl, (DataContext as SetupDisplayViewModel).PropPreviewDrawingEngine);
		}

		/// <summary>
		/// Event when the Mouse wheel is moved over the OpenTK control.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OpenTkControl_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			// Call helper method to zoom the OpenTK viewport
			OpenTkControl_MouseWheel(sender, e, OpenTkControl, (DataContext as SetupDisplayViewModel).PropPreviewDrawingEngine);
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
				PrevMousePositionX = (int)e.GetPosition(OpenTkControl).X;
				PrevMousePositionY = (int)e.GetPosition(OpenTkControl).Y;
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
			int eX = (int)e.GetPosition(OpenTkControl).X;
			int eY = (int)e.GetPosition(OpenTkControl).Y;

			// If the mouse has not moved significantly then exit
			if (eX == PrevMousePositionX && eY == PrevMousePositionY) return;

			// If the Left mouse button is down then...			
			if (_mouseDown && e.LeftButton == MouseButtonState.Pressed)
			{
				// Move the view camera 
				(DataContext as SetupDisplayViewModel).PropPreviewDrawingEngine.MoveCamera(PrevMousePositionX,PrevMousePositionY, eX, eY);

				// Save off the mouse position
				PrevMousePositionX = eX;
				PrevMousePositionY = eY;

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
		
		#endregion
	}
}
