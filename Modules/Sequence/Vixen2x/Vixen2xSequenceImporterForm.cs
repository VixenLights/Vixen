using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Vixen.Module.Effect;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.Sequence.Timed;
using VixenModules.App.Curves;
using ZedGraph;
using VixenModules.App.ColorGradients;


namespace VixenModules.SequenceType.Vixen2x {
	public partial class Vixen2xSequenceImporterForm : Form {
		public ISequence Sequence { get; set; }

		private Vixen2SequenceData parsedV2Sequence = null;
		private List<ChannelMapping> mappings;

		private enum patternType {
			[DescriptionAttribute("Groups of Similar Values")]
			SetLevelTrend,
			[DescriptionAttribute("Fades")]
			PulseFadeTrend,
			[DescriptionAttribute("Ramps")]
			PulseRampTrend,
			[DescriptionAttribute("Single Cells")]
			SingleSetLevel
		};

		private const double curveDivisor = byte.MaxValue / 100.0;
		private const double startX = 0.0;
		private const double endX = 100.0;
		private const int resetEventPosition = 0;
		private const int zeroEventValue = 0;

		public Vixen2xSequenceImporterForm() {
			InitializeComponent();
		}

		public bool ProcessFile(string Vixen2File) {
			var result = false;
			this.Show();
			this.Text = "Importing " + Path.GetFileName(Vixen2File);
			
			parsedV2Sequence = new Vixen2SequenceData(Vixen2File);

			// make a channel mapping, and present that to the user. Not editable at the moment.
			mappings = new List<ChannelMapping>();

			foreach (ElementNode element in VixenSystem.Nodes.GetLeafNodes()) {
				if (mappings.Count >= parsedV2Sequence.ElementCount)
					break;

				string newName = "Channel " + (mappings.Count + 1).ToString();
				mappings.Add(new ChannelMapping(newName, element));
			}

			// pad out the mappings with null elements; we want to be able to not map some stuff (later on)
			while (mappings.Count < parsedV2Sequence.ElementCount) {
				string newName = "Channel " + (mappings.Count + 1).ToString();
				mappings.Add(new ChannelMapping(newName, null));
			}
			
			initializeProgressBar();

			// show the user the mapping form
			Vixen2xSequenceImporterChannelMapper mappingForm = new Vixen2xSequenceImporterChannelMapper(mappings);
			mappingForm.ShowDialog();

			createTimedSequence();
			importSequenceData(mappings);
	
			result = true;

			Close();

			return result;
		}

		private void initializeProgressBar() {
			pbImport.Minimum = 0;
			pbImport.Maximum = sizeof(patternType) * parsedV2Sequence.ElementCount;
			pbImport.Value = 0;

			lblStatusLine.Text = "";
		}

		private void createTimedSequence() {
			Sequence = new TimedSequence();
			Sequence.SequenceData = new TimedSequenceData();
			// TODO: use this mark collection (maybe generate a grid?)
			MarkCollection mc = new MarkCollection();
			Sequence.Length = TimeSpan.FromMilliseconds(parsedV2Sequence.SeqLengthInMills);

			var songFileName = parsedV2Sequence.SongFileName;
			if (songFileName != null) {
				if (File.Exists(songFileName)) {
					Sequence.AddMedia(MediaService.Instance.GetMedia(songFileName));
				}
				else {
					var message = String.Format("Could not locate the audio file '{0}'; please add it manually " + 
						"after import (Under Tools -> Associate Audio).", Path.GetFileName(songFileName));
					MessageBox.Show(message, "Couldn't find audio");
				}
			}
		}

		private void importSequenceData(List<ChannelMapping> mappings)
		{
			int startEventPosition = resetEventPosition;
			var endEventPosition = resetEventPosition;
			var priorEventNum = resetEventPosition;

			var startEventValue = zeroEventValue;
			var endEventValue = zeroEventValue;
			var priorEventValue = zeroEventValue;
			var currentEventValue = zeroEventValue;
			
			var pbImportValue = 0;

			// These flags are here just to make the code below easier to read, at least for me.
			var patternFound = false;
			var processingSingleEvents = false;
			var processingGroupEvents = false;
			var processingRamps = false;
			var processingFades = false;
			var currentEventIsZero = true;
			var currentEventIsNotZero = false;
			var priorEventisNotZero = false;

			foreach (patternType pattern in Enum.GetValues(typeof(patternType))) {
				processingSingleEvents = pattern == patternType.SingleSetLevel;
				processingGroupEvents = pattern == patternType.SetLevelTrend;
				processingRamps = pattern == patternType.PulseRampTrend;
				processingFades = pattern == patternType.PulseFadeTrend;

				var patternText = ((DescriptionAttribute)((pattern.GetType().GetMember(pattern.ToString()))[0]
							.GetCustomAttributes(typeof(DescriptionAttribute), false)[0])).Description;

				currentEventValue = zeroEventValue;
				for (var currentElementNum = 0; currentElementNum < parsedV2Sequence.ElementCount; currentElementNum++) {
					lblStatusLine.Text = String.Format("Finding {0} on Element {1}", patternText, currentElementNum + 1);
					pbImport.Value = ++pbImportValue;

					patternFound = false;
					priorEventValue = zeroEventValue;
					priorEventNum = resetEventPosition;

					for (var currentEventNum = 0; currentEventNum < parsedV2Sequence.EventsPerElement; currentEventNum++) {
						// To keep the progress bar looking snappy
						if ((currentEventNum % 10) == 0) {
							Application.DoEvents();
						}
						currentEventValue = parsedV2Sequence.EventData[currentElementNum * parsedV2Sequence.EventsPerElement + currentEventNum];

						currentEventIsZero = currentEventValue == zeroEventValue;
						currentEventIsNotZero = !currentEventIsZero;
						priorEventisNotZero = priorEventValue != zeroEventValue;
						
						// Add a non zero single set level event.
						if (processingSingleEvents && currentEventIsNotZero) {
							addEvent(pattern, currentElementNum, currentEventNum, currentEventValue, currentEventNum);

							startEventPosition = resetEventPosition;
							endEventPosition = resetEventPosition;
						}
						// Add a ramp, fade or multi set level event since it just ended (a zero event was found)
						else if (patternFound && !processingSingleEvents && currentEventIsZero && endEventPosition != resetEventPosition) {
							addEvent(pattern, currentElementNum, startEventPosition, startEventValue, endEventPosition, endEventValue);

							patternFound = false;
							startEventPosition = resetEventPosition;
							endEventPosition = resetEventPosition;
						}
						// Beggining of a pattern found, set flag and start event postion and value
						else if (!patternFound && currentEventNum > resetEventPosition
									&& ((processingGroupEvents && currentEventIsNotZero && currentEventValue == priorEventValue)
										|| (processingFades && currentEventIsNotZero && currentEventValue < priorEventValue)
										|| (processingRamps && priorEventisNotZero && currentEventValue > priorEventValue))) {

							patternFound = true;
							startEventPosition = currentEventNum - 1;
							startEventValue = priorEventValue;
							endEventPosition = currentEventNum;
							endEventValue = currentEventValue;
						}
						// Pattern continuing, update the end event postion and value.
						else if (patternFound
									&& ((processingGroupEvents && currentEventValue == priorEventValue)
										|| (processingFades && currentEventValue < priorEventValue)
										|| (processingRamps && priorEventisNotZero && currentEventValue > priorEventValue))) {

							endEventPosition = currentEventNum;
							endEventValue = currentEventValue;
						}
						// End of a pattern because none of the other conditions were met.
						else if (patternFound) {
							addEvent(pattern, currentElementNum, startEventPosition, startEventValue, priorEventNum, priorEventValue);

							patternFound = false;
							startEventPosition = resetEventPosition;
							endEventPosition = resetEventPosition;
						}
						priorEventValue = currentEventValue;
						priorEventNum = currentEventNum;
					} // for currentEvent

					// End of the Element, so process any existing patterns.
					if (patternFound) {
						addEvent(pattern, currentElementNum, startEventPosition, priorEventValue, priorEventNum);
					}

				} // for currentElementNum
			} // foreach patternType
		}

		private void addEvent(patternType pattern, int chan, int startPos, int startValue, int endPos, int endValue = 0) {
			//System.Diagnostics.Debug.Print("Pattern: {0}, SPos (Val): {1}({2}), EPos (Val): {3}({4}), Chan: {5}", pattern, startPos, startValue, endPos, endValue, chan);
			ElementNode targetNode = mappings[chan].ElementNode;

			if (targetNode != null) {
				EffectNode node = null;
				switch (pattern) {
					case patternType.SetLevelTrend:
					case patternType.SingleSetLevel:
						node = GenerateSetLevelEffect(startValue, startPos, endPos, targetNode);
						break;
					case patternType.PulseFadeTrend:
					case patternType.PulseRampTrend:
						node = GeneratePulseEffect(startValue, endValue, startPos, endPos, targetNode);
						break;
				}
				if (node != null) {
					Sequence.InsertData(node);
				}
			}
			markEventsProcessed(chan * parsedV2Sequence.EventsPerElement + startPos, chan * parsedV2Sequence.EventsPerElement + endPos);
		}

		private EffectNode GenerateSetLevelEffect(int eventValue, int startEvent, int endEvent, ElementNode targetNode) {
			IEffectModuleInstance setLevelInstance = ApplicationServices.Get<IEffectModuleInstance>(Guid.Parse("32cff8e0-5b10-4466-a093-0d232c55aac0")); // Clone() Doesn't work! :(
			setLevelInstance.TargetNodes = new ElementNode[]{targetNode};
			setLevelInstance.TimeSpan = TimeSpan.FromMilliseconds(parsedV2Sequence.EventPeriod * (endEvent - startEvent + 1));

			EffectNode effectNode = new EffectNode(setLevelInstance, TimeSpan.FromMilliseconds(parsedV2Sequence.EventPeriod * startEvent));
			effectNode.Effect.ParameterValues = new object[] { ((double)eventValue / byte.MaxValue), Color.White };

			return effectNode;
		}

		private EffectNode GeneratePulseEffect(int eventStartValue, int eventEndValue, int startEvent, int endEvent, ElementNode targetNode) {
			IEffectModuleInstance pulseInstance = ApplicationServices.Get<IEffectModuleInstance>(Guid.Parse("cbd76d3b-c924-40ff-bad6-d1437b3dbdc0")); // Clone() Doesn't work! :(
			pulseInstance.TargetNodes = new ElementNode[]{targetNode};
			pulseInstance.TimeSpan = TimeSpan.FromMilliseconds(parsedV2Sequence.EventPeriod * (endEvent - startEvent + 1));

			EffectNode effectNode = new EffectNode(pulseInstance, TimeSpan.FromMilliseconds(parsedV2Sequence.EventPeriod * startEvent));
			effectNode.Effect.ParameterValues = new Object[] { 
				new Curve(new PointPairList(new double[] { startX, endX }, new double[] { getY(eventStartValue), getY(eventEndValue) })), 
				new ColorGradient() 
			};

			return effectNode;
		}

		private double getY(int value) {
			return value / curveDivisor;
		}

		private void markEventsProcessed(int StartEvent, int EndEvent) {
			for (var i = StartEvent; i <= EndEvent; i++) {
				parsedV2Sequence.EventData[i] = zeroEventValue;
			}
		}
	}
}