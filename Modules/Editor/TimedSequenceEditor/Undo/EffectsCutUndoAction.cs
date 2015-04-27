using System.Collections.Generic;
using Vixen.Sys;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class EffectsCutUndoAction : EffectsAddedRemovedUndoAction
	{
		public EffectsCutUndoAction(TimedSequenceEditorForm form, IEnumerable<EffectNode> nodes)
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
			get { return string.Format("Cut {0} effect{1}", Count, (Count == 1 ? string.Empty : "s")); }
		}
	}
}