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
            OnUndoItemsChanged();
		}

		public void Undo(int n = 1)
		{
			if ((n < 1) || (n > NumUndoable))
				throw new ArgumentOutOfRangeException("n");

            for (int i = 0; i < n; i++)
            {
                UndoAction item = m_undoable.Pop();
                item.Undo();
                m_redoable.Push(item);
            }
            OnUndoItemsChanged();
            OnRedoItemsChanged();
		}

		public void Redo(int n = 1)
		{
			if ((n < 1) || (n > NumRedoable))
				throw new ArgumentOutOfRangeException("n");

            bool undo_changed = false;

            for (int i = 0; i < n; i++)
            {
                UndoAction item = m_redoable.Pop();
                item.Redo();
                
                // Initially planned on not putting it back on the undo stack, and instead expecting
                // it to eventually be re-added by the client (through events, etc).  However, since
                // deciding that element events will *not* be forwarded up through the control, it
                // might make sense to just re-add the undo action.
                m_undoable.Push(item);
                undo_changed = true;
            }

            if (undo_changed)
                OnUndoItemsChanged();
            OnRedoItemsChanged();
		}


        public event EventHandler UndoItemsChanged;
        public event EventHandler RedoItemsChanged;

        protected void OnUndoItemsChanged()
        {
            if (UndoItemsChanged != null)
                UndoItemsChanged(this, EventArgs.Empty);
        }

        protected void OnRedoItemsChanged()
        {
            if (RedoItemsChanged != null)
                RedoItemsChanged(this, EventArgs.Empty);
        }

	}
}