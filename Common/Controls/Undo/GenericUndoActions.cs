using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;


namespace CommonElements
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