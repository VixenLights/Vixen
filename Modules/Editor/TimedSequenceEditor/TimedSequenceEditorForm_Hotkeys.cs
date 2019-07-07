using System;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.Timeline;
using Element = Common.Controls.Timeline.Element;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class TimedSequenceEditorForm
	{
		internal void HandleSpacebarAction(bool pause)
		{
			if (!_context.IsRunning)
				PlaySequence();
			else
			{
				if (_context.IsPaused)
					PlaySequence();
				else
				if (pause)
				{
					PauseSequence();
				}
				else
				{
					StopSequence();
				}
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (IgnoreKeyDownEvents) return;

			Element element;
			// do anything special we want to here: keyboard shortcuts that are in
			// the menu will be handled by them instead.
			switch (e.KeyCode)
			{
				//case Keys.Delete:
				//	TimelineControl.ruler.DeleteSelectedMarks();
				//	break;
				case Keys.Home:
					if (e.Control)
						TimelineControl.VisibleTimeStart = TimeSpan.Zero;
					else
						TimelineControl.VerticalOffset = 0;
					break;

				case Keys.End:
					if (e.Control)
						TimelineControl.VisibleTimeStart = TimelineControl.TotalTime - TimelineControl.VisibleTimeSpan;
					else
						TimelineControl.VerticalOffset = int.MaxValue; // a bit iffy, but we know that the grid caps it to what's visible
					break;

				case Keys.PageUp:
					if (e.Control)
						TimelineControl.VisibleTimeStart -= TimelineControl.VisibleTimeSpan.Scale(0.5);
					else
						TimelineControl.VerticalOffset -= (TimelineControl.VisibleHeight / 2);
					break;

				case Keys.PageDown:
					if (e.Control)
						TimelineControl.VisibleTimeStart += TimelineControl.VisibleTimeSpan.Scale(0.5);
					else
						TimelineControl.VerticalOffset += (TimelineControl.VisibleHeight / 2);
					break;

				case Keys.Space:
					if (e.Shift)
					{
						if (TimingSource != null) AddMarkAtTime(TimingSource.Position, false);
					}
					else
					{
						HandleSpacebarAction(e.Control);
					}
					break;

				case Keys.Left:
					if (e.Control)
					{
						TimelineControl.MoveSelectedElementsByTime(TimelineControl.TimePerPixel.Scale(-2));
					}
					
					break;

				case Keys.Right:
					if (e.Control)
					{
						TimelineControl.MoveSelectedElementsByTime(TimelineControl.TimePerPixel.Scale(2));
					}
					
					break;
				
				case Keys.S:
					if (e.Shift & e.Control)
					{
						AlignEffectsToNearestMarks("Start");
						break;
					}
					element = TimelineControl.grid.ElementAtPosition(MousePosition);
					if (element != null && TimelineControl.SelectedElements.Count() > 1 && TimelineControl.SelectedElements.Contains(element))
					{
						TimelineControl.grid.AlignElementStartTimes(TimelineControl.SelectedElements, element, e.Shift);
					}
					break;
				case Keys.E:

					if (e.Shift & e.Control)
					{
						AlignEffectsToNearestMarks("End");
						break;
					}
					element = TimelineControl.grid.ElementAtPosition(MousePosition);
					if (element != null && TimelineControl.SelectedElements.Count() > 1 && TimelineControl.SelectedElements.Contains(element))
					{
						TimelineControl.grid.AlignElementEndTimes(TimelineControl.SelectedElements, element, e.Shift);
					}
					break;
					
				case Keys.B:
					if (e.Shift & e.Control)
					{
						AlignEffectsToNearestMarks("Both");
						break;
					}
					element = TimelineControl.grid.ElementAtPosition(MousePosition);
					if (element != null && TimelineControl.SelectedElements.Count() > 1 && TimelineControl.SelectedElements.Contains(element))
					{
						TimelineControl.grid.AlignElementStartEndTimes(TimelineControl.SelectedElements, element);
					}
					
					break;

				case Keys.Escape:
					if (TimelineControl.grid._beginEffectDraw) //If we are drawing, prevent escape
						return;
					EffectsForm.DeselectAllNodes();
					TimelineControl.grid.EnableDrawMode = false;
					modeToolStripButton_DrawMode.Checked = false;
					modeToolStripButton_SelectionMode.Checked = true;
					break;

				case Keys.OemMinus:
					if (e.Control && e.Shift)
						TimelineControl.ZoomRows(.8);
					else if (e.Control)
						TimelineControl.Zoom(1.25);
					break;

				case Keys.Oemplus:
					if (e.Control && e.Shift)
						TimelineControl.ZoomRows(1.25);
					else if (e.Control)
						TimelineControl.Zoom(.8);
					break;
				case Keys.T:
					TimelineControl.grid.ToggleSelectedRows(e.Control);
					break;

				case Keys.D:
					if (e.Control)
					{
						TimelineControl.grid.SplitSelectedElementsAtMouseLocation();
					}
					break;
			}
			// Prevents sending keystrokes to child controls. 
			// This was causing serious slowdowns if random keys were pressed.
			//e.SuppressKeyPress = true;
			base.OnKeyDown(e);
		}

	}
}