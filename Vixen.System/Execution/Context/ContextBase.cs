using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vixen.Execution.DataSource;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution.Context
{
	public abstract class ContextBase : IContext
	{
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		internal ContextCurrentEffectsFull _currentEffects; 
		private readonly IntentStateBuilder _elementStateBuilder;
		private bool _disposed;

		public event EventHandler ContextStarted;
		public event EventHandler ContextEnded;

		protected ContextBase()
		{
			Id = Guid.NewGuid();

			_currentEffects = new ContextCurrentEffectsFull();
			_elementStateBuilder = new IntentStateBuilder();
		}

		public abstract IExecutor Executor { set; }

		public Guid Id { get; private set; }

		public abstract string Name { get; }

		public void Start()
		{
			try {
				_OnStart();
				IsRunning = true;
			}
			catch (Exception ex) {
				Logging.Error(ex.Message,ex);
			}
		}

		public void Pause()
		{
			if (IsRunning && !IsPaused) {
				IsPaused = true;
				_OnPause();
			}
		}

		public void Resume()
		{
			if (IsPaused) {
				_OnResume();
				IsPaused = false;
			}
		}

		public void Stop()
		{
			if (IsRunning) {
				_OnStop();
				IsPaused = false;
				IsRunning = false;
			}
		}

		public virtual bool IsPaused { get; private set; }

		// A context may or may not wrap a sequence, so the state of this property cannot be
		// dependent upon a sequence starting.
		public virtual bool IsRunning { get; private set; }

		public TimeSpan GetTimeSnapshot()
		{
			return (_SequenceTiming != null) ? _SequenceTiming.Position : TimeSpan.Zero;
		}

		public bool UpdateElementStates(TimeSpan currentTime)
		{
			
			if (IsRunning && !IsPaused)
			{
				var t = ResetElementStates();
				_UpdateCurrentEffectList(currentTime);
				t.Wait();
				_DiscoverIntentsFromEffects(currentTime);
			}

			return _currentEffects.Count>0;
		}

		public IIntentStates GetState(Guid key)
		{
			//Refactored to get state directly from the builder. Avoid copying them into another object with no real value.
			return _elementStateBuilder.GetElementState(key);
		}

		private async Task ResetElementStates()
		{
			await Task.Run(() => _elementStateBuilder.Clear());
		}

		private bool _UpdateCurrentEffectList(TimeSpan currentTime)
		{
			// We have an object that does this for us.
			return _currentEffects.UpdateCurrentEffects(_DataSource, currentTime);
		}

		private void _DiscoverIntentsFromEffects(TimeSpan currentTime)
		{
			// For each effect in the in-effect list for the context...
			Parallel.ForEach(_currentEffects, effectNode =>
			{
				TimeSpan effectRelativeTime = currentTime - effectNode.StartTime;
				EffectIntents effectIntents = effectNode.Effect.Render();
				foreach (var effectIntent in effectIntents)
				{
					foreach (IIntentNode intentNode in effectIntent.Value)
					{
						if (TimeNode.IntersectsInclusively(intentNode, effectRelativeTime))
						{
							IIntentState intentState = intentNode.CreateIntentState(effectRelativeTime - intentNode.StartTime,
								effectNode.Effect.Layer);
							_elementStateBuilder.AddElementState(effectIntent.Key, intentState);
						}
					}
				}

			});
		}

		protected void ClearCurrentEffects()
		{
			_currentEffects.Reset();
		}

		protected abstract IDataSource _DataSource { get; }

		protected abstract ITiming _SequenceTiming { get; }

		protected virtual void _OnStart()
		{
		}

		protected virtual void _OnPause()
		{
		}

		protected virtual void _OnResume()
		{
		}

		protected virtual void _OnStop()
		{
		}

		protected virtual void OnContextStarted(EventArgs e)
		{
			if (ContextStarted != null) {
				ContextStarted(this, e);
			}
		}

		protected virtual void OnContextEnded(EventArgs e)
		{
			if (ContextEnded != null) {
				ContextEnded(this, e);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed) {
				if (disposing)
				{
					if (IsRunning)
					{
						Stop();		
					}
				}
				_disposed = true;
			}
		}

	}
}