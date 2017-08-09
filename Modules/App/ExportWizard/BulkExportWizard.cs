using System.Collections.Generic;
using System.Windows.Forms;
using Common.Controls.Wizard;
using Vixen.Sys;

namespace VixenModules.App.ExportWizard
{
	internal class BulkExportWizard : Wizard
	{
		private readonly BulkExportWizardData _data;
		private readonly List<WizardStage> _stages;

		public BulkExportWizard(BulkExportWizardData data)
		{
			_data = data;
			_stages = new List<WizardStage>
			{
				new BulkExportCreateOrSelectStage(_data),
				new BulkExportSourcesStage(_data),
				new BulkExportControllersStage(_data),
				new BulkExportOutputFormatStage(_data),
				new BulkExportSummaryStage(_data),
				new BulkExportFinishedStage()
			};

			
		}

		public BulkExportWizardData Data
		{
			get { return _data; }
		}

		protected override List<WizardStage> Stages
		{
			get { return _stages; }
		}

		public override string WizardTitle
		{
			get { return "Export Wizard"; }
		}
	}
}
