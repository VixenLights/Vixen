using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using OpenTK.Graphics.OpenGL;
using OpenTK.Wpf;

using VixenApplication.SetupDisplay.Wizards.ViewModels;

namespace VixenApplication.SetupDisplay.Wizards.Views
{
	/// <summary>
	/// Base class for a prop wizard page view.
	/// </summary>
	public class WizardPageViewBase : Catel.Windows.Controls.UserControl
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

		#endregion

		#region Protected Properties

		/// <summary>
		/// OpenTK WPF Control.
		/// </summary>
		protected GLWpfControl? OpenTkCntrl { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Initialize the WPF OpenTK control.
		/// </summary>
		protected void Initialize()
		{
			// Selects OpenGL version 3.3
			GLWpfControlSettings settings = new GLWpfControlSettings
			{
				MajorVersion = 3,
				MinorVersion = 3,
				RenderContinuously = true,
			};

			if (OpenTkCntrl is null)
			{
				throw new InvalidOperationException(nameof(OpenTkCntrl) + " is null!");
			}

			// Sets up the OpenGL context and prepares the control to render OpenGL content 
			OpenTkCntrl.Start(settings);
		}
		
		/// <summary>re
		/// User control loaded event, used to initialize the drawing engine and camera.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e"><Event arguments/param>
		protected void PropWizardPageView_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			// Initialize the camera to the origin
			float cameraX = 0.0f;
			float cameraY = 0.0f;
			float cameraZ = 0.0f;

			// Initialize the drawing engine with the camera position and size of the drawing area
			GetViewModel().DrawingEngine.Initialize(
				cameraX,
				cameraY,
				cameraZ);
		}

		/// <summary>
		/// User control unloaded event, used to dispose or OpenGL resources.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		protected void PropWizardPageView_Unloaded(object sender, RoutedEventArgs e)
		{
			// Dispose of the OpenGL resources
			GetViewModel().DrawingEngine.Dispose();
		}

		/// <summary>
		/// This is the main render method for the OpenTK WPF control.
		/// </summary>
		/// <param name="delta">Time between render calls</param>
		protected void OpenTkControl_OnRender(TimeSpan delta)
		{
			// Have the drawing engine refresh the frame
			GetViewModel().DrawingEngine.RenderPreview();			
		}
		
		/// <summary>
		/// Event when the OpenTK control changes sizes.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		protected void OpenTkControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			// Forward the call to the drawing engine
			GetViewModel().DrawingEngine.OpenTKDrawingAreaChanged(e.NewSize.Width, e.NewSize.Height);

			if (OpenTkCntrl is null)
			{
				throw new InvalidOperationException(nameof(OpenTkCntrl) + " is null!");
			}

			var dpiScale = VisualTreeHelper.GetDpi(OpenTkCntrl);
			int pixelWidth = (int)(OpenTkCntrl.ActualWidth * dpiScale.DpiScaleX);
			int pixelHeight = (int)(OpenTkCntrl.ActualHeight * dpiScale.DpiScaleY);

			GL.Viewport(0, 0, pixelWidth, pixelHeight);			
		}
		
		/// <summary>
		/// Event when the Mouse wheel is moved over the OpenTK control.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		protected void OpenTkControl_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			int factor = 20;
			if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
			{
				factor = 5;
			}
			int direction = -(e.Delta * SystemInformation.MouseWheelScrollLines / factor);

			// Update the zoom of the preview
			GetViewModel().DrawingEngine.Zoom(direction);

			if (OpenTkCntrl is null)
			{
				throw new InvalidOperationException(nameof(OpenTkCntrl) + " is null!");
			}

			// This should trigger the control to redraw
			OpenTkCntrl.InvalidateVisual();			
		}

		/// <summary>
		/// Event when the mouse down button is pressed over the OpenTK control.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		protected void OpenTkControl_MouseDown(object sender, MouseButtonEventArgs e)
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
		protected void OpenTkControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
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
				GetViewModel().DrawingEngine.MoveCamera(_prevMousePositionX, _prevMousePositionY, eX, eY);

				// Save off the mouse position
				_prevMousePositionX = eX;
				_prevMousePositionY = eY;

				if (OpenTkCntrl is null)
				{
					throw new InvalidOperationException(nameof(OpenTkCntrl) + " is null!");
				}

				// This should trigger the control to redraw
				OpenTkCntrl.InvalidateVisual();
			}
		}

		/// <summary>
		/// Event when mouse is released over the OpenTK control.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		protected void OpenTkControl_MouseUp(object sender, MouseButtonEventArgs e)
		{
			// Restore the mouse cursor
			Cursor = System.Windows.Input.Cursors.Arrow;

			// Set flag to remember that the mouse button is no longer being pressed
			_mouseDown = false;			
		}

		/// <summary>
		/// Gets the view model associated with the view.
		/// </summary>
		/// <returns><see cref="IPropWizardPageViewModel"></returns>
		/// <exception cref="Exception"></exception>
		private IPropWizardPageViewModel GetViewModel()
		{
			if (DataContext is null)
			{
				throw new InvalidOperationException(nameof(DataContext) + " is null!");
			}

			if (!(DataContext is IPropWizardPageViewModel viewModel))
			{
				throw new Exception("DataContext is wrong type!");
			}

			return viewModel;
		}

		#endregion
	}
}
