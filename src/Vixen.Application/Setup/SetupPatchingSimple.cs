#nullable disable

using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using System.Diagnostics;
using Vixen.Data.Flow;
using Vixen.Module.OutputFilter;
using Vixen.Rule;
using Vixen.Sys;
using Vixen.Sys.Output;
using VixenModules.Property.Order;

namespace VixenApplication.Setup
{
	public partial class SetupPatchingSimple : UserControl, ISetupPatchingControl
	{
		//Logger Class
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public SetupPatchingSimple()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);

			labelPatchPointCount.Font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
			label5.Font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
			labelOutputCount.Font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
			label20.Font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
			buttonDoPatching.Font = new Font(Font.FontFamily, 12F);
			labelPatchWarning.ForeColor = Color.FromArgb(255, 255, 0);
			_UpdateEverything(Enumerable.Empty<ElementNode>(), new ControllersAndOutputsSet());
		}

		private void SetupPatchingSimple_Load(object sender, EventArgs e)
		{
			selectedControllersLayout.Resize += SelectedControllersLayout_Resize;
		}

		private void SelectedControllersLayout_Resize(object sender, EventArgs e)
		{
			labelLastOutput.MaximumSize = labelFirstOutput.MaximumSize = new Size(selectedControllersLayout.GetColumnWidths()[1], 0);
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

		public DisplaySetup MasterForm { get; set; }

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
			_countedNodes.Clear();
			_countTypesDescendingFromElements(nodes, out leafCount, out groupCount, out filterCount, out outputs);
			_componentOutputs = outputs.ToList();
			int patchedCount = _componentOutputs.Count(x => x.Patched);
			int unpatchedCount = _componentOutputs.Count(x => !x.Patched);

			labelPatchPointCount.Text = (patchedCount + unpatchedCount).ToString();
			labelConnectedPatchPointCount.Text = patchedCount.ToString();
			labelUnconnectedPatchPointCount.Text = unpatchedCount.ToString();
			labelGroupCount.Text = groupCount.ToString();
			labelElementCount.Text = leafCount.ToString();
			labelFilterCount.Text = filterCount.ToString();

			buttonUnpatchElements.Enabled = patchedCount > 0;
		}

		private List<PatchStatusItem<IDataFlowComponentReference>> GetOrderedElementOutputs(IEnumerable<ElementNode> nodes)
		{
			List<PatchStatusItem<IDataFlowComponentReference>> outputList = new List<PatchStatusItem<IDataFlowComponentReference>>();

			//Compile the list of leaf nodes
			List<ElementNode> leafNodes = new List<ElementNode>();
			foreach (var node in nodes)
			{
				leafNodes.AddRange(node.GetLeafEnumerator());
			}

			leafNodes = leafNodes.Distinct().ToList();
			var orderedLeafNodes = leafNodes;
			//check to see if we have any order properties 
			if (leafNodes.Any(x => x.Properties.Contains(OrderDescriptor.ModuleId)))
			{
				//We have some, but do all of them have an order property??
				if (leafNodes.All(x => x.Properties.Contains(OrderDescriptor.ModuleId)))
				{
					//They all have them but are there any dupes???
					if (leafNodes.GroupBy(x => ((OrderModule)x.Properties.Get(OrderDescriptor.ModuleId)).Order)
						.Any(g => g.Count() > 1))
					{
						MessageBoxForm mbf = new MessageBoxForm(@"Some elements have a duplicate preferred patching order set. Would you like to fix this before proceeding?" +
							"\n\nYes) Open the order wizard.\nNo) Proceed using the element tree order.\nCancel) Abort patching.",
							"Patching Order Conflict", MessageBoxButtons.YesNoCancel, SystemIcons.Warning);
						var result = mbf.ShowDialog(this);
						if (result == DialogResult.Cancel)
						{
							return null;
						}

						if (result == DialogResult.OK)
						{
							if (!ConfigureOrder())
							{
								return null;
							}

							orderedLeafNodes = OrderNodes(leafNodes);
						}
					}
					else
					{
						orderedLeafNodes = OrderNodes(leafNodes);
					}

				}
				else
				{
					MessageBoxForm mbf = new MessageBoxForm(@"Not all elements have a preferred patching order set. Would you like to fix this before proceding?" +
						"\n\nYes) Open the order wizard.\nNo) Proceed using the element tree order.\nCancel) Abort patching.",
						"Patching Order Conflict", MessageBoxButtons.YesNoCancel, SystemIcons.Warning);
					var result = mbf.ShowDialog(this);
					if (result == DialogResult.Cancel)
					{
						return null;
					}

					if (result == DialogResult.OK)
					{
						if (ConfigureOrder())
						{
							orderedLeafNodes = OrderNodes(leafNodes);
						}
						else
						{
							return null;
						}
					}
				}
			}

			//Reverse if we need to.
			if (_reverseElementOrder)
			{
				orderedLeafNodes.Reverse();
			}

			foreach (var orderedLeafNode in orderedLeafNodes)
			{
				if (orderedLeafNode.Element != null)
				{
					IDataFlowComponent dfc = VixenSystem.DataFlow.GetComponent(orderedLeafNode.Element.Id);
					var childOutputs = _findPatchedAndUnpatchedOutputsFromComponent(dfc);
					outputList.AddRange(childOutputs);
				}
			}

			return outputList;
		}

		private static List<ElementNode> OrderNodes(List<ElementNode> leafNodes)
		{
			return leafNodes.OrderBy(x => ((OrderModule)x.Properties.Get(OrderDescriptor.ModuleId)).Order).ToList();
		}

		private bool ConfigureOrder()
		{
			IElementSetupHelper helper = new OrderSetupHelper();
			return helper.Perform(_cachedElementNodes);
		}

		private readonly HashSet<ElementNode> _countedNodes = new HashSet<ElementNode>();
		private void _countTypesDescendingFromElements(IEnumerable<ElementNode> elements,
							out int leafElementCount, out int groupCount, out int filterCount,
							out IEnumerable<PatchStatusItem<IDataFlowComponentReference>> outputs)
		{
			leafElementCount = groupCount = filterCount = 0;
			List<PatchStatusItem<IDataFlowComponentReference>> outputList = new List<PatchStatusItem<IDataFlowComponentReference>>();

			// process each element
			foreach (var element in elements)
			{
				if (_countedNodes.Contains(element)) continue;
				_countedNodes.Add(element);
				int lec, gc, fc;
				IEnumerable<PatchStatusItem<IDataFlowComponentReference>> childOutputs;

				// process any child elements
				_countTypesDescendingFromElements(element.Children, out lec, out gc, out fc, out childOutputs);

				outputList.AddRange(childOutputs);

				if (element.Children.Any())
				{
					gc++;
				}
				else
				{
					lec++;
				}

				IEnumerable<IOutputFilterModuleInstance> filters = _findFiltersThatDescendFromElement(element);
				fc += filters.Count();

				if (element.Element != null)
				{
					IDataFlowComponent dfc = VixenSystem.DataFlow.GetComponent(element.Element.Id);
					childOutputs = _findPatchedAndUnpatchedOutputsFromComponent(dfc);
					outputList.AddRange(childOutputs);
				}

				// do some accounting
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
			if (component == null || component.Outputs == null)
			{
				return outputList;
			}

			for (int i = 0; i < component.Outputs.Length; i++)
			{
				IEnumerable<IDataFlowComponent> children = VixenSystem.DataFlow.GetDestinationsOfComponentOutput(component, i);

				// these will only count as outputs at the end of the output tree. So, if the output has no children, it's obviously the end.
				// If it does have children, it will only count if the child doesn't have any outputs at all. (ie. most likely a controller output).

				if (children.Any())
				{
					bool anyChildHasOutputs = false;

					foreach (IDataFlowComponent child in children)
					{
						IEnumerable<PatchStatusItem<IDataFlowComponentReference>> childOutputs = _findPatchedAndUnpatchedOutputsFromComponent(child);
						if (childOutputs.Any())
						{
							anyChildHasOutputs = true;
							outputList.AddRange(childOutputs);
						}
					}

					if (!anyChildHasOutputs)
						outputList.Add(new PatchStatusItem<IDataFlowComponentReference>(new DataFlowComponentReference(component, i), true));
				}
				else
				{
					outputList.Add(new PatchStatusItem<IDataFlowComponentReference>(new DataFlowComponentReference(component, i), false));
				}
			}

			return outputList;
		}




		private List<PatchStatusItem<IDataFlowComponent>> _controllerInputs;

		private void _updateControllerDetails(ControllersAndOutputsSet controllersAndOutputs, bool skipStats = false)
		{
			int controllerCount = 0;
			int patchedCount = 0;
			int unpatchedCount = 0;

			_controllerInputs = new List<PatchStatusItem<IDataFlowComponent>>();

			foreach (KeyValuePair<IControllerDevice, HashSet<int>> controllersAndOutput in controllersAndOutputs)
			{
				controllerCount++;
				IControllerDevice controller = controllersAndOutput.Key;

				foreach (int outputIndex in controllersAndOutput.Value)
				{
					if (outputIndex >= controller.Outputs.Length)
					{
						Logging.Warn("passed an output index greater than the controller output length: " + controller.Name + ", " + outputIndex);
						continue;
					}

					bool patched;
					if (controller.Outputs[outputIndex].Source == null || controller.Outputs[outputIndex].Source.Component == null)
					{
						unpatchedCount++;
						patched = false;
					}
					else
					{
						patchedCount++;
						patched = true;
					}
					_controllerInputs.Add(new PatchStatusItem<IDataFlowComponent>((controller as OutputController).GetDataFlowComponentForOutput(controller.Outputs[outputIndex]), patched));
				}
			}

			if (checkBoxReverseOutputOrder.Checked)
			{
				_controllerInputs.Reverse();
			}

			if (skipStats) return;

			labelControllerCount.Text = controllerCount.ToString();
			labelOutputCount.Text = (patchedCount + unpatchedCount).ToString();
			labelPatchedOutputCount.Text = patchedCount.ToString();
			labelUnpatchedOutputCount.Text = unpatchedCount.ToString();

			buttonUnpatchControllers.Enabled = patchedCount > 0;

			labelFirstOutput.Text = "";
			labelLastOutput.Text = "";

			if (_controllerInputs.Any())
			{
				IControllerDevice controller;
				int outputIndex;
				VixenSystem.OutputControllers.getOutputDetailsForDataFlowComponent(_controllerInputs.First().Item, out controller, out outputIndex);
				labelFirstOutput.Text = string.Format("{0} #{1}", controller.Name, outputIndex + 1);

				VixenSystem.OutputControllers.getOutputDetailsForDataFlowComponent(_controllerInputs.Last().Item, out controller, out outputIndex);
				labelLastOutput.Text = string.Format("{0} #{1}", controller.Name, outputIndex + 1);
			}
		}



		private List<PatchStatusItem<IDataFlowComponentReference>> _selectedPatchSources;
		private List<PatchStatusItem<IDataFlowComponent>> _selectedPatchDestinations;
		private bool _reverseElementOrder = false;

		private void _updatePatchingSummary()
		{
			int patchSources = 0;
			int patchDestinations = 0;

			if (_componentOutputs != null)
			{
				if (radioButtonAllAvailablePatchPoints.Checked)
				{
					_selectedPatchSources = _componentOutputs;
				}
				else
				{
					if (!radioButtonUnconnectedPatchPointsOnly.Checked)
					{
						Logging.Warn("no radio button selected for patch sources options");
					}
					_selectedPatchSources = _componentOutputs.Where(x => !x.Patched).ToList();
				}
				patchSources = _selectedPatchSources.Count;
			}

			if (_controllerInputs != null)
			{
				if (radioButtonAllOutputs.Checked)
				{
					_selectedPatchDestinations = _controllerInputs;
				}
				else
				{
					if (!radioButtonUnpatchedOutputsOnly.Checked)
					{
						Logging.Warn("no radio button selected for patch destination options");
					}
					_selectedPatchDestinations = _controllerInputs.Where(x => !x.Patched).ToList();
				}
				patchDestinations = _selectedPatchDestinations.Count;
			}

			string summary = string.Format("This will patch {0} element patch points to {1} controller outputs.", patchSources, patchDestinations);
			labelPatchSummary.Text = summary;

			string warning = "";

			if (patchSources > 0 && patchDestinations > 0)
			{
				if (patchSources > patchDestinations)
				{
					warning = "WARNING: too many elements, some will not be patched";
				}
				if (patchDestinations > patchSources)
				{
					warning = "WARNING: too many outputs, some will not be patched";
				}
			}

			labelPatchWarning.Text = warning;

			buttonDoPatching.Enabled = patchSources > 0 && patchDestinations > 0;
		}

		private void buttonDoPatching_Click(object sender, EventArgs e)
		{
			//update the element settings in preparation for patching.
			var outputs = GetOrderedElementOutputs(_cachedElementNodes);
			if (outputs == null)
			{
				//user canceled or somethng went wrong
				return;
			}

			_componentOutputs = outputs;

			if (_componentOutputs != null)
			{
				_selectedPatchSources = radioButtonAllAvailablePatchPoints.Checked ? _componentOutputs : _componentOutputs.Where(x => !x.Patched).ToList();
			}

			//Now handle the controller side.
			_updateControllerDetails(_cachedControllersAndOutputs, true);

			if (_controllerInputs != null)
			{
				_selectedPatchDestinations = radioButtonAllOutputs.Checked ? _controllerInputs : _controllerInputs.Where(x => !x.Patched).ToList();
			}

			if (_selectedPatchSources == null || _selectedPatchDestinations == null)
			{
				Logging.Error("null patch sources or destinations!");
				return;
			}

			int max = Math.Min(_selectedPatchSources.Count, _selectedPatchDestinations.Count);

			for (int i = 0; i < max; i++)
			{
				VixenSystem.DataFlow.SetComponentSource(_selectedPatchDestinations[i].Item, _selectedPatchSources[i].Item);
			}

			OnPatchingUpdated();
			_UpdateEverything(_cachedElementNodes, _cachedControllersAndOutputs);

			var messageBox = new MessageBoxForm("Patched " + max + " element patch points to controllers.", "Patching Complete", MessageBoxButtons.OK, SystemIcons.Information);
			messageBox.ShowDialog(this);
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

			int patchedCount = _componentOutputs.Count(x => x.Patched);
			if (patchedCount > 20)
			{
				string message = string.Format("Are you sure you want to unpatch {0} patch points?", patchedCount);

				var messageBox = new MessageBoxForm(message, "Unpatch Elements?", MessageBoxButtons.YesNo, SystemIcons.Question);
				messageBox.ShowDialog(this);
				if (messageBox.DialogResult == DialogResult.No)
					return;
			}

			List<IDataFlowComponent> directElementChildren = new List<IDataFlowComponent>();
			List<IDataFlowComponent> nonDirectElementChildren = new List<IDataFlowComponent>();

			foreach (ElementNode selectedNode in _cachedElementNodes)
			{
				foreach (ElementNode leafNode in selectedNode.GetLeafEnumerator())
				{
					if (leafNode.Element == null)
						continue;

					IDataFlowComponent leafNodeComponent = VixenSystem.DataFlow.GetComponent(leafNode.Element.Id);

					List<IDataFlowComponent> children = VixenSystem.DataFlow.GetDestinationsOfComponent(leafNodeComponent).ToList();
					directElementChildren.AddRange(children);

					foreach (IDataFlowComponent child in children)
					{
						nonDirectElementChildren.AddRange(_findComponentsOfTypeInTreeFromComponent(child, typeof(IDataFlowComponent)));
					}
				}
			}

			bool removeFilters = false;
			if (nonDirectElementChildren.Any(x => x is IOutputFilterModuleInstance))
			{
				var messageBox = new MessageBoxForm("Some elements are patched to filters.  Should these filters be removed as well?",
									 "Remove Filters?", MessageBoxButtons.YesNoCancel, SystemIcons.Question);
				messageBox.ShowDialog(this);

				if (messageBox.DialogResult == DialogResult.Cancel)
					return;

				removeFilters = (messageBox.DialogResult == DialogResult.OK);
			}

			foreach (IDataFlowComponent directElementChild in directElementChildren)
			{
				VixenSystem.DataFlow.ResetComponentSource(directElementChild);

				if (removeFilters && directElementChild is IOutputFilterModuleInstance)
				{
					VixenSystem.Filters.RemoveFilter(directElementChild as IOutputFilterModuleInstance);
				}
			}

			if (removeFilters)
			{
				foreach (IDataFlowComponent nonDirectElementChild in nonDirectElementChildren)
				{
					if (nonDirectElementChild is IOutputFilterModuleInstance)
					{
						VixenSystem.Filters.RemoveFilter(nonDirectElementChild as IOutputFilterModuleInstance);
					}
				}
			}

			OnPatchingUpdated();
			_UpdateEverything(_cachedElementNodes, _cachedControllersAndOutputs);
		}

		public void UnpatchControllers()
		{
			buttonUnpatchControllers.PerformClick();
		}

		private void buttonUnpatchControllers_Click(object sender, EventArgs e)
		{
			int patchedCount = Convert.ToInt32(labelPatchedOutputCount.Text);
			if (patchedCount > 20)
			{
				string message = string.Format("Are you sure you want to unpatch {0} patch points?", patchedCount);

				var messageBox = new MessageBoxForm(message, "Unpatch Controllers?", MessageBoxButtons.YesNo, SystemIcons.Question);
				messageBox.ShowDialog(this);
				if (messageBox.DialogResult == DialogResult.No)
					return;
			}

			foreach (KeyValuePair<IControllerDevice, HashSet<int>> controllerAndOutput in _cachedControllersAndOutputs)
			{
				OutputController controller = controllerAndOutput.Key as OutputController;
				if (controller == null)
					continue;

				foreach (int i in controllerAndOutput.Value)
				{
					IDataFlowComponent outputComponent = controller.GetDataFlowComponentForOutput(controller.Outputs[i]);
					if (outputComponent == null)
						continue;

					VixenSystem.DataFlow.ResetComponentSource(outputComponent);
				}
			}

			OnPatchingUpdated();
			_UpdateEverything(_cachedElementNodes, _cachedControllersAndOutputs);
		}

		private void checkBoxReverseOutputOrder_CheckedChanged(object sender, EventArgs e)
		{
			_updateControllerDetails(_cachedControllersAndOutputs);
		}

		/// <summary>
		/// record the state of the reverse element flag
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkBoxReverseElementOrder_CheckedChanged(object sender, EventArgs e)
		{
			_reverseElementOrder = checkBoxReverseElementOrder.Checked;
		} // end checkBoxReverseElementOrder_CheckedChanged

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;

		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
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
