using System;
using System.Reflection;

namespace Common.Controls.Undo
{
	public class ModifyItemUndoAction : UndoAction
	{
		public ModifyItemUndoAction(Object obj, PropertyInfo property, Object oldValue)
		{
			Object = obj;
			Property = property;
			OldValue = oldValue;
		}

		public Object Object { get; private set; }
		public PropertyInfo Property { get; private set; }

		/// <summary>
		/// Gets the value of the property before this action.
		/// </summary>
		public Object OldValue { get; private set; }


		public override void Undo()
		{
			object temp = Property.GetValue(Object, null);
			Property.SetValue(Object, OldValue, null);
			OldValue = temp;

			base.Undo();
		}

		public override void Redo()
		{
			Property.SetValue(Object, OldValue, null);
			base.Redo();
		}
	}
}