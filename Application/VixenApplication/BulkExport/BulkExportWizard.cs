using System.Collections.Generic;
using Common.Controls.Wizard;

namespace VixenApplication.BulkExport
{
	internal class BulkExportWizard : Wizard
	{

		private readonly List<WizardStage> _stages;

		public BulkExportWizard()
		{
			_stages = new List<WizardStage>
			{
				new BulkExportSources()
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
