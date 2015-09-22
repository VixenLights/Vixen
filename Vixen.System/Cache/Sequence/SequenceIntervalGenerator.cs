using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Cache.Sequence
{
	public class SequenceIntervalGenerator
	{
		private bool _lastUpdateClearedStates;
		private PreCachingSequenceContext _context;
		private IEnumerable<IContext> _runningContexts = Enumerable.Empty<IContext>();
		
		#region Contructors

		/// <summary>
		/// Create using the default update interval
		/// </summary>
		public SequenceIntervalGenerator()
		{	
			State = new OutputStateList();
			TimingSource = new FixedIntervalManualTiming();
		}

		/// <summary>
		/// Create using a specified sequence and default interval.
		/// </summary>
		/// <param name="sequence"></param>
		public SequenceIntervalGenerator(ISequence sequence):this()
		{
			Sequence = sequence;
		}

		/// <summary>
		/// Create using a specified update interval.
		/// </summary>
		/// <param name="interval"></param>
		public SequenceIntervalGenerator(int interval)
			: this()
		{
			TimingSource.Interval = interval;
		}

		/// <summary>
		/// Create using a specified update interval and sequence.
		/// </summary>
		/// <param name="interval"></param>
		/// <param name="sequence"></param>
		public SequenceIntervalGenerator(int interval, ISequence sequence)
			: this(interval)
		{
			Sequence = sequence;
		}

		#endregion

		#region Properties


		private FixedIntervalManualTiming TimingSource { get; set; }

		public ISequence Sequence { get; set; }

		public int Interval
		{
			get { return TimingSource.Interval; }
			set { TimingSource.Interval = value; }
		}

		public TimeSpan Position
		{
			get { return TimingSource.Position; }
		}

		#endregion

		#region Operational

		public bool HasNextInterval()
		{
			return TimingSource.Position + TimeSpan.FromMilliseconds(TimingSource.Interval) <= Sequence.Length;
		}

		public OutputStateList State { get; set; }

		/// <summary>
		/// Gets the command outputs for the current interval and then increments the timing source.
		/// </summary>
		/// <returns>Command outputs or null if there are no more intervals.</returns>
		public void NextInterval()
		{
			//Advance the timing
			TimingSource.Increment();
			UpdateState();
		}

		private void UpdateState()
		{
			if (HasNextInterval())
			{
				List<CommandOutput> commands = _UpdateState(TimingSource.Position);
				State.AddCommands(commands);
			}
		}

		/// <summary>
		/// Initializes the generator processes to begin the data extraction
		/// </summary>
		public void BeginGeneration()
		{
			//Stop the output devices from driving the execution engine.
			VixenSystem.OutputDeviceManagement.PauseAll();
			_runningContexts = VixenSystem.Contexts.Where(x => x.IsRunning);
			foreach (var runningContext in _runningContexts)
			{
				runningContext.Pause();
			}
			VixenSystem.Elements.ClearStates();
			//Special context to pre cache commands. We don't need all the other fancy executor or timing as we will advance it ourselves
			_context = VixenSystem.Contexts.GetCacheCompileContext();
			_context.Sequence = Sequence;
			TimingSource.Start();
			_context.Start();
			UpdateState();
		}

		/// <summary>
		/// Completes the generation process and restarts all the contexts that were running when the BeginGeneration method was called.
		/// </summary>
		public void EndGeneration()
		{
			TimingSource.Stop();
			if (_context != null)
			{
				_context.Stop();
			}
			foreach (var runningContext in _runningContexts)
			{
				runningContext.Resume();
			}
			//restart the devices
			VixenSystem.OutputDeviceManagement.ResumeAll();
		}


		private List<CommandOutput> _UpdateState(TimeSpan time)
		{
			var outputCommands = new List<CommandOutput>();

			//Advance our context to specified time and do all the normal update stuff
			HashSet<Guid> elementsAffected = VixenSystem.Contexts.UpdateCacheCompileContext(time);
			//Check to see if any elements are affected
			if (elementsAffected != null && elementsAffected.Any())
			{
				VixenSystem.Elements.Update(elementsAffected);
				_lastUpdateClearedStates = false;
				VixenSystem.Filters.Update();
			}
			else if (!_lastUpdateClearedStates)
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

			return outputCommands;
		}
		
		#endregion	
	}
}
