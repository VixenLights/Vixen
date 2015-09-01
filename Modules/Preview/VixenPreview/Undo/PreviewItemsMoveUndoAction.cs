using System;
using System.Collections.Generic;
using Common.Controls;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Editor.VixenPreviewSetup3.Undo
{
	public class PreviewItemsMoveUndoAction : UndoAction
	{
		private Dictionary<DisplayItem, VixenPreviewControl.PreviewItemPositionInfo> m_changedPreviewItem;
		private VixenPreviewControl m_form;

		public PreviewItemsMoveUndoAction(VixenPreviewControl form, Dictionary<DisplayItem, VixenPreviewControl.PreviewItemPositionInfo> ChangedPreviewItems)
		{
			m_changedPreviewItem = ChangedPreviewItems;
			m_form = form;
		}

		public override void Undo()
		{
			m_form.resizePreviewItems(m_changedPreviewItem);
			m_form.ResizeSwapPlaces(m_changedPreviewItem);
			base.Undo();
		}

		public override void Redo()
		{
			m_form.resizePreviewItems(m_changedPreviewItem);
			m_form.ResizeSwapPlaces(m_changedPreviewItem);
			base.Redo();
		}

		public override string Description
		{
			get
			{
				return string.Format("Move {0} ", m_changedPreviewItem.Count);
			}
		}
	}
}