using System.Collections.Generic;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Preview.VixenPreview.Undo
{
	public class PreviewItemResizeMoveInfo
	{
		public PreviewItemResizeMoveInfo(List<DisplayItem> modifyingElements)
		{
			OriginalPreviewItem = new Dictionary<DisplayItem, PreviewItemPositionInfo>();

			foreach (var previewItem in modifyingElements)
			{
				if (OriginalPreviewItem.ContainsKey(previewItem))
				{
					OriginalPreviewItem[previewItem] = new PreviewItemPositionInfo(previewItem);
				}
				else
				{
					OriginalPreviewItem.Add(previewItem, new PreviewItemPositionInfo(previewItem));
				}
			}
		}

		///<summary>The point on the grid where the mouse first went down.</summary>
		//		public PreviewPoint InitialGridLocation { get; private set; }

		///<summary>All elements being modified and their original parameters.</summary>
		public Dictionary<DisplayItem, PreviewItemPositionInfo> OriginalPreviewItem { get; private set; }
	}
}
