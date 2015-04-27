using System.Collections.Generic;
using Vixen.Sys;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
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
			get { return string.Format("Added {0} effect{1}", Count, (Count == 1 ? string.Empty : "s")); }
		}
	}
}