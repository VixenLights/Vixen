using System;
using System.Collections.Generic;

namespace CommonElements
{
	public class UndoManager
	{
		private Stack<UndoAction> m_undoable = new Stack<UndoAction>();
		private Stack<UndoAction> m_redoable = new Stack<UndoAction>();

		public IEnumerable<UndoAction> UndoActions
		{
			get { return m_undoable; }
		}
		public IEnumerable<UndoAction> RedoActions
		{
			get { return m_redoable; }
		}

		/// <summary>
		/// The number of undoable actions in the manager.
		/// </summary>
		public int NumUndoable { get { return m_undoable.Count; } }

		/// <summary>
		/// The number of redoable actions in the manager.
		/// </summary>
		public int NumRedoable { get { return m_redoable.Count; } }


		public void AddUndoAction(UndoAction action)
		{
			m_undoable.Push(action);
		}

		public void Undo()
		{
			if (NumUndoable == 0)
				return;

			UndoAction item = m_undoable.Pop();
			item.Undo();
			m_redoable.Push(item);
		}

		public void Undo(int n)
		{
			if ((n < 1) || (n > NumUndoable))
				throw new ArgumentOutOfRangeException("n");

			for (int i = 0; i < n; i++)
				Undo();
		}


		public void Redo()
		{
			if (NumRedoable == 0)
				return;

			UndoAction item = m_redoable.Pop();
			item.Redo();
			// don't put it back on the undo stack - we expect it to be re-added (through events, etc)
		}

		public void Redo(int n)
		{
			if ((n < 1) || (n > NumRedoable))
				throw new ArgumentOutOfRangeException("n");

			for (int i = 0; i < n; i++)
				Redo();
		}
	}
}