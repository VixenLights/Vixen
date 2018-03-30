using System.Collections.Generic;
using Common.Controls;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Preview.VixenPreview.Undo
{
	public class PreviewItemPixelSizeChangeUndoAction : UndoAction
	{
		private readonly List<DisplayItem> _changedPreviewItems;
		private readonly PreviewItemPixelSizeInfo _itemPixelSizeInfo;
		private readonly VixenPreviewControl _previewForm;

		public PreviewItemPixelSizeChangeUndoAction(VixenPreviewControl form, List<DisplayItem> changedPreviewItems, PreviewItemPixelSizeInfo info)
		{
			_changedPreviewItems = changedPreviewItems;
			_itemPixelSizeInfo = info;
			_previewForm = form;
		}

		public override void Undo()
		{
			_previewForm.PixelResizeSwapPlaces(_changedPreviewItems, _itemPixelSizeInfo);
			base.Undo();
		}

		public override void Redo()
		{
			_previewForm.PixelResizeSwapPlaces(_changedPreviewItems, _itemPixelSizeInfo);
			base.Redo();
		}

		public override string Description => $"Pixel size on {_changedPreviewItems.Count} items.";
	}
}
