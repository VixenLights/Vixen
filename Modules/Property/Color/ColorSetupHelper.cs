using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Data.Flow;
using Vixen.Module.Effect;
using Vixen.Module.OutputFilter;
using Vixen.Module.Property;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;
using System.Linq;
using Vixen;
using VixenModules.OutputFilter.ColorBreakdown;

namespace VixenModules.Property.Color
{
	public partial class ColorSetupHelper : BaseForm, IElementSetupHelper
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private const string Red = "Red";
		private const string Green = "Green";
		private const string Blue = "Blue";

		public ColorSetupHelper()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			colorPanelSingleColor.BackColor = System.Drawing.Color.RoyalBlue;
			colorPanelSingleColor.Color = System.Drawing.Color.RoyalBlue;
			comboBoxColorOrder.SelectedIndex = 0;
		}

		public string HelperName
		{
			get { return "Color Handling"; }
		}

		public bool Perform(IEnumerable<IElementNode> selectedNodes)
		{
			if (!SilentMode)
			{
				DialogResult dr = ShowDialog();
				if (dr != DialogResult.OK)
					return false;
			}
			
			// note: the color property can only be applied to leaf nodes.

			// pull out the new data settings from the form elements
			ElementColorType colorType;
			string colorSetName = "";
			System.Drawing.Color singleColor = System.Drawing.Color.Black;

			if (radioButtonOptionSingle.Checked) {
				colorType = ElementColorType.SingleColor;
				singleColor = colorPanelSingleColor.Color;
				colorSetName = null;
			}
			else if (radioButtonOptionMultiple.Checked) {
				colorType = ElementColorType.MultipleDiscreteColors;
				colorSetName = comboBoxColorSet.SelectedItem.ToString();
				singleColor = System.Drawing.Color.Empty;
			}
			else if (radioButtonOptionFullColor.Checked) {
				colorType = ElementColorType.FullColor;
				singleColor = System.Drawing.Color.Empty;
				colorSetName = null;
			}
			else {
				Logging.Warn("Unexpected radio option selected");
				colorType = ElementColorType.SingleColor;
				singleColor = colorPanelSingleColor.Color;
				colorSetName = null;
			}


			// PROPERTY SETUP
			// go through all elements, making a color property for each one.
			// (If any has one already, check with the user as to what they want to do.)
			IEnumerable<IElementNode> leafElements = selectedNodes.SelectMany(x => x.GetLeafEnumerator()).Distinct();
			List<IElementNode> leafElementList = leafElements.ToList();

			bool askedUserAboutExistingProperties = false;
			bool overrideExistingProperties = false;

			int colorPropertiesAdded = 0;
			int colorPropertiesConfigured = 0;
			int colorPropertiesSkipped = 0;

			MessageBoxForm messageBox;

			foreach (IElementNode leafElement in leafElementList) {
				bool skip = false;
				ColorModule existingProperty = null;

				if (leafElement.Properties.Contains(ColorDescriptor.ModuleId)) {
					if (!askedUserAboutExistingProperties) {
						//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
						MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
						messageBox = new MessageBoxForm("Some elements already have color properties set up. Should these be overwritten?",
											"Color Setup", true, false);
						messageBox.ShowDialog();
						overrideExistingProperties = (messageBox.DialogResult == DialogResult.OK);
						askedUserAboutExistingProperties = true;
					}

					skip = !overrideExistingProperties;
					existingProperty = leafElement.Properties.Get(ColorDescriptor.ModuleId) as ColorModule;
				}
				else {
					existingProperty = leafElement.Properties.Add(ColorDescriptor.ModuleId) as ColorModule;
					colorPropertiesAdded++;
				}

				if (!skip) {
					if (existingProperty == null) {
						Logging.Error("Null color property for element " + leafElement.Name);
					}
					else {
						existingProperty.ColorType = colorType;
						existingProperty.SingleColor = singleColor;
						existingProperty.ColorSetName = colorSetName;
						colorPropertiesConfigured++;
					}
				}
				else {
					colorPropertiesSkipped++;
				}
			}


			// PATCHING
			// go through each element, walking the tree of patches, building up a list.  If any are a 'color
			// breakdown' already, warn/check with the user if it's OK to overwrite them.  Make a new breakdown
			// filter for each 'leaf' of the patching process. If it's fully patched to an output, ignore it.

			List<IDataFlowComponentReference> leafOutputs = new List<IDataFlowComponentReference>();
			foreach (IElementNode leafElement in leafElementList.Where(x => x.Element != null)) {
				leafOutputs.AddRange(_FindLeafOutputsOrBreakdownFilters(VixenSystem.DataFlow.GetComponent(leafElement.Element.Id)));
			}

			bool askedUserAboutExistingFilters = false;
			bool overrideExistingFilters = false;
			ColorBreakdownModule breakdown = null;

			int colorFiltersAdded = 0;
			int colorFiltersConfigured = 0;
			int colorFiltersSkipped = 0;

			foreach (IDataFlowComponentReference leaf in leafOutputs) {
				bool skip = false;

				if (leaf.Component is ColorBreakdownModule) {
					if (!askedUserAboutExistingFilters) {
						//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
						MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
						messageBox = new MessageBoxForm("Some elements are already patched to color filters. Should these be overwritten?",
											"Color Setup", true, false);
						messageBox.ShowDialog();
						overrideExistingFilters = (messageBox.DialogResult == DialogResult.OK);
						askedUserAboutExistingFilters = true;
					}

					skip = !overrideExistingFilters;
					breakdown = leaf.Component as ColorBreakdownModule;
				}
				else if (leaf.Component.OutputDataType == DataFlowType.None) {
					// if it's a dead-end -- ie. most likely a controller output -- skip it
					skip = true;
				}
				else {
					// doesn't exist? make a new module and assign it
					breakdown =
						ApplicationServices.Get<IOutputFilterModuleInstance>(ColorBreakdownDescriptor.ModuleId) as ColorBreakdownModule;
					VixenSystem.DataFlow.SetComponentSource(breakdown, leaf);
					VixenSystem.Filters.AddFilter(breakdown);
					colorFiltersAdded++;
				}

				if (!skip) {
					List<ColorBreakdownItem> newBreakdownItems = new List<ColorBreakdownItem>();
					bool mixColors = false;
					ColorBreakdownItem cbi;

					switch (colorType) {
						case ElementColorType.FullColor:
							mixColors = true;

							foreach (var color in comboBoxColorOrder.SelectedItem.ToString().ToCharArray())
							{
								switch (color)
								{
									case 'R':
										cbi = new ColorBreakdownItem();
										cbi.Color = System.Drawing.Color.Red;
										cbi.Name = Red;
										newBreakdownItems.Add(cbi);
										break;
									case 'G':
										cbi = new ColorBreakdownItem();
										cbi.Color = System.Drawing.Color.Lime;
										cbi.Name = Green;
										newBreakdownItems.Add(cbi);
										break;
									case 'B':
										cbi = new ColorBreakdownItem();
										cbi.Color = System.Drawing.Color.Blue;
										cbi.Name = Blue;
										newBreakdownItems.Add(cbi);
										break;
								}
							}
							break;

						case ElementColorType.MultipleDiscreteColors:
							mixColors = false;

							ColorStaticData csd = ApplicationServices.GetModuleStaticData(ColorDescriptor.ModuleId) as ColorStaticData;

							if (!csd.ContainsColorSet(colorSetName)) {
								Logging.Error("Color sets doesn't contain " + colorSetName);
							}
							else {
								ColorSet cs = csd.GetColorSet(colorSetName);
								foreach (var c in cs) {
									cbi = new ColorBreakdownItem();
									cbi.Color = c;
									// heh heh, this can be.... creative.
									cbi.Name = c.Name;
									newBreakdownItems.Add(cbi);
								}
							}

							break;

						case ElementColorType.SingleColor:
							mixColors = false;
							cbi = new ColorBreakdownItem();
							cbi.Color = singleColor;
							newBreakdownItems.Add(cbi);
							break;
					}

					breakdown.MixColors = mixColors;
					breakdown.BreakdownItems = newBreakdownItems;

					colorFiltersConfigured++;

				}
				else {
					colorFiltersSkipped++;
				}
			}

			if (!SilentMode)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Information; //this is used if you want to add a system icon to the message form.
				messageBox = new MessageBoxForm("Color Properties:  " + colorPropertiesAdded + " added, " +
				                                colorPropertiesConfigured + " configured, " + colorPropertiesSkipped + " skipped. " +
				                                "Color Filters:  " + colorFiltersAdded + " added, " + colorFiltersConfigured + " configured, " +
				                                colorFiltersSkipped + " skipped.",
					"Color Setup", false, false);
				messageBox.ShowDialog();
			}
			
			return true;
		}

		private IEnumerable<IDataFlowComponentReference> _FindLeafOutputsOrBreakdownFilters(IDataFlowComponent component)
		{
			if (component == null) {
				yield break;
			}

			if (component is ColorBreakdownModule) {
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
				}
				else {
					foreach (IDataFlowComponent child in children) {
						foreach (IDataFlowComponentReference result in _FindLeafOutputsOrBreakdownFilters(child)) {
							yield return result;
						}
					}
				}
			}
		}

		public void SetColorType(ElementColorType colorType)
		{
			switch (colorType)
			{
				case ElementColorType.FullColor:
					radioButtonOptionFullColor.Checked = true;
					break;
				case ElementColorType.MultipleDiscreteColors:
					radioButtonOptionMultiple.Checked = true;
					break;
				default:
					radioButtonOptionSingle.Checked = true;
					break;
			}
		}

		public ElementColorType GetColorType()
		{
			if (radioButtonOptionFullColor.Checked)
			{
				return ElementColorType.FullColor;
			}

			if (radioButtonOptionMultiple.Checked)
			{
				return ElementColorType.MultipleDiscreteColors;
			}

			return ElementColorType.SingleColor;

		}

		public bool SilentMode { get; set; }

		private void ColorSetupHelper_Load(object sender, EventArgs e)
		{
			PopulateColorSetsComboBox();

		}

		private void PopulateColorSetsComboBox()
		{
			comboBoxColorSet.BeginUpdate();
			comboBoxColorSet.Items.Clear();

			foreach (string colorSetName in (ApplicationServices.GetModuleStaticData(ColorDescriptor.ModuleId) as ColorStaticData).GetColorSetNames()) {
				comboBoxColorSet.Items.Add(colorSetName);
			}

			if (comboBoxColorSet.SelectedIndex < 0) {
				comboBoxColorSet.SelectedIndex = 0;
			}

			comboBoxColorSet.EndUpdate();
		}

		private void AnyRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			colorPanelSingleColor.Enabled = radioButtonOptionSingle.Checked;
			comboBoxColorSet.Enabled = radioButtonOptionMultiple.Checked;
			buttonColorSetsSetup.Enabled = radioButtonOptionMultiple.Checked;
			comboBoxColorOrder.Enabled = radioButtonOptionFullColor.Checked;

			buttonOk.Enabled = radioButtonOptionSingle.Checked || radioButtonOptionMultiple.Checked || radioButtonOptionFullColor.Checked;
		}

		private void buttonColorSetsSetup_Click(object sender, EventArgs e)
		{
			using (ColorSetsSetupForm cssf = new ColorSetsSetupForm(ApplicationServices.GetModuleStaticData(ColorDescriptor.ModuleId) as ColorStaticData)) {
				cssf.ShowDialog();
				PopulateColorSetsComboBox();
			}
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
	}
}
