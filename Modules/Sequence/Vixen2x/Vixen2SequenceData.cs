using System;
using System.Xml.Linq;
using System.IO;

namespace VixenModules.SequenceType.Vixen2x
{
	internal class Vixen2SequenceData
	{
		protected internal string FileName { get; private set; }

		protected internal int EventPeriod { get; private set; }

		protected internal byte[] EventData { get; private set; }

		protected internal string SongFileName { get; private set; }

		protected internal int SeqLengthInMills { get; private set; }

		protected internal int TotalEventsCount { get; private set; }

		protected internal int ElementCount { get; private set; }

		protected internal int EventsPerElement { get; private set; }

		protected internal Vixen2SequenceData(string fileName)
		{
			if (!File.Exists(fileName)) {
				throw new FileNotFoundException("Cannot Locate " + fileName);
			}
			FileName = fileName;
			ParseFile();
		}

		private void ParseFile()
		{
			XElement root = null;
			using (FileStream stream = new FileStream(FileName, FileMode.Open)) {
				root = XElement.Load(stream);
			}
			foreach (XElement element in root.Descendants()) {
				switch (element.Name.ToString()) {
					case "Time":
						SeqLengthInMills = Int32.Parse(element.Value);
						break;
					case "EventPeriodInMilliseconds":
						EventPeriod = Int32.Parse(element.Value);
						break;
					case "EventValues":
						EventData = Convert.FromBase64String(element.Value);
						break;
					case "Audio":
						SongFileName = element.Attribute("filename").Value;
						break;
					default:
						//ignore
						break;
				}
			}
			// These calculations could have been put in the properties, but then it gets confusing to debug because of all the jumping around.
			TotalEventsCount = Convert.ToInt32(Math.Ceiling((double) (SeqLengthInMills/EventPeriod)));
			;
			ElementCount = EventData.Length/TotalEventsCount;
			EventsPerElement = EventData.Length/ElementCount;
		}
	}
}