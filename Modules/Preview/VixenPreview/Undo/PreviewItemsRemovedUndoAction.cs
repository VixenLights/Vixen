using System.Collections.Generic;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Editor.VixenPreviewSetup3.Undo
{
	public class PreviewItemsRemovedUndoAction : PreviewItemsAddedRemovedUndoAction
	{
		public PreviewItemsRemovedUndoAction(VixenPreviewControl form, List<DisplayItem> items)
			: base(form, items)
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
			get { return string.Format("Removed {0} ", Count); }
		}
	}
}