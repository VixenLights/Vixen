using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Windows.Forms.VisualStyles;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Sys.Instrumentation;
using Vixen.Sys.Output;

namespace Vixen.Sys
{
	public class PreCompileExecution
	{
		private static Object lockObject = new Object();
		private bool lastUpdateClearedStates = false;

		private MillisecondsValue _contextUpdate = new MillisecondsValue("Context Update");

		public PreCompileExecution()
		{
			VixenSystem.Instrumentation.AddValue(_contextUpdate);
		}

		/// <summary>
		/// Compile an entire sequence. Prolly a better way to get here, but works for POC.
		/// </summary>
		/// <param name="sequence"></param>
		public void CompileSequence(ISequence sequence)
		{
			Execution.CloseExecution(); //Stop the whole engine so nothing interferes with us or make a pause
			
			//Special context to pre cache commands. We don't need all the other fancy executor or timing as we will advance it ourselves
			CacheCompileContext context = VixenSystem.Contexts.GetCacheCompileContext();
			context.Sequence = sequence;
			context.Start();
			TimeSpan currentTime = TimeSpan.Zero;

			Dictionary<TimeSpan, List<CommandOutput>> commandIntervals = new Dictionary<TimeSpan, List<CommandOutput>>();
			while (currentTime <= sequence.Length)
			{
				List<CommandOutput> commands = UpdateState(currentTime);
				commandIntervals.Add(currentTime,commands);
				//Advance by the default interval. I was using 50ms for ease of visualizing the slices.
				currentTime = currentTime.Add(TimeSpan.FromMilliseconds(VixenSystem.DefaultUpdateInterval));
			}

			context.Stop();
			Execution.OpenExecution();
			
		
			//Now do something with the commandIntervals. Save to disc along with sequence so they can be used later.
			//Figure out file format
			
		}


		public List<CommandOutput> UpdateState(TimeSpan time)
		{
			var outputCommands = new List<CommandOutput>();
			lock (lockObject)
			{
				//Advance our context to specified time and do all the normal update stuff
				Stopwatch sw = Stopwatch.StartNew();
				HashSet<Guid> elementsAffected = VixenSystem.Contexts.UpdateCacheCompileContext(time);
				_contextUpdate.Set(sw.ElapsedMilliseconds);
				if (elementsAffected != null && elementsAffected.Any())
				{
					VixenSystem.Elements.Update(elementsAffected);
					lastUpdateClearedStates = false;
					VixenSystem.Filters.Update();
				}
				else if(!lastUpdateClearedStates)
				{
					VixenSystem.Elements.ClearStates();
					lastUpdateClearedStates = true;
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

	}
}
