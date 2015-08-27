using System.Collections.Generic;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Editor.VixenPreviewSetup3.Undo
{
	public class ElementsAddedGroupUndoAction : ElementsAddedSeparateGroupUndoAction
	{
		public ElementsAddedGroupUndoAction(VixenPreviewControl form, List<DisplayItem> items, DisplayItem newDisplayItem)
			: base(form, items, newDisplayItem)
		{
		}

		public override void Undo()
		{
			removeEffects();
			base.Undo();
		}

		public override void Redo()
		{
			addGroupEffects();
			base.Redo();
		}

		public override string Description
		{
			get { return string.Format("Added Group"); }
		}
	}
}