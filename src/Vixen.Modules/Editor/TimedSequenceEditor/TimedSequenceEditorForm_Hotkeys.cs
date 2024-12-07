using Common.Controls.Timeline;
using Element = Common.Controls.Timeline.Element;
using Common.Controls.ControlsEx;

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

		/// <summary>
		/// Preprocess Quick Keys before handing the key to OnKeyDown
		/// </summary>
		/// <param name="e">Contains the key data (for System.Windows.Forms)</param>
		public void HandleQuickKey(KeyEventArgs e)
		{
			// Convert some special quick keys (sent from GDIPreviewForms) to regular keys then post to
			// the Windows Message queue.  Posting allows thread-isolation from an outside process.
			if (e.KeyCode == Keys.MediaPlayPause)
				Win32.PostMessage(this.Handle, Win32.WM_KEYDOWN, (int)Keys.Space, 0);
			else if (e.KeyCode == Keys.MediaNextTrack)
				Win32.PostMessage(this.Handle, Win32.WM_KEYDOWN, (int)Keys.F5, 0);
			else if (e.KeyCode == Keys.MediaStop)
				Win32.PostMessage(this.Handle, Win32.WM_KEYDOWN, (int)Keys.F8, 0);
			else
				OnKeyDown(e);
		}

		/// <summary>
		/// Preprocess Quick Keys before handing the key to OnKeyDown
		/// </summary>
		/// <param name="e">Contains the key data (for System.Windows.Input)</param>
		public void HandleQuickKey(System.Windows.Input.KeyEventArgs swiKey)
		{
			var wpfKey = swiKey.Key == System.Windows.Input.Key.System ? swiKey.SystemKey : swiKey.Key;
			var swfKeys = (System.Windows.Forms.Keys)System.Windows.Input.KeyInterop.VirtualKeyFromKey(wpfKey);

			System.Windows.Forms.Keys swfModifiers;
			swfModifiers = swiKey.KeyboardDevice.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Alt) ? System.Windows.Forms.Keys.Alt : System.Windows.Forms.Keys.None;
			swfModifiers |= swiKey.KeyboardDevice.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control) ? System.Windows.Forms.Keys.Control : System.Windows.Forms.Keys.None;
			swfModifiers |= swiKey.KeyboardDevice.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Shift) ? System.Windows.Forms.Keys.Shift : System.Windows.Forms.Keys.None;

			var swfkey = new System.Windows.Forms.KeyEventArgs(swfKeys | swfModifiers);
			HandleQuickKey(swfkey);
			swiKey.Handled = swfkey.Handled;
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

				case Keys.MediaPlayPause:
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
					if (TimelineControl.grid.IsResizeDragInProgress)
					{
						break;
					}
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
					if (TimelineControl.grid.IsResizeDragInProgress)
					{
						break;
					}
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
					if (TimelineControl.grid.IsResizeDragInProgress)
					{
						break;
					}
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
					else
					{
						DistributeSelectedEffectsEqually();
					}
					break;

				case Keys.C:
					if (TimelineControl.grid.IsResizeDragInProgress)
					{
						break;
					}
					element = TimelineControl.grid.ElementAtPosition(MousePosition);
					if (element != null && TimelineControl.SelectedElements.Count() > 1 && TimelineControl.SelectedElements.Contains(element))
					{
						TimelineControl.grid.AlignElementCenters(TimelineControl.SelectedElements, element);
					}

					break;

				case Keys.U:
					if (TimelineControl.grid.IsResizeDragInProgress)
					{
						break;
					}
					element = TimelineControl.grid.ElementAtPosition(MousePosition);
					if (element != null && TimelineControl.SelectedElements.Count() > 1 && TimelineControl.SelectedElements.Contains(element))
					{
						TimelineControl.grid.AlignElementDurations(TimelineControl.SelectedElements, element, ModifierKeys == Keys.Shift );
					}

					break;

				case Keys.R:
					if (TimelineControl.grid.IsResizeDragInProgress)
					{
						break;
					}
					element = TimelineControl.grid.ElementAtPosition(MousePosition);
					if (element != null && TimelineControl.SelectedElements.Count() > 1 && TimelineControl.SelectedElements.Contains(element))
					{
						TimelineControl.grid.AlignElementStartToEndTimes(TimelineControl.SelectedElements, element, ModifierKeys == Keys.Shift);
					}

					break;

				case Keys.N:
					if (TimelineControl.grid.IsResizeDragInProgress)
					{
						break;
					}
					element = TimelineControl.grid.ElementAtPosition(MousePosition);
					if (element != null && TimelineControl.SelectedElements.Count() > 1 && TimelineControl.SelectedElements.Contains(element))
					{
						TimelineControl.grid.AlignElementEndToStartTime(TimelineControl.SelectedElements, element, ModifierKeys == Keys.Shift);
					}

					break;

				case Keys.I:
					if (TimelineControl.grid.IsResizeDragInProgress)
					{
						break;
					}
					element = TimelineControl.grid.ElementAtPosition(MousePosition);
					if (element != null && TimelineControl.SelectedElements.Count() > 1 && TimelineControl.SelectedElements.Contains(element))
					{
						DistributeSelectedEffects();
					}
					break;

				case Keys.MediaNextTrack:
				case Keys.F5:
					playToolStripMenuItem_Click();
					e.Handled = true;
					break;

				case Keys.MediaStop:
				case Keys.F8:
					stopToolStripMenuItem_Click();
					e.Handled = true;
					break;

				case Keys.F9:
					playBackToolStripButton_Loop.PerformClick();
					e.Handled = true;
					break;
			}
			// Prevents sending keystrokes to child controls. 
			// This was causing serious slowdowns if random keys were pressed.
			//e.SuppressKeyPress = true;
//			base.OnKeyDown(e);
		}

	}
}