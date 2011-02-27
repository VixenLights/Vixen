using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Reflection;
using Vixen.Common;
// Using System.Timers.Timer because it exposes a SynchronizingObject member that lets
// you specify the thread context for the Elapsed event.
using System.Timers;
using Vixen.Hardware;
using System.Threading;
using System.Threading.Tasks;
using Vixen.Sequence;
using Vixen.Sys;
using Vixen.Module.Sequence;
using Vixen.Module.Output;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Input;

namespace Vixen.Execution {
	class SequenceExecutor : IExecutor, IDisposable {
		//Data must be compiled into the channels before it can be enumerated
		//-> Compile only channels that are not masked
		//-> Compile only intervals in the specified execution time range
		//-> Could compile in parallel and live by having a method be an enumerator
		//   for a channel's CommandNode data or the whole sequence's data.
		//   -> Would need to specify how many threads
		//   -> An open set like that...will it gain from parallelism?
		private Dictionary<OutputChannel, IEnumerator<CommandData>> _channelEnumerators;
		private SequenceBuffer _updateBuffer = null;
		private System.Timers.Timer _updateTimer;
		private Action _UpdateStrategy;
		private ISequenceModuleInstance _sequence;
		private IRuntimeBehaviorModuleInstance[] _runtimeBehaviors;
		//*** buckets need to be weeded regularly...separate timer to avoid interfering
		//    with updates?  Thread pool thread from an update?
		private Dictionary<int, EnumeratedCommandNodeList> _buckets = new Dictionary<int, EnumeratedCommandNodeList>();
		private List<int> _bucketIndex = new List<int>();
		private IComparer<CommandNode> _commandNodeComparer = new CommandNode.Comparer();

		public event EventHandler<SequenceStartedEventArgs> SequenceStarted;
		public event EventHandler SequenceEnded;
		public event EventHandler<ExecutorMessageEventArgs> Message;
		public event EventHandler<ExecutorMessageEventArgs> Error;

		private const int BUCKET_TIME_SPAN = 10 * 1000; // in ms

		// Public for Activator.
		public SequenceExecutor() {
			_updateTimer = new System.Timers.Timer(10);
			_updateTimer.Elapsed += _UpdateTimerElapsed;
		}

		public ISequenceModuleInstance Sequence {
			get { return _sequence; }
			set {
				if(_sequence != value) {
					_sequence = value;

					// Get runtime behavior.
					_runtimeBehaviors = value.RuntimeBehaviors;

					// Setup buckets.
					_buckets.Clear();

					if(_sequence != null) {
						switch(_sequence.UpdateBehavior) {
							case UpdateBehavior.DeltaOnly:
								_UpdateStrategy = _DeltaUpdate;
								break;
							case UpdateBehavior.FullUpdate:
								_UpdateStrategy = _FullUpdate;
								break;
							default:
								// Default behavior is delta update.
								_UpdateStrategy = _DeltaUpdate;
								break;
						}
					} else {
						_UpdateStrategy = null;
					}
				}
			}
		}

		private void _DataListener(InsertDataParameters parameters) {
			// Data has been inserted into the sequence.
			foreach(IRuntimeBehavior behavior in _runtimeBehaviors) {
				if(behavior.Enabled) {
					_AddData(behavior.GenerateCommandNodes(parameters));
				}
			}
		}

		protected OutputController[] OutputControllers { get; private set; }

		public void Play(int startTime, int endTime) {
			if(this.Sequence != null) {
				// Only hook the input stream during execution.
				// Hook before starting the behaviors.
				_sequence.InsertDataListener += _DataListener;

				// Bound the execution range.
				StartTime = Math.Min(startTime, this.Sequence.Length);
				EndTime = Math.Min(endTime, this.Sequence.Length);

				// Get a collection of channels.
				OutputChannel[] channels =
					(from fixture in this.Sequence.Fixtures
					 from channel in fixture.Channels
					 select channel).ToArray();

				// Get the controllers.
				// While the program executor has already done this to maintain a
				// constant reference as sequences are iterated, we can't assume to
				// always be in a program context (though we actually are).
				// Besides, we need the controller references for the update buffer.
				OutputControllers = OutputController.InitializeControllers(Sequence.ModuleDataSet, StartTime).ToArray();

				// Create an update buffer.
				_updateBuffer = new SequenceBuffer(Sequence, OutputControllers);

				TimingSource = _GetTimingSource(TimingSourceId);

				// Notify any subclass that we're ready to start and allow it to do
				// anything it needs to prepare.
				OnPlaying(StartTime, EndTime);

				// Initialize behaviors BEFORE data is pulled from the sequence,
				// they may influence the data.
				foreach(IRuntimeBehavior behavior in _runtimeBehaviors) {
					behavior.Startup(this.Sequence, TimingSource);
				}

				// CommandNodes that have any intervals within the time frame.
				var qualifiedData = this.Sequence.Data.GetCommandRange(StartTime, EndTime)
					// Ordered by the start time of the intervals.
					.OrderBy(x => x.StartTime);
				// Add the qualified sequence data to the buckets.
				_AddData(qualifiedData);

				_StartDataGeneration();

				// After the subclass is prepared and data generation has started,
				// get an enumerator for each channel.
				// This enumerator will be used to pull data from a channel for execution.
				_channelEnumerators = channels.ToDictionary(x => x, x => new ExecutorChannelEnumerator(x, TimingSource, StartTime, EndTime) as IEnumerator<CommandData>);

				// Start the crazy train.
				IsRunning = true;
				OnSequenceStarted(new SequenceStartedEventArgs(TimingSource));
				OutputController.StartControllers(OutputControllers);
				// Fire the first event manually because it takes a while for the timer
				// to elapse the first time.
				_UpdateOutputs();
				// If there is no length, we may have been stopped as a cascading result
				// of that update.
				if(IsRunning) {
					_updateTimer.Start();
				}
			}
		}

		private void _AddData(CommandNode commandNode) {
			if(commandNode == null) return;

			_AddCommandNode(commandNode);

			// Order the buckets.
			foreach(EnumeratedCommandNodeList bucket in _buckets.Values) {
				bucket.CommandNodes.Sort(_commandNodeComparer);
			}
		}

		private void _AddData(IEnumerable<CommandNode> commandNodes) {
			if(commandNodes == null) return;

			foreach(CommandNode commandNode in commandNodes) {
				_AddCommandNode(commandNode);
			}

			// Order the buckets.
			foreach(EnumeratedCommandNodeList bucket in _buckets.Values) {
				bucket.CommandNodes.Sort(_commandNodeComparer);
			}
		}

		private void _AddCommandNode(CommandNode commandNode) {
			EnumeratedCommandNodeList list;
			int key = commandNode.StartTime / BUCKET_TIME_SPAN;

			if(!_buckets.TryGetValue(key, out list)) {
				_buckets[key] = list = new EnumeratedCommandNodeList();
				_bucketIndex.Add(key);
			}
			list.CommandNodes.Add(commandNode);
		}

		private void _StartDataGeneration() {
			// Start the data generation.
			Thread thread = new Thread(_DataGenerationThread);
			thread.Name = "DataGeneration-" + Sequence.Name;
			thread.IsBackground = true;
			thread.Start();
		}

		private void _DataGenerationThread() {
			// We are going to use IsRunning to tell us when to stop running, but we want
			// to get a head start so we're going to start running before IsRunning is
			// set.  Therefore, we have to watch for the set->reset transition to know
			// when to stop.
			bool transitionToSet = false;
			bool transitionToReset = false;
			CommandNode commandNode;
			CommandData[][] data;
			CommandData[] channelData;
			OutputChannel channel;
			int channelIndex, intervalIndex;
			CommandData dataItem;
			int intervalCount;
			//Interval[] intervals = null;
			int[] intervals = null;
			int bucketKey;
			EnumeratedCommandNodeList bucket;
			int intervalTime = 0;
			int intervalLength = 0;

			do {
				if(IsRunning) transitionToSet = true;
				if(transitionToSet && !IsRunning) transitionToReset = true;

				// Get the bucket for the current time.
				bucketKey = TimingSource.Position / 10000;
				if(!_buckets.TryGetValue(bucketKey, out bucket)) {
					// There is no bucket for the current time.
					Thread.Sleep(5);
					continue;
				}

				// Get the next command.
				if(!bucket.Enumerator.MoveNext()) {
					// There is no data in this bucket.
					Thread.Sleep(5);
					continue;
				}
				commandNode = bucket.Enumerator.Current;

				if(!_sequence.IsUntimed) {
					intervals = _sequence.Data.GetIntervalRange(commandNode.StartTime, commandNode.EndTime).ToArray();
					intervalCount = intervals.Length;
				} else {
					// All intervals are the same length when untimed.
					intervalLength = _sequence.Data.TimingInterval;
					// Cannot start until the next interval boundary.
					int startIndex = commandNode.StartTime / intervalLength;
					// End on an interval boundary.
					int endIndex = commandNode.EndTime / intervalLength;
					// If it falls past the interval boundary, add another interval.
					if(commandNode.EndTime % intervalLength != 0) endIndex++;
					intervalCount = endIndex - startIndex;
					// This will be incremented by _sequence.IntervalTiming before the first piece of data.
					intervalTime = (startIndex - 1) * intervalLength;
				}
				// Command may be null to clear the outputs.
				if(commandNode.Command != null) {
					// Get the primitive data generated by the command.
					data = commandNode.Command.Spec.Generate(commandNode.TargetChannels.Length, intervalCount, commandNode.Command.ParameterValues);

					//TODO: Can this be parallelized?
					// Each row of data is destined for a different channel.
					for(channelIndex = 0; channelIndex < commandNode.TargetChannels.Length; channelIndex++) {
						// Get the channel.
						channel = commandNode.TargetChannels[channelIndex];
						// Get the data destined for the channel.
						channelData = data[channelIndex];
						// Each column of data is bound to a different interval.
						for(intervalIndex = 0; intervalIndex < channelData.Length; intervalIndex++) {
							// CommandData is immutable, so a new instance needs to be created.
							dataItem = channelData[intervalIndex];
							if(!_sequence.IsUntimed) {
								intervalTime = intervals[intervalIndex];
								// If there is another interval in our subset, use that to
								// determine this intervals length.
								// If not, use the end time of the command we're implementing.
								intervalLength =
									(intervalIndex < intervals.Length - 1) ?
									intervals[intervalIndex + 1] - intervalTime :
									commandNode.EndTime - intervalTime;
							} else {
								intervalTime += intervalLength;
							}
							// Create the primitive command data that will be executed by this executor.
							channel.AddData(
								new CommandData(intervalTime, intervalTime + intervalLength,
									dataItem.CommandIdentifier,
									commandNode.IsRequired,
									dataItem.ParameterValues));
						}
					}
				} else {
					// Null command, clear the outputs.
					for(channelIndex = 0; channelIndex < commandNode.TargetChannels.Length; channelIndex++) {
						channel = commandNode.TargetChannels[channelIndex];
						channel.AddData(new CommandData(intervalTime, intervalTime + intervalLength, null, commandNode.IsRequired));
					}
				}
			} while(!transitionToReset);
		}

		virtual protected void OnPlaying(int startTime, int endTime) { }

		public void Pause() {
			if(_updateTimer.Enabled) {
				OutputController.PauseControllers(OutputControllers);
				OnPausing();
				_updateTimer.Enabled = false;
			}
		}

		virtual protected void OnPausing() { }

		public void Resume() {
			if(!_updateTimer.Enabled && this.Sequence != null) {
				OutputController.ResumeControllers(OutputControllers);
				_updateTimer.Enabled = true;
				OnResumed();
			}
		}

		virtual protected void OnResumed() { }

		public void Stop() {
			if(IsRunning) {
				_Stop();
			}
		}

		private void _Stop() {
			// Stop whatever is driving this crazy train.
			lock(_updateTimer) {
				_updateTimer.Enabled = false;
			}

			// Notify the world.
			OnStopping();

			// Release the hook before the behaviors are shut down so that
			// they can affect the sequence.
			_sequence.InsertDataListener -= _DataListener;

			// Shutdown the behaviors.
			foreach(IRuntimeBehavior behavior in _runtimeBehaviors) {
				behavior.Shutdown();
			}

			// Dispose the channel enumerators.
			foreach(IEnumerator<CommandData> enumerator in _channelEnumerators.Values) {
				enumerator.Dispose();
			}
			_channelEnumerators.Clear();

			// Release the update buffer.
			_updateBuffer.Dispose();
			_updateBuffer = null;
			IsRunning = false;

			OnSequenceEnded(EventArgs.Empty);

			// Stop the controllers.
			OutputController.StopControllers(OutputControllers);
		}

		virtual protected void OnStopping() { }

		public bool IsRunning { get; private set; }

		private void _UpdateTimerElapsed(object sender, ElapsedEventArgs e) {
			lock(_updateTimer) {
				// To catch events that may trail after the timer's been disabled
				// due to it being a threaded timer and Stop being called between the
				// timer message being posted and acted upon.
				if(_updateTimer == null || !_updateTimer.Enabled) return;

				_updateTimer.Enabled = false;

				_UpdateOutputs();

				// Cannot do _updateTimer.Enabled = IsRunning because _updateTimer is null
				if(IsRunning) {
					_updateTimer.Enabled = true;
				}
			}
		}

		private void _UpdateOutputs() {
			_updateBuffer.BeginUpdate();

			_UpdateStrategy();

			_updateBuffer.EndUpdate();

			if(_IsEndOfSequence()) {
				Stop();
			}
		}

		private bool _IsEndOfSequence() {
			return EndTime >= StartTime && TimingSource.Position >= EndTime;
		}

		// This has an auto-reset behavior for the outputs.
		private void _FullUpdate() {
			IEnumerator<CommandData> enumerator;
			// Update the state from *every* channel, regardless of it
			// having data or not.
			// An dataless channel or channel state will result in a
			// cleared controller output.
			foreach(OutputChannel channel in _channelEnumerators.Keys) {
				enumerator = _channelEnumerators[channel];
				enumerator.MoveNext();
				channel.Patch.Write(enumerator.Current, _updateBuffer);
			}
		}

		// This has a latching behavior -- channel states set are not automatically reset.
		private void _DeltaUpdate() {
			// Can't rely on the channel enumerator to return false for a delta update because
			// no channels may need to update, but the sequence needs to continue.
			IEnumerator<CommandData> enumerator;
			int index = 0;
			// Only update from channels that provide data.
			foreach(OutputChannel channel in _channelEnumerators.Keys) {
				index++;
				enumerator = _channelEnumerators[channel];
				if(enumerator.MoveNext()) {
					channel.Patch.Write(enumerator.Current, _updateBuffer);
				}
			}
		}


		protected virtual void OnSequenceStarted(SequenceStartedEventArgs e) {
			if(SequenceStarted != null) {
				SequenceStarted(this.Sequence, e);
			}
		}

		protected virtual void OnSequenceEnded(EventArgs e) {
			if(SequenceEnded != null) {
				SequenceEnded(this.Sequence, e);
			}
		}

		protected virtual void OnMessage(ExecutorMessageEventArgs e) {
			if(Message != null) {
				Message(this.Sequence, e);
			}
		}

		protected virtual void OnError(ExecutorMessageEventArgs e) {
			if(Error != null) {
				Error(this.Sequence, e);
			}
		}

		public int StartTime { get; protected set; }
		public int EndTime { get; protected set; }

		static public IExecutor GetExecutor(ISequenceModuleInstance executable) {
			Type attributeType = typeof(ExecutorAttribute);
			IExecutor executor = null;
			// If the executable is decorated with [Executor], get that executor.
			// Since sequences are implemented as modules now, we need to look in the inheritance chain
			// for the attribute.
			ExecutorAttribute attribute = (ExecutorAttribute)executable.GetType().GetCustomAttributes(attributeType, true).FirstOrDefault();
			if(attribute != null) {
				// Create the executor.
				executor = Activator.CreateInstance(attribute.ExecutorType) as IExecutor;
				// Assign the sequence to the executor.
				executor.Sequence = executable;
			}
			return executor;
		}

		~SequenceExecutor() {
			Dispose(false);
		}

		public void Dispose() {
			Dispose(true);
		}

		public virtual void Dispose(bool disposing) {
			if(_updateTimer != null) {
				lock(_updateTimer) {
					Stop();
					_updateTimer.Elapsed -= _UpdateTimerElapsed;
					_updateTimer.Dispose();
					_updateTimer = null;
				}
			}
			GC.SuppressFinalize(this);
		}

		#region Timing implementation

		protected ITimingSource TimingSource { get; set; }

		private ITimingSource _GetTimingSource(Guid outputModuleId) {
			// Going to initially limit it to the controllers that are used by the sequence and
			// will therefore be started.
			ITimingSourceFactory timingSourceFactory = null;
			ITimingSource timingSource = null;
			OutputController outputController = OutputControllers.FirstOrDefault(x => x.HardwareModule.TypeId == outputModuleId);
			if(outputController != null) {
				timingSourceFactory = outputController.HardwareModule as ITimingSourceFactory;
				if(timingSourceFactory != null) {
					timingSource = timingSourceFactory.CreateTimingSource();
				}
			} else if(outputModuleId != Guid.Empty) {
				// Try to get the controller that's using the generic timer.
				timingSource = _GetTimingSource(Guid.Empty);
			}

			if(timingSource != null) return timingSource;

			throw new Exception("Unable to find a timing source.");
		}

		virtual protected Guid TimingSourceId {
			get { return _sequence.TimingSourceId; }
		}

		#endregion
	}

}
