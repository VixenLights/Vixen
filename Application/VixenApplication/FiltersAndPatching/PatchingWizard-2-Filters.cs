using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Wizard;
using Dataweb.NShape;
using Vixen.Data.Flow;

namespace VixenApplication.FiltersAndPatching
{
	public partial class PatchingWizard_2_Filters : WizardStage
	{
		private readonly PatchingWizardData _data;

		public PatchingWizard_2_Filters(PatchingWizardData data)
		{
			_data = data;
			InitializeComponent();
		}

		void FilterSetupForm_DiagramShapesSelected(object sender, EventArgs e)
		{
			_data.Filters = new List<FilterShape>();

			listViewFilters.BeginUpdate();
			listViewFilters.Items.Clear();
			
			foreach (Shape selectedShape in _data.FilterSetupForm.SelectedShapes.Reverse()) {
				FilterShape shape = selectedShape as FilterShape;
				if (shape != null && shape.OutputCount > 0) {
					_data.Filters.Add(shape);
					string title = shape.Title;
					listViewFilters.Items.Add(title);
				}
			}

			listViewFilters.EndUpdate();

			_WizardStageChanged();
		}

		public override void StageStart()
		{
			_data.FilterSetupForm.DiagramShapesSelected += FilterSetupForm_DiagramShapesSelected;
		}

		public override void StageEnd()
		{
			_data.FilterSetupForm.DiagramShapesSelected -= FilterSetupForm_DiagramShapesSelected;
		}

	}
}
