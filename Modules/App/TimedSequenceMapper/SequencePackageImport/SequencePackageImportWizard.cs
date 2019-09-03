using System.Collections.Generic;
using Common.Controls.Wizard;

namespace VixenModules.App.TimedSequenceMapper.SequencePackageImport
{
	public class SequencePackageImportWizard:Wizard
	{
		public SequencePackageImportWizard(ImportConfig data)
		{
			Stages = new List<WizardStage>
			{
				new SequencePackageImportInputStage(data),
				new SequencePackageImportSequencesStage(data),
				new SequencePackageImportSummaryStage(data),
				new SequencePackageImportFinishedStage()
			};
		}

		#region Overrides of Wizard

		/// <inheritdoc />
		protected override List<WizardStage> Stages { get; }

		/// <inheritdoc />
		public override string WizardTitle => @"Sequence Package Import";

		#endregion
	}
}
