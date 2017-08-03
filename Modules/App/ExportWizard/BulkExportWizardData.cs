using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Export;
using Vixen.Module;

namespace VixenModules.App.ExportWizard
{
	[DataContract]
	public class BulkExportWizardData: ModuleDataModelBase
	{
		private Export _export;
		public BulkExportWizardData()
		{
			SequenceFiles = new List<string>();
			Interval = 50;
			Format = Export.FormatTypes[0];
			OutputFolder = Export.ExportDir;
			AudioOutputFolder = Export.ExportDir;
			InitializeControllerInfo();
		}

		public Export Export
		{
			get
			{
				if (_export == null)
				{
					_export = new Export();
				}
				return _export;
			}
		}

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
		public List<ControllerExportInfo> ControllerInfo { get; private set; }

		internal void InitializeControllerInfo()
		{
			ControllerInfo = Export.CreateControllerInfo();
		}

		internal void ConfigureExport()
		{
			Export.ControllerExportInfo = ControllerInfo;
			Export.UpdateInterval = Interval;
		}

		public BulkExportWizardData CopyInto(BulkExportWizardData data)
		{
			if (data == null) return null;
			data.SequenceFiles = SequenceFiles.ToList();
			data.ControllerInfo = ControllerInfo.Select(x => x.Clone() as ControllerExportInfo).ToList();
			data.Format = Format;
			data.OutputFolder = OutputFolder;
			data.Interval = Interval;
			data.IncludeAudio = IncludeAudio;
			data.RenameAudio = RenameAudio;
			data.AudioOutputFolder = AudioOutputFolder;
			return data;
		}

		public override IModuleDataModel Clone()
		{
			var data = new BulkExportWizardData
			{
				SequenceFiles = SequenceFiles.ToList(),
				ControllerInfo = ControllerInfo.Select(x => x.Clone() as ControllerExportInfo).ToList(),
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
