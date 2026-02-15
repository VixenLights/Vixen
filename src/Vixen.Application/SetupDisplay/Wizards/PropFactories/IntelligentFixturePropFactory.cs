using Vixen.Sys;
using Vixen.Sys.Props;
using VixenApplication.Setup.ElementTemplates;
using VixenModules.App.FixtureSpecificationManager;
using VixenModules.App.Props.Models.Arch;
using VixenModules.Editor.FixtureWizard.Wizard.Models;
using VixenModules.Editor.PropWizard;

namespace VixenApplication.SetupDisplay.Wizards.PropFactories
{
	/// <summary>
	/// Creates intelligent fixture props from wizard data.
	/// </summary>
	internal class IntelligentFixturePropFactory : IPropFactory
	{
		#region IPropFactory
		
		/// <inheritdoc/>		
		public IPropGroup GetProps(IPropWizard wizard)
		{						
			// Retrieve the pages from the wizard so that the selected state can be extracted
			SelectProfileWizardPage selectProfilePage = (SelectProfileWizardPage)wizard.Pages.Single(page => page is SelectProfileWizardPage);
			GroupingWizardPage groupingPage = (GroupingWizardPage)wizard.Pages.Single(page => page is GroupingWizardPage);
			AutomationWizardPage automationPage = (AutomationWizardPage)wizard.Pages.Single(page => page is AutomationWizardPage);
			ColorSupportWizardPage colorSupportPage = (ColorSupportWizardPage)wizard.Pages.Single(page => page is ColorSupportWizardPage);
			DimmingCurveWizardPage dimmingCurvePage = (DimmingCurveWizardPage)wizard.Pages.Single(page => page is DimmingCurveWizardPage);

			// Save the fixture
			FixtureSpecificationManager.Instance().Save(selectProfilePage.Fixture);

			// Create the fixture element node configurator
			IntelligentFixtureNodeConfigurator fixtureNodeConfigurator = new IntelligentFixtureNodeConfigurator();
									
			// Create a collection for the fixture props
			IPropGroup fixturePropGroup = new PropGroup();

			// Loop over the number of fixture nodes to create
			for (int index = 0; index < groupingPage.NumberOfFixtures; index++)
			{
				// Base the fixture name on the index of the node
				string fixtureName = groupingPage.ElementPrefix + (index + 1).ToString();

				// Create the intelligent fixture prop
				IntelligentFixtureProp fixtureProp = VixenSystem.Props.CreateProp<IntelligentFixtureProp>(fixtureName);
				
				// Assign the Element Node the fixture name
				fixtureProp.TargetNode.Name = fixtureName;

				// Configure the fixture node
				fixtureNodeConfigurator.ConfigureFixtureNode(
					(ElementNode)fixtureProp.TargetNode,
					fixtureName,
					selectProfilePage.Fixture,
					colorSupportPage.ColorMixing,
					automationPage.AutomaticallyOpenAndCloseShutter,
					automationPage.AutomaticallyControlColorWheel,
					automationPage.AutomaticallyControlDimmer,
					automationPage.AutomaticallyOpenAndClosePrism,
					dimmingCurvePage.GetDimmingCurveSelection(),
					dimmingCurvePage.DimmingCurve);

				// Add the fixture prop to the collection
				fixturePropGroup.Props.Add(fixtureProp);
			}

			// If the fixtures should be created in a group then...
			if (groupingPage.CreateGroup)
			{
				// Indicate if the fixture props should be in a group
				fixturePropGroup.CreateGroup = true;

				// Assign the group name
				fixturePropGroup.GroupName = groupingPage.GroupName;	
			}
			
			// Return the collection of fixture props
			return fixturePropGroup;			
		}

		public (IProp, IPropGroup) CreateBaseProp()
		{
			return (null, null);
		}

		public IPropGroup EditExistingProp(IProp prop)
		{
			return null;
		}

		public void LoadWizard(IProp prop, IPropWizard wizard)
		{
			throw new NotImplementedException();
		}

		public void UpdateProp(IProp prop, IPropWizard wizard)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
