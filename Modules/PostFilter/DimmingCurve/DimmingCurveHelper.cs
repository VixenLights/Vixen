using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Data.Flow;
using Vixen.Module.OutputFilter;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.App.Curves;

namespace VixenModules.OutputFilter.DimmingCurve
{
	public partial class DimmingCurveHelper : BaseForm, IElementSetupHelper
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public DimmingCurveHelper(bool simpleMode)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			SimpleMode = simpleMode;
			_curve = new Curve();
		}

		public DimmingCurveHelper():this(false)
		{
		}

		private bool SimpleMode { get; set; }

		private void DimmingCurveHelper_Load(object sender, EventArgs e)
		{
			buttonOk.Enabled = false;
			if (SimpleMode)
			{
				radioButtonExistingUpdate.Enabled = false;
				radioButtonExistingAddNew.Enabled = false;
				radioButtonExistingDoNothing.Enabled = false;
				radioButtonInsertAfter.Checked = true;
			}
		}


		public string HelperName
		{
			get { return "Dimming Curve"; }
		}

		public bool Perform(IEnumerable<IElementNode> selectedNodes)
		{
			DialogResult dr = ShowDialog();
			if (dr != DialogResult.OK)
				return false;

			ExistingBehaviour existingBehaviour = ExistingBehaviour.DoNothing;

			if (radioButtonExistingDoNothing.Checked)
				existingBehaviour = ExistingBehaviour.DoNothing;
			else if (radioButtonExistingUpdate.Checked)
				existingBehaviour = ExistingBehaviour.UpdateExisting;
			else if (radioButtonExistingAddNew.Checked)
				existingBehaviour = ExistingBehaviour.AddNew;
			else if (radioButtonInsertAfter.Checked)
				existingBehaviour = ExistingBehaviour.AddInsertAfterElement;
			else
				Logging.Warn("no radio button selected");


			IEnumerable<IElementNode> leafElements = selectedNodes.SelectMany(x => x.GetLeafEnumerator()).Distinct();
			int modulesCreated = 0;
			int modulesConfigured = 0;
			int modulesSkipped = 0;
			

			foreach (IElementNode leafNode in leafElements) {

				// get the leaf 'things' to deal with -- ie. either existing dimming curves on a filter branch, or data component outputs
				// (if we're adding new ones, ignore any existing dimming curves: always go to the outputs and we'll add new ones)
				IDataFlowComponent elementComponent = VixenSystem.DataFlow.GetComponent(leafNode.Element.Id);

				if (existingBehaviour == ExistingBehaviour.AddInsertAfterElement)
				{
					DimmingCurveModule dimmingCurve = ApplicationServices.Get<IOutputFilterModuleInstance>(DimmingCurveDescriptor.ModuleId) as DimmingCurveModule;
					dimmingCurve.DimmingCurve = _curve;
					var inputSource = VixenSystem.DataFlow.GetDestinationsOfComponentOutput(elementComponent, 0).ToList();

					VixenSystem.DataFlow.SetComponentSource(dimmingCurve, elementComponent, 0);
					VixenSystem.Filters.AddFilter(dimmingCurve);
					foreach (var dataFlowComponent in inputSource)
					{
						VixenSystem.DataFlow.ResetComponentSource(dataFlowComponent);
						VixenSystem.DataFlow.SetComponentSource(dataFlowComponent, dimmingCurve, 0);
					}

					modulesCreated++;
					modulesConfigured++;
				}
				else
				{
					IEnumerable<IDataFlowComponentReference> references = _FindLeafOutputsOrDimmingCurveFilters(elementComponent, existingBehaviour == ExistingBehaviour.AddNew);

					foreach (IDataFlowComponentReference reference in references)
					{
						int outputIndex = reference.OutputIndex;

						if (reference.Component is DimmingCurveModule)
						{
							switch (existingBehaviour)
							{
								case ExistingBehaviour.DoNothing:
									modulesSkipped++;
									continue;

								case ExistingBehaviour.UpdateExisting:
									(reference.Component as DimmingCurveModule).DimmingCurve = _curve;
									modulesConfigured++;
									continue;

								case ExistingBehaviour.AddNew:
									outputIndex = 0;
									break;
							}
						}

						// assuming we're making a new one and going from there
						DimmingCurveModule dimmingCurve = ApplicationServices.Get<IOutputFilterModuleInstance>(DimmingCurveDescriptor.ModuleId) as DimmingCurveModule;
						VixenSystem.DataFlow.SetComponentSource(dimmingCurve, reference.Component, outputIndex);
						VixenSystem.Filters.AddFilter(dimmingCurve);

						dimmingCurve.DimmingCurve = _curve;

						modulesCreated++;
						modulesConfigured++;
					}
				
				}
			}
			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			var messageBox = new MessageBoxForm(modulesCreated + " Dimming Curves created, " + modulesConfigured + " configured, and " + modulesSkipped + " skipped.", "", false, false);
			messageBox.ShowDialog();

			return true;
		}

		private IEnumerable<IDataFlowComponentReference> _FindLeafOutputsOrDimmingCurveFilters(IDataFlowComponent component, bool skipDimmingCurves)
		{
			if (component == null) {
				yield break;
			}

			if (component is DimmingCurveModule && !skipDimmingCurves) {
				yield return new DataFlowComponentReference(component, -1);
				// this is a bit iffy -- -1 as a component output index -- but hey.
			}

			if (component.Outputs == null || component.OutputDataType == DataFlowType.None) {
				yield break;
			}

			for (int i = 0; i < component.Outputs.Length; i++) {
				IEnumerable<IDataFlowComponent> children = VixenSystem.DataFlow.GetDestinationsOfComponentOutput(component, i);

				if (!children.Any()) {
					yield return new DataFlowComponentReference(component, i);
				} else {
					foreach (IDataFlowComponent child in children) {
						foreach (IDataFlowComponentReference result in _FindLeafOutputsOrDimmingCurveFilters(child, skipDimmingCurves)) {
							yield return result;
						}
					}
				}
			}
		}




		private Curve _curve;
		private void buttonSetupCurve_Click(object sender, EventArgs e)
		{
			using (CurveEditor editor = new CurveEditor(_curve)) {
				if (editor.ShowDialog() == DialogResult.OK) {
					_curve = editor.Curve;
					buttonOk.Enabled = true;
				}
			}
		}


		enum ExistingBehaviour
		{
			DoNothing,
			UpdateExisting,
			AddNew,
			AddInsertAfterElement
		}

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

		private void DimmingCurveHelper_HelpButtonClicked(object sender, CancelEventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Patching);
		}
	}
}
