using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Data.Flow;
using Vixen.Module.OutputFilter;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace VixenApplication.Setup
{
	public partial class SetupPatchingSimple : UserControl, ISetupPatchingControl
	{
		//Logger Class
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public SetupPatchingSimple()
		{
			InitializeComponent();
			_UpdateEverything(Enumerable.Empty<ElementNode>(), new ControllersAndOutputsSet());
		}

		private void SetupPatchingSimple_Load(object sender, EventArgs e)
		{
		}


		private List<ElementNode> _cachedElementNodes;
		public void UpdateElementSelection(IEnumerable<ElementNode> nodes)
		{
			_cachedElementNodes = nodes.ToList();
			_UpdateElementDetails(_cachedElementNodes);
			_updatePatchingSummary();
		}

		public void UpdateElementDetails(IEnumerable<ElementNode> nodes)
		{
			_cachedElementNodes = nodes.ToList();
			_UpdateElementDetails(_cachedElementNodes);
			_updatePatchingSummary();
		}

		private ControllersAndOutputsSet _cachedControllersAndOutputs;
		public void UpdateControllerSelection(ControllersAndOutputsSet controllersAndOutputs)
		{
			_cachedControllersAndOutputs = controllersAndOutputs;
			_updateControllerDetails(controllersAndOutputs);
			_updatePatchingSummary();
		}

		public void UpdateControllerDetails(ControllersAndOutputsSet controllersAndOutputs)
		{
			_cachedControllersAndOutputs = controllersAndOutputs;
			_updateControllerDetails(controllersAndOutputs);
			_updatePatchingSummary();
		}

		public Control SetupPatchingControl
		{
			get { return this; }
		}


		public event EventHandler<FiltersEventArgs> FiltersAdded;
		public void OnFiltersAdded(FiltersEventArgs args)
		{
			if (FiltersAdded == null)
				return;

			FiltersAdded(this, args);
		}

		public event EventHandler PatchingUpdated;
		public void OnPatchingUpdated()
		{
			if (PatchingUpdated == null)
				return;

			PatchingUpdated(this, EventArgs.Empty);
		}




		private void _UpdateEverything(IEnumerable<ElementNode> selectedNodes, ControllersAndOutputsSet controllersAndOutputs)
		{
			_UpdateElementDetails(selectedNodes);
			_updateControllerDetails(controllersAndOutputs);
			_updatePatchingSummary();
		}


		private List<PatchStatusItem<IDataFlowComponentReference>> _componentOutputs;

		private void _UpdateElementDetails(IEnumerable<ElementNode> selectedNodes)
		{
			List<ElementNode> nodes = selectedNodes.ToList();

			labelItemCount.Text = nodes.Count.ToString();

			IEnumerable<PatchStatusItem<IDataFlowComponentReference>> outputs;
			int leafCount, groupCount, filterCount;
			_countTypesDescendingFromElements(nodes, out leafCount, out groupCount, out filterCount, out outputs);
			_componentOutputs = outputs.ToList();
			int patchedCount = _componentOutputs.Where(x => x.Patched).Count();
			int unpatchedCount = _componentOutputs.Where(x => !x.Patched).Count();

			labelPatchPointCount.Text = (patchedCount + unpatchedCount).ToString();
			labelConnectedPatchPointCount.Text = patchedCount.ToString();
			labelUnconnectedPatchPointCount.Text = unpatchedCount.ToString();
			labelGroupCount.Text = groupCount.ToString();
			labelElementCount.Text = leafCount.ToString();
			labelFilterCount.Text = filterCount.ToString();

			buttonUnpatchElements.Enabled = patchedCount > 0;
		}


		private void _countTypesDescendingFromElements(IEnumerable<ElementNode> elements,
			out int leafElementCount, out int groupCount, out int filterCount,
			out IEnumerable<PatchStatusItem<IDataFlowComponentReference>> outputs)
		{
			leafElementCount = groupCount = filterCount = 0;
			List<PatchStatusItem<IDataFlowComponentReference>> outputList = new List<PatchStatusItem<IDataFlowComponentReference>>();

			foreach (ElementNode element in elements) {
				int lec, gc, fc;
				IEnumerable<PatchStatusItem<IDataFlowComponentReference>> childOutputs;

				_countTypesDescendingFromElements(element.Children, out lec, out gc, out fc, out childOutputs);

				outputList.AddRange(childOutputs);

				if (element.Children.Any()) {
					gc++;
				} else {
					lec++;
				}

				if (element.Element != null) {
					IDataFlowComponent dfc = VixenSystem.DataFlow.GetComponent(element.Element.Id);
					childOutputs = _findPatchedAndUnpatchedOutputsFromComponent(dfc);
					outputList.AddRange(childOutputs);
				}

				IEnumerable<IOutputFilterModuleInstance> filters = _findFiltersThatDescendFromElement(element);
				fc += filters.Count();

				leafElementCount += lec;
				groupCount += gc;
				filterCount += fc;
			}

			outputs = outputList;
		}

		private IEnumerable<IOutputFilterModuleInstance> _findFiltersThatDescendFromElement(ElementNode element)
		{
			if (element == null || element.Element == null)
				return Enumerable.Empty<IOutputFilterModuleInstance>();

			return _findComponentsOfTypeInTreeFromComponent(
					VixenSystem.DataFlow.GetComponent(element.Element.Id),
					typeof(IOutputFilterModuleInstance))
				.Cast<IOutputFilterModuleInstance>();
		}

		private IEnumerable<IDataFlowComponent> _findComponentsOfTypeInTreeFromComponent(IDataFlowComponent dataFlowComponent, Type dfctype)
		{
			return VixenSystem.DataFlow.GetDestinationsOfComponent(dataFlowComponent)
				.SelectMany(x => _findComponentsOfTypeInTreeFromComponent(x, dfctype))
				.Concat(new[] { dataFlowComponent })
				.Where(dfc => dfctype.IsAssignableFrom(dfc.GetType()))
				;
		}

		private IEnumerable<PatchStatusItem<IDataFlowComponentReference>> _findPatchedAndUnpatchedOutputsFromComponent(IDataFlowComponent component)
		{
			List<PatchStatusItem<IDataFlowComponentReference>> outputList = new List<PatchStatusItem<IDataFlowComponentReference>>();
			if (component == null || component.Outputs == null) {
				return outputList;
			}

			for (int i = 0; i < component.Outputs.Length; i++) {
				IEnumerable<IDataFlowComponent> children = VixenSystem.DataFlow.GetDestinationsOfComponentOutput(component, i);

				// these will only count as outputs at the end of the output tree. So, if the output has no children, it's obviously the end.
				// If it does have children, it will only count if the child doesn't have any outputs at all. (ie. most likely a controller output).

				if (children.Any()) {
					bool anyChildHasOutputs = false;

					foreach (IDataFlowComponent child in children) {
						IEnumerable<PatchStatusItem<IDataFlowComponentReference>> childOutputs = _findPatchedAndUnpatchedOutputsFromComponent(child);
						if (childOutputs.Any()) {
							anyChildHasOutputs = true;
							outputList.AddRange(childOutputs);
						}
					}

					if (!anyChildHasOutputs)
						outputList.Add(new PatchStatusItem<IDataFlowComponentReference>(new DataFlowComponentReference(component, i), true));
				} else {
					outputList.Add(new PatchStatusItem<IDataFlowComponentReference>(new DataFlowComponentReference(component, i), false));
				}
			}

			return outputList;
		}




		private List<PatchStatusItem<IDataFlowComponent>> _controllerInputs;

		private void _updateControllerDetails(ControllersAndOutputsSet controllersAndOutputs)
		{
			int controllerCount = 0;
			int patchedCount = 0;
			int unpatchedCount = 0;

			_controllerInputs = new List<PatchStatusItem<IDataFlowComponent>>();

			foreach (KeyValuePair<IControllerDevice, HashSet<int>> controllersAndOutput in controllersAndOutputs) {
				controllerCount++;
				IControllerDevice controller = controllersAndOutput.Key;

				foreach (int outputIndex in controllersAndOutput.Value) {
					if (outputIndex > controller.Outputs.Length) {
						Logging.Warn("passed an output index greater than the controller output length: " + controller.Name + ", " + outputIndex);
						continue;
					}

					bool patched;
					if (controller.Outputs[outputIndex].Source == null || controller.Outputs[outputIndex].Source.Component == null) {
						unpatchedCount++;
						patched = false;
					} else {
						patchedCount++;
						patched = true;
					}
					_controllerInputs.Add(new PatchStatusItem<IDataFlowComponent>((controller as OutputController).GetDataFlowComponentForOutput(controller.Outputs[outputIndex]), patched));
				}
			}

			labelControllerCount.Text = controllerCount.ToString();
			labelOutputCount.Text = (patchedCount + unpatchedCount).ToString();
			labelPatchedOutputCount.Text = patchedCount.ToString();
			labelUnpatchedOutputCount.Text = unpatchedCount.ToString();

			buttonUnpatchControllers.Enabled = patchedCount > 0;
		}



		private List<PatchStatusItem<IDataFlowComponentReference>> _selectedPatchSources;
		private List<PatchStatusItem<IDataFlowComponent>> _selectedPatchDestinations;

		private void _updatePatchingSummary()
		{
			int patchSources = 0;
			int patchDestinations = 0;

			if (_componentOutputs != null) {
				if (radioButtonAllAvailablePatchPoints.Checked) {
					_selectedPatchSources = _componentOutputs;
				}
				else {
					if (!radioButtonUnconnectedPatchPointsOnly.Checked) {
						Logging.Warn("no radio button selected for patch sources options");
					}
					_selectedPatchSources = _componentOutputs.Where(x => !x.Patched).ToList();
				}
				patchSources = _selectedPatchSources.Count;
			}

			if (_controllerInputs != null) {
				if (radioButtonAllOutputs.Checked) {
					_selectedPatchDestinations = _controllerInputs;
				}
				else {
					if (!radioButtonUnpatchedOutputsOnly.Checked) {
						Logging.Warn("no radio button selected for patch destination options");
					}
					_selectedPatchDestinations = _controllerInputs.Where(x => !x.Patched).ToList();
				}
				patchDestinations = _selectedPatchDestinations.Count;
			}

			string summary = string.Format("This will patch {0} element patch points to {1} controller outputs.", patchSources, patchDestinations);
			labelPatchSummary.Text = summary;

			string warning = "";

			if (patchSources > 0 && patchDestinations > 0) {
				if (patchSources > patchDestinations) {
					warning = "Warning: too many elements, some will not be patched";
				}
				if (patchDestinations > patchSources) {
					warning = "Warning: too many outputs, some will not be patched";
				}
			}

			labelPatchWarning.Text = warning;

			buttonDoPatching.Enabled = patchSources > 0 && patchDestinations > 0;
		}





		private void buttonDoPatching_Click(object sender, EventArgs e)
		{
			if (_selectedPatchSources == null || _selectedPatchDestinations == null) {
				Logging.Error("null patch sources or destinations!");
				return;
			}

			int max = Math.Min(_selectedPatchSources.Count, _selectedPatchDestinations.Count);

			for (int i = 0; i < max; i++) {
				VixenSystem.DataFlow.SetComponentSource(_selectedPatchDestinations[i].Item, _selectedPatchSources[i].Item);
			}

			OnPatchingUpdated();
			_UpdateEverything(_cachedElementNodes, _cachedControllersAndOutputs);

			MessageBox.Show("Patched " + max + " element patch points to controllers.", "Patching Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void radioButtonPatching_CheckedChanged(object sender, EventArgs e)
		{
			_updatePatchingSummary();
		}


		private void buttonUnpatchElements_Click(object sender, EventArgs e)
		{
			// find all patches that will be removed. keep track of the element->filter patches,
			// then all other patches, in case we want to keep them.
			// TODO: eh, I may have written this while drunk, it doesn't seem like a particlarly good way to do it

			List<IDataFlowComponent> directElementChildren = new List<IDataFlowComponent>();
			List<IDataFlowComponent> nonDirectElementChildren = new List<IDataFlowComponent>();

			foreach (ElementNode selectedNode in _cachedElementNodes) {
				foreach (ElementNode leafNode in selectedNode.GetLeafEnumerator()) {
					if (leafNode.Element == null)
						continue;

					IDataFlowComponent leafNodeComponent = VixenSystem.DataFlow.GetComponent(leafNode.Element.Id);

					List<IDataFlowComponent> children = VixenSystem.DataFlow.GetDestinationsOfComponent(leafNodeComponent).ToList();
					directElementChildren.AddRange(children);

					foreach (IDataFlowComponent child in children) {
						nonDirectElementChildren.AddRange(_findComponentsOfTypeInTreeFromComponent(child, typeof(IDataFlowComponent)));
					}
				}
			}

			bool removeFilters = false;
			if (nonDirectElementChildren.Any(x => x is IOutputFilterModuleInstance)) {
				DialogResult dr;
				dr = MessageBox.Show("Some elements are patched to filters.  Should these filters be removed as well?",
				                     "Remove Filters?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

				if (dr == DialogResult.Cancel)
					return;

				removeFilters = (dr == DialogResult.Yes);
			}

			foreach (IDataFlowComponent directElementChild in directElementChildren) {
				VixenSystem.DataFlow.ResetComponentSource(directElementChild);

				if (removeFilters && directElementChild is IOutputFilterModuleInstance) {
					VixenSystem.Filters.RemoveFilter(directElementChild as IOutputFilterModuleInstance);
				}
			}

			if (removeFilters) {
				foreach (IDataFlowComponent nonDirectElementChild in nonDirectElementChildren) {
					if (nonDirectElementChild is IOutputFilterModuleInstance) {
						VixenSystem.Filters.RemoveFilter(nonDirectElementChild as IOutputFilterModuleInstance);
					}
				}
			}

			OnPatchingUpdated();
			_UpdateEverything(_cachedElementNodes, _cachedControllersAndOutputs);
		}


		private void buttonUnpatchControllers_Click(object sender, EventArgs e)
		{
			foreach (KeyValuePair<IControllerDevice, HashSet<int>> controllerAndOutput in _cachedControllersAndOutputs) {
				OutputController controller = controllerAndOutput.Key as OutputController;
				if (controller == null)
					continue;

				foreach (int i in controllerAndOutput.Value) {
					IDataFlowComponent outputComponent = controller.GetDataFlowComponentForOutput(controller.Outputs[i]);
					if (outputComponent == null)
						continue;

					VixenSystem.DataFlow.ResetComponentSource(outputComponent);
				}
			}

			OnPatchingUpdated();
			_UpdateEverything(_cachedElementNodes, _cachedControllersAndOutputs);
		}


	}

	public class PatchStatusItem<T>
	{
		public T Item;
		public bool Patched;

		public PatchStatusItem(T item, bool patched)
		{
			Item = item;
			Patched = patched;
		}
	}
}
