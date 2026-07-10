namespace Common.Controls
{
	public enum UndoState
	{
		ReadyForUndo,
		ReadyForRedo
	}

	public abstract class UndoAction
	{
		private UndoState m_state = UndoState.ReadyForUndo;

		protected UndoState State
		{
			get { return m_state; }
		}

		public virtual void Undo()
		{
			m_state = UndoState.ReadyForRedo;
		}

		public virtual void Redo()
		{
			m_state = UndoState.ReadyForUndo;
		}

		public virtual string Description
		{
			get { return "[UndoAction]"; }
		}
	}
}