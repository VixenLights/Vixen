using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Hardware;
using Vixen.Execution;
using System.Diagnostics;
using Vixen.Common;
using System.Threading;
using Vixen.Module.Effect;

namespace Vixen.Sys {
	public class Execution {
		static private Dictionary<Guid, ProgramContext> _contexts = new Dictionary<Guid, ProgramContext>();
		static private SystemClock _systemTime = new SystemClock();
		static private Thread _channelReadThread;
		// Creating channels in here instead of VixenSystem so that the collection
		// will be locally available for EffectRenderer instances.
		static private Dictionary<OutputChannel, SystemChannelEnumerator> _channels = new Dictionary<OutputChannel, SystemChannelEnumerator>();
		static private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		// These are system-level events.
		static public event EventHandler ExecutionContextCreated;
		static public event EventHandler ExecutionContextReleased;
		static public event EventHandler NodesChanged {
			add { Nodes.NodesChanged += value; }
			remove { Nodes.NodesChanged -= value; }
		}

		private enum ExecutionState { Starting, Started, Stopping, Stopped };
		static private volatile ExecutionState _state = ExecutionState.Stopped;

		static Execution() {
			// Create the node manager.
			Nodes = new NodeManager();
			// Load channels.
			foreach(OutputChannel channel in VixenSystem.UserData.LoadChannels()) {
				_AddChannel(channel);
			}
			// Load branch nodes.
			IEnumerable<ChannelNode> branchNodes = VixenSystem.UserData.LoadBranchNodes();
			// Get the branch nodes into the node manager.
			foreach(ChannelNode branchNode in branchNodes) {
				Nodes.AddNode(branchNode);
			}
		}

		static public OutputChannel AddChannel(string channelName) {
			channelName = _Uniquify(channelName);
			OutputChannel channel = new OutputChannel(channelName);
			_AddChannel(channel);
			// Add a root node for the channel.
			Nodes.AddChannelLeaf(channel);
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

		static private void _AddChannel(OutputChannel channel) {
			// Create an enumerator.
			_CreateChannelEnumerators(channel);
		}

		static public void RemoveChannel(OutputChannel channel) {
			SystemChannelEnumerator enumerator;
			if(_channels.TryGetValue(channel, out enumerator)) {
				lock(_channels) {
					// Kill enumerator.
					enumerator.Dispose();
					// Remove from channel dictionary.
					_channels.Remove(channel);
					// Remove from nodes via node manager.
					Nodes.RemoveChannelLeaf(channel);
				}
			}
		}

		static private void _CreateChannelEnumerators(params OutputChannel[] channels) {
			_CreateChannelEnumerators(channels as IEnumerable<OutputChannel>);
		}

		static private void _CreateChannelEnumerators(IEnumerable<OutputChannel> channels) {
			lock(_channels) {
				foreach(OutputChannel channel in channels) {
					_channels[channel] = new SystemChannelEnumerator(channel, _systemTime);
				}
			}
		}

		static private void _ResetChannelEnumerators() {
			lock(_channels) {
				foreach(OutputChannel channel in Channels) {
					_channels[channel].Dispose();
					_channels[channel] = null;
				}
			}
		}

		static public IEnumerable<OutputChannel> Channels {
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
			IEnumerator<CommandData> enumerator;
			foreach(OutputChannel channel in Channels) {
				lock(_channels) {
					enumerator = _channels[channel];
					// Will return true if state has changed.
					if(enumerator.MoveNext()) {
						channel.Patch.Write(enumerator.Current);
					}
				}
			}
		}


		static public ProgramContext CreateContext(Program program) {
			ProgramContext context = new ProgramContext(program);
			_contexts[context.Id] = context;
			if(ExecutionContextCreated != null) {
				ExecutionContextCreated(context, EventArgs.Empty);
			}

			return context;
		}

		static public ProgramContext CreateContext(ISequence sequence) {
			Program program = new Program(sequence.Name);
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
						if(ExecutionContextReleased != null) {
							ExecutionContextReleased(context, EventArgs.Empty);
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
				context = CreateContext(sequence);
				// If they explicitly specified a context name, override the existing name.
				if(!string.IsNullOrWhiteSpace(contextName)) {
					context.Program.Name = contextName;
				}
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
		static public void Write(IEnumerable<CommandNode> state) {
			// Give the renderer a separate collection instance.
			EffectRenderer renderer = new EffectRenderer(state.ToArray());
			ThreadPool.QueueUserWorkItem((o) => renderer.Start());
		}

		static public NodeManager Nodes { get; private set; }

		class EffectRenderer {
			private long _timeStarted;
			private Stack<CommandNode> _effects;
			//*** to be user data, the offset to add to make live data more live
			private int _syncDelta = 0;

			public EffectRenderer(CommandNode[] state) {
				_timeStarted = Execution._systemTime.Position;
				_effects = new Stack<CommandNode>(state);
			}

			public void Start() {
				// Keep going while there is data to render and the system is running.
				while(_effects.Count > 0 && _systemTime.IsRunning) {
					CommandNode commandNode = _effects.Pop();

					if(commandNode.Command != null && commandNode.TargetNodes.Length > 0) {
						// Get the channels that are to be affected by this effect.
						// If they are targetting multiple nodes, the resulting channels
						// will be treated as a single collection of channels.  There will be
						// no differentiation between channels of different trees.
						Dictionary<Guid,OutputChannel> targetChannels = commandNode.TargetNodes.SelectMany(x => x).ToDictionary(x => x.Id);
						
						// Render the effect for the whole span of the command's time.
						ChannelData channelData = commandNode.RenderEffectData(0, commandNode.TimeSpan);
						
						if(channelData != null) {
							// Get it into the channels.
							foreach(Guid channelId in channelData.Keys) {
								OutputChannel targetChannel = targetChannels[channelId];

								Monitor.Enter(targetChannel);

								// Offset the data's time frame by the command's time offset.
								foreach(CommandData data in channelData[channelId]) {
									CommandData targetChannelData = data ?? CommandData.Empty;
									// The data from an effect is effect-local.
									// Adding the command's start time makes it context-local.
									// Adding the system time makes it system-local.
									long systemTime = _systemTime.Position + _syncDelta;
									// Can't assume the safety of the instance the provided by the effect,
									// so create a local instance.
									targetChannelData.StartTime += commandNode.StartTime + systemTime;
									targetChannelData.EndTime += commandNode.StartTime + systemTime;
									targetChannel.AddData(targetChannelData);
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
