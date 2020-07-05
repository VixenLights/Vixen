using System.Windows;
using System.Windows.Media;

namespace VixenModules.Editor.PolygonEditor.Adorners
{
	/// <summary>
	/// Defines an interface so that the ResizeAdorner can interact with a view model.
	/// </summary>
	public interface IResizeable
	{
		/// <summary>
		/// Rotates the selected items.
		/// </summary>
		/// <param name="angle">Angle of rotation</param>
		/// <param name="center">Center of rotation</param>
		void RotateSelectedItems(double angle, Point center);
		
		/// <summary>
		/// Returns true if the rotation is valid.
		/// A rotation is only valid if the points won't rotate off the drawing canvas.
		/// </summary>
		/// <param name="angle">Angle of rotation</param>
		/// <param name="center">Center of rotation</param>
		/// <returns></returns>
		bool IsRotateable(double angle, Point center);

		/// <summary>
		/// Gives view models the opportunity to refresh state when the rotation is complete.
		/// </summary>
		void DoneRotating();
		
		void ClipSelectedPoints();

		/// <summary>
		/// Transforms the selected items using the specified transform group.
		/// </summary>
		/// <param name="transformGroup">Transform group to apply to the selected items</param>
		void TransformSelectedItems(TransformGroup transformGroup);
		
		/// <summary>
		/// Translates the selected items.
		/// </summary>
		/// <param name="transform">Transform to apply to the selected items</param>
		void MoveSelectedItems(Transform transform);

		/// <summary>
		/// Gets the width of the drawing canvas.
		/// </summary>
		/// <returns></returns>
		double GetWidth();
		
		/// <summary>
		/// Gets the height of the drawing canvas.
		/// </summary>
		/// <returns>Height of the drawing canvas</returns>
		double GetHeight();
	}
}
