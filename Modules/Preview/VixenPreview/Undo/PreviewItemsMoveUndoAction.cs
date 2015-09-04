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
		private Dictionary<DisplayItem, VixenPreviewControl.PreviewItemPositionInfo> m_changedPreviewItems;
		private VixenPreviewControl m_form;

		public PreviewItemsMoveUndoAction(VixenPreviewControl form, Dictionary<DisplayItem, VixenPreviewControl.PreviewItemPositionInfo> ChangedPreviewItems)
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
				return string.Format("Move {0} ", m_changedPreviewItems.Count);
			}
		}
	}
}