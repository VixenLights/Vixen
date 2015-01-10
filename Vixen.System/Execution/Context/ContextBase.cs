using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Execution.DataSource;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution.Context
{
	public abstract class ContextBase : IContext, IStateSourceCollection<Guid, IIntentStates>
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private ElementStateSourceCollection _elementStates;
		private IContextCurrentEffects _currentEffects;
		private HashSet<Guid> _affectedElements;
		private IntentStateBuilder _elementStateBuilder;
		private bool _disposed;

		public event EventHandler ContextStarted;
		public event EventHandler ContextEnded;

		private delegate void IntentDiscoveryAction(Guid elementId, IIntentNode intentNode, TimeSpan intentRelativeTime);

		protected ContextBase()
		{
			Id = Guid.NewGuid();

			_elementStates = new ElementStateSourceCollection();
			_currentEffects = new ContextCurrentEffectsIncremental();
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

		public HashSet<Guid> UpdateElementStates(TimeSpan currentTime)
		{
			if (IsRunning && !IsPaused) {
				_affectedElements = _UpdateCurrentEffectList(currentTime);
				_RepopulateElementBuffer(currentTime, _affectedElements);
			}

			return _affectedElements;
		}

		public IStateSource<IIntentStates> GetState(Guid key)
		{
			return _elementStates.GetState(key);
		}

		private HashSet<Guid> _UpdateCurrentEffectList(TimeSpan currentTime)
		{
			// We have an object that does this for us.
			return _currentEffects.UpdateCurrentEffects(_DataSource, currentTime);
		}

		private void _RepopulateElementBuffer(TimeSpan currentTime, IEnumerable<Guid> affectedElementIds)
		{
			_InitializeElementStateBuilder();
			_DiscoverIntentsFromCurrentEffects(currentTime, _AddIntentToElementStateBuilder);
			_LatchElementStatesFromBuilder(affectedElementIds);
		}

		private void _DiscoverIntentsFromCurrentEffects(TimeSpan currentTime, IntentDiscoveryAction intentDiscoveryAction)
		{
			_DiscoverIntentsFromEffects(currentTime, _currentEffects, intentDiscoveryAction);
		}

		private void _DiscoverIntentsFromEffects(TimeSpan currentTime, IEnumerable<IEffectNode> effects,
		                                         IntentDiscoveryAction intentDiscoveryAction)
		{
			// For each effect in the in-effect list for the context...
			foreach (IEffectNode effectNode in effects) {
				// Get a time value relative to the start of the effect.
				TimeSpan effectRelativeTime = Helper.GetEffectRelativeTime(currentTime, effectNode);
				// Get the elements the effect affects at this time and the ways it will do so.
				ElementIntents elementIntents = effectNode.Effect.GetElementIntents(effectRelativeTime);
				// For each element...
				foreach (Guid elementId in elementIntents.ElementIds) {
					// Get the intent nodes.
					IIntentNode[] intentNodes = elementIntents[elementId];
					// For each intent node.
					foreach (IIntentNode intentNode in intentNodes) {
						// Get a timing value relative to the intent.
						TimeSpan intentRelativeTime = Helper.GetIntentRelativeTime(effectRelativeTime, intentNode);
						// Do whatever is going to be done.
						intentDiscoveryAction(elementId, intentNode, intentRelativeTime);
					}
				}
			}
		}

		private void _InitializeElementStateBuilder()
		{
			_elementStateBuilder.Clear();
		}

		private void _AddIntentToElementStateBuilder(Guid elementId, IIntentNode intentNode, TimeSpan intentRelativeTime)
		{
			IIntentState intentState = intentNode.CreateIntentState(intentRelativeTime);
			_elementStateBuilder.AddElementState(elementId, intentState);
		}

		private void _LatchElementStatesFromBuilder(IEnumerable<Guid> affectedElementIds)
		{
			foreach (Guid elementId in affectedElementIds) {
				_elementStates.SetValue(elementId, _elementStateBuilder.GetElementState(elementId));
			}
		}

		private void _ResetElementStates()
		{
			_currentEffects.Reset();
			_InitializeElementStateBuilder();
			_LatchElementStatesFromBuilder(_elementStates.ElementsInCollection);
			
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