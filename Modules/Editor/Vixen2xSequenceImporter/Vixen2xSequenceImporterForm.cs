using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;

using Vixen.Module.Effect;
using Vixen.Services;
using Vixen.Sys;

using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Sequence.Timed;

using ZedGraph;

namespace VixenModules.Editor.Vixen2xSequenceImporter {
	public partial class Vixen2xSequenceImporterForm : Form {
		public ISequence Sequence { get; set; }

		private Vixen2SequenceData parsedV2Sequence = null;
		private IEnumerator<ChannelNode> _channelNodes = VixenSystem.Nodes.AsEnumerable<ChannelNode>().GetEnumerator();
		private ChannelNode[] _currentChannelNode = null;

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

			if (isValidLayout()) {
				initializeProgressBar();
				createTimedSequence();
				importSequenceData();
				result = true;
			}
			else {
				var message = String.Format("With this version of the importer, you must have at least {0} elements defined to import {1}"
					,parsedV2Sequence.ChannelCount, parsedV2Sequence.FileName);
				MessageBox.Show(message);
			}
			this.Close();
			return result;
		}

		private bool isValidLayout() {
			return channelLeafNodeCount() >= parsedV2Sequence.ChannelCount;
		}

		private int channelLeafNodeCount() {
			var i = 0;
			try {
				while (true) {
					_channelNodes.MoveNext();
					while (!_channelNodes.Current.IsLeaf) {
						_channelNodes.MoveNext();
					}
					i++;
					System.Diagnostics.Debug.Print("" + i);
				}
			}
			catch(NullReferenceException ex){
				System.Diagnostics.Debug.Print(ex.ToString());
				// ignore - just means we're done counting
			}
			return i;
		}

		private void initializeProgressBar() {
			pbImport.Minimum = 0;
			pbImport.Maximum = sizeof(patternType) * parsedV2Sequence.ChannelCount;
			pbImport.Value = 0;
		}

		private void createTimedSequence() {
			Sequence = new TimedSequence();
			Sequence.SequenceData = new TimedSequenceData();
			VixenModules.Sequence.Timed.MarkCollection mc = new VixenModules.Sequence.Timed.MarkCollection();
			Sequence.Length = TimeSpan.FromMilliseconds(parsedV2Sequence.SeqLengthInMills);

			var songFileName = parsedV2Sequence.SongFileName;
			if (songFileName != null) {
				if (File.Exists(songFileName)) {
					Sequence.AddMedia(MediaService.Instance.GetMedia(songFileName));
				}
				else {
					dialogOpen.CheckFileExists = true;
					dialogOpen.CheckPathExists = true;
					dialogOpen.Title = "Audio Not Found!  Please Locate " + Path.GetFileName(songFileName);
					dialogOpen.Multiselect = false;
					dialogOpen.FileName = Path.GetFileName(songFileName);
					if (dialogOpen.ShowDialog() == DialogResult.OK) {
						Sequence.AddMedia(MediaService.Instance.GetMedia(dialogOpen.FileName));
					}
				}
			}
		}

		private void importSequenceData() {
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
				_channelNodes.Reset();
				for (var currentChannelNum = 0; currentChannelNum < parsedV2Sequence.ChannelCount; currentChannelNum++) {
					lblStatusLine.Text = String.Format("Finding {0} on Channel {1}", patternText, currentChannelNum + 1);
					pbImport.Value = ++pbImportValue;
					Application.DoEvents();

					setNextChannelNodeLeaf();

					patternFound = false;
					priorEventValue = zeroEventValue;
					priorEventNum = resetEventPosition;

					for (var currentEventNum = 0; currentEventNum < parsedV2Sequence.EventsPerChannel; currentEventNum++) {
						currentEventValue = parsedV2Sequence.EventData[currentChannelNum * parsedV2Sequence.EventsPerChannel + currentEventNum];

						currentEventIsZero = currentEventValue == zeroEventValue;
						currentEventIsNotZero = !currentEventIsZero;
						priorEventisNotZero = priorEventValue != zeroEventValue;
						
						// Add a non zero single set level event.
						if (processingSingleEvents && currentEventIsNotZero) {
							addEvent(pattern, currentChannelNum, currentEventNum, currentEventValue, currentEventNum);

							startEventPosition = resetEventPosition;
							endEventPosition = resetEventPosition;
						}
						// Add a ramp, fade or multi set level event since it just ended (a zero event was found)
						else if (patternFound && !processingSingleEvents && currentEventIsZero && endEventPosition != resetEventPosition) {
							addEvent(pattern, currentChannelNum, startEventPosition, startEventValue, endEventPosition, endEventValue);

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
						// End of a pattern because ...
						else if (patternFound) {
							addEvent(pattern, currentChannelNum, startEventPosition, startEventValue, priorEventNum, priorEventValue);

							patternFound = false;
							startEventPosition = resetEventPosition;
							endEventPosition = resetEventPosition;
						}
						priorEventValue = currentEventValue;
						priorEventNum = currentEventNum;
					} // for currentEvent

					// End of the channel, so process any existing patterns.
					if (patternFound) {
						addEvent(pattern, currentChannelNum, startEventPosition, priorEventValue, priorEventNum);
					}

				} // for currentChannel
			} // foreach patternType
		}

		private void setNextChannelNodeLeaf() {
			_channelNodes.MoveNext();
			while (!_channelNodes.Current.IsLeaf) {
				_channelNodes.MoveNext();
			}
			_currentChannelNode = new ChannelNode[] { _channelNodes.Current };
		}

		private void addEvent(patternType pattern, int chan, int startPos, int startValue, int endPos, int endValue = 0) {
			//System.Diagnostics.Debug.Print("Pattern: {0}, SPos (Val): {1}({2}), EPos (Val): {3}({4}), Chan: {5}", pattern, startPos, startValue, endPos, endValue, chan);
			EffectNode node = null;
			switch (pattern) {
				case patternType.SetLevelTrend:
				case patternType.SingleSetLevel:
					node = GenerateSetLevelEffect(startValue, startPos, endPos);
					break;
				case patternType.PulseFadeTrend:
				case patternType.PulseRampTrend:
					node = GeneratePulseEffect(startValue, endValue, startPos, endPos);
					break;
			}
			if (node != null) {
				Sequence.InsertData(node);
			}
			markEventsProcessed(chan * parsedV2Sequence.EventsPerChannel + startPos, chan * parsedV2Sequence.EventsPerChannel + endPos);
		}

		private EffectNode GenerateSetLevelEffect(int eventValue, int startEvent, int endEvent) {
			IEffectModuleInstance setLevelInstance = ApplicationServices.Get<IEffectModuleInstance>(Guid.Parse("32cff8e0-5b10-4466-a093-0d232c55aac0")); // Clone() Doesn't work! :(
			setLevelInstance.TargetNodes = _currentChannelNode;
			setLevelInstance.TimeSpan = TimeSpan.FromMilliseconds(parsedV2Sequence.EventPeriod * (endEvent - startEvent + 1));

			EffectNode effectNode = new EffectNode(setLevelInstance, TimeSpan.FromMilliseconds(parsedV2Sequence.EventPeriod * startEvent));
			effectNode.Effect.ParameterValues = new object[] { ((double)eventValue / byte.MaxValue), Color.White };

			return effectNode;
		}

		private EffectNode GeneratePulseEffect(int eventStartValue, int eventEndValue, int startEvent, int endEvent) {
			IEffectModuleInstance pulseInstance = ApplicationServices.Get<IEffectModuleInstance>(Guid.Parse("cbd76d3b-c924-40ff-bad6-d1437b3dbdc0")); // Clone() Doesn't work! :(
			pulseInstance.TargetNodes = _currentChannelNode;
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

		private void btnExit_Click(object sender, EventArgs e) {
			this.Close();
		}
	}
}