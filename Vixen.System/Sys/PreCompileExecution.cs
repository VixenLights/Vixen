using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Windows.Forms.VisualStyles;
using Vixen.Commands;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Sys.Instrumentation;
using Vixen.Sys.Output;

namespace Vixen.Sys
{
	/// <summary>
	/// This class is designed to create a cache of output commands for sequences. It follows the same
	/// processes as the normal execution engine only in a controlled step by step fashion.
	/// </summary>
	public class PreCompileExecution
	{
		private static readonly Object LockObject = new Object();
		private bool _lastUpdateClearedStates;

		/// <summary>
		/// Create using the default update interval
		/// </summary>
		public PreCompileExecution()
		{
			Interval = VixenSystem.DefaultUpdateInterval;
		}

		/// <summary>
		/// Create using a specified update interval.
		/// </summary>
		/// <param name="interval"></param>
		public PreCompileExecution(int interval)
		{
			Interval = interval;
		}

		public int Interval { get; set; }

		/// <summary>
		/// Create a cache of the entire sequence command outputs
		/// </summary>
		/// <param name="sequence"></param>
		public void CacheSequence(ISequence sequence)
		{
			//Stop the output devices from driving the execution engine.
			VixenSystem.OutputDeviceManagement.PauseAll();
			
			//Special context to pre cache commands. We don't need all the other fancy executor or timing as we will advance it ourselves
			PreCachingSequenceContext context = VixenSystem.Contexts.GetCacheCompileContext();
			context.Sequence = sequence;
			context.Start();
			TimeSpan currentTime = TimeSpan.Zero;

			Dictionary<TimeSpan, Dictionary<Guid,ICommand>> commandIntervals = new Dictionary<TimeSpan, Dictionary<Guid,ICommand>>();
			while (currentTime <= sequence.Length)
			{
				List<CommandOutput> commands = UpdateState(currentTime);
				commandIntervals.Add(currentTime,ExtractCommands(commands));
				//Advance by the default interval. I was using 50ms for ease of visualizing the slices.
				currentTime = currentTime.Add(TimeSpan.FromMilliseconds(Interval));
			}

			context.Stop();
			//restart the devices
			VixenSystem.OutputDeviceManagement.ResumeAll();
		
			//Now do something with the commandIntervals. Save to disc along with sequence so they can be used later.
			//Figure out file format, the id is the unique id of the controller output. Do we need to further qualify them by 
			//controller? The ICommand will be the type of command the controller uses or null if the output is not active on the slice.
			
		}


		private List<CommandOutput> UpdateState(TimeSpan time)
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

		private Dictionary<Guid,ICommand> ExtractCommands(List<CommandOutput> commandOutputs)
		{
			Dictionary<Guid,ICommand> commands = new Dictionary<Guid, ICommand>(commandOutputs.Count);
			foreach (var commandOutput in commandOutputs)
			{
				commands.Add(commandOutput.Id,commandOutput.Command);	
			}
			return commands;
		}

	}
}
