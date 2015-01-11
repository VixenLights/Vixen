using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Timer = System.Timers.Timer;

namespace VixenModules.App.Shows
{
	public abstract class Action: IDisposable
	{
		Timer completeTimer;

		public Action(ShowItem showItem)
		{
			ShowItem = showItem;
			completeTimer = new Timer(100);
			completeTimer.Elapsed += CompleteTimer_Elapsed;
		}

		// Properties
		public ShowItem ShowItem { get; set; }
		public string ResultString { get; set; }
		public bool IsRunning { get; set; }
		public virtual bool PreProcessingCompleted { get; set; }
		public CancellationTokenSource CancellationTknSource  { get; set; }

		// Must be overridden
		public virtual void Execute() 
		{
			IsRunning = true;
		}

		public virtual void Stop() 
		{
			Complete();
		}

		// Complete MUST be called when your action is complete or else the next event won't
		// happen in the scheduler. Even if there is NO time involved in running your event, you should 
		// still call Complete()
		public virtual void Complete()
		{
			completeTimer.Start();
		}

		public virtual void FinalizeComplete() 
		{
			if (IsRunning)
			{
				IsRunning = false;

				if (ActionComplete != null)
				{
					ActionComplete(this, EventArgs.Empty);
				}
			}
		}

		private void CompleteTimer_Elapsed(object sender, EventArgs e)
		{
			completeTimer.Stop();
			FinalizeComplete();
		}

		// Override if you please
		public virtual TimeSpan Duration()
		{
			return TimeSpan.Zero;
		}

		// PreProcess gets called for every event upon show startup. This lets you put stuff in the
		// cache for later processing if you like.
		public virtual void PreProcess(CancellationTokenSource cancellationTokenSource = null)
		{
			CancellationTknSource = cancellationTokenSource;
			// Do nothing, override if your action requires pre-processing (such as a sequence)
			PreProcessingCompleted = true;
		}

		public delegate void ActionCompleteHandler(object sender, EventArgs e);
		public virtual event ActionCompleteHandler ActionComplete;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) { }
	}
}
