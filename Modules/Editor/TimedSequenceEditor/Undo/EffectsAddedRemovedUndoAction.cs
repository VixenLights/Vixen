using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class EffectsAddedRemovedUndoAction : Common.Controls.UndoAction
	{
		private TimedSequenceEditorForm m_form;
		private readonly Dictionary<EffectNode, ILayer> m_effectNodes= new Dictionary<EffectNode, ILayer>(); 
		private readonly int m_count;

		public EffectsAddedRemovedUndoAction(TimedSequenceEditorForm form, IEnumerable<EffectNode> nodes)
		{
			m_form = form;
			var layerManager = form.Sequence.GetSequenceLayerManager();
			foreach (var effectNode in nodes)
			{
				m_effectNodes.Add(effectNode, layerManager.GetLayer(effectNode));	
			}
			
			m_count = m_effectNodes.Count();
		}

		protected void removeEffects()
		{
			foreach (var node in m_effectNodes)
				m_form.RemoveEffectNodeAndElement(node.Key);
		}

		protected void addEffects()
		{
			foreach (var node in m_effectNodes)
				m_form.AddEffectNode(node.Key);
		}

		protected int Count
		{
			get { return m_count; }
		}
	}
}