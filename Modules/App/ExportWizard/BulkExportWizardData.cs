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
			InitializeProfiles();
			//ActiveProfile = Profiles[0];
		}

		private void InitializeProfiles()
		{
			Profiles = new List<ExportProfile>();
			//Profiles.Add(new ExportProfile("Default", Export.DefaultFormatType(), Export.ExportDir, Export.ExportDir));
		}

		internal ExportProfile CreateDefaultProfile()
		{
			return new ExportProfile("Default", Export.DefaultFormatType(), Export.ExportDir, Export.ExportDir, true);
		}

		public Export Export
		{
			get { return _export ?? (_export = new Export()); }
		}

		[DataMember]
		public List<ExportProfile> Profiles { get; set; }

		public ExportProfile ActiveProfile { get; set; }

		internal void ConfigureExport(ExportProfile profile)
		{
			Export.ControllerExportInfo = profile.Controllers;
			Export.UpdateInterval = profile.Interval;
		}

		[OnDeserialized]
		private void OnDeserialization(StreamingContext context)
		{
			if (Profiles == null)
			{
				InitializeProfiles();
			}
		}

		public BulkExportWizardData CopyInto(BulkExportWizardData data)
		{
			if (data == null) return null;

			data.Profiles = Profiles;
			return data;
		}

		public override IModuleDataModel Clone()
		{
			var data = new BulkExportWizardData
			{
				Profiles = Profiles.Select(x => x.Clone() as ExportProfile).ToList(),
			};

			if(ActiveProfile != null)
			{
				data.ActiveProfile = data.Profiles.FirstOrDefault(x => ActiveProfile.Id == x.Id);
			}
			return data;
		}
	}
}
