using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using Vixen.Sys;

namespace VixenModules.App.TimedSequenceMapper
{
	[DataContract]
	public class TimedSequencePackagerData : ModuleDataModelBase
	{
		public TimedSequencePackagerData()
		{
			ExportSequenceFiles = new List<string>();
			ExportOutputFile = Path.Combine(Paths.DataRootPath, "Export");
			ExportIncludeAudio = true;
		}

		[DataMember]
		public List<string> ExportSequenceFiles { get; set; }

		[DataMember]
		public bool ExportIncludeAudio { get; set; }

		[DataMember]
		public string ExportOutputFile { get; set; }

		public override IModuleDataModel Clone()
		{
			return DeepCopy();
		}

		public TimedSequencePackagerData DeepCopy()
		{
			TimedSequencePackagerData newData = (TimedSequencePackagerData)MemberwiseClone();
			newData.ExportSequenceFiles = ExportSequenceFiles.ToList();
			newData.ExportIncludeAudio = ExportIncludeAudio;
			newData.ExportOutputFile = ExportOutputFile;
			return newData;
		}
	}
}