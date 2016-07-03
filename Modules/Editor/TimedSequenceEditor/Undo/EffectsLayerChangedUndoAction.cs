using System.Collections.Generic;
using System.Linq;
using Common.Controls;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class EffectsLayerChangedUndoAction: UndoAction
	{
		private readonly TimedSequenceEditorForm _form;
		private readonly Dictionary<IEffectNode, ILayer> _effectNodes;
		
		public EffectsLayerChangedUndoAction(TimedSequenceEditorForm form, Dictionary<IEffectNode, ILayer> nodes)
		{
			_form = form;
			_effectNodes = nodes;
		}

		public override void Undo()
		{
			_form.SwapLayers(_effectNodes);
			base.Undo();
		}

		public override void Redo()
		{
			_form.SwapLayers(_effectNodes);
			base.Redo();
		}
		
		public override string Description
		{
			get { return string.Format("Modified {0} effect layer{1}", Count, (Count == 1 ? string.Empty : "s")); }
		}

		protected int Count
		{
			get { return _effectNodes.Count; }
		}
	}
}
