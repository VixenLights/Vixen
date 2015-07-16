using System;
using System.Collections;

namespace Common.Controls.Undo
{
	/// <summary>
	/// Represents an action that removed something from a list.
	/// Undone by re-adding the item to that list.
	/// </summary>
	public class RemoveFromListUndoAction : UndoAction
	{
		public RemoveFromListUndoAction(Object obj, IList list)
		{
			Object = obj;
			List = list;
		}

		public Object Object { get; private set; }
		public IList List { get; private set; }

		public override void Undo()
		{
			List.Add(Object);
			base.Undo();
		}

		public override void Redo()
		{
			List.Remove(Object);
			base.Redo();
		}
	}
}