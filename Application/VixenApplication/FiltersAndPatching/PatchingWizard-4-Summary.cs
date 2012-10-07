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
using Vixen.Sys;

namespace VixenApplication.FiltersAndPatching
{
	public partial class PatchingWizard_4_Summary : WizardStage
	{
		private readonly PatchingWizardData _data;

		public PatchingWizard_4_Summary(PatchingWizardData data)
		{
			_data = data;
			InitializeComponent();
		}

		private int _sourceCount;
		private int _filterInstancesCount;
		private int _destinationCount;
		private int _filterInstancesOutputCount;
		private int _filterLoops;
		private int _totalFilterOutputs;

		private void PatchingWizard_4_Summary_Load(object sender, EventArgs e)
		{
			string warning1 = "";
			string warning2 = "";

			_sourceCount = _data.Sources.Count;
			_filterInstancesCount = _data.Filters.Count;
			_destinationCount = _data.Destinations.Count;

			if (_filterInstancesCount > 0) {
				_filterInstancesOutputCount = _data.Filters.Sum(x => x.FilterInstance.Outputs.Length);
				_filterLoops = (int)Math.Ceiling((double)_sourceCount / _filterInstancesCount);
				_totalFilterOutputs = _filterLoops * _filterInstancesOutputCount;

				if (_destinationCount > _totalFilterOutputs) {
					warning1 = "Warning: there are more destination components than total filter outputs.";
					warning2 = "Some destination components won't be patched to.";
				}

				if (_destinationCount < _totalFilterOutputs) {
					warning1 = "Warning: there are more filter outputs than destination components.";
					warning2 = "Some filters will not be patched to anything.";
				}
			} else {
				if (_destinationCount > _sourceCount) {
					warning1 = "Warning: there are more destination components than sources.";
					warning2 = "Some destination components won't be patched to.";
				}

				if (_destinationCount < _sourceCount) {
					warning1 = "Warning: there are more sources than destination components.";
					warning2 = "Some sources will not be patched to anything.";
				}
			}

			labelWarning1.Text = warning1;
			labelWarning2.Text = warning2;

			_populateLists();
		}

		private void _populateLists()
		{
			listViewSources.BeginUpdate();
			listViewSources.Items.Clear();
			listViewDestinations.BeginUpdate();
			listViewDestinations.Items.Clear();
			listViewFilters.BeginUpdate();
			listViewFilters.Items.Clear();

			foreach (Tuple<FilterSetupShapeBase, int> source in _data.Sources) {
				listViewSources.Items.Add(source.Item1.Title + " [" + source.Item2 + "]");
			}

			foreach (FilterShape filterShape in _data.Filters) {
				listViewFilters.Items.Add(filterShape.Title);
			}

			foreach (FilterSetupShapeBase destination in _data.Destinations) {
				listViewDestinations.Items.Add(destination.Title);
			}

			listViewSources.EndUpdate();
			listViewDestinations.EndUpdate();
			listViewFilters.EndUpdate();

			listViewFilters.Visible = _data.Filters.Count > 0;
			labelFilters.Visible = _data.Filters.Count > 0;

			labelSources.Text = _sourceCount + " Sources:";
			labelDestinations.Text = _destinationCount + " Destinations:";
			labelFilters.Text = _filterInstancesCount + " Filters (" + _filterInstancesOutputCount + " outputs), x" + _filterLoops + ":";
		}

		private bool _doPatching()
		{
			bool success = true;

			if (_filterInstancesCount > 0) {
				// filters are being used in the patching process; duplicate the given filters as many times as needed,
				// then link up the sources to the filters. After that, link up the filters to destinations.

				List<FilterShape> clonedFilters = new List<FilterShape>(_data.FilterSetupForm.CopyFilterShapes(_data.Filters, _filterLoops));
				for (int i = 0; i < _sourceCount; i++) {
					success = success && VixenSystem.DataFlow.SetComponentSource(clonedFilters[i].FilterInstance, _data.Sources[i].Item1.DataFlowComponent, _data.Sources[i].Item2);
					_data.FilterSetupForm.ConnectShapes(_data.Sources[i].Item1, _data.Sources[i].Item2, clonedFilters[i]);
				}

				List<Tuple<FilterShape, int>> filterOutputReferences = new List<Tuple<FilterShape, int>>();
				foreach (FilterShape filterShape in clonedFilters) {
					for (int i = 0; i < filterShape.FilterInstance.Outputs.Length; i++) {
						filterOutputReferences.Add(new Tuple<FilterShape, int>(filterShape, i));
					}
				}

				for (int i = 0; i < Math.Min(_destinationCount, filterOutputReferences.Count); i++) {
					success = success && VixenSystem.DataFlow.SetComponentSource(_data.Destinations[i].DataFlowComponent, filterOutputReferences[i].Item1.DataFlowComponent, filterOutputReferences[i].Item2);
					_data.FilterSetupForm.ConnectShapes(filterOutputReferences[i].Item1, filterOutputReferences[i].Item2, _data.Destinations[i]);
				}
			} else {
				// no filters -- just directly patch sources with destinations.
				for (int i = 0; i < Math.Min(_sourceCount, _destinationCount); i++) {
					success = success && VixenSystem.DataFlow.SetComponentSource(_data.Destinations[i].DataFlowComponent, _data.Sources[i].Item1.DataFlowComponent, _data.Sources[i].Item2);
					_data.FilterSetupForm.ConnectShapes(_data.Sources[i].Item1, _data.Sources[i].Item2, _data.Destinations[i]);
				}
			}

			return success;
		}

		public override void StageEnd()
		{
			bool patchingSuccess = _doPatching();

			if (!patchingSuccess) {
				MessageBox.Show("There was an error performing some of the patching. Please verify the results.", "Patching error", MessageBoxButtons.OK);
			}
		}
	}
}
