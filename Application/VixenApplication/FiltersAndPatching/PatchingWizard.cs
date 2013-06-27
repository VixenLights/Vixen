using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Controls.Wizard;
using Vixen.Data.Flow;
using Vixen.Module.OutputFilter;
using Vixen.Sys;

namespace VixenApplication.FiltersAndPatching
{
	internal class PatchingWizard : Wizard
	{
		private readonly List<WizardStage> _stages;

		public PatchingWizard(ConfigFiltersAndPatching filterSetupForm)
		{
			PatchingWizardData data = new PatchingWizardData(filterSetupForm);

			_stages = new List<WizardStage>
			          	{
			          		new PatchingWizard_1_Sources(data),
			          		new PatchingWizard_2_Filters(data),
			          		new PatchingWizard_3_Destinations(data),
			          		new PatchingWizard_4_Summary(data),
			          	};
		}

		protected override List<WizardStage> Stages
		{
			get { return _stages; }
		}

		public override string WizardTitle
		{
			get { return "Patching Wizard"; }
		}
	}

	public class PatchingWizardData
	{
		public ConfigFiltersAndPatching FilterSetupForm { get; private set; }

		public List<Tuple<FilterSetupShapeBase, int>> Sources { get; set; }

		public List<FilterSetupShapeBase> Destinations { get; set; }

		public List<IOutputFilterModuleInstance> Filters { get; set; }

		public PatchingWizardData(ConfigFiltersAndPatching filterSetupForm)
		{
			FilterSetupForm = filterSetupForm;
			Sources = new List<Tuple<FilterSetupShapeBase, int>>();
			Destinations = new List<FilterSetupShapeBase>();
			Filters = new List<IOutputFilterModuleInstance>();
		}
	}
}