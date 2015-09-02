using System;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace VixenModules.SequenceType.Vixen2x
{
	public class Vixen2SequenceData
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		protected internal string FileName { get; private set; }

		protected internal string ProfileName { get; private set; }

		protected internal string ProfilePath { get; private set; }

		protected internal int EventPeriod { get; private set; }

		protected internal byte[] EventData { get; private set; }

		protected internal string SongFileName { get; private set; }

		protected internal string SongPath { get; private set; }

		protected internal int SeqLengthInMills { get; private set; }

		protected internal int TotalEventsCount { get; private set; }

		protected internal int ElementCount { get; private set; }

		protected internal int EventsPerElement { get; private set; }

		protected internal List<ChannelMapping> mappings { get; private set; }

		protected internal Vixen2SequenceData(string fileName)
		{
			if (!File.Exists(fileName))
			{
				throw new FileNotFoundException("Cannot Locate " + fileName);
			}
			FileName = fileName;
			mappings = new List<ChannelMapping>();
			ParseFile();
		}

		private void ParseFile()
		{
			XElement root = XElement.Load(FileName);
			
			//For all version of Vixen the Time, EventPeriod, EventValues and Audio are at the root level,
			//so just select them.
			SeqLengthInMills = Int32.Parse(root.Element("Time").Value);
			EventPeriod = Int32.Parse(root.Element("EventPeriodInMilliseconds").Value);
			EventData = Convert.FromBase64String(root.Element("EventValues").Value);
			
			//Someone may have decided to not use audio so we need to check for that as well.
			var songElement = root.Elements("Audio").SingleOrDefault();
			if (songElement != null)
			{
				SongFileName = root.Element("Audio").Attribute("filename").Value;
			}

			//if the sequence is flattened then the profile element will not exists so lets check for it.
			var profileElement = root.Elements("Profile").SingleOrDefault();
			if(profileElement != null)
			{
			ProfileName = root.Element("Profile").Value;
			}

			//If NO profile is set, parse channels from sequence (since the profile is flattened into 
			if (String.IsNullOrEmpty(ProfileName))
			{
				foreach (XElement e in root.Elements("Channels").Elements("Channel"))
				{
					XAttribute nameAttrib = e.Attribute("name");
					XAttribute colorAttrib = e.Attribute("color");

					//This exists in the 2.5.x versions of Vixen
					//<Channel name="Mini Tree Red 1" color="-65536" output="0" id="5576725746726704001" enabled="True" />
					if (nameAttrib != null)
					{
						CreateMappingList(e, 2);
					}
					//This exists in the older versions
					//<Channel color="-262330" output="0" id="633580705216250000" enabled="True">FenceIcicles-1</Channel>
					else if (colorAttrib != null)
					{
						CreateMappingList(e, 1);
					}
				}
			}

			if (!String.IsNullOrEmpty(SongFileName))
				MessageBox.Show(
					string.Format("Audio File {0} is associated with this sequence, please select the location of the audio file.",
								  SongFileName), "Select Audio Location", MessageBoxButtons.OK, MessageBoxIcon.Information);
			var dialog = new OpenFileDialog
							{
								Multiselect = false,
								Title = string.Format("Open Vixen 2.x Audio  [{0}]", SongFileName),
								Filter = "Audio|*.mp3|All Files (*.*)|*.*",
								RestoreDirectory = true,
								InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Vixen\Audio"
							};

			using (dialog)
			{
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					SongFileName = dialog.SafeFileName;
					SongPath = Path.GetDirectoryName(dialog.FileName);
				}
			}

			//check to see if the ProfileName is not null, if it isn't lets notify the user so they
			//can let us load the data
			if (!String.IsNullOrEmpty(ProfileName))
			{
				MessageBox.Show(
					string.Format("Vixen {0}.pro is associated with this sequence, please select the location of the profile.",
								  ProfileName), "Select Profile Location", MessageBoxButtons.OK, MessageBoxIcon.Information);
				dialog = new OpenFileDialog
							{
								Multiselect = false,
								Title = string.Format("Open Vixen 2.x Profile [{0}]", ProfileName),
								Filter = "Profile|*.pro",
								RestoreDirectory = true,
								InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Vixen\Profiles"
							};

				using (dialog)
				{
					if (dialog.ShowDialog() == DialogResult.OK)
					{
						ProfilePath = Path.GetDirectoryName(dialog.FileName);
						ProfileName = dialog.SafeFileName;
						root = null;
						root = XElement.Load(dialog.FileName);

						foreach (XElement e in root.Elements("ChannelObjects").Elements("Channel"))
						{
							XAttribute nameAttrib = e.Attribute("name");
							XAttribute colorAttrib = e.Attribute("color");

							//This exists in the 2.5.x versions of Vixen
							//<Channel name="Mini Tree Red 1" color="-65536" output="0" id="5576725746726704001" enabled="True" />
							if (nameAttrib != null)
							{
								CreateMappingList(e, 2);
							}
							//This exists in the older versions
							//<Channel color="-262330" output="0" id="633580705216250000" enabled="True">FenceIcicles-1</Channel>
							else if (colorAttrib != null)
							{
								CreateMappingList(e, 1);
							}

						}
					}
				}
			}
			else
			//if the profile name is null or empty then the sequence must have been flattened so indicate that.
			{
				ProfileName = "Sequence has been flattened no profile is available";
			}

			// These calculations could have been put in the properties, but then it gets confusing to debug because of all the jumping around.
			TotalEventsCount = Convert.ToInt32(Math.Ceiling((double)(SeqLengthInMills / EventPeriod)));
			;
			ElementCount = EventData.Length / TotalEventsCount;
			EventsPerElement = EventData.Length / ElementCount;

			// are we seeing an error in the mappings counts?
			if (mappings.Count != ElementCount)
			{
				Logging.Error("ParseFile: Actual mappings (" + mappings.Count + ") and calculated mappings (" + ElementCount + ") do not match. Using the Actual mappings value.");
				ElementCount = mappings.Count;
			} // end fix error in the V2 element reporting calculations
		} // ParseFile

		private void CreateMappingList(XElement element, int version)
		{
			//if version == 1 then we have an old profile that we are dealing with so we have
			//to get the node value for the channel name
			var channelname = string.Empty;
			if (version == 1)
			{
				channelname = element.FirstNode.ToString();
			}
			//must be version 2.5 so get the channel name from attribute 'name'
			else if (version == 2)
			{
				channelname = element.Attribute("name").Value;
			}

			mappings.Add(new ChannelMapping(channelname,
											Color.FromArgb(int.Parse(element.Attribute("color").Value)),
											(mappings.Count + 1).ToString(),
											element.Attribute("output").Value));
		}
	}
}