using System.Media;
using Common.Controls.Wizard;

namespace VixenModules.App.ExportWizard
{
	public partial class BulkExportFinishedStage : WizardStage
	{
		public BulkExportFinishedStage()
		{
			InitializeComponent();
		}

		public override void StageStart()
		{
			SystemSounds.Asterisk.Play();
		}

		public override bool CanMovePrevious
		{
			get { return false; }
		}

		public override bool IsCancelVisible
		{
			get { return false; }
		}

		public override bool IsPreviousVisible
		{
			get { return false; }
		}
	}
}
