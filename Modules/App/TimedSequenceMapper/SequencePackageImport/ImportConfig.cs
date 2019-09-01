using System.Collections.Generic;

namespace VixenModules.App.TimedSequenceMapper.SequencePackageImport
{
	public class ImportConfig
	{
		public ImportConfig()
		{
			Sequences = new Dictionary<string, bool>();
		}
		public string InputFile { get; set; }

		public string MapFile { get; set; }

		public Dictionary<string, bool> Sequences { get; set; }
	}
}
