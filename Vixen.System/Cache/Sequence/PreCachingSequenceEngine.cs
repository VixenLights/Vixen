using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vixen.Commands;
using Vixen.Execution.Context;
using Vixen.IO.Loader;
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
		private CommandHandler _commandHandler = new CommandHandler();

		/// <summary>
		/// Create using the default update interval
		/// </summary>
		public PreCachingSequenceEngine()
		{
			Interval = VixenSystem.DefaultUpdateInterval;
		}

		/// <summary>
		/// Create using a specified update interval.
		/// </summary>
		/// <param name="interval"></param>
		public PreCachingSequenceEngine(int interval)
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
			//Need some hooks in here for progess....... Need to think about that.
			//Stop the output devices from driving the execution engine.
			VixenSystem.OutputDeviceManagement.PauseAll();
			
			//Special context to pre cache commands. We don't need all the other fancy executor or timing as we will advance it ourselves
			PreCachingSequenceContext context = VixenSystem.Contexts.GetCacheCompileContext();
			context.Sequence = sequence;
			context.Start();
			TimeSpan currentTime = TimeSpan.Zero;
			var cache = GetCache(sequence);
			
			while (currentTime <= sequence.Length)
			{
				List<CommandOutput> commands = UpdateState(currentTime);
				var commandBytes = ExtractCommands(commands);
				cache.AppendData(commandBytes.Values.ToList());
				if (cache.Outputs == null) //Figure out a better way.
				{
					cache.Outputs = commandBytes.Keys.ToList();
				}
				//Advance by the default interval. I was using 50ms for ease of visualizing the slices.
				currentTime = currentTime.Add(TimeSpan.FromMilliseconds(Interval));
			}

			context.Stop();
			//restart the devices
			VixenSystem.OutputDeviceManagement.ResumeAll();
			
			cache.Save();
			// To load the cache use the following
			//ISequenceCache cache2 = SequenceService.Instance.LoadCache(cache.SequenceFilePath);
			
		}

		private ISequenceCache GetCache(ISequence sequence)
		{
			ISequenceCache cache = SequenceService.Instance.CreateNewCache(sequence.FilePath);
			cache.Length = sequence.Length;
			cache.Interval = Interval;
			return cache;
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

		private Dictionary<Guid,byte> ExtractCommands(List<CommandOutput> commandOutputs)
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

	}
}
