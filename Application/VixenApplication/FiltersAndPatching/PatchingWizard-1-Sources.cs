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
	public partial class PatchingWizard_1_Sources : WizardStage
	{
		private readonly PatchingWizardData _data;

		public PatchingWizard_1_Sources(PatchingWizardData data)
		{
			_data = data;
			InitializeComponent();
		}

		private void FilterSetupForm_DiagramShapesSelected(object sender, EventArgs e)
		{
			_searchAndPopulateShapes();
		}

		private void PatchingWizard_1_Sources_Load(object sender, EventArgs e)
		{
			_searchAndPopulateShapes();
		}

		private void _searchAndPopulateShapes()
		{
			_data.Sources = new List<Tuple<FilterSetupShapeBase, int>>();

			listViewSources.BeginUpdate();
			listViewSources.Items.Clear();

			foreach (Shape selectedShape in _data.FilterSetupForm.SelectedShapes.Reverse()) {
				FilterSetupShapeBase shape = selectedShape as FilterSetupShapeBase;
				if (shape != null && shape.OutputCount > 0 && shape.DataFlowComponent != null) {
					string title = shape.Title;

					for (int i = 0; i < shape.OutputCount; i++) {
						_data.Sources.Add(new Tuple<FilterSetupShapeBase, int>(shape, i));
						// offset the index by 1 to be human-readable
						listViewSources.Items.Add(shape.OutputCount == 1 ? title : (title + " [Output " + (i + 1) + "]"));
					}
				}
			}

			listViewSources.EndUpdate();

			_WizardStageChanged();
		}

		public override void StageStart()
		{
			_data.FilterSetupForm.DiagramShapesSelected += FilterSetupForm_DiagramShapesSelected;
		}

		public override async Task StageEnd()
		{
			 _data.FilterSetupForm.DiagramShapesSelected -= FilterSetupForm_DiagramShapesSelected;
		}

		public override bool CanMoveNext
		{
			get { return _data.Sources.Count > 0; }
		}
	}
}