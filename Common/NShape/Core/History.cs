/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Dataweb.NShape.Commands;


namespace Dataweb.NShape.Advanced
{
	/// <summary>
	/// Provides data for events concerning a <see cref="T:Dataweb.NShape.ICommand" />.
	/// </summary>
	public class CommandEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.CommandEventArgs" />.
		/// </summary>
		public CommandEventArgs(ICommand command, bool reverted)
			: this()
		{
			if (command == null) throw new ArgumentNullException("command");
			this.command = command;
			this.reverted = reverted;
		}


		/// <summary>
		/// Gets the <see cref="T:Dataweb.NShape.ICommand"></see> processed by the action that raised the event.
		/// </summary>
		public ICommand Command
		{
			get { return this.command; }
			internal set { command = value; }
		}


		/// <summary>
		/// Specifies if the <see cref="T:Dataweb.NShape.ICommand" /> was reverted.
		/// </summary>
		public bool Reverted
		{
			get { return reverted; }
			internal set { reverted = value; }
		}


		internal CommandEventArgs()
		{
		}


		private ICommand command = null;
		private bool reverted = false;
	}


	/// <summary>
	/// Provides data for events concerning a collection of <see cref="T:Dataweb.NShape.ICommand" />.
	/// </summary>
	public class CommandsEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.CommandsEventArgs" />.
		/// </summary>
		public CommandsEventArgs(IEnumerable<ICommand> commands, bool reverted)
		{
			if (commands == null) throw new ArgumentNullException("commands");
			AddRange(commands);
		}


		/// <summary>
		/// Gets a <see cref="T:Dataweb.NShape.Advanced.IReadOnlyCollection" /> of <see cref="T:Dataweb.NShape.ICommand" /> processed by the action that raised the event.
		/// </summary>
		public IReadOnlyCollection<ICommand> Commands
		{
			get { return commands; }
		}


		/// <summary>
		/// Specifies if the command was reverted.
		/// </summary>
		public bool Reverted
		{
			get { return reverted; }
			internal set { reverted = value; }
		}


		internal CommandsEventArgs()
		{
		}


		internal void Add(ICommand command)
		{
			commands.Add(command);
		}


		internal void AddRange(IEnumerable<ICommand> commands)
		{
			this.Clear();
			this.commands.AddRange(commands);
		}


		internal void Clear()
		{
			commands.Clear();
		}


		private ReadOnlyList<ICommand> commands = new ReadOnlyList<ICommand>();
		private bool reverted;
	}


	/// <summary>
	/// Stores a sequence of commands for undo and redo operations.
	/// </summary>
	public class History
	{
		/// <summary>
		/// Constructs a new history.
		/// </summary>
		public History()
		{
			commands = new List<ICommand>(100);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public event EventHandler<CommandEventArgs> CommandAdded;


		///// <ToBeCompleted></ToBeCompleted>
		//public event EventHandler<CommandEventArgs> CommandExecuted;


		/// <ToBeCompleted></ToBeCompleted>
		public event EventHandler<CommandsEventArgs> CommandsExecuted;


		/// <summary>
		/// Starts collecting all added commands in an aggregated command.
		/// Aggregated commands can be undone with a single call of Undo()/Redo().
		/// While aggregating commands, the CommandAdded event will not be raised.
		/// </summary>
		public void BeginAggregatingCommands()
		{
			aggregatingCommands = true;
			aggregatedCommand = new AggregatedCommand();
		}


		/// <summary>
		/// End aggregation of commands and adds the AggregatedCommand to the History. 
		/// Raises a CommandAdded event.
		/// Does not execute the collected commands.
		/// </summary>
		public void EndAggregatingCommands()
		{
			aggregatingCommands = false;
			AddCommand(aggregatedCommand);
			aggregatedCommand = null;
		}


		/// <summary>
		/// Cancels the aggregation of commands. All collected commands will be undone and not added to the history.
		/// </summary>
		public void CancelAggregatingCommands()
		{
			if (aggregatingCommands) {
				aggregatingCommands = false;
				aggregatedCommand.Revert();
				aggregatedCommand = null;
			}
		}


		/// <summary>
		/// Returns descriptions of all available redo commands
		/// </summary>
		public IEnumerable<string> GetRedoCommandDescriptions(int count)
		{
			int stopIdx = currentPosition + count;
			if (stopIdx >= commands.Count) stopIdx = commands.Count - 1;
			for (int i = currentPosition + 1; i <= stopIdx; ++i)
				yield return commands[i].Description;
		}


		/// <summary>
		/// Returns descriptions of all available undo commands
		/// </summary>
		public IEnumerable<string> GetUndoCommandDescriptions(int count)
		{
			int stopIdx = currentPosition - count + 1;
			if (stopIdx < 0) stopIdx = 0;
			for (int i = currentPosition; i >= stopIdx; --i)
				yield return commands[i].Description;
		}


		/// <summary>
		/// Returns descriptions of all available redo commands
		/// </summary>
		public IEnumerable<string> GetRedoCommandDescriptions()
		{
			return GetRedoCommandDescriptions(RedoCommandCount);
		}


		/// <summary>
		/// Returns descriptions of all available undo commands
		/// </summary>
		public IEnumerable<string> GetUndoCommandDescriptions()
		{
			return GetUndoCommandDescriptions(UndoCommandCount);
		}


		/// <summary>
		/// Returns description of the next available redo command
		/// </summary>
		public string GetRedoCommandDescription()
		{
			ICommand cmd = GetNextCommand();
			if (cmd != null) return cmd.Description;
			else return string.Empty;
		}


		/// <summary>
		/// Returns description of the next available undo command
		/// </summary>
		public string GetUndoCommandDescription()
		{
			ICommand cmd = GetPreviousCommand();
			if (cmd != null) return cmd.Description;
			else return string.Empty;
		}


		/// <summary>
		/// Returns the number of commands that can be redone.
		/// </summary>
		public int RedoCommandCount
		{
			get
			{
				if (currentPosition < 0) return commands.Count;
				else return (commands.Count - 1) - currentPosition;
			}
		}


		/// <summary>
		/// Returns the number of commands that can be undone.
		/// </summary>
		public int UndoCommandCount
		{
			get { return currentPosition + 1; }
		}


		/// <summary>
		/// Indicates, whether the given command is the next one to undo.
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public bool IsNextUndoCommand(ICommand command)
		{
			if (command == null) throw new ArgumentNullException("command");
			return GetPreviousCommand() == command;
		}


		/// <summary>
		/// Undo the latest action
		/// </summary>
		public void Undo()
		{
			if (UndoCommandCount <= 0) throw new InvalidOperationException("There is no command to be undone");
			ICommand cmd = PerformUndo();
			if (CommandsExecuted != null)
				CommandsExecuted(this, GetCommandsEventArgs(cmd, true));
		}


		/// <summary>
		/// Undo the given number of commands at once.
		/// </summary>
		/// <param name="commandCount"></param>
		public void Undo(int commandCount)
		{
			commandsEventArgsBuffer.Clear();
			commandsEventArgsBuffer.Reverted = true;

			for (int i = 0; i < commandCount; ++i) {
				ICommand cmd = PerformUndo();
				commandsEventArgsBuffer.Add(cmd);
			}

			if (CommandsExecuted != null) CommandsExecuted(this, commandsEventArgsBuffer);
		}


		/// <summary>
		/// Redo the last undone action
		/// </summary>
		public void Redo()
		{
			if (RedoCommandCount <= 0) throw new InvalidOperationException("There is no command to be redone");
			ICommand cmd = PerformRedo();
			if (CommandsExecuted != null)
				CommandsExecuted(this, GetCommandsEventArgs(cmd, false));
		}


		/// <summary>
		/// Redo the given number of commands at once.
		/// </summary>
		/// <param name="commandCount"></param>
		public void Redo(int commandCount)
		{
			commandsEventArgsBuffer.Clear();
			commandsEventArgsBuffer.Reverted = true;

			for (int i = 0; i < commandCount; ++i) {
				ICommand cmd = PerformRedo();
				commandsEventArgsBuffer.Add(cmd);
			}

			if (CommandsExecuted != null) CommandsExecuted(this, commandsEventArgsBuffer);
		}


		/// <summary>
		/// Executes the given command and adds it to the Undo/Redo list
		/// </summary>
		/// <param name="command"></param>
		public void ExecuteAndAddCommand(ICommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

			command.Execute();
			if (CommandsExecuted != null)
				CommandsExecuted(this, GetCommandsEventArgs(command, false));

			AddCommand(command);
		}


		/// <summary>
		/// Adds a command to the History. The command will not be executed by this method.
		/// </summary>
		/// <param name="command"></param>
		public void AddCommand(ICommand command)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (aggregatingCommands) {
				Debug.Assert(aggregatedCommand != null);
				aggregatedCommand.Add(command);
			}
			else {
				int redoPos = currentPosition + 1;
				if (redoPos >= 0 && commands.Count > redoPos)
					commands.RemoveRange(redoPos, commands.Count - redoPos);
				commands.Add(command);
				currentPosition = commands.Count - 1;

				if (CommandAdded != null)
					CommandAdded(this, GetCommandEventArgs(command, false));
				// TODO 2: Remove oldest command, if list grows over its capacity.
			}
		}


		/// <summary>
		/// Clears all commands in the history.
		/// </summary>
		public void Clear()
		{
			commands.Clear();
			currentPosition = -1;
		}


		// Returns the previous command to undo
		private ICommand GetPreviousCommand()
		{
			ICommand result = null;
			if (currentPosition >= 0)
				result = commands[currentPosition];
			return result;
		}


		// Returns the next command to redo
		private ICommand GetNextCommand()
		{
			ICommand result = null;
			if (currentPosition < commands.Count - 1)
				result = commands[currentPosition + 1];
			return result;
		}


		private ICommand PerformUndo()
		{
			if (aggregatingCommands)
				EndAggregatingCommands();
			ICommand cmd = GetPreviousCommand();
			RevertCommand(cmd);
			return cmd;
		}


		private ICommand PerformRedo()
		{
			if (aggregatingCommands)
				EndAggregatingCommands();
			ICommand cmd = GetNextCommand();
			ExecuteCommand(cmd);
			return cmd;
		}


		private void ExecuteCommand(ICommand command)
		{
			Debug.Assert(command != null);
			Debug.Assert(commands.Contains(command));
			command.Execute();
			++currentPosition;
		}


		private void RevertCommand(ICommand command)
		{
			Debug.Assert(command != null);
			Debug.Assert(commands.Contains(command));
			command.Revert();
			--currentPosition;
		}


		private CommandEventArgs GetCommandEventArgs(ICommand command, bool reverted)
		{
			commandEventArgsBuffer.Command = command;
			commandEventArgsBuffer.Reverted = reverted;
			return commandEventArgsBuffer;
		}


		private CommandsEventArgs GetCommandsEventArgs(ICommand command, bool reverted)
		{
			commandsEventArgsBuffer.Clear();
			commandsEventArgsBuffer.Add(command);
			commandsEventArgsBuffer.Reverted = reverted;
			return commandsEventArgsBuffer;
		}

		#region Fields

		// Undo/redo list of commands
		private List<ICommand> commands;
		private int currentPosition = -1;
		private bool aggregatingCommands = false;
		private AggregatedCommand aggregatedCommand;

		private CommandEventArgs commandEventArgsBuffer = new CommandEventArgs();
		private CommandsEventArgs commandsEventArgsBuffer = new CommandsEventArgs();

		#endregion
	}
}