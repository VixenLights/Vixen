using System;
using System.Collections;

namespace Common.Controls.Undo
{
	/// <summary>
	/// Represents an action that added something to a list.
	/// Undone by removing the item from that list.
	/// </summary>
	public class AddToListUndoAction : UndoAction
	{
		public AddToListUndoAction(Object obj, IList list)
		{
			Object = obj;
			List = list;
		}

		public Object Object { get; private set; }
		public IList List { get; private set; }

		public override void Undo()
		{
			List.Remove(Object);
			base.Undo();
		}

		public override void Redo()
		{
			List.Add(Object);
			base.Redo();
		}
	}
}