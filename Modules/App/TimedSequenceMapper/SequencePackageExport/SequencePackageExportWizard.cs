using System.Collections.Generic;
using Common.Controls.Wizard;

namespace VixenModules.App.TimedSequenceMapper.SequencePackageExport
{
	public class SequencePackageExportWizard: Wizard
	{
		public SequencePackageExportWizard(TimedSequencePackagerData data)
		{
			Stages = new List<WizardStage>
			{
				new SequencePackageExportSourcesStage(data),
				new SequencePackageExportOutputStage(data),
				new SequencePackageExportSummaryStage(data),
				new SequencePackageExportFinishedStage()
			};
		}

		#region Overrides of Wizard

		/// <inheritdoc />
		protected override List<WizardStage> Stages { get; }

		/// <inheritdoc />
		public override string WizardTitle => @"Sequence Package Export";

		#endregion
	}
}
