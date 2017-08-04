using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Export;

namespace VixenModules.App.ExportWizard
{
	[DataContract]
	public class ExportProfile:ICloneable
	{
		public ExportProfile(string name, string format, string sequenceFolder, string audioFolder)
		{
			SequenceFiles = new List<string>();
			Interval = 50;
			Format = format;
			OutputFolder = sequenceFolder;
			AudioOutputFolder = audioFolder;
			Id = Guid.NewGuid();
			Name = name;
			SyncronizeControllerInfo();
		}

		private ExportProfile()
		{
			
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public List<string> SequenceFiles { get; set; }

		[DataMember]
		public int Interval { get; set; }

		[DataMember]
		public string Format { get; set; }

		[DataMember]
		public string OutputFolder { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool IncludeAudio { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public bool RenameAudio { get; set; }

		[DataMember]
		public string AudioOutputFolder { get; set; }

		[DataMember]
		public List<Controller> Controllers { get; private set; }

		internal void SyncronizeControllerInfo()
		{

			if (Controllers == null)
			{
				Controllers = Export.CreateControllerInfo(true);
				return;
			}
			//get our current list
			var currentControllerInfo = Export.CreateControllerInfo(false);

			Dictionary<Guid, Controller> lookup = Controllers.ToDictionary(x => x.Id);

			foreach (var controllerExportInfo in currentControllerInfo)
			{
				Controller controllerInfo;
				if (lookup.TryGetValue(controllerExportInfo.Id, out controllerInfo))
				{
					controllerExportInfo.IsActive = controllerInfo.IsActive;
					controllerExportInfo.Index = controllerInfo.Index;
				}

			}

			//re-index
			int index = 0;
			foreach (var controllerExportInfo in currentControllerInfo.OrderBy(x => x.Index))
			{
				controllerExportInfo.Index = index++;
			}

			Controllers = currentControllerInfo;
		}

		public object Clone()
		{
			var data = new ExportProfile()
			{
				Name = Name,
				Id = Id,
				SequenceFiles = SequenceFiles.ToList(),
				Controllers = Controllers.Select(x => x.Clone() as Controller).ToList(),
				Format = Format,
				Interval = Interval,
				OutputFolder = OutputFolder,
				IncludeAudio = IncludeAudio,
				RenameAudio = RenameAudio,
				AudioOutputFolder = AudioOutputFolder
			};
			return data;
		}
	}
}
