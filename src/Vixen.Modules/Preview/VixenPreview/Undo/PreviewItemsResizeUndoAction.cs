using Common.Controls;
using VixenModules.Preview.VixenPreview;
using VixenModules.Preview.VixenPreview.Shapes;
using VixenModules.Preview.VixenPreview.Undo;

namespace VixenModules.Editor.VixenPreviewSetup3.Undo
{
	public class PreviewItemsResizeUndoAction : UndoAction
	{
		private Dictionary<DisplayItem, PreviewItemPositionInfo> m_changedPreviewItems;
		private VixenPreviewControl m_form;

		public PreviewItemsResizeUndoAction(VixenPreviewControl form, Dictionary<DisplayItem, PreviewItemPositionInfo> ChangedPreviewItems)
		{
			m_changedPreviewItems = ChangedPreviewItems;
			m_form = form;
		}

		public override void Undo()
		{
			m_form.Resize_MoveSwapPlaces(m_changedPreviewItems);
			base.Undo();
		}

		public override void Redo()
		{
			m_form.Resize_MoveSwapPlaces(m_changedPreviewItems);
			base.Redo();
		}

		public override string Description
		{
			get
			{
				return string.Format("Resize 1");
			}
		}
	}
}