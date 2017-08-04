using Common.Controls.Wizard;

namespace VixenModules.App.ExportWizard
{
	public partial class BulkExportFinishedStage : WizardStage
	{
		public BulkExportFinishedStage()
		{
			InitializeComponent();
		}

		public override bool CanMovePrevious
		{
			get { return false; }
		}

		public override bool IsPreviousVisible
		{
			get { return false; }
		}
	}
}
