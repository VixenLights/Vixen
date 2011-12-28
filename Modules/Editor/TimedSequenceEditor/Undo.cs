using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommonElements;
using CommonElements.Timeline;
using Vixen.Module.Effect;

namespace VixenModules.Editor.TimedSequenceEditor
{
    public class ElementsTimeChangedUndoAction : CommonElements.UndoAction
    {
        private Dictionary<Element, ElementTimeInfo> m_changedElements;
        private ElementMoveType m_moveType;

        public ElementsTimeChangedUndoAction(Dictionary<Element, ElementTimeInfo> changedElements, ElementMoveType moveType)
            :base()
        {
            m_changedElements = changedElements;
            m_moveType = moveType;
        }



        public override void Undo()
        {
            foreach (KeyValuePair<Element,ElementTimeInfo> e in m_changedElements)
            {
                // Key is reference to actual element. Value is class with its times before move.
                // Swap the element's times with the saved times from before the move, so we can restore them later in redo.
                Element.SwapTimes(e.Key, e.Value);
            }

            base.Undo();
        }

        public override void Redo()
        {
            foreach (KeyValuePair<Element, ElementTimeInfo> e in m_changedElements)
            {
                // Key is reference to actual element. Value is class with the times before undo.
                // Swap the element's times with the saved times from before the undo, essentially re-doing the original action.
                Element.SwapTimes(e.Key, e.Value);
            }

            base.Redo();
        }

        public override string Description
        {
            get
            {
                string typestr = string.Empty;
                switch (m_moveType)
                {
                    case ElementMoveType.Move:      typestr = "Move"; break;
                    case ElementMoveType.Resize:    typestr = "Resize"; break;
                }
                return String.Format("{0} {1} element{2}", typestr,
                    m_changedElements.Count, (m_changedElements.Count == 1 ? "" : "s"));
			}

        }

    }



	// Undo adding elements by removing them.
	public class ElementsAddedUndoAction : CommonElements.UndoAction
	{
		private TimedSequenceEditorForm m_form;
		private IEnumerable<Element> m_elements;

		public ElementsAddedUndoAction(TimedSequenceEditorForm form, IEnumerable<Element> elements)
		{
			m_form = form;
			m_elements = elements;
		}

		public override void Undo()
        {

        }

        public override void Redo()
        {

        }

		public override string Description
		{
			get { return "Remove X elements"; }
		}
	}




	public class EffectsRemovedUndoAction : CommonElements.UndoAction
	{
		private TimedSequenceEditorForm m_form;
		private IEnumerable<IEffectModuleInstance> m_effects;

		public EffectsRemovedUndoAction(TimedSequenceEditorForm form, IEnumerable<IEffectModuleInstance> effects)
		{
			m_form = form;
			m_effects = effects;
		}

		public override void Undo()
        {
			foreach (var e in m_effects)
			{

			}
        }

        public override void Redo()
        {

        }

		public override string Description
		{
			get { return "Remove X elements"; }
		}
	}
 
}
