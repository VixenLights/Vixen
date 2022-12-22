namespace Common.Controls.Timeline
{
	internal enum DragState
	{
		///<summary>Not dragging, mouse is up.</summary>
		Normal = 0,

		///<summary>Mouse down, but hasn't moved past threshold yet to be considered dragging.</summary>
		Waiting,

		///<summary>Actively dragging objects.</summary>
		Moving,

		///<summary>Like "Dragging", but dragging on the background, not an object.</summary>
		Selecting,

		///<summary>Dragging the mouse horizontally to resize an object in time.</summary>
		HResizing,

		///<summary>Drawing, like "Dragging", but anywhere on timeline.</summary>
		Drawing,
	}
}