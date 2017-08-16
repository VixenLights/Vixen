using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls.Wizard;
using Dataweb.NShape;
using Vixen.Data.Flow;

namespace VixenApplication.FiltersAndPatching
{
	public partial class PatchingWizard_3_Destinations : WizardStage
	{
		private readonly PatchingWizardData _data;

		public PatchingWizard_3_Destinations(PatchingWizardData data)
		{
			_data = data;
			InitializeComponent();
		}

		private void FilterSetupForm_DiagramShapesSelected(object sender, EventArgs e)
		{
			_searchAndPopulateShapes();
		}

		private void PatchingWizard_3_Destinations_Load(object sender, EventArgs e)
		{
			_searchAndPopulateShapes();
		}

		private void _searchAndPopulateShapes()
		{
			_data.Destinations = new List<FilterSetupShapeBase>();

			listViewDestinations.BeginUpdate();
			listViewDestinations.Items.Clear();

			foreach (Shape selectedShape in _data.FilterSetupForm.SelectedShapes.Reverse()) {
				FilterSetupShapeBase shape = selectedShape as FilterSetupShapeBase;
				if (shape != null && shape.InputCount > 0 && shape.DataFlowComponent != null) {
					string title = shape.Title;
					_data.Destinations.Add(shape);
					listViewDestinations.Items.Add(title);
				}
			}

			listViewDestinations.EndUpdate();

			_WizardStageChanged();
		}

		public override void StageStart()
		{
			_data.FilterSetupForm.DiagramShapesSelected += FilterSetupForm_DiagramShapesSelected;
		}

		public override async Task StageEnd()
		{
			_data.FilterSetupForm.DiagramShapesSelected -= FilterSetupForm_DiagramShapesSelected;
			await Task.FromResult(true);
		}

		public override bool CanMoveNext
		{
			get { return _data.Destinations.Count > 0; }
		}
	}
}