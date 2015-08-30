using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using Common.Controls.ControlsEx.ListControls;

namespace Common.Controls
{
	public class UndoManager
	{
		/* 
         * Since we need to be able to remove items from the bottom of the stack
         * (internally, in order to limit the number of items on the stack), we
         * have to use a LinkedList<T> instead of an actual Stack<T>. For our purposes:
         *    Top of stack == Front of list
         *    Bottom of stack == End of list.
         */
		public LinkedList<UndoAction> m_undoable = new LinkedList<UndoAction>();

		/*
         * The redo stack however, can be a simple Stack<T>. Because items can only
         * make it onto the redo stack if they came from the undo stack, we are guaranteed
         * to only reach the same maximum number of items.
         */
		public Stack<UndoAction> m_redoable = new Stack<UndoAction>();

		private const int DefaultMaxItems = 1000;

		public UndoManager() : this(DefaultMaxItems)
		{
		}

		public UndoManager(int maxItems)
		{
			MaxItems = maxItems;
		}

		#region Proprerties

		///<summary>Gets or sets the maximum number of items that the undo manager will track.</summary>
		public int MaxItems { get; set; }

		/// <summary>Gets a list of the undo actions available in the manager.</summary>
		public IEnumerable<UndoAction> UndoActions
		{
			get { return m_undoable; }
		}

		/// <summary>Gets a list of the redo actions available in the manager.</summary>
		public IEnumerable<UndoAction> RedoActions
		{
			get { return m_redoable; }
		}

		/// <summary>The number of undoable actions in the manager.<summary>
		public int NumUndoable
		{
			get { return m_undoable.Count; }
		}

		/// <summary>The number of redoable actions in the manager.</summary>
		public int NumRedoable
		{
			get { return m_redoable.Count; }
		}

		#endregion

		#region Public Methods

		public void AddUndoAction(UndoAction action)
		{
			_pushOntoUndoStack(action);
			OnUndoItemsChanged();
		}

		public void Undo(int n = 1)
		{
			if ((n < 1) || (n > NumUndoable))
				throw new ArgumentOutOfRangeException("n");

			for (int i = 0; i < n; i++) {
				UndoAction item = _popFromUndoStack();
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
			for (int i = 0; i < n; i++) {
				UndoAction item = m_redoable.Pop();
				
				item.Redo();

				// Initially planned on not putting it back on the undo stack, and instead expecting
				// it to eventually be re-added by the client (through events, etc).  However, since
				// deciding that element events will *not* be forwarded up through the control, it
				// might make sense to just re-add the undo action.
				_pushOntoUndoStack(item);
				undo_changed = true;
			}

			if (undo_changed)
				OnUndoItemsChanged();
			OnRedoItemsChanged();
		}

		#endregion

		#region Private Methods

		public void _pushOntoUndoStack(UndoAction action)
		{
			// Push the item onto the top of the stack.
			m_undoable.AddFirst(action);

			if (m_undoable.Count > MaxItems) {
				// Remove one item from the bottom of the stack.
				m_undoable.RemoveLast();
			}
		}

		private UndoAction _popFromUndoStack()
		{
			// Pop the item from the top of the stack.
			UndoAction action = m_undoable.First.Value;
			m_undoable.RemoveFirst();
			return action;
		}

		#endregion

		#region Events

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

		#endregion
	}
}