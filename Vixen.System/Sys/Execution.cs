using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Hardware;
using Vixen.Execution;
using System.Diagnostics;
using System.Threading;
using Vixen.Sys;
using Vixen.Module.Effect;
using Vixen.Commands;

namespace Vixen.Sys {
	public class Execution {
		static private Dictionary<Guid, ProgramContext> _contexts = new Dictionary<Guid, ProgramContext>();
		static private SystemClock _systemTime = new SystemClock();
		static private Thread _channelReadThread;
		// Creating channels in here instead of VixenSystem so that the collection
		// will be locally available for EffectRenderer instances.
		static private Dictionary<Channel, SystemChannelEnumerator> _channels = new Dictionary<Channel, SystemChannelEnumerator>();
		static private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		// These are system-level events.
		static public event EventHandler<ProgramContextEventArgs> ProgramContextCreated;
		static public event EventHandler<ProgramContextEventArgs> ProgramContextReleased;
		static public event EventHandler NodesChanged {
			add { Nodes.NodesChanged += value; }
			remove { Nodes.NodesChanged -= value; }
		}
		static public event Action<ExecutionStateValues> ValuesChanged;

		private enum ExecutionState { Starting, Started, Stopping, Stopped };
		static private volatile ExecutionState _state = ExecutionState.Stopped;

		static Execution() {
			// Create the node manager.
			Nodes = new NodeManager();

			if(VixenSystem.SystemConfig != null) {
				// Get channels.
				foreach(Channel channel in VixenSystem.SystemConfig.Channels) {
					_AddChannel(channel);
				}

				// Get the branch nodes into the node manager.
				foreach(ChannelNode branchNode in VixenSystem.SystemConfig.Nodes) {
					Nodes.AddNode(branchNode);
				}
			}
		}

		static public Channel AddChannel(string channelName) {
			channelName = _Uniquify(channelName);
			Channel channel = new Channel(channelName);
			_AddChannel(channel);
			return channel;
		}

		static private string _Uniquify(string name) {
			if(_channels.Keys.Any(x => x.Name == name)) {
				string originalName = name;
				bool unique;
				int counter = 2;
				do {
					name = originalName + "-" + counter++;
					unique = !_channels.Keys.Any(x => x.Name == name);
				} while(!unique);
			}
			return name;
		}

		static private void _AddChannel(Channel channel) {
			// Create an enumerator.
			_CreateChannelEnumerators(channel);
		}

		static public void RemoveChannel(Channel channel) {
			SystemChannelEnumerator enumerator;
			if(_channels.TryGetValue(channel, out enumerator)) {
				lock(_channels) {
					// Kill enumerator.
					enumerator.Dispose();
					// Remove from channel dictionary.
					_channels.Remove(channel);
				}
			}
		}

		static private void _CreateChannelEnumerators(params Channel[] channels) {
			_CreateChannelEnumerators(channels as IEnumerable<Channel>);
		}

		static private void _CreateChannelEnumerators(IEnumerable<Channel> channels) {
			lock(_channels) {
				foreach(Channel channel in channels) {
					if(!_channels.ContainsKey(channel) || _channels[channel] == null) {
						_channels[channel] = new SystemChannelEnumerator(channel, _systemTime);
					}
				}
			}
		}

		static private void _ResetChannelEnumerators() {
			lock(_channels) {
				foreach(Channel channel in Channels) {
					_channels[channel].Dispose();
					_channels[channel] = null;
				}
			}
		}

		static public IEnumerable<Channel> Channels {
			// The collection may be modified while they are iterating this collection,
			// so return a copy.
			get { return _channels.Keys.ToArray(); }
		}

		/// <summary>
		/// Allows data to be executed.
		/// </summary>
		static public bool OpenExecution() {
			if(_State == ExecutionState.Stopped) {
				_State = ExecutionState.Starting;

				// Open the channels.
				_CreateChannelEnumerators(Channels);

				_systemTime.Start();
				OutputController.StartAll();
				_channelReadThread = new Thread(_ChannelReaderThread);
				_channelReadThread.Start();
				_State = ExecutionState.Started;
				return true;
			}
			return false;
		}

		static public bool CloseExecution() {
			if(_State == ExecutionState.Started) {
				// Release all contexts.
				// Releasing a context removes it from the collection, so
				// enumerate a copy of the collection.
				foreach(ProgramContext context in _contexts.Values.ToArray()) {
					ReleaseContext(context);
				}
				// Stop reading from channels.
				_State = ExecutionState.Stopping;
				while(_State != ExecutionState.Stopped) { Thread.Sleep(1); }
				_channelReadThread = null;
				// Close the channels.
				_ResetChannelEnumerators();
				// Stop the controllers.
				OutputController.StopAll();
				// Stop internal timing.
				_systemTime.Stop();
				return true;
			}
			return false;
		}

		// Something went kaflooey with the threaded use of the _state variable,
		// so it's been wrapped in the safe and fluffy blankets of this property.
		static private ExecutionState _State {
			get {
				_lock.EnterReadLock();
				try {
					return _state;
				} finally {
					_lock.ExitReadLock();
				}
			}
			set {
				_lock.EnterWriteLock();
				try {
					_state = value;
				} finally {
					_lock.ExitWriteLock();
				}
			}
		}

		static public TimeSpan CurrentExecutionTime { get { return (_systemTime.IsRunning) ? _systemTime.Position : TimeSpan.Zero; } }

		static public string CurrentExecutionTimeString { get { return CurrentExecutionTime.ToString("m\\:ss\\.fff"); } }

		static private void _ChannelReaderThread() {
			// Our mission:
			// Read data from the channel enumerators and write to the channel patches.

			while(_State != ExecutionState.Stopping) {
				_UpdateChannelStates();
				Thread.Sleep(1);
			}
			_State = ExecutionState.Stopped;
		}

		static private void _UpdateChannelStates() {
			ExecutionStateValues stateBuffer = new ExecutionStateValues(_systemTime.Position);
			IEnumerator<CommandNode[]> enumerator;

			foreach(Channel channel in Channels) {
				lock(_channels) {
					enumerator = _channels[channel];
					// Will return true if state has changed.
					// State changes when data qualifies for execution.
					if(enumerator.MoveNext()) {
						Command channelState = Command.Combine(enumerator.Current.Select(x => x.Command));
						stateBuffer[channel] = channelState;
						channel.Patch.Write(channelState);
						lock (VixenSystem.Logging) {
							if (channelState == null)
								VixenSystem.Logging.Debug(Execution.CurrentExecutionTimeString + ": Execution UpdateChannelStates: channel=" + channel + ", command=null");
							else
								VixenSystem.Logging.Debug(Execution.CurrentExecutionTimeString + ": Execution UpdateChannelStates: channel=" + channel + ", command=" + channelState.Identifier + ", " + channelState.GetParameterValue(0));
						}
					}
				}
			}

			if(ValuesChanged != null) {
				ValuesChanged(stateBuffer);
			}
		}


		static public ProgramContext CreateContext(Program program) {
			ProgramContext context = new ProgramContext(program);
			_contexts[context.Id] = context;
			if(ProgramContextCreated != null) {
				ProgramContextCreated(null, new ProgramContextEventArgs(context));
			}

			return context;
		}

		static public ProgramContext CreateContext(ISequence sequence, string contextName = null) {
			Program program = new Program(contextName ?? sequence.Name);
			program.Add(sequence);
			return CreateContext(program);
		}

		static public void ReleaseContext(ProgramContext context) {
			// Double-check locking.
			// Do we have the context?
			// Great.  Lock.
			// Do we still have the context?
			// Okay, now we can release it.
			if(_contexts.ContainsKey(context.Id)) {
				lock(_contexts) {
					if(_contexts.ContainsKey(context.Id)) {
						_contexts.Remove(context.Id);
						if(ProgramContextReleased != null) {
							ProgramContextReleased(null, new ProgramContextEventArgs(context));
						}
						context.Dispose();
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequence"></param>
		/// <param name="contextName"></param>
		/// <returns>The resulting length of the queue.  0 if it cannot be added.</returns>
		static public int QueueSequence(ISequence sequence, string contextName = null) {
			// Look for an execution context with that name.
			ProgramContext context = _contexts.Values.FirstOrDefault(x => x.Name.Equals(contextName, StringComparison.OrdinalIgnoreCase));
			if(context != null) {
				// Context already exists.  Add sequence to it.
				// Can't just add the sequence to the program because it's executing and the
				// thing executing it has likely cached the state.  Need to go through the
				// appropriate layers.
				return context.Queue(sequence);
			} else {
				// Context does not exist.
				// The context must be created and managed since the user is not doing it.
				context = CreateContext(sequence, contextName);
				// When the program ends, release the context.
				context.ProgramEnded += (sender, e) => {
					ReleaseContext(context);
				};
				context.Play();
				// It is the sequence playing now.
				return 0;
			}
		}

		/// <summary>
		/// Write data for immediate execution.
		/// </summary>
		/// <param name="state"></param>
		static public void Write(IEnumerable<EffectNode> state) {
			// Give the renderer a separate collection instance.
			EffectRenderer renderer = new EffectRenderer(state.ToArray());
			ThreadPool.QueueUserWorkItem((o) => renderer.Start());
		}

		static public NodeManager Nodes { get; private set; }

		class EffectRenderer {
			private TimeSpan _timeStarted;
			private Stack<EffectNode> _effects;
			//*** to be user data, the offset to add to make live data more live
			private TimeSpan _syncDelta = TimeSpan.Zero;

			public EffectRenderer(EffectNode[] state) {
				_timeStarted = Execution._systemTime.Position;
				_effects = new Stack<EffectNode>(state);
			}

			public void Start() {
				// Keep going while there is data to render and the system is running.
				while(_effects.Count > 0 && _systemTime.IsRunning) {
					EffectNode effectNode = _effects.Pop();

					if(!effectNode.IsEmpty && effectNode.Effect.TargetNodes.Length > 0) {
						// Get the channels that are to be affected by this effect.
						// If they are targetting multiple nodes, the resulting channels
						// will be treated as a single collection of channels.  There will be
						// no differentiation between channels of different trees.
						Dictionary<Guid,Channel> targetChannels = effectNode.Effect.TargetNodes.SelectMany(x => x).ToDictionary(x => x.Id);
						
						// Render the effect for the whole span of the command's time.
						ChannelData channelData = effectNode.RenderEffectData(TimeSpan.Zero, effectNode.TimeSpan);
						lock (VixenSystem.Logging) {
						    VixenSystem.Logging.Debug(Execution.CurrentExecutionTimeString + ": EffectRenderer: rendering data for effect " + effectNode.Effect.Descriptor.TypeName + ", S=" + effectNode.StartTime + ", D=" + effectNode.TimeSpan + ", target=" + effectNode.Effect.TargetNodes[0].Name);
						}
						
						if(channelData != null) {
							// Get it into the channels.
							foreach(Guid channelId in channelData.Keys) {
								Channel targetChannel = targetChannels[channelId];

								Monitor.Enter(targetChannel);

								TimeSpan systemTimeDelta = _timeStarted + _syncDelta;

								// Offset the data's time frame by the command's time offset.
								foreach(CommandNode data in channelData[channelId]) {
									// The data time needs to be translated from effect-local to
									// system-local.
									// Adding the command's start time makes it context-local.
									// Adding the system time makes it system-local.
									// Creating a new instance instead of changing the time members because
									// changing them affects the data that the effect has created, affecting
									// the relative timing of the data.  The data that the effect generates
									// should always be relative to the start of the effect.
									CommandNode targetChannelData = new CommandNode(data.Command, data.StartTime + systemTimeDelta, data.TimeSpan);
									targetChannel.AddData(targetChannelData);
									lock (VixenSystem.Logging) {
										VixenSystem.Logging.Debug(Execution.CurrentExecutionTimeString + ": EffectRenderer: just added data for channel " + targetChannel.Name + " (" + effectNode.Effect.TargetNodes[0].Name + ")");
									}
								}

								Monitor.Exit(targetChannel);
							}
						}
					}
				}
			}
		}
	}
}
