using System.Collections.Generic;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Editor.VixenPreviewSetup3.Undo
{
	public class ElementsSeparateGroupUndoAction : ElementsAddedSeparateGroupUndoAction
	{
		public ElementsSeparateGroupUndoAction(VixenPreviewControl form, List<DisplayItem> items, DisplayItem newDisplayItem)
			: base(form, items, newDisplayItem)
		{
		}

		public override void Undo()
		{
			addGroupEffects();
			base.Undo();
		}

		public override void Redo()
		{
			removeEffects();
			base.Redo();
		}

		public override string Description
		{
			get { return string.Format("Separate Group"); }
		}
	}
}