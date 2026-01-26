using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Common.OpenGLCommon.Constructs.DrawingEngine;

using OpenTK.Graphics.OpenGL;
using OpenTK.Wpf;

using VixenApplication.SetupDisplay.OpenGL;

namespace VixenApplication.SetupDisplay.Views
{
	/// <summary>
	/// Base class for an WPF OpenTK User Control.
	/// </summary>
	public abstract class OpenTkPreviewUserControl : System.Windows.Controls.UserControl
	{
		#region Protected Properties

		/// <summary>
		/// Previous X position of the mouse.
		/// </summary>
		protected int PrevMousePositionX { get; set; }

		/// <summary>
		/// Previous Y position of the mouse.
		/// Used to short circuit the mouse move event.
		/// </summary>
		protected int PrevMousePositionY { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Event when the Mouse wheel is moved over the OpenTK control.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>		
		/// <param name="openTKControl">OpenTK WPF control</param>
		/// <param name="drawingEngine">Associated drawing engine</param>
		protected void OpenTkControl_MouseWheel(
			object sender,
			MouseWheelEventArgs e,
			GLWpfControl openTKControl,
			OpenGLDrawingEngineBase<ILightPropOpenGLData> drawingEngine)
		{
			int factor = 20;
			if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
			{
				factor = 5;
			}
			int direction = -(e.Delta * SystemInformation.MouseWheelScrollLines / factor);

			// Update the zoom of the preview
			drawingEngine.Zoom(direction);

			// This should trigger the control to redraw
			openTKControl.InvalidateVisual();
		}

		/// <summary>
		/// Sets the OpenGL viewport on the specified OpenTK control.
		/// </summary>
		/// <param name="openTKControl">OpenTK control to set the viewport</param>
		protected void SetViewport(GLWpfControl openTKControl)
		{
			// Get the dots per inch
			DpiScale dpiScale = VisualTreeHelper.GetDpi(openTKControl);

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
		/// <param name="openTKControl">OpenTK WPF control</param>
		/// <param name="drawingEngine">Associated drawing engine</param>
		protected void OpenTkControl_SizeChanged(object sender, SizeChangedEventArgs e, GLWpfControl openTKControl, OpenGLDrawingEngineBase<ILightPropOpenGLData> drawingEngine)
		{
			// Set the view port of the OpenTK control
			SetViewport(openTKControl);

			// Forward the call to the drawing engine
			drawingEngine.OpenTKDrawingAreaChanged(e.NewSize.Width, e.NewSize.Height);
		}

		/// <summary>
		/// Initializes the specified OpenTK control.
		/// </summary>
		/// <param name="openTKControl">OpenTK control to initialize</param>
		protected void InitializeOpenTk(GLWpfControl openTKControl)
		{
			// Selects OpenGL version 3.3
			GLWpfControlSettings settings = new GLWpfControlSettings
			{
				MajorVersion = 3,
				MinorVersion = 3,
				RenderContinuously = true,
			};

			// Start the OpenTK control						
			openTKControl.Start(settings);
		}

		#endregion
	}
}
