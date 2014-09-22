using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vixen.Sys;
using VixenModules.Sequence.Timed;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using Vixen.Services;
using Vixen.Module.Effect;
using VixenModules.App.Curves;
using VixenModules.App.ColorGradients;
using System.Drawing;
using ZedGraph;

namespace VixenModules.SequenceType.Vixen2x
{
	public class Vixen3SequenceCreator
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public ISequence Sequence { get; set; }

		private CoversionProgressForm conversionProgressBar = null;
		public Vixen2SequenceData parsedV2Sequence = null;
		private List<ChannelMapping> mappings = null;
		public Dictionary<Guid, List<ChannelMapping>> m_GuidToV2ChanList = new Dictionary<Guid, List<ChannelMapping>>();

		public Vixen3SequenceCreator(Vixen2SequenceData v2Sequence, List<ChannelMapping> list)
		{
			parsedV2Sequence = v2Sequence;
			mappings = list;
			m_GuidToV2ChanList = convertMapping(mappings);

			conversionProgressBar = new CoversionProgressForm();
			conversionProgressBar.Show();

			conversionProgressBar.SetupProgressBar(0, parsedV2Sequence.ElementCount);

			conversionProgressBar.StatusLineLabel = string.Empty;

			createTimedSequence();
			importSequenceData();

			conversionProgressBar.Close();
		}

		/// <summary>
		/// Pass through the list of V2 channels and build a list of V3 channels that map back to the V2 channels
		/// This will be used to try and consolidate color mapping of RGB outputs from tri channel inputs.
		/// </summary>
		/// <param name="mappings"></param>
		/// <returns></returns>
		private Dictionary<Guid, List<ChannelMapping>> convertMapping(List<ChannelMapping> mappings)
		{
			Dictionary<Guid, List<ChannelMapping>> outputMap = new Dictionary<Guid, List<ChannelMapping>>();
			foreach (var v2channel in mappings)
			{
				// does the requested V3 Channel GUID exist in our list?
				if (false == outputMap.ContainsKey(v2channel.ElementNodeId))
				{
					// create a new V3 GUID entry for this V2 channel
					List<ChannelMapping> tempChanMap = new List<ChannelMapping>();
					outputMap.Add(v2channel.ElementNodeId, tempChanMap);
				} // end need to create a new GUID entry
				else
				{
					// [debug]
					// Logging.Info("convertMapping: Ignoring Duplicate V2 Channel '" + v2channel.ChannelName + "' with GUID = '" + v2channel.ElementNodeId + ".");
				}

				// add this V2 channel to the list of V2 channels for this V3 channel
				Logging.Info("convertMapping: Adding V2 Channel '" + v2channel.ChannelName + "' to the output map for '" + v2channel.ElementNodeId + ".");
				outputMap[v2channel.ElementNodeId].Add(v2channel);
			} // end process each V2 channel

			// tell the caller how it went
			return outputMap;
		} // end convertMapping


		private void createTimedSequence()
		{
			Sequence = new TimedSequence() { SequenceData = new TimedSequenceData() };

			// TODO: use this mark collection (maybe generate a grid?)
			//I am not sure what to do with this, but it looks like John had a plan.
			MarkCollection mc = new MarkCollection();

			Sequence.Length = TimeSpan.FromMilliseconds(parsedV2Sequence.SeqLengthInMills);

			var songFileName = parsedV2Sequence.SongPath + Path.DirectorySeparatorChar + parsedV2Sequence.SongFileName;
			if (songFileName != null)
			{
				if (File.Exists(songFileName))
				{
					Sequence.AddMedia(MediaService.Instance.GetMedia(songFileName));
				}
				else
				{
					var message = string.Format("Could not locate the audio file '{0}'; please add it manually " +
												"after import (Under Tools -> Associate Audio).", Path.GetFileName(songFileName));
					MessageBox.Show(message, "Couldn't find audio");
				}
			}
		}

		/// <summary>
		/// Convert parsedV2Sequence into a V3 sequence
		/// </summary>
		private void importSequenceData()
		{
			// instantiate the state machine to process incoming data
			Vixen2xSequenceImportSM import = new Vixen2xSequenceImportSM(Sequence, parsedV2Sequence.EventPeriod);

			// the current color is based on the intensity of a three channel group
			int red = 0;
			int green = 0;
			int blue = 0;

			// for each channel in the V2 sequence
			for (var currentElementNum = 0; currentElementNum < parsedV2Sequence.ElementCount; currentElementNum++)
			{
				// Check to see if we are processing more elements than we have mappings. This showed up as an error for a user
				if (currentElementNum >= mappings.Count)
				{
					Logging.Error("importSequenceData: Trying to process more elements (" + parsedV2Sequence.ElementCount + ") than we have mappings. (" + mappings.Count + ")");
					continue;
				}
				ChannelMapping v2ChannelMapping = mappings[currentElementNum];

				// set the channel number and the time for each v2 event.
				import.OpenChannel(v2ChannelMapping.ElementNodeId, Convert.ToDouble(parsedV2Sequence.EventPeriod));

				// Logging.Debug("importSequenceData:currentElementNum: " + currentElementNum);

				string elementName = v2ChannelMapping.ChannelName;
				Color currentColor = Color.FromArgb(255, 255, 255);
				byte currentIntensity = 0;

				conversionProgressBar.UpdateProgressBar(currentElementNum);
				Application.DoEvents();

				// is this an unmapped output channel?
				if (Guid.Empty == v2ChannelMapping.ElementNodeId)
				{
					// no output channel. Move on to the next input channel
					continue;
				} // end no output channel defined

				// do we have a valid guid conversion?
				if ( false == m_GuidToV2ChanList.ContainsKey( v2ChannelMapping.ElementNodeId ))
				{
					Logging.Error("importSequenceData: Configuration error. GUID: '" + v2ChannelMapping.ElementNodeId + "' not found in m_GuidToV2ChanList.");
					continue;
				}

				// is this a valid pixel configuration
				if ((true == v2ChannelMapping.RgbPixel) && (3 != m_GuidToV2ChanList[v2ChannelMapping.ElementNodeId].Count))
				{
					Logging.Error("importSequenceData: Configuration error. Found '" + m_GuidToV2ChanList[v2ChannelMapping.ElementNodeId].Count + "' V2 channels attached to element '" + elementName + "'. Expected 3. Skipping Element");
					continue;
				}

				// process each event for this V2 channel
				for (uint currentEventNum = 0; currentEventNum < parsedV2Sequence.EventsPerElement; currentEventNum++)
				{
					// get the intensity for this V2 channel
					currentIntensity = parsedV2Sequence.EventData[currentElementNum * parsedV2Sequence.EventsPerElement + currentEventNum];

					// is this an RGB Pixel?
					if (true == v2ChannelMapping.RgbPixel)
					{
						// Only process the RED channel of a three channel pixel
						if (Color.Red != v2ChannelMapping.DestinationColor)
						{
							// this is not the red channel of a pixel
							continue;
						} // end not red pixel channel

						red = 0;
						green = 0;
						blue = 0;

						// process the input colors bound to this output channel
						foreach (ChannelMapping v2Channel in m_GuidToV2ChanList[v2ChannelMapping.ElementNodeId])
						{
							// Logging.Info("convertMapping: Processing V2 Channel '" + v2Channel.ChannelName + "' color intensity.");

							switch (v2Channel.DestinationColor.Name)
							{
								case "Red":
									{
										red = Math.Max( red, parsedV2Sequence.EventData[(Convert.ToInt64(v2Channel.ChannelNumber)-1) * parsedV2Sequence.EventsPerElement + currentEventNum]);
										break;
									} // end Red

								case "Green":
									{
										green = Math.Max(green, parsedV2Sequence.EventData[(Convert.ToInt64(v2Channel.ChannelNumber) - 1) * parsedV2Sequence.EventsPerElement + currentEventNum]);
										break;
									} // end Green

								case "Blue":
									{
										blue = Math.Max(blue, parsedV2Sequence.EventData[(Convert.ToInt64(v2Channel.ChannelNumber) - 1) * parsedV2Sequence.EventsPerElement + currentEventNum]);
										break;
									} // end Red

								default:
									{
										Logging.Error("importSequenceData pixel conversion processing error. Skipping processing unexpected color '" + v2Channel.DestinationColor.Name + "' for V2 Channel '" + v2Channel.ChannelName + "(" + v2Channel.ChannelNumber + ")'. Color must be one of 'RED', 'GREEN' or 'BLUE'");
										break;
									} // end default
							} // end switch on color
						} // end process each V2 channel assigned to the v3 channel

						// get the max intensity for this v2 channel set
						currentIntensity = Convert.ToByte(Math.Min( (int)255, Math.Max(red, Math.Max(green, blue))));

						// Scale the color to full intensity and let the intensity value attenuate it.
						if (0 != currentIntensity)
						{
							double multplier = Convert.ToDouble(byte.MaxValue) / Convert.ToDouble(currentIntensity);

							red = Math.Min(((int)255), Convert.ToInt32(Convert.ToDouble(red) * multplier));
							green = Math.Min(((int)255), Convert.ToInt32(Convert.ToDouble(green) * multplier));
							blue = Math.Min(((int)255), Convert.ToInt32(Convert.ToDouble(blue) * multplier));
						}

						// set the final color
						currentColor = Color.FromArgb(red, green, blue);
					} // end pixel processing
					else
					{
						// set the non pixel color value
						currentColor = mappings[currentElementNum].DestinationColor;
					} // end non pixel processing

					// process the event through the state machine.
					import.processEvent(currentEventNum, currentColor, currentIntensity);
				} // end for each event in the element / channel

				// close this channel
				import.closeChannel();
			} // end for each input channel

		} // end importSequenceData
	}
}