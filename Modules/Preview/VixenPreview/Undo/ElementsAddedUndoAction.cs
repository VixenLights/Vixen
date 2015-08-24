using System.Collections.Generic;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Editor.VixenPreviewSetup3.Undo
{
	public class ElementsAddedUndoAction : ElementsAddedRemovedUndoAction
	{
		public ElementsAddedUndoAction(VixenPreviewControl form, IEnumerable<DisplayItem> items)
			: base(form, items)
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
			get { return string.Format("Added {0} ", Count); }
		}
	}
}