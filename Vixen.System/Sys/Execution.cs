using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Execution;
using Vixen.Sys.Instrumentation;
using Vixen.Sys.State.Execution;

namespace Vixen.Sys {
	public class Execution {
		static internal SystemClock SystemTime = new SystemClock();
		//static private Thread _channelReadThread;
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

			//_channelReadThread = new Thread(_ChannelReaderThread) { IsBackground = true };
			//_channelReadThread.Start();

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

		//static public ProgramContext CreateContext(Program program) {
		//    ProgramContext context = new ProgramContext(program);
		//    _contexts[context.Id] = context;
		//    if(ProgramContextCreated != null) {
		//        ProgramContextCreated(null, new ProgramContextEventArgs(context));
		//    }

		//    return context;
		//}

		//static public ProgramContext CreateContext(ISequence sequence, string contextName = null) {
		//    Program program = new Program(contextName ?? sequence.Name);
		//    program.Add(sequence);
		//    return CreateContext(program);
		//}

		//---------

		//static public Context CreateContext(Program program) {
		//    ProgramContext context = new ProgramContext(program);
		//    _contexts[context.Id] = context;
		//    OnProgramContextCreated(new ProgramContextEventArgs(context));

		//    return context;
		//}

		//static public Context CreateContext(ISequence sequence, string contextName = null) {
		//    Program program = new Program(contextName ?? sequence.Name) { sequence };
		//    return CreateContext(program);
		//}

		//static internal Context CreateContext(string name, IDataSource dataSource, ITiming timingSource, ChannelNode logicalTree) {
		//    Context context = new Context(name, dataSource, timingSource, logicalTree);
		//    return context;
		//}

		//static public void ReleaseContext(ProgramContext context) {
		//    // Double-check locking.
		//    // Do we have the context?
		//    // Great.  Lock.
		//    // Do we still have the context?
		//    // Okay, now we can release it.
		//    if(_contexts.ContainsKey(context.Id)) {
		//        lock(_contexts) {
		//            if(_contexts.ContainsKey(context.Id)) {
		//                context.Stop();
		//                _contexts.Remove(context.Id);
		//                OnProgramContextReleased(new ProgramContextEventArgs(context));
		//                context.Dispose();
		//            }
		//        }
		//    }
		//}

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

			// Give the renderer a separate collection instance.
			//EffectRenderer renderer = new EffectRenderer(effectNodes);
			//ThreadPool.QueueUserWorkItem(o => renderer.Start());
			_liveDataSource.AddData(effectNodes);
		}

		static public void UpdateState() {
			//lock(_contexts) {
			//    foreach(ProgramContext context in _contexts.Values) {
			//        context.Update();
			//    }
			//}
			bool allowUpdate = _updateAdjudicator.PetitionForUpdate();
			if(allowUpdate) {
				VixenSystem.Contexts.Update();
				VixenSystem.Channels.Update();
			}
		}

		//static private void _ChannelReaderThread() {
		//    // Our mission:
		//    // Read data from the channel enumerators and write to the channel patches.

		//    while(_state.IsRunning) {
		//        _UpdateChannelStates();
		//        Thread.Sleep(1);
		//    }
		//}

		//static private void _UpdateChannelStates() {
		//    ExecutionStateValues stateBuffer = new ExecutionStateValues(SystemTime.Position);

		//    foreach(Channel channel in VixenSystem.Channels) {
		//        bool updatedState;
		//        Command channelState = VixenSystem.Channels.UpdateChannelState(channel, out updatedState);
		//        if(updatedState) {
		//            stateBuffer[channel] = channelState;
		//        }
		//    }

		//    if(ValuesChanged != null) {
		//        ValuesChanged(stateBuffer);
		//    }
		//}


		//private static void OnProgramContextCreated(ProgramContextEventArgs programContextEventArgs) {
		//    if(ProgramContextCreated != null) {
		//        ProgramContextCreated(null, programContextEventArgs);
		//    }
		//}

		//private static void OnProgramContextReleased(ProgramContextEventArgs programContextEventArgs) {
		//    if(ProgramContextReleased != null) {
		//        ProgramContextReleased(null, programContextEventArgs);
		//    }
		//}

		//#region EffectRenderer class
		//class EffectRenderer {
		//    private TimeSpan _timeStarted;
		//    private Stack<EffectNode> _effects;
		//    //*** to be user data, the offset to add to make live data more live
		//    private TimeSpan _syncDelta = TimeSpan.Zero;

		//    public EffectRenderer(IEnumerable<EffectNode> state) {
		//        _timeStarted = SystemTime.Position;
		//        _effects = new Stack<EffectNode>(state);
		//    }

		//    public void Start() {
		//        try {
		//            // Keep going while there is data to render and the system is running.
		//            while (_effects.Count > 0 && SystemTime.IsRunning) {
		//                EffectNode effectNode = _effects.Pop();

		//                if (!effectNode.IsEmpty && effectNode.Effect.TargetNodes.Length > 0) {
		//                    // Get the channels that are to be affected by this effect.
		//                    // If they are targetting multiple nodes, the resulting channels
		//                    // will be treated as a single collection of channels.  There will be
		//                    // no differentiation between channels of different trees.
		//                    Dictionary<Guid, Channel> targetChannels = effectNode.Effect.TargetNodes.SelectMany(x => x).ToDictionary(x => x.Id);

		//                    // Render the effect for the whole span of the command's time.
		//                    ChannelData channelData = effectNode.RenderEffectData(TimeSpan.Zero, effectNode.TimeSpan);

		//                    if (channelData != null) {
		//                        // Get it into the channels.
		//                        foreach (Guid channelId in channelData.Keys) {
		//                            Channel targetChannel = targetChannels[channelId];

		//                            lock(targetChannel) {
		//                                TimeSpan systemTimeDelta = _timeStarted + _syncDelta;

		//                                // Offset the data's time frame by the command's time offset.
		//                                foreach(CommandNode data in channelData[channelId]) {
		//                                    // The data time needs to be translated from effect-local to
		//                                    // system-local.
		//                                    // Adding the command's start time makes it context-local.
		//                                    // Adding the system time makes it system-local.
		//                                    // Creating a new instance instead of changing the time members because
		//                                    // changing them affects the data that the effect has created, affecting
		//                                    // the relative timing of the data.  The data that the effect generates
		//                                    // should always be relative to the start of the effect.
		//                                    CommandNode targetChannelData = new CommandNode(data.Command, data.StartTime + systemTimeDelta, data.TimeSpan);
		//                                    targetChannel.AddData(targetChannelData);
		//                                }
		//                            }
		//                        }
		//                    }
		//                }
		//            }
		//        }
		//        catch (Exception ex) {
		//            VixenSystem.Logging.Error("EffectRender: Exception while trying to render an effect. (Has the effect " +
		//                "generated data for a channel or node it doesn't target?)", ex);
		//        }
		//    }
		//}
		//#endregion

		private class LiveDataSource : IDataSource {
			private IntervalTree<EffectNode> _data;

			public LiveDataSource() {
				_data = new IntervalTree<EffectNode>();
			}

			public void AddData(EffectNode effectNode) {
				_data.Add(effectNode);
			}

			public void AddData(IEnumerable<EffectNode> effectNodes) {
				// Live data has a context that has system-relative timing, so data written here
				// needs to be made system-relative to be context-relative, which is a requirement
				// for calling Execution.Write.
				foreach(EffectNode effectNode in effectNodes) {
					//*** live data won't work without this, sequence data won't work with it
					//-> Live data is consumed; okay to overwrite the start time.  But sequence data
					//   is persistent.  It needs to be left intact.
					//-> So why is the timing different?
					//-> System time versus context time?
					//-> AHA!  This is making the effect node system-absolute (bad, touching that time)
					//   but then the context is comparing the new system-absolute effect time against
					//   a context-absolute timing source when determine the "in-effect" effects.
					//   -> In the case of live data, the system time IS the context time, so it works.
					//-> The timing must remain *context-absolute*.  It's the context that determines
					//   its qualification against context timing.  That means whatever writes the
					//   live data to here needs to offset the live data by the system time to make
					//   it also context-relative.
					effectNode.StartTime += SystemTime.Position;
					_data.Add(effectNode);
				}
			}

			public IEnumerable<EffectNode> GetDataAt(TimeSpan time) {
				// Live data is to be consumed.
				EffectNode[] data = _data.Get(time).ToArray();
				_data.Remove(data);
				return data;
			}
		}
	}
}
