using System.Collections.Generic;
using Vixen.Export;

namespace VixenModules.App.ExportWizard
{
	public class BulkExportWizardData
	{
		

		public BulkExportWizardData()
		{
			SequenceFiles = new List<string>();
			Export = new Export();
		}
		public List<string> SequenceFiles { get; set; }

		public Export Export { get; private set; }

	}
}
