using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Sys.Managers;
using Vixen.Sys.State.Execution;
using System.Collections.Concurrent;
using Vixen.Sys.Instrumentation;

namespace Vixen.Sys
{
	public class Execution
	{
		internal static SystemClock SystemTime = new SystemClock();
		private static ExecutionStateEngine _state;
		private static ControllerUpdateAdjudicator _updateAdjudicator;
		private static MillisecondsValue _systemAllowedUpdateTime;
		private static MillisecondsValue _systemAllowedBlockTime;
		private static MillisecondsValue _systemDeniedUpdateTime;
		private static MillisecondsValue _systemDeniedBlockTime;
		private static Stopwatch _stopwatch;
		private static long lastMs = 0;
		private static bool lastUpdateClearedStates = false;

		public static void initInstrumentation()
		{
			_stopwatch = Stopwatch.StartNew();
			_systemAllowedUpdateTime = new MillisecondsValue("System allowed update");
			VixenSystem.Instrumentation.AddValue(_systemAllowedUpdateTime);
			_systemAllowedBlockTime = new MillisecondsValue("System allowed block");
			VixenSystem.Instrumentation.AddValue(_systemAllowedBlockTime);
			_systemDeniedUpdateTime = new MillisecondsValue("System denied update");
			VixenSystem.Instrumentation.AddValue(_systemDeniedUpdateTime);
			_systemDeniedBlockTime = new MillisecondsValue("System denied block");
			VixenSystem.Instrumentation.AddValue(_systemDeniedBlockTime);
		}

		// These are system-level events.
		public static event EventHandler NodesChanged
		{
			add { NodeManager.NodesChanged += value; }
			remove { NodeManager.NodesChanged -= value; }
		}

		public static event EventHandler ExecutionStateChanged
		{
			add { _State.StateChanged += value; }
			remove { _State.StateChanged -= value; }
		}

		public static void OpenExecution()
		{
			_State.ToOpen();
		}

		public static void CloseExecution()
		{
			_State.ToClosed();
		}

		public static void OpenTest()
		{
			_State.ToTest();
		}

		public static void CloseTest()
		{
			_State.ToClosed();
		}

		internal static void Startup()
		{

		}

		internal static void Shutdown()
		{
		}

		private static ControllerUpdateAdjudicator _UpdateAdjudicator
		{
			get
			{
				//*** user-configurable threshold value
				return _updateAdjudicator ?? (_updateAdjudicator = new ControllerUpdateAdjudicator(10));
			}
		}

		private static ExecutionStateEngine _State
		{
			get { return _state ?? (_state = new ExecutionStateEngine()); }
		}

		public static string State
		{
			get { return _State.CurrentState.Name; }
		}

		public static bool IsOpen
		{
			get { return State == OpenState.StateName || State == OpeningState.StateName; }
		}

		public static bool IsClosed
		{
			get { return State == ClosedState.StateName || State == ClosingState.StateName; }
		}

		public static bool IsInTest
		{
			get { return State == TestOpeningState.StateName || State == TestOpenState.StateName; }
		}

		public static TimeSpan CurrentExecutionTime
		{
			get { return (SystemTime.IsRunning) ? SystemTime.Position : TimeSpan.Zero; }
		}

		public static string CurrentExecutionTimeString
		{
			get { return CurrentExecutionTime.ToString("m\\:ss\\.fff"); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequence"></param>
		/// <param name="contextName"></param>
		/// <returns>The resulting length of the queue.  0 if it cannot be added.</returns>
		public static int QueueSequence(ISequence sequence, string contextName = null)
		{
			// Look for an execution context with that name.
			IContext context =
				VixenSystem.Contexts.FirstOrDefault(x => x.Name.Equals(contextName, StringComparison.OrdinalIgnoreCase));

			if (context == null) {
				// Context does not exist.
				// The context must be created and managed since the user is not doing it.
				context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.SequenceLevelCaching),
				                                                     sequence);
				// When the program ends, release the context.
				context.ContextEnded += (sender, e) => VixenSystem.Contexts.ReleaseContext(context);
				context.Start();
				// It is the sequence playing now.
				return 1;
			}

			if (context is IProgramContext) {
				// Context already exists as a program context.  Add sequence to it.
				// Can't just add the sequence to the program because it's executing and the
				// thing executing it has likely cached the state.  Need to go through the
				// appropriate layers.
				return (context as IProgramContext).Queue(sequence);
			}

			// Else context exists, but it's not a program context, so it can't be queued
			// into.
			return 0;
		}


		private static ConcurrentDictionary<string, TimeSpan> lastSnapshots = new ConcurrentDictionary<string, TimeSpan>();
		private static Object lockObject = new Object();

		public static ConcurrentDictionary<string, TimeSpan> UpdateState( out bool allowed)
		{
			if (_stopwatch == null)
				initInstrumentation();
			long nowMs = _stopwatch.ElapsedMilliseconds;
			lock (lockObject) {
				long lockMs = _stopwatch.ElapsedMilliseconds - nowMs;
				bool allowUpdate = _UpdateAdjudicator.PetitionForUpdate();
				if (allowUpdate) {
					bool elementsAffected = VixenSystem.Contexts.Update();
					if (elementsAffected)
					{
						VixenSystem.Elements.Update();
						lastUpdateClearedStates = false;
						if (VixenSystem.OutputControllers.Any(x => x.IsRunning))
						{
							//Only update the filter chain if we have a controller running
							VixenSystem.Filters.Update();
						}
					}
					else if(!lastUpdateClearedStates)
					{
						//No need to sample all the contexts as we were just told there are no elements effected.
						VixenSystem.Elements.ClearStates();
						lastUpdateClearedStates = true;
						if (VixenSystem.OutputControllers.Any(x => x.IsRunning))
						{
							//Only update the filter chain if we have a controller running
							VixenSystem.Filters.Update();
						}
					}

					_systemAllowedBlockTime.Set( lockMs);
					_systemAllowedUpdateTime.Set(_stopwatch.ElapsedMilliseconds - nowMs - lockMs);
				}
				else {
					_systemDeniedBlockTime.Set(lockMs);
					_systemDeniedUpdateTime.Set(_stopwatch.ElapsedMilliseconds - nowMs - lockMs);
				}
				lastMs = nowMs;
				allowed = allowUpdate;
				return lastSnapshots;
			}
		}
	}
}