using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Xml;
using Common.Controls;
using Common.Controls.ControlsEx.ValueControls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Controls.Timeline;
using Common.Resources;
using Common.Resources.Properties;
using NLog;
using Vixen;
using Vixen.Cache.Sequence;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Module.App;
using VixenModules.App.Curves;
using VixenModules.App.LipSyncApp;
using VixenModules.Effect.Video;
using VixenModules.Media.Audio;
using VixenModules.Effect.LipSync;
using Vixen.Module.Editor;
using Vixen.Module.Effect;
using Vixen.Module.Media;
using Vixen.Module.Timing;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;
using Vixen.Sys.State;
using VixenModules.Analysis.BeatsAndBars;
using VixenModules.App.ColorGradients;
using VixenModules.Editor.EffectEditor;
using VixenModules.Editor.TimedSequenceEditor.Undo;
using VixenModules.Sequence.Timed;
using WeifenLuo.WinFormsUI.Docking;
using Element = Common.Controls.Timeline.Element;
using Timer = System.Windows.Forms.Timer;
using VixenModules.Property.Color;

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

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Left:
					TimelineControl.ruler.NudgeMark(-TimelineControl.ruler.StandardNudgeTime);
					break;
				case (Keys.Left | Keys.Shift):
					TimelineControl.ruler.NudgeMark(-TimelineControl.ruler.SuperNudgeTime);
					break;
				case Keys.Right:
					TimelineControl.ruler.NudgeMark(TimelineControl.ruler.StandardNudgeTime);
					break;
				case (Keys.Right | Keys.Shift):
					TimelineControl.ruler.NudgeMark(TimelineControl.ruler.SuperNudgeTime);
					break;
				//case Keys.Escape:
					//EffectsForm.DeselectAllNodes();
					//toolStripButton_DrawMode.Checked = false;
					//toolStripButton_SelectionMode.Checked = true;
					//break;
			}
			return base.ProcessCmdKey(ref msg, keyData);
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
					toolStripButton_DrawMode.Checked = false;
					toolStripButton_SelectionMode.Checked = true;
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
			}
			// Prevents sending keystrokes to child controls. 
			// This was causing serious slowdowns if random keys were pressed.
			//e.SuppressKeyPress = true;
			base.OnKeyDown(e);
		}

	}
}