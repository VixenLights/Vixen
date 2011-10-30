using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;


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


        public void AddUndoAction(UndoAction action)
        {
            m_undoable.Push(action);
        }

        public void Undo()
        {
            if (m_undoable.Count == 0)
                return;

            UndoAction item = m_undoable.Pop();
            item.Undo();
            m_redoable.Push(item);
        }

        public void Undo(int n)
        {
            if ((n < 1) || (n > m_undoable.Count))
                throw new ArgumentOutOfRangeException("n");

            for (int i = 0; i < n; i++)
                Undo();
        }


        public void Redo()
        {
            if (m_redoable.Count == 0)
                return;

            UndoAction item = m_redoable.Pop();
            item.Redo();
            // don't put it back on the undo stack - we expect it to be re-added (through events, etc)
        }

        public void Redo(int n)
        {
            if ((n < 1) || (n > m_redoable.Count))
                throw new ArgumentOutOfRangeException("n");

            for (int i = 0; i < n; i++)
                Redo();
        }
    }

}