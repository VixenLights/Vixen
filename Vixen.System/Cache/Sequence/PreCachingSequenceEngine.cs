using System;
using System.Collections;
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
		private bool _statesClear;
		private ISequence _sequence;
		private static bool _isRunning; //static to prevent more than one instance from running at the same time.
		private Thread _cacheThread;
		private int _outputCount;
		private PreCachingSequenceContext _context;

		public event EventHandler<CacheStartedEventArgs> SequenceCacheStarted;
		public event EventHandler<CacheEventArgs> SequenceCacheEnded;
		
		private FixedIntervalManualTiming TimingSource { get; set; }

		#region Contructors

		/// <summary>
		/// Create using the default update interval
		/// </summary>
		public PreCachingSequenceEngine()
		{	
			TimingSource = new FixedIntervalManualTiming();
		}

		/// <summary>
		/// Create using a specified sequence and default interval.
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
			TimingSource.Interval = interval;
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

		#endregion

		#region Properties

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

		public int Interval
		{
			get { return TimingSource.Interval; } 
			set { TimingSource.Interval = value; }
		}

        public TimeSpan Position
        {
            get { return TimingSource.Position;  }
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

		#region Control

		/// <summary>
		/// Start the caching process
		/// </summary>
		public void Start()
		{
			if (!IsRunning && Sequence!=null)
			{
				_StartThread();	
			}
			
		}

		/// <summary>
		/// Interrupt the caching process.
		/// </summary>
		public void Stop()
		{
			if (IsRunning)
			{
				_StopThread();
			}
		}

		

		#endregion 

		#region Operational

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
			IEnumerable<IContext> runningContexts = VixenSystem.Contexts.Where(x => x.IsRunning);
			foreach (var runningContext in runningContexts)
			{
				runningContext.Pause();
			}
			_outputCount = VixenSystem.OutputControllers.GetAll().Sum(x => x.OutputCount);
			VixenSystem.Elements.ClearStates();
			//Special context to pre cache commands. We don't need all the other fancy executor or timing as we will advance it ourselves
			PreCachingSequenceContext context = VixenSystem.Contexts.GetCacheCompileContext();
			context.Sequence = Sequence;
			TimingSource.Start();
			context.Start();

			while (TimingSource.Position <= Sequence.Length && IsRunning)
			{
				List<CommandOutput> commands = _UpdateState(TimingSource.Position);
				Cache.OutputStateListAggregator.AddCommands(commands);
				//Advance the timing
				TimingSource.Increment();
			}

			TimingSource.Stop();
			context.Stop();
			foreach (var runningContext in runningContexts)
			{
				runningContext.Resume();
			}

			VixenSystem.Contexts.ReleaseContext(_context);
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
			cache.Interval = TimingSource.Interval;
			Cache = cache;
		}


		private List<CommandOutput> _UpdateState(TimeSpan time)
		{
			var outputCommands = new List<CommandOutput>(_outputCount);

			//Advance our context to specified time and do all the normal update stuff
			bool elementsAffected = VixenSystem.Contexts.UpdateCacheCompileContext(time, _context);
			//Check to see if any elements are affected
			if (elementsAffected != null && elementsAffected)
			{
				VixenSystem.Elements.Update();
				_statesClear = false;
				VixenSystem.Filters.Update();
			}
			else if (!_statesClear)
			{
				//Nothing is happening so clear out the states instead of sampling empty context interval
				VixenSystem.Elements.ClearStates();
				_statesClear = true;
				VixenSystem.Filters.Update();
			}

			//Now walk the outputs and collect our data
			foreach (OutputController outputController in VixenSystem.OutputControllers.GetAll())
			{
				outputController.UpdateCommands();
				outputCommands.AddRange(outputController.Outputs);
			}

			return outputCommands;
		}

		#endregion

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
