
namespace CommonElements
{
	internal enum UndoState { None, Undone, Redone }

	public abstract class UndoAction
	{
		private UndoState state = UndoState.None;
		public virtual void Undo()
		{
			state = UndoState.Undone;
		}

		public virtual void Redo()
		{
			state = UndoState.Redone;
		}
	}

}