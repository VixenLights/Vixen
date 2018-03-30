using System.Collections.Generic;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Preview.VixenPreview.Undo
{
	public class PreviewItemPositionInfo
	{
		public PreviewItemPositionInfo(DisplayItem previewItem)
		{
			TopPosition = previewItem.Shape.Top;
			LeftPosition = previewItem.Shape.Left;
			List<DisplayItem> temp = new List<DisplayItem>();
			temp.Add(previewItem);
			OriginalPreviewItem = new List<string>();
			OriginalPreviewItem.Add(PreviewTools.SerializeToString(temp));
		}

		public List<string> OriginalPreviewItem { get; set; }

		public int TopPosition { get; set; }
		public int LeftPosition { get; set; }
	}
}
