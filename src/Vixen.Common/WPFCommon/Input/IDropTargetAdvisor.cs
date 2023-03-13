using System.Windows;

namespace Common.WPFCommon.Input
{
	public interface IDropTargetAdvisor
	{
		UIElement TargetUI { get; set; }

		bool ApplyMouseOffset { get; }
		bool IsValidDataObject(IDataObject obj);
		void OnDropCompleted(IDataObject obj, Point dropPoint);
		UIElement GetVisualFeedback(IDataObject obj);
		UIElement GetTopContainer();
	}
}