using System.Timers;

using VixenModules.Editor.FixtureGraphics;

using Timer = System.Timers.Timer;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	/// <summary>
	/// Maintains a moving head strobe timer.
	/// </summary>
	internal class MovingHeadTimer
	{
		#region Fields
		
		/// <summary>
		/// Timer used to strobe the moving head beam.
		/// </summary>
		private Timer _timer;

		/// <summary>
		/// Collection of moving heads registered with this timer.
		/// </summary>
		private List<Tuple<IMovingHead, Action>> _movingHeads;

		/// <summary>
		/// Delegate to refresh the Vixen preview.
		/// </summary>
		private Action _redraw;

		/// <summary>
		/// Duration of the strobe flash in milliseconds.
		/// </summary>
		private int _strobeDuration;

		/// <summary>
		/// Lock to ensure thread safety with respect to the moving heads collection.
		/// </summary>
		private object _movingHeadsLock;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="interval">Interval of the strobe timer in ms</param>
		/// <param name="maxStrobeDuration">Maximum strobe duration in ms</param>
		/// <param name="redraw">Delegate to redraw the Vixen preview</param>
		public MovingHeadTimer(
			int interval, 
			int maxStrobeDuration, 
			Action redraw)
		{
			// Store off the timer interval
			Interval = interval;

			// Store off the maximum strobe duration
			MaximumStrobeDuration = maxStrobeDuration;

			// Store off the delegate to refresh the Vixen preview
			_redraw = redraw;

			// Initialize the moving heads collection lock
			_movingHeadsLock = new object();

			// Initialize the moving heads collection
			_movingHeads = new List<Tuple<IMovingHead, Action>>();

			// Create the OS timer
			_timer = new Timer(interval);

			// Hook up the Elapsed event for the timer
			_timer.Elapsed += OnStrobeTimerEvent;

			// Configure the timer to continuously fire
			_timer.AutoReset = true;

			// Default the strobe duration to 25% of the interval
			_strobeDuration = interval / 4;

			// If the duration is greater than the max duration then...
			if (_strobeDuration > MaximumStrobeDuration)
			{
				// Limit the duration to 50 ms
				_strobeDuration = MaximumStrobeDuration;
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Interval of the moving head strobe timer in milliseconds.
		/// </summary>
		public int Interval { get; set; }

		/// <summary>
		/// Maximum strobe duration in ms.
		/// </summary>
		public int MaximumStrobeDuration { get; set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds the specified moving head to the timer.
		/// </summary>
		/// <param name="movingHead">Moving head to associate with the strobe timer</param>
		public void AddMovingHead(Tuple<IMovingHead, Action> movingHead)
		{
			// Lock the collection
			lock (_movingHeadsLock)
			{
				// Add the moving head to the collection
				_movingHeads.Add(movingHead);
			}

			// If the timer has NOT already been started then...
			if (!_timer.Enabled)
			{
				// Start the timer
				_timer.Start();
			}
		}

		/// <summary>
		/// Removes the specified moving head from the timer.
		/// </summary>
		/// <param name="movingHead">Moving head to remove from the timer</param>
		public void RemoveMovingHead(Tuple<IMovingHead, Action> movingHead)
		{
			// Lock the collection
			lock (_movingHeadsLock)
			{
				// Remove the moving head from the collection
				_movingHeads.Remove(movingHead);
			}

			// If there are no moving heads associated with this timer then...
			if (_movingHeads.Count == 0)
			{
				// Stop the OS timer
				_timer.Stop();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Event handler when the strobe timer expires.
		/// </summary>
		/// <param name="source">Source of the event</param>
		/// <param name="e">Event arguments</param>
		private void OnStrobeTimerEvent(Object source, ElapsedEventArgs e)
		{
			// Declare a local collection of moving heads
			List<Tuple<IMovingHead, Action>> heads;
			
			// Lock the moving head collection
			lock (_movingHeadsLock)
			{
				// Make a copy of the moving heads collection to avoid
				// threading issues while working with the collection
				heads = _movingHeads.ToList();
			}

			// Loop over the moving heads
			foreach (Tuple<IMovingHead, Action> movingHead in heads)
			{
				// Turn on the beam
				movingHead.Item1.OnOff = true;
			}

			// Redraw the preview
			_redraw();

			// Sleep for the duration of the strobe flash
			Thread.Sleep(_strobeDuration);

			// Loop over the moving heads
			foreach (Tuple<IMovingHead, Action> movingHead in heads)
			{
				// Turn off the beam
				movingHead.Item1.OnOff = false;
			}

			// Redraw the preview
			_redraw();

			// Loop over the moving heads
			foreach (Tuple<IMovingHead, Action> movingHead in heads)
			{
				// If the moving head strobe interval no longer matches this timer then...
				if (movingHead.Item1.StrobeRate != Interval)
				{
					// Update the strobe rate timer associated with the moving head
					movingHead.Item2();
				}
			}
		}

		#endregion
	}
}
