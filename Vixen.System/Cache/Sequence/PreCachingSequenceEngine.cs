using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using Vixen.Cache.Event;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Cache.Sequence
{
	/// <summary>
	/// This class is designed to create a cache of output commands for sequences. It follows the same
	/// processes as the normal execution engine only in a controlled step by step fashion.
	/// </summary>
	public class PreCachingSequenceEngine 
	{
		private static readonly Object LockObject = new Object();
		private bool _lastUpdateClearedStates;
		private readonly CommandHandler _commandHandler = new CommandHandler();
		private ISequence _sequence;
		private bool _isRunning;
		private Thread _cacheThread;

		public event EventHandler<CacheStartedEventArgs> SequenceCacheStarted;
		public event EventHandler<CacheEventArgs> SequenceCacheEnded;
		
		private ManualTiming TimingSource { get; set; }

		/// <summary>
		/// Create using the default update interval
		/// </summary>
		public PreCachingSequenceEngine()
		{	
			TimingSource = new ManualTiming();
		}

		/// <summary>
		/// Create using a specified update interval and sequence.
		/// </summary>
		/// <param name="sequence"></param>
		public PreCachingSequenceEngine(ISequence sequence):this()
		{
			Sequence = sequence;
		}

		/// <summary>
		/// Create using a specified update interval.
		/// </summary>
		/// <param name="interval"></param>
		public PreCachingSequenceEngine(int interval):this()
		{
			TimingSource.Speed = interval;
		}

		/// <summary>
		/// Create using a specified update interval and sequence.
		/// </summary>
		/// <param name="interval"></param>
		/// <param name="sequence"></param>
		public PreCachingSequenceEngine(int interval, ISequence sequence):this(interval)
		{
			Sequence = sequence;
		}

		public ISequence Sequence
		{
			get { return _sequence; } 
			set
			{
				if (value != null)
				{
					_sequence = value;
					Cache = null;
				}
			} 
		}

		public ISequenceCache Cache { get; private set; }


		#region Control

		public void Start()
		{
			if (!IsRunning && Sequence!=null)
			{
				_StartThread();	
			}
			
		}

		public void Stop()
		{
			if (IsRunning)
			{
				_StopThread();
			}
		}

		public bool IsRunning
		{
			get { return _isRunning; }
			private set
			{
				_isRunning = value;
				if (_isRunning)
				{
					OnCacheStarted(new CacheStartedEventArgs(TimingSource, Sequence.Length, TimeSpan.Zero));
				} else
				{
					OnCacheEnded(new CacheEventArgs(Sequence));
				}
			}
		}
		

		#endregion 

		private void _StartThread()
		{
			IsRunning = true;
			_InitializeCache();
			_cacheThread = _CreateCacheThread();
			_cacheThread.Start();
			
		}

		private void _StopThread()
		{
			IsRunning = false;
			_cacheThread.Join(1000);
			_cacheThread = null;
			
		}

		private Thread _CreateCacheThread()
		{
			return new Thread(_BuildSequenceCache) { IsBackground = true, Name = Sequence.Name + " cache" };
		}


		/// <summary>
		/// Create a cache of the entire sequence command outputs
		/// </summary>
		private void _BuildSequenceCache()
		{
			//Need some hooks in here for progess....... Need to think about that.
			//Stop the output devices from driving the execution engine.
			VixenSystem.OutputDeviceManagement.PauseAll();
			
			//Special context to pre cache commands. We don't need all the other fancy executor or timing as we will advance it ourselves
			PreCachingSequenceContext context = VixenSystem.Contexts.GetCacheCompileContext();
			context.Sequence = Sequence;
			TimingSource.Start();
			context.Start();
			
			while (TimingSource.Position <= Sequence.Length && IsRunning)
			{
				List<CommandOutput> commands = _UpdateState(context.GetTimeSnapshot());
				var commandBytes = _ExtractCommands(commands);
				Cache.AppendData(commandBytes.Values.ToList());
				if (Cache.Outputs == null) //Figure out a better way.
				{
					Cache.Outputs = commandBytes.Keys.ToList();
				}
				//Advance the timing
				TimingSource.Increment();
			}

			TimingSource.Stop();
			context.Stop();
			
			//restart the devices
			VixenSystem.OutputDeviceManagement.ResumeAll();
			IsRunning = false;
			//Cache is now ready to use or save.
			//cache.Save();
			// To load the cache use the following
			//ISequenceCache cache2 = SequenceService.Instance.LoadCache(cache.SequenceFilePath);

		}

		private void _InitializeCache()
		{
			ISequenceCache cache = SequenceService.Instance.CreateNewCache(Sequence.FilePath);
			cache.Length = Sequence.Length;
			cache.Interval = (int)TimingSource.Speed;
			Cache = cache;
		}


		private List<CommandOutput> _UpdateState(TimeSpan time)
		{
			var outputCommands = new List<CommandOutput>();
			lock (LockObject)
			{
				//Advance our context to specified time and do all the normal update stuff
				HashSet<Guid> elementsAffected = VixenSystem.Contexts.UpdateCacheCompileContext(time);
				//Check to see if any elements are affected
				if (elementsAffected != null && elementsAffected.Any())
				{
					VixenSystem.Elements.Update(elementsAffected);
					_lastUpdateClearedStates = false;
					VixenSystem.Filters.Update();
				}
				else if(!_lastUpdateClearedStates)
				{
					//Nothing is happening so clear out the states instead of sampling empty context interval
					VixenSystem.Elements.ClearStates();
					_lastUpdateClearedStates = true;
					VixenSystem.Filters.Update();
				}
				
				//Now walk the outputs and collect our data
				foreach (OutputController outputController in VixenSystem.OutputControllers.GetAll())
				{
					outputController.UpdateCommands();
					outputCommands.AddRange(outputController.Outputs);
				}
				
			}

			return outputCommands;
		}

		private Dictionary<Guid,byte> _ExtractCommands(List<CommandOutput> commandOutputs)
		{
			var commands = new Dictionary<Guid, byte>(commandOutputs.Count);
			foreach (var commandOutput in commandOutputs)
			{
				_commandHandler.Reset();
				if (commandOutput.Command != null)
				{
					commandOutput.Command.Dispatch(_commandHandler);	
				}
				commands.Add(commandOutput.Id, _commandHandler.Value);	
			}
			return commands;
		}

		#region Events

		protected virtual void OnCacheStarted(CacheStartedEventArgs e)
		{
			if (SequenceCacheStarted != null)
			{
				SequenceCacheStarted(null, e);
			}
		}

		protected virtual void OnCacheEnded(CacheEventArgs e)
		{
			if (SequenceCacheEnded != null)
			{
				SequenceCacheEnded(null, e);
			}
		}

		#endregion
	}
}
