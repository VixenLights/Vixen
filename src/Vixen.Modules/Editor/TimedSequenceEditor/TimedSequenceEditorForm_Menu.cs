using Common.Controls;
using Common.Controls.Timeline;
using VixenModules.App.Curves;
using VixenModules.App.LipSyncApp;
using VixenModules.Media.Audio;
using VixenModules.Effect.LipSync;
using Vixen.Module.Effect;
using Vixen.Module.Media;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.Analysis.BeatsAndBars;
using VixenModules.App.ColorGradients;
using VixenModules.Property.Face;
using VixenModules.Sequence.Timed;
using WeifenLuo.WinFormsUI.Docking;
using TimedSequenceEditor;
using VixenModules.App.Marks;
using Catel.Reflection;
using System.Text.RegularExpressions;


namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class TimedSequenceEditorForm
	{

		#region Sequence Menu

		private void toolStripMenuItem_Save_Click(object sender, EventArgs e)
		{
			SaveSequence();
		}

		private void toolStripMenuItem_AutoSave_Click(object sender, EventArgs e)
		{
			fileToolStripButton_AutoSave.Checked = autoSaveToolStripMenuItem.Checked;
			SetAutoSave();
		}

		private void toolStripMenuItem_SaveAs_Click(object sender, EventArgs e)
		{
			SaveSequence(null, true);
		}

		private void toolStripMenuItem_Close_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void playToolStripMenuItem_Click(object sender = null, EventArgs e = null)
		{
			PlayPauseToggle();
		}

		private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PlayPauseToggle();
		}

		private void stopToolStripMenuItem_Click(object sender = null, EventArgs e = null)
		{
			StopSequence();
		}

		private void toolStripMenuItem_Loop_CheckedChanged(object sender = null, EventArgs e = null)
		{
			playBackToolStripButton_Loop.Checked = toolStripMenuItem_Loop.Checked;
			if (playBackToolStripButton_Loop.Checked && delayOffToolStripMenuItem.Checked != true)
			{
				//No way, we're not doing both! Turn off the delay.
				foreach (ToolStripMenuItem item in playOptionsToolStripMenuItem.DropDownItems)
				{
					item.Checked = false;
				}
				delayOffToolStripMenuItem.Checked = true;
				toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = false;
			}
		}

		private void delayOffToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timerPostponePlay.Interval = 100;
			ClearDelayPlayItemChecks();
			delayOffToolStripMenuItem.Checked = true;
			toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = false;
			playBackToolStripButton_Play.ToolTipText = @"Play F5";
		}

		private void delay5SecondsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timerPostponePlay.Interval = 5000;
			ClearDelayPlayItemChecks();
			delay5SecondsToolStripMenuItem.Checked = toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = true;
			toolStripStatusLabel_delayPlay.Text = @"5 Seconds";
		}

		private void delay10SecondsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timerPostponePlay.Interval = 10000;
			ClearDelayPlayItemChecks();
			delay10SecondsToolStripMenuItem.Checked = toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = true;
			toolStripStatusLabel_delayPlay.Text = @"10 Seconds";
		}

		private void delay20SecondsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timerPostponePlay.Interval = 20000;
			ClearDelayPlayItemChecks();
			delay20SecondsToolStripMenuItem.Checked = toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = true;
			toolStripStatusLabel_delayPlay.Text = @"20 Seconds";
		}

		private void delay30SecondsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timerPostponePlay.Interval = 30000;
			ClearDelayPlayItemChecks();
			delay30SecondsToolStripMenuItem.Checked = toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = true;
			toolStripStatusLabel_delayPlay.Text = @"30 Seconds";
		}

		private void delay60SecondsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timerPostponePlay.Interval = 60000;
			ClearDelayPlayItemChecks();
			delay60SecondsToolStripMenuItem.Checked = toolStripStatusLabel3.Visible = toolStripStatusLabel_delayPlay.Visible = true;
			toolStripStatusLabel_delayPlay.Text = @"60 Seconds";
		}

		private void exportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EffectEditorForm.PreviewStop();
			ExportDialog ed = new ExportDialog(Sequence);
			ed.ShowDialog();
			EffectEditorForm.ResumePreview();
		}

		#endregion

		#region Edit Menu

		private void toolStripMenuItem_Cut_Click(object sender, EventArgs e)
		{
			ClipboardCut();
		}

		private void toolStripMenuItem_Copy_Click(object sender, EventArgs e)
		{
			ClipboardCopy();
		}

		private void toolStripMenuItem_Paste_Click(object sender, EventArgs e)
		{
			PastingMode = PastingMode.Default;
			PasteEffects();
		}

		private void toolStripMenuItem_PasteInvert_Click(object sender, EventArgs e)
		{
			PastingMode = PastingMode.Invert;
			PasteEffects();
		}

		private void toolStripMenuItem_PasteToMarks_Click(object sender, EventArgs e)
		{
			PastingMode = PastingMode.VisibleMarks;
			PasteEffects();
		}

		private void PasteEffects()
		{
			Row targetRow = TimelineControl.SelectedRow ?? TimelineControl.ActiveRow ?? TimelineControl.TopVisibleRow;
			ClipboardPaste(targetRow.Selected ? TimeSpan.Zero : _timeLineGlobalStateManager.CursorPosition);
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_undoMgr.NumUndoable > 0)
			{
				_undoMgr.Undo();	
			}
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_undoMgr.NumRedoable > 0)
			{
				_undoMgr.Redo();
			}
		}

		private void toolStripMenuItem_deleteElements_Click(object sender, EventArgs e)
		{
			//TimelineControl.ruler.DeleteSelectedMarks(); //Why would we delete marks in element delete operations???
			RemoveSelectedElements();
		}

		private void selectAllElementsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TimelineControl.SelectAllElements();
		}

		private void toolStripMenuItem_SnapTo_CheckedChanged(object sender, EventArgs e)
		{
			modeToolStripButton_SnapTo.Checked = toolStripMenuItem_SnapTo.Checked;
			TimelineControl.grid.EnableSnapTo = toolStripMenuItem_SnapTo.Checked;
		}

		private void fullWaveformToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			TimelineControl.waveform.WaveformStyle =
				fullWaveformToolStripMenuItem.Checked ? WaveformStyle.Full : WaveformStyle.Half;
			TimelineControl.waveform.Invalidate();
		}

		// this seems to break the keyboard shortcuts; the key shortcuts don't get enabled again
		// until the menu is dropped down, which is annoying. These really should be enabled/disabled
		// on select of elements, but that's too annoying for now...
		//private void editToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		//{
		//    toolStripMenuItem_EditEffect.Enabled = TimelineControl.SelectedElements.Any() ;
		//    toolStripMenuItem_Cut.Enabled = TimelineControl.SelectedElements.Any() ;
		//    toolStripMenuItem_Copy.Enabled = TimelineControl.SelectedElements.Any() ;
		//    toolStripMenuItem_Paste.Enabled = _clipboard != null;		//TODO: fix this when clipboard fixed
		//}
		
		private void toolStripMenuItem_ResizeIndicator_CheckStateChanged(object sender, EventArgs e)
		{
			TimelineControl.grid.ResizeIndicator_Enabled = toolStripMenuItem_ResizeIndicator.Checked;
		}

		private void CheckRiColorMenuItem(string color)
		{
			TimelineControl.grid.ResizeIndicator_Color = color;
			toolStripMenuItem_RIColor_Blue.Checked = color == "Blue";
			toolStripMenuItem_RIColor_Yellow.Checked = color == "Yellow";
			toolStripMenuItem_RIColor_Green.Checked = color == "Green";
			toolStripMenuItem_RIColor_White.Checked = color == "White";
			toolStripMenuItem_RIColor_Red.Checked = color == "Red";
		}

		private void toolStripMenuItem_RIColor_Blue_Click(object sender, EventArgs e)
		{
			CheckRiColorMenuItem("Blue");
			toolStripMenuItem_ResizeIndicator.Checked = true;
		}

		private void toolStripMenuItem_RIColor_Yellow_Click(object sender, EventArgs e)
		{
			CheckRiColorMenuItem("Yellow");
			toolStripMenuItem_ResizeIndicator.Checked = true;
		}

		private void toolStripMenuItem_RIColor_Green_Click(object sender, EventArgs e)
		{
			CheckRiColorMenuItem("Green");
			toolStripMenuItem_ResizeIndicator.Checked = true;
		}

		private void toolStripMenuItem_RIColor_White_Click(object sender, EventArgs e)
		{
			CheckRiColorMenuItem("White");
			toolStripMenuItem_ResizeIndicator.Checked = true;
		}

		private void toolStripMenuItem_RIColor_Red_Click(object sender, EventArgs e)
		{
			CheckRiColorMenuItem("Red");
			toolStripMenuItem_ResizeIndicator.Checked = true;
		}

		private void cADStyleSelectionBoxToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TimelineControl.grid.aCadStyleSelectionBox = cADStyleSelectionBoxToolStripMenuItem.Checked;
		}

		#endregion

		#region View Menu

		private void showEffectInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TimelineControl.grid.ShowEffectToolTip = showEffectInfoToolStripMenuItem.Checked;
		}

		private void toolStripMenuItem_zoomTimeIn_Click(object sender, EventArgs e)
		{
			TimelineControl.Zoom(0.8);
		}

		private void toolStripMenuItem_zoomTimeOut_Click(object sender, EventArgs e)
		{
			TimelineControl.Zoom(1.25);
		}

		private void toolStripMenuItem_zoomRowsIn_Click(object sender, EventArgs e)
		{
			TimelineControl.ZoomRows(1.25);
		}

		private void toolStripMenuItem_zoomRowsOut_Click(object sender, EventArgs e)
		{
			TimelineControl.ZoomRows(0.8);
		}

		private void zoomUnderMousePositionToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			TimelineControl.ZoomToMousePosition = zoomUnderMousePositionToolStripMenuItem.Checked;
		}

		private void resetRowHeightToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TimelineControl.ResetRowHeight();
		}

		private void collapeAllElementGroupsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TimelineControl.RowListMenuCollapse();
		}
		#endregion

		#region Tools Menu

		private void speedTempoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (TimelineControl.Audio != null)
			{
				TimelineControl.Audio.UseTempo = speedTempoToolStripMenuItem.Checked;
			}
		}

		private void toolStripMenuItem_removeAudio_Click(object sender, EventArgs e)
		{
			RemoveAudioAssociation();
		}

		private void toolStripMenuItem_associateAudio_Click(object sender, EventArgs e)
		{
			AddAudioAssociation();
		}

		private void modifySequenceLengthToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var prompt = new SetSequenceLength(SequenceLength);

			if (prompt.ShowDialog() == DialogResult.OK)
			{
				SequenceLength = prompt.SequenceLength;
				SequenceModified();
			}
		}

		private void gridWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//Gridform or the main timeline should not be closed.
			if (!GridForm.IsDisposed)
			{
				if (GridForm.IsHidden || GridForm.DockState == DockState.Unknown)
				{
					GridForm.Show(dockPanel, DockState.Document);
				}
			}
		}


		private void HighlightRowsWithEffectsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			TimelineControl.grid.HighlightRowsWithEffects(highlightRowsWithEffectsToolStripMenuItem.Checked);
		}

		private void effectEditorWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			HandleDockContentToolStripMenuClick(EffectEditorForm, DockState.DockRight);
			}

		private void effectWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			HandleDockContentToolStripMenuClick(EffectsForm, DockState.DockLeft);
			}

		private void markWindowToolStripMenuItem_Click(object sender, EventArgs e)
			{
			HandleDockContentToolStripMenuClick(MarksForm, DockState.DockRight);
			}

		private void mixingFilterEditorWindowToolStripMenuItem_Click(object sender, EventArgs e)
			{
			HandleDockContentToolStripMenuClick(LayerEditor, DockState.DockLeft);
			}

		private void toolStripMenuItemFindEffects_Click(object sender, EventArgs e)
		{
			HandleDockContentToolStripMenuClick(FindEffects, DockState.DockLeft);
		}

		private void toolStripMenuItemColorLibrary_Click(object sender, EventArgs e)
		{
			HandleDockContentToolStripMenuClick(ColorLibraryForm, DockState.DockRight);
		}

		private void toolStripMenuItemCurveLibrary_Click(object sender, EventArgs e)
		{
			HandleDockContentToolStripMenuClick(CurveLibraryForm, DockState.DockRight);
		}

		private void toolStripMenuItemGradientLibrary_Click(object sender, EventArgs e)
		{
			HandleDockContentToolStripMenuClick(GradientLibraryForm, DockState.DockRight);
		}

		private void HandleDockContentToolStripMenuClick(DockContent dockWindow, DockState state)
		{
			if (dockWindow.IsHidden || dockWindow.DockState == DockState.Unknown)
			{
				dockWindow.Show(dockPanel, state);
			}
			else
			{
				dockWindow.Close();
			}
		}

		private void ColorCollectionsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ColorCollectionLibrary_Form rccf = new ColorCollectionLibrary_Form(new List<ColorCollection>(_colorCollections));
			if (rccf.ShowDialog() == DialogResult.OK)
			{
				_colorCollections = rccf.ColorCollections;
				SaveColorCollections();
			}
			else
			{
				LoadColorCollections();
			}
		}

		private void curveEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var selector = new CurveLibrarySelector{DoubleClickMode = CurveLibrarySelector.Mode.Edit};
			selector.ShowDialog();
			_currentToolStrip = null;
			Populate_Curves();
		}

		private void colorGradientToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var selector = new ColorGradientLibrarySelector{DoubleClickMode = ColorGradientLibrarySelector.Mode.Edit};
			selector.ShowDialog();
			_currentToolStrip = null;
			Populate_Gradients();
		}

		private async void editMapsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LipSyncMapSelector mapSelector = new LipSyncMapSelector();
			DialogResult dr = mapSelector.ShowDialog();
			if (mapSelector.Changed)
			{
				mapSelector.Changed = false;
				SequenceModified();
				ResetLipSyncNodes();
				await VixenSystem.SaveSystemAndModuleConfigAsync();
			}
		}

		private void setDefaultMap_Click(object sender,EventArgs e)
		{
			ToolStripMenuItem menu = (ToolStripMenuItem)sender;
			if ( _library.DefaultMapping != null && !_library.DefaultMappingName.Equals(menu.Text))
			{
				_library.DefaultMappingName = menu.Text; 
				SequenceModified();
			}            
		}

		private void defaultMapToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
           defaultMapToolStripMenuItem.DropDownItems.Clear();            
            foreach (LipSyncMapData mapping in _library.Library.Values)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(mapping.LibraryReferenceName);
                menuItem.Click += setDefaultMap_Click;
                menuItem.Checked = _library.IsDefaultMapping(mapping.LibraryReferenceName);
                defaultMapToolStripMenuItem.DropDownItems.Add(menuItem);
            }            
        }

        private void papagayoImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
	        PapagayoDoc papagayoFile = new PapagayoDoc();
            FileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = @"Papagayo files (*.pgo)|*.pgo|All files (*.*)|*.*";
            openDialog.FilterIndex = 1;
            if (openDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string fileName = openDialog.FileName;
            papagayoFile.Load(fileName);
            TimelineElementsClipboardData result = new TimelineElementsClipboardData
            {
                FirstVisibleRow = -1,
                EarliestStartTime = TimeSpan.MaxValue,
            };
            result.FirstVisibleRow = 0;
            int rownum = 0;
            foreach (string voice in papagayoFile.VoiceList)
            {
                List<PapagayoPhoneme> phonemes = papagayoFile.PhonemeList(voice);
                if (phonemes.Count > 0)
                {
                    foreach (PapagayoPhoneme phoneme in phonemes)
                    {
                        if (phoneme.DurationMS == 0.0)
                        {
                            continue;
                        }
                        IEffectModuleInstance effect =
                            ApplicationServices.Get<IEffectModuleInstance>(new LipSyncDescriptor().TypeId);
						((LipSync)effect).StaticPhoneme = (App.LipSyncApp.PhonemeType)Enum.Parse(typeof(App.LipSyncApp.PhonemeType), phoneme.TypeName.ToUpper());
                        ((LipSync)effect).LyricData = phoneme.LyricData;
                        TimeSpan startTime = TimeSpan.FromMilliseconds(phoneme.StartMS);
                        EffectModelCandidate modelCandidate =
                              new EffectModelCandidate(effect)
                              {
                                  Duration = TimeSpan.FromMilliseconds(phoneme.DurationMS - 1),
                                  StartTime = startTime
                              };
                        result.EffectModelCandidates.Add(modelCandidate, rownum);
                        if (startTime < result.EarliestStartTime)
                            result.EarliestStartTime = startTime;
                        effect.PreRender();
                    }                   
                    IDataObject dataObject = new DataObject(ClipboardFormatName);
                    dataObject.SetData(result);
                    Clipboard.SetDataObject(dataObject, true);
                    _TimeLineSequenceClipboardContentsChanged(EventArgs.Empty);
                    SequenceModified();
                }
                rownum++;
            }
            string displayStr = rownum + " Voices imported to clipboard as seperate rows\n\n";            
            int j = 1;
            foreach (string voiceStr in papagayoFile.VoiceList)
            {
                displayStr += "Row #" + j +" - " + voiceStr + "\n";
                j++;
            }
			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Information; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm(displayStr, @"Papagayo Import", false, false);
			messageBox.ShowDialog();
        }

		private void textConverterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (LipSyncTextConvertForm.Active == false)
			{
				LipSyncTextConvertForm textConverter = new LipSyncTextConvertForm();
				textConverter.NewTranslation += textConverterHandler;
				textConverter.TranslateFailure += translateFailureHandler;
				textConverter.MarkCollections = _sequence.LabeledMarkCollections;
				textConverter.Show(this);
			}
		}

		private void lipSyncMappingsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			
		}

		private async void changeMapToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LipSyncNodeSelect nodeSelectDlg = new LipSyncNodeSelect();
			//nodeSelectDlg.MaxNodes = _mapping.MapItems.Count;
			nodeSelectDlg.MaxNodes = Int32.MaxValue;
			nodeSelectDlg.MatrixOptionsOnly = false;
			nodeSelectDlg.AllowGroups = true;
			nodeSelectDlg.AllowRecursiveAdd = false;
			
			DialogResult dr = nodeSelectDlg.ShowDialog(this);
			if (dr == DialogResult.OK)
			{
				if (nodeSelectDlg.SelectedElementNodes.Any())
				{
					FaceSetupHelper helper = new FaceSetupHelper();
					var success = helper.Perform(nodeSelectDlg.SelectedElementNodes);
					if (success)
					{
						RenderLipSyncElementsAsync();
						await VixenSystem.SaveSystemAndModuleConfigAsync();
					}
				}
			}
		}

		private void toolStripMenuItem_BeatBarDetection_Click(object sender, EventArgs e)
		{
			foreach (IMediaModuleInstance module in _sequence.GetAllMedia())
			{
				if (module is Audio)
				{
					BeatsAndBars audioFeatures = new BeatsAndBars((Audio)module);
					 
					audioFeatures.DoBeatBarDetection(_sequence.LabeledMarkCollections);
					SequenceModified();
					break;

				}
			}
		}

		/// <summary>
		///  Processes the menu command "Create Evenly Divided Marks"
		/// </summary>
		/// <param name="sender">Contains a reference to the control/object that raised the event</param>
		/// <param name="e">Contains the event data</param>
		private void toolStripMenuItem_divideMarksEvenly_Click(object sender, EventArgs e)
		{
			const int MinMarkWidthPx = 12;
			TimeSpan lengthTime = TimeSpan.MinValue;
			double lengthDivision = 0;

			var marksForm = new CreateEvenMarksForm(_mPrevPlaybackStart, _mPrevPlaybackEnd, SequenceLength);
			DialogResult dialogReturn;
			do
			{
				dialogReturn = marksForm.ShowDialog();
				if (dialogReturn == DialogResult.Cancel)
					return;

				lengthTime = marksForm.End - marksForm.Start;
				lengthDivision = lengthTime.TotalMilliseconds / marksForm.Divisions;

				// Verify that the time span and number of divisions can sufficiently fit within the defined space
				if (lengthDivision < TimelineControl.grid.PixelsToTime(MinMarkWidthPx).TotalMilliseconds)
				{
					dialogReturn = DialogResult.Retry;
					MessageBoxForm.msgIcon = SystemIcons.Error;
					var messageBox = new MessageBoxForm("Individual mark length is smaller than allowed.\n\nEither increase the difference in time " +
						"between Start and End times or decrease the number of divisions.", "Mark Length Error", false, false);
					messageBox.ShowDialog();
				}
			} while (dialogReturn != DialogResult.OK);

			// Remove any exact match mark collection
			var mark = _sequence.LabeledMarkCollections.Where(x => x.Marks.Count == marksForm.Divisions + 1 &&
																   x.Marks.First().StartTime == marksForm.Start &&
																   x.Marks.Last().StartTime == marksForm.End);
			if (mark.Count() > 0)
				_sequence.LabeledMarkCollections.Remove(mark.First());

			// Create a new mark group
			VixenModules.App.Marks.MarkCollection mc = new VixenModules.App.Marks.MarkCollection();
			mc.Name = $"{marksForm.Divisions} Divisions";

			// Check if an existing mark has a similar name
			int nextSimilarMark = 1;
			foreach(var markItem in _sequence.LabeledMarkCollections)
			{
				if (markItem.Name.StartsWith(mc.Name) == true)
				{
					Regex regex = new Regex(@"\d+");
					var match = regex.Match(markItem.Name.Substring(mc.Name.Length));
					int value = match.Length > 0 ? match.Value.CastToInt32() : 1;
					if (value >= nextSimilarMark)
					{
						nextSimilarMark = value + 1;
					}
				}
			}

			// ... and if a similarly named mark exists, then alter the name
			if (nextSimilarMark > 1)
			{
				mc.Name = $"{marksForm.Divisions} Divisions #{nextSimilarMark}";
			}

			// Create the collection of individual marks
			TimeSpan nextTime = marksForm.Start;
			for (int iteration = 0; iteration <= marksForm.Divisions; iteration++)
			{
				var newMark = new Mark(nextTime);
				newMark.Duration = TimeSpan.FromMilliseconds(lengthDivision * .8);
				mc.AddMark(newMark);

				nextTime = nextTime + TimeSpan.FromMilliseconds(lengthDivision);
			}

			// Add the newly created mark group to the collection
			_sequence.LabeledMarkCollections.Add(mc);

			// Mark the sequence as changed
			SequenceModified();
		}

		private void bulkEffectMoveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var dialog = new BulkEffectMoveForm(_timeLineGlobalStateManager.CursorPosition, SequenceLength);

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				TimeSpan offset = dialog.IsForward ? dialog.Offset : -dialog.Offset;
				offset = TimelineControl.grid.MoveElementsInRangeByTime(dialog.Start, dialog.End, offset, dialog.ProcessVisibleRows, dialog.ClipEffects);

				if (dialog.ProcessMarks == true && offset != TimeSpan.Zero)
				{
					MoveMarksInRangeByTime(dialog.Start, dialog.End, offset, dialog.ProcessVisibleRows);
				}
			}
		}
		#endregion

		#region Help Menu

		private void helpDocumentationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Sequencer);
		}

		private void vixenYouTubeChannelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.YouTubeChannel);
		}

		#endregion
		
	}
}
