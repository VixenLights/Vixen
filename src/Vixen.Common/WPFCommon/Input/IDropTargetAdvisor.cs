using System.Windows;

namespace Common.WPFCommon.Input
{
	public interface IDropTargetAdvisor
	{
		UIElement TargetUI { get; set; }

		bool ApplyMouseOffset { get; }

		/// <summary>
		/// Gets the drag-and-drop effects that this target accepts.
		/// </summary>
		/// <value>A bitwise combination of the <see cref="DragDropEffects" /> enumeration values that specifies the operations this target permits.</value>
		DragDropEffects AcceptedEffects { get; }

		bool IsValidDataObject(IDataObject obj);
		void OnDropCompleted(IDataObject obj, Point dropPoint);
		UIElement GetVisualFeedback(IDataObject obj);
		UIElement GetTopContainer();
	}
}
