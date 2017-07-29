using System.Collections.Generic;
using Common.Controls.Wizard;

namespace VixenModules.App.ExportWizard
{
	internal class BulkExportWizard : Wizard
	{

		private readonly List<WizardStage> _stages;
		private BulkExportWizardData _data = new BulkExportWizardData();

		public BulkExportWizard()
		{
			_stages = new List<WizardStage>
			{
				new BulkExportSources(_data),
				new BulkExportControllers(_data)
			};
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
