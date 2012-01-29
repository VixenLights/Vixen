using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Execution;
using Vixen.Sys.Instrumentation;
using Vixen.Sys.State.Execution;

namespace Vixen.Sys {
	public class Execution {
		static internal SystemClock SystemTime = new SystemClock();
		static private ExecutionStateEngine _state;
		static private TotalEffectsValue _totalEffectsValue;
		static private EffectsPerSecondValue _effectsPerSecondValue;
		static private Context _systemContext;
		static private LiveDataSource _liveDataSource;
		static private ControllerUpdateAdjudicator _updateAdjudicator;

		// These are system-level events.
		//static public event EventHandler<ProgramContextEventArgs> ProgramContextCreated;
		//static public event EventHandler<ProgramContextEventArgs> ProgramContextReleased;
		static public event EventHandler NodesChanged {
			add { NodeManager.NodesChanged += value; }
			remove { NodeManager.NodesChanged -= value; }
		}
		
		static public event Action<ExecutionStateValues> ValuesChanged;
		static public event EventHandler ExecutionStateChanged {
			add { _State.StateChanged += value; }
			remove { _State.StateChanged -= value; }
		}

		static public void OpenExecution() {
			_State.ToOpen();
		}

		static public void CloseExecution() {
			_State.ToClosed();
		}

		static public void OpenTest() {
			_State.ToTest();
		}

		static public void CloseTest() {
			_State.ToClosed();
		}

		static internal void Startup() {
			//*** user-configurable
			_updateAdjudicator = new ControllerUpdateAdjudicator(10);

			//*** if the logical node tree changes structure or attributes, such as
			//    the pre-filters, the system context will need to be rebuilt
			//-> or if the physical list of channels changes
			// Create the system context for live execution.
			_liveDataSource = new LiveDataSource();
			_systemContext = VixenSystem.Contexts.CreateContext("System", _liveDataSource, SystemTime, VixenSystem.Nodes);
			_systemContext.Play();
		}

		static internal void Shutdown() {
		}

		static private TotalEffectsValue _TotalEffects {
			get {
				if(_totalEffectsValue == null) {
					_totalEffectsValue = new TotalEffectsValue();
					VixenSystem.Instrumentation.AddValue(_totalEffectsValue);
				}
				return _totalEffectsValue;
			}
		}

		static private EffectsPerSecondValue _EffectsPerSecond {
			get {
				if(_effectsPerSecondValue == null) {
					_effectsPerSecondValue = new EffectsPerSecondValue();
					VixenSystem.Instrumentation.AddValue(_effectsPerSecondValue);
				}
				return _effectsPerSecondValue;
			}
		}

		private static ExecutionStateEngine _State {
			get { return _state ?? (_state = new ExecutionStateEngine()); }
		}

		public static string State {
			get {
				return _State.CurrentState.Name;
			}
		}

		public static bool IsOpen {
			get { return State == OpenState.StateName || State == OpeningState.StateName; }
		}

		public static bool IsClosed {
			get { return State == ClosedState.StateName || State == ClosingState.StateName; }
		}

		public static bool IsInTest {
			get { return State == TestOpeningState.StateName || State == TestOpenState.StateName; }
		}

		static public TimeSpan CurrentExecutionTime { get { return (SystemTime.IsRunning) ? SystemTime.Position : TimeSpan.Zero; } }

		static public string CurrentExecutionTimeString { get { return CurrentExecutionTime.ToString("m\\:ss\\.fff"); } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequence"></param>
		/// <param name="contextName"></param>
		/// <returns>The resulting length of the queue.  0 if it cannot be added.</returns>
		static public int QueueSequence(ISequence sequence, string contextName = null) {
			// Look for an execution context with that name.
			Context context = VixenSystem.Contexts.FirstOrDefault(x => x.Name.Equals(contextName, StringComparison.OrdinalIgnoreCase));
			
			if(context == null) {
				// Context does not exist.
				// The context must be created and managed since the user is not doing it.
				ProgramContext programContext = (ProgramContext)VixenSystem.Contexts.CreateContext(sequence, contextName);
				// When the program ends, release the context.
				programContext.ProgramEnded += (sender, e) => VixenSystem.Contexts.ReleaseContext(programContext);
				programContext.Play();
				// It is the sequence playing now.
				return 1;
			} 
			
			if(context is ProgramContext) {
				// Context already exists as a ProgramContext.  Add sequence to it.
				// Can't just add the sequence to the program because it's executing and the
				// thing executing it has likely cached the state.  Need to go through the
				// appropriate layers.
				return (context as ProgramContext).Queue(sequence);
			}

			// Else context exists, but it's not a ProgramContext, so it can't be queued
			// into.
			return 0;
		}

		/// <summary>
		/// Adds data to the system context to be executed as a system-relative time.
		/// </summary>
		/// <param name="data"></param>
		static public void Write(IEnumerable<EffectNode> data) {
			EffectNode[] effectNodes = data.ToArray();

			_TotalEffects.Add(effectNodes.Length);
			_EffectsPerSecond.Increment(effectNodes.Length);

			_liveDataSource.AddData(effectNodes);
		}

		static public void UpdateState() {
			bool allowUpdate = _updateAdjudicator.PetitionForUpdate();
			if(allowUpdate) {
				VixenSystem.Contexts.Update();
				VixenSystem.Channels.Update();
			}
		}

		#region LiveDataSource Class
		private class LiveDataSource : IDataSource {
			private EffectNodeQueue _data;

			public LiveDataSource() {
				_data = new EffectNodeQueue();
			}

			public void AddData(EffectNode effectNode) {
				_data.Add(effectNode);
			}

			public void AddData(IEnumerable<EffectNode> effectNodes) {
				foreach(EffectNode effectNode in effectNodes) {
					effectNode.StartTime += SystemTime.Position;
					_data.Add(effectNode);
				}
			}

			public IEnumerable<EffectNode> GetDataAt(TimeSpan time) {
				EffectNode[] data = _data.Get(time).ToArray();
				return data;
			}
		}
		#endregion
	}
}
