using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class EffectsAddedRemovedUndoAction : Common.Controls.UndoAction
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

		protected int Count
		{
			get { return m_count; }
		}
	}
}