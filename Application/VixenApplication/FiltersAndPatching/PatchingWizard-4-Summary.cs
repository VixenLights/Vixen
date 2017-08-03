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
using Vixen.Module.OutputFilter;
using Vixen.Sys;

namespace VixenApplication.FiltersAndPatching
{
	public partial class PatchingWizard_4_Summary : WizardStage
	{
		private readonly PatchingWizardData _data;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public PatchingWizard_4_Summary(PatchingWizardData data)
		{
			_data = data;
			InitializeComponent();
		}

		private int _sourceCount;
		private int _filterCount;
		private int _destinationCount;
		private Dictionary<IOutputFilterModuleInstance, int> _filterInstanceCount;
		private int _totalFilterInstances;
		private int _totalFilterOutputs;

		private void PatchingWizard_4_Summary_Load(object sender, EventArgs e)
		{
			string warning1 = string.Empty;
			string warning2 = string.Empty;

			_sourceCount = _data.Sources.Count;
			_filterCount = _data.Filters.Count;
			_destinationCount = _data.Destinations.Count;

			if (_filterCount > 0) {
				_totalFilterOutputs = _sourceCount;
				_totalFilterInstances = 0;

				_filterInstanceCount = new Dictionary<IOutputFilterModuleInstance, int>();
				foreach (IOutputFilterModuleInstance instance in _data.Filters) {
					_filterInstanceCount[instance] = _totalFilterOutputs;
					_totalFilterInstances += _totalFilterOutputs;
					_totalFilterOutputs *= instance.Outputs.Length;
				}

				if (_destinationCount > _totalFilterOutputs) {
					warning1 = "Warning: there are more destination components than total filter outputs.";
					warning2 = "Some destination components won't be patched to.";
				}

				if (_destinationCount < _totalFilterOutputs) {
					warning1 = "Warning: there are more filter outputs than destination components.";
					warning2 = "Some filters will not be patched to anything.";
				}
			}
			else {
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
				// offset the index by 1 to be human-readable
				listViewSources.Items.Add(source.Item1.Title + " [" + (source.Item2 + 1) + "]");
			}

			foreach (IOutputFilterModuleInstance instance in _data.Filters) {
				listViewFilters.Items.Add(instance.Name + " (x" + _filterInstanceCount[instance] + ")");
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
			labelFilters.Text = _totalFilterInstances + " Filters (" + _totalFilterOutputs + " outputs):";
		}

		private async Task<bool> _doPatching()
		{

			Task<bool> t = Task.Run(() => { 

				bool result = true;
				bool success;

				// if filters are being used in the patching process; duplicate the given filters as many times as needed, linking
				// up each layer with the last. the 'currentSources' variable tracks the last layer (which should be used as sources).
				List<Tuple<FilterSetupShapeBase, int>> currentSources = new List<Tuple<FilterSetupShapeBase, int>>(_data.Sources);
				int filterIteration = 0;
				foreach (IOutputFilterModuleInstance instance in _data.Filters) {
					// calculate the relative horizontal position the filters should be placed at (for multiple layers)
					double xPositionProportion = 0.2 + (0.6*((double) filterIteration/(_filterCount - 1)));
					if (_filterCount <= 1)
						xPositionProportion = 0.5;

					List<FilterShape> clonedFilters =
						new List<FilterShape>(_data.FilterSetupForm.DuplicateFilterInstancesToShapes(new[] {instance},
																									 _filterInstanceCount[instance], null,
																									 xPositionProportion));
					for (int i = 0; i < currentSources.Count; i++) {
						if (i >= clonedFilters.Count) {
							Logging.Error(
								"Patching Wizard: ran out of cloned filters when autopatching. We should have automatically generated enough!");
							return false;
						}
						success = VixenSystem.DataFlow.SetComponentSource(clonedFilters[i].FilterInstance,
																		  currentSources[i].Item1.DataFlowComponent,
																		  currentSources[i].Item2);
						if (success)
							_data.FilterSetupForm.ConnectShapes(currentSources[i].Item1, currentSources[i].Item2, clonedFilters[i]);
						result = result && success;
					}

					// propogate the filter outputs as the sources for the next iteration
					currentSources = new List<Tuple<FilterSetupShapeBase, int>>();
					foreach (FilterShape clonedFilter in clonedFilters) {
						for (int i = 0; i < clonedFilter.FilterInstance.Outputs.Length; i++) {
							currentSources.Add(new Tuple<FilterSetupShapeBase, int>(clonedFilter, i));
						}
					}
					filterIteration++;
				}

				// link up the final bunch -- the destinations with whatever sources are left
				for (int i = 0; i < Math.Min(_destinationCount, currentSources.Count); i++) {
					success = VixenSystem.DataFlow.SetComponentSource(_data.Destinations[i].DataFlowComponent,
																	  currentSources[i].Item1.DataFlowComponent, currentSources[i].Item2);
					if (success)
						_data.FilterSetupForm.ConnectShapes(currentSources[i].Item1, currentSources[i].Item2, _data.Destinations[i]);
					result = result && success;
				}

				return result;

			});

			return await t;
		}

		public override async Task StageEnd()
		{
			bool patchingSuccess = await _doPatching();

			if (!patchingSuccess) {
				MessageBox.Show("There was an error performing some of the patching. Please verify the results.", "Patching error",
				                MessageBoxButtons.OK);
			}
		}
	}
}