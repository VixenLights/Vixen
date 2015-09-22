using System;
using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys.Output;

namespace Vixen.Sys
{
	public class OutputStateList
	{

		private readonly Dictionary<Guid, ICommand> _outputStateList;

		public OutputStateList()
		{
			_outputStateList = new Dictionary<Guid, ICommand>();
		}

		public void Clear()
		{
			_outputStateList.Clear();
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
			_outputStateList[commandOutput.Id] = commandOutput.Command;
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

		public ICommand GetCommandForOutput(Guid id)
		{
			ICommand command;
			_outputStateList.TryGetValue(id, out command);
			return command;
		}

		private void _AddOutput(CommandOutput commandOutput)
		{
			_outputStateList.Add(commandOutput.Id, commandOutput.Command);
		}
	}
}
