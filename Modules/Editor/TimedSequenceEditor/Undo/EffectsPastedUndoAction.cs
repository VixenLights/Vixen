using System.Collections.Generic;
using Vixen.Sys;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class EffectsPastedUndoAction : EffectsAddedRemovedUndoAction
	{
		public EffectsPastedUndoAction(TimedSequenceEditorForm form, IEnumerable<EffectNode> nodes)
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
			get { return string.Format("Paste {0} effect{1}", Count, (Count == 1 ? string.Empty : "s")); }
		}
	}
}