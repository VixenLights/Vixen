using Common.Controls;
using VixenModules.Preview.VixenPreview;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Editor.VixenPreviewSetup3.Undo
{
	public class PreviewItemsLockUndoAction : UndoAction
	{
		private List<DisplayItem> m_changedPreviewItems;
		private VixenPreviewControl m_form;

		public PreviewItemsLockUndoAction(VixenPreviewControl form, List<DisplayItem> ChangedPreviewItems)
		{
			m_changedPreviewItems = new List<DisplayItem>();
			foreach ( var previewItems in ChangedPreviewItems)
			{
				m_changedPreviewItems.Add((DisplayItem)previewItems.Clone());
			}
			m_form = form;
		}

		public PreviewItemsLockUndoAction(VixenPreviewControl form, DisplayItem ChangedPreviewItems)
		{
			m_changedPreviewItems = new List<DisplayItem>( );
			m_changedPreviewItems.Add((DisplayItem)ChangedPreviewItems.Clone());
			m_form = form;
		}

		public override void Undo()
		{
			m_form.Undo_Lock(m_changedPreviewItems);
			base.Undo();
		}

		public override void Redo()
		{
			m_form.Undo_Lock(m_changedPreviewItems, false);
			base.Redo();
		}

		public override string Description
		{
			get
			{
				return string.Format($"Lock {m_changedPreviewItems.Count} props" );
			}
		}
	}
}