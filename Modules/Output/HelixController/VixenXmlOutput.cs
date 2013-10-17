
using System.Xml.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Module.Controller;
using Vixen.Sys;
using Vixen.Execution;
using Vixen.Commands;
using System.Text;


namespace VixenModules.Output.HelixController
{
	[Serializable()]
	[XmlRoot("Program")]
	public class VixenXmlOutput
	{
		public string Time { get; set; }
		public string EventPeriodInMilliseconds { get; set; }
		public string EventValues { get; set; }

		[XmlArrayItem("Channel")]
		public List<string> Channels { get; set; }
		public Audio Audio { get; set; }
  	}

	public class Audio
	{
		[XmlAttribute]
		public string filename { get; set; }

		[XmlText]
		public string Value { get; set; }
	}
}