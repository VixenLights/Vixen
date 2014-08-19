using System;
using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys.Output;

namespace Vixen.Sys
{
	[Serializable]
	public class OutputStateListAggregator
	{
		private readonly Dictionary<Guid, List<ICommand>> _outputStateList;

		public OutputStateListAggregator()
		{
			_outputStateList = new Dictionary<Guid, List<ICommand>>();
		}

		/// <summary>
		/// Append a command to the list of commands for a given output controller.
		/// </summary>
		public void AddCommand(CommandOutput commandOutput)
		{
			if (!_outputStateList.ContainsKey(commandOutput.Id))
			{
				_AddOutput(commandOutput);	
			}
			_outputStateList[commandOutput.Id].Add(commandOutput.Command);
		}

		public void AddCommands(IEnumerable<CommandOutput> commands)
		{
			foreach (CommandOutput command in commands)
			{
				AddCommand(command);
			}
		}

		public IEnumerable<Guid> GetOutputIds()
		{
			return _outputStateList.Keys;
		}

		public IEnumerator<Guid> GetEnumerator()
		{
			return _outputStateList.Keys.GetEnumerator();	
		} 

		public IEnumerable<ICommand> GetCommandsForOutput(Guid id)
		{
			List<ICommand> commands;
			_outputStateList.TryGetValue(id, out commands);
			return commands;
		}

		private void _AddOutput(CommandOutput commandOutput)
		{
			_outputStateList.Add(commandOutput.Id,new List<ICommand>());
		}

	}
}
