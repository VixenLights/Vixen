using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using VixenModules.Editor.FixtureGraphics.WPF;
using VixenModules.Editor.FixturePropertyEditor.ViewModels;

namespace VixenModules.Editor.FixturePropertyEditor.Views
{
	/// <summary>
	/// Maintains fixture pan / tilt data view.
	/// </summary>
	public partial class PanTiltView
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>		
		public PanTiltView()
		{
			// Initialize the user control
			InitializeComponent();

			// Initialize the moving head 3-D graphic
			_movingHead = new MovingHeadWPF();

			// Configure the moving head
			_movingHead.MovingHead.IncludeLegend = false;
			_movingHead.MovingHead.BeamColor = Color.Cyan;
			_movingHead.MovingHead.BeamLength = 20;
			_movingHead.MovingHead.Focus = 20;
		}

		#endregion

		#region Constants

		/// <summary>
		/// Width and height of the moving head graphic.
		/// </summary>
		const int SizeOfMovingHeadGraphic = 300;

		#endregion

		#region Fields

		/// <summary>
		/// Moving head graphic.
		/// </summary>
		private MovingHeadWPF _movingHead;

		/// <summary>
		/// Thread for animating the moving head graphic.
		/// </summary>
		private Thread _animateMovingHeadThread;
		
		/// <summary>
		/// Flag to control whether the pan/tilt angle is increasing or decreasing.
		/// </summary>
		private bool _increasingAngle = true;

		/// <summary>
		/// This flag controls the moving head animation thread.
		/// When the flag is set false the animation thread will exit.
		/// </summary>
		private bool _displayMovingHead = true;

		#endregion

		#region Private Methods

		/// <summary>
		/// Event when the user control is loaded.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			// Draw the moving head			
			_movingHead.DrawFixtureNoBitmap(SizeOfMovingHeadGraphic, SizeOfMovingHeadGraphic, 1.0, 0, 0, MainViewport);
			
			// Create a thread to animate the moving head			
			_animateMovingHeadThread = new Thread(AnimateMovingHead);
			_animateMovingHeadThread.IsBackground = true;
			_animateMovingHeadThread.Start();			
		}

		/// <summary>
		/// Update the angle of the pan or tilt of the moving head graphic.
		/// The angle will increase to the maximum limit and then start decreasing back to the minimum limit.
		/// </summary>
		/// <param name="minLimit">Minimum limit of the angle</param>
		/// <param name="maxLimit">Maximum limit of the angle</param>
		/// <param name="increasing">True when the angle is increasing</param>
		/// <param name="pan">True when the pan angle is being animated</param>
		private double AnimateAngle(double minLimit, double maxLimit, ref bool increasing, bool pan, double angle)
		{
			// If the angle is greater than or equal to the maximum limit then...
			if (angle >= maxLimit)
			{
				// Switch direction of the animation
				increasing = false;

				// Set the angle to the maximum limit
				angle = maxLimit;
			}
			// Otherwise if the angle is less than or equal to the minimum limit then...
			else if (angle <= minLimit)
			{
				// Switch direction of the animation
				increasing = true;

				// Set the angle to the minimum limit
				angle = minLimit;
			}

			const double AngleChange = 1.0d;

			// If the angle is increasing then...
			if (increasing)
			{				
				angle += AngleChange;				
			}
			// Otherwise the angle is decreasing
			else
			{
				angle -= AngleChange;
			}

			// Return the updated angle
			return angle;
		}

		/// <summary>
		/// Animates the moving head to illustrate the axis of the movement limits being inputed.
		/// </summary>
		private void AnimateMovingHead()
		{	
			// Loop (animate) until the user control is unloaded
			do
			{
				// Retrieve the pan/tilt view model
				PanTiltViewModel panTiltViewModel = (PanTiltViewModel)ViewModel;

				// If the pan/tilt view model is non null then...
				if (panTiltViewModel != null)
				{
					// Make sure the user control is visible otherwise put the thread to sleep
					panTiltViewModel.Animate.WaitOne();

					// If the user control is configuring the pan limits then...
					if (panTiltViewModel.IsPan)
					{
						// Fix the tilt angle at 90 degrees
						_movingHead.MovingHead.TiltAngle = 90.0;

						// Animate the pan angle
						_movingHead.MovingHead.PanAngle = AnimateAngle(0.0, 360.0, ref _increasingAngle, true, _movingHead.MovingHead.PanAngle);																		
					}
					else
					{							
						// Fix the pan angle at zero degrees
						_movingHead.MovingHead.PanAngle = 0.0;

						// Animate the tilt angle
						_movingHead.MovingHead.TiltAngle = AnimateAngle(180.0, 360.0, ref _increasingAngle, false, _movingHead.MovingHead.TiltAngle);						
					}
						
					// Since this is background thread dispatch the draw method on the GUI thread
					MainViewport.Dispatcher.Invoke
						(new Action(() => _movingHead.DrawFixtureNoBitmap(SizeOfMovingHeadGraphic, SizeOfMovingHeadGraphic, 1.0, 0, 0, MainViewport)));

					// Sleep the thread between the angle changes
					Thread.Sleep(10);
				}
			}
			// Loop (animate) until the user control is unloaded
			while (_displayMovingHead);			
		}

		/// <summary>
		/// Event for when the user control is unloaded.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{	
			// Clear flag so the animating thread dies of natural causes
			_displayMovingHead = false;	
		
			// Clear out the animation thread
			_animateMovingHeadThread = null;	
		}

		#endregion
	}	
}
