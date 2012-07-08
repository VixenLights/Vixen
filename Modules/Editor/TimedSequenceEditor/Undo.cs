using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommonElements;
using CommonElements.Timeline;
using Vixen.Module.Effect;
using Vixen.Sys;

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
                ElementTimeInfo.SwapTimes(e.Key, e.Value);
            }

            base.Undo();
        }

        public override void Redo()
        {
            foreach (KeyValuePair<Element, ElementTimeInfo> e in m_changedElements)
            {
                // Key is reference to actual element. Value is class with the times before undo.
                // Swap the element's times with the saved times from before the undo, essentially re-doing the original action.
				ElementTimeInfo.SwapTimes(e.Key, e.Value);
            }

            base.Redo();
        }

        public override string Description
        {
            get
            {
				string s = (m_changedElements.Count == 1 ? "" : "s");
				switch (m_moveType)
				{
					case ElementMoveType.Move:
						return String.Format("Move {0} effect{1} horizontally", m_changedElements.Count, s);
					case ElementMoveType.Resize:
						return String.Format("Resize {0} effect{1}", m_changedElements.Count, s);
					default:
						throw new Exception("Unknown ElementMoveType!");
				}
			}
        }

    }




	public class EffectsAddedRemovedUndoAction : CommonElements.UndoAction
	{
		private TimedSequenceEditorForm m_form;
		private IEnumerable<EffectNode> m_effectNodes;
		private int m_count;

		public EffectsAddedRemovedUndoAction(TimedSequenceEditorForm form, IEnumerable<EffectNode> nodes)
		{
			m_form = form;
			m_effectNodes = nodes;
			m_count = m_effectNodes.Count();
		}

		protected void removeEffects()
		{
			foreach (var node in m_effectNodes)
				m_form.RemoveEffectNodeAndElement(node);
		}

		protected void addEffects()
		{
			foreach (var node in m_effectNodes)
				m_form.AddEffectNode(node);
		}

		protected int Count { get { return m_count; } }
	}

	public class EffectsAddedUndoAction : EffectsAddedRemovedUndoAction
	{
		public EffectsAddedUndoAction(TimedSequenceEditorForm form, IEnumerable<EffectNode> nodes)
			: base(form, nodes)
		{
		}

		public override void Undo()
		{
			removeEffects();
			base.Undo();
		}

		public override void Redo()
		{
			addEffects();
			base.Redo();
		}

		public override string Description
		{
			get { return String.Format("Added {0} effect{1}", Count, (Count==1 ? "" : "s")); }
		}
	}

	public class EffectsRemovedUndoAction : EffectsAddedRemovedUndoAction
	{
		public EffectsRemovedUndoAction(TimedSequenceEditorForm form, IEnumerable<EffectNode> nodes)
			: base(form, nodes)
		{
		}

		public override void Undo()
		{
			addEffects();
			base.Undo();
		}

		public override void Redo()
		{
			removeEffects();
			base.Redo();
		}

		public override string Description
		{
			get { return String.Format("Removed {0} effect{1}", Count, (Count == 1 ? "" : "s")); }
		}
	}

 
}
