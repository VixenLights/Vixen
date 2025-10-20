using Catel.IoC;
using Orc.Theming;
using Orc.Wizard;
using System.Diagnostics;
using Vixen.Commands;
using Vixen.Data.Flow;
using Vixen.Data.Value;
using Vixen.Extensions;
using Vixen.Module.OutputFilter;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.App.Curves;
using VixenModules.App.Fixture;
using VixenModules.App.FixtureSpecificationManager;
using VixenModules.Editor.FixtureWizard.Wizard;
using VixenModules.Editor.FixtureWizard.Wizard.Models;
using VixenModules.OutputFilter.CoarseFineBreakdown;
using VixenModules.OutputFilter.ColorBreakdown;
using VixenModules.OutputFilter.ColorWheelFilter;
using VixenModules.OutputFilter.DimmingCurve;
using VixenModules.OutputFilter.DimmingFilter;
using VixenModules.OutputFilter.PrismFilter;
using VixenModules.OutputFilter.ShutterFilter;
using VixenModules.OutputFilter.TaggedFilter;
using VixenModules.Property.Color;
using VixenModules.Property.IntelligentFixture;
using static Dataweb.NShape.Advanced.PathFigureShape;

namespace VixenApplication.Setup.ElementTemplates
{
	/// <summary>
	/// Maintains an intelligent fixture element template.
	/// </summary>
	public class IntelligentFixtureTemplate : IElementTemplate
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public IntelligentFixtureTemplate()
		{
			// Default the nodes to delete to empty list
			_nodesToDelete = new List<ElementNode>();
		}

		#endregion

		#region IElementTemplate

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public string TemplateName
		{
			get
			{
				return "Intelligent Fixture";
			}
		}

		#endregion

		#region Fields
		
		/// <summary>
		/// Fixture wizard for configuring intelligent fixtures.
		/// </summary>
		private IFixtureWizard? _wizard;

		/// <summary>
		/// Fixture nodes created by this template.
		/// When a group is created the individual nodes do not need to appear in the element tree.
		/// </summary>
		private List<ElementNode> _nodesToDelete;

		/// <summary>
		/// Fixture leaf nodes created by this template.
		/// </summary>
		private List<ElementNode>? _leafNodes;

		#endregion

		#region Private Methods

		/// <summary>
		/// Shows the intelligent fixture wizard.
		/// </summary>		
		/// <returns>Intelligent fixture wizard task</returns>
		private async Task<bool?> ShowWizardAsync()
		{
			// Get the Catel type factory
			ITypeFactory typeFactory = this.GetTypeFactory();

			// Use the type factory to create the intelligent fixture wizard
			_wizard = typeFactory.CreateInstance(typeof(IntelligentFixtureWizard)) as IFixtureWizard;

			if (_wizard == null)
			{
				throw new ArgumentNullException(nameof(_wizard));
			}

			// Configure the wizard window to show up in the Windows task bar
			_wizard.ShowInTaskbarWrapper = true;

			// Enable the help button
			_wizard.ShowHelpWrapper = true;

			// Configure the wizard to allow the user to jump between already visited pages
			_wizard.AllowQuickNavigationWrapper = true;

			// Allow Catel to help determine when it is safe to transition to the next wizard page
			_wizard.HandleNavigationStatesWrapper = true;

			// Configure the wizard to NOT cache views
			_wizard.CacheViewsWrapper = false;

			// Configure the wizard with a navigation controller														
			_wizard.NavigationControllerWrapper = typeFactory.CreateInstanceWithParametersAndAutoCompletion<FixtureWizardNavigationController>(_wizard);

			// Create the wizard service
			IDependencyResolver dependencyResolver = this.GetDependencyResolver();
			IWizardService wizardService = (IWizardService)dependencyResolver.Resolve(typeof(IWizardService));

			// Display the intelligent fixture wizard
			bool? result = (await wizardService.ShowWizardAsync(_wizard)).DialogResult;

			// Return whether the wizard was completed or cancelled
			return result;
		}
		
		/// <summary>
		/// This utility method was copied from ColorSetupHelper.cs.
		/// </summary>
		/// <param name="component">Data flow to find leaves from</param>
		/// <returns>Collection of data flow leaves</returns>
		private IEnumerable<IDataFlowComponentReference> FindLeafOutputsOrBreakdownFilters(IDataFlowComponent? component)
		{
			if (component == null)
			{
				yield break;
			}

			if (component is ColorBreakdownModule)
			{
				yield return new DataFlowComponentReference(component, -1);
				// this is a bit iffy -- -1 as a component output index -- but hey.
			}

			if (component.Outputs == null || component.OutputDataType == DataFlowType.None)
			{
				yield break;
			}

			for (int i = 0; i < component.Outputs.Length; i++)
			{
				IEnumerable<IDataFlowComponent> children = VixenSystem.DataFlow.GetDestinationsOfComponentOutput(component, i);

				if (!children.Any())
				{
					yield return new DataFlowComponentReference(component, i);
				}
				else
				{
					foreach (IDataFlowComponent child in children)
					{
						foreach (IDataFlowComponentReference result in FindLeafOutputsOrBreakdownFilters(child))
						{
							yield return result;
						}
					}
				}
			}
		}

		#endregion

		#region IElementTemplate

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public async Task<IEnumerable<ElementNode>> GenerateElements(IEnumerable<ElementNode>? selectedNodes = null)
		{
			// Without this call the wizard will produce the following exception:
			// 'Theming is not yet initialized, make sure to initialize a theme via ThemeManager first'
			ThemeManager.Current.SynchronizeTheme();

			// Create the Catel dependency resolver
			IDependencyResolver dependencyResolver = this.GetDependencyResolver();

			// Retrieve the color scheme service
			IBaseColorSchemeService baseColorService = (IBaseColorSchemeService)dependencyResolver.Resolve(typeof(IBaseColorSchemeService));

			// Select the dark color scheme
			baseColorService.SetBaseColorScheme("Dark");

			// Retrieve the accent color service
			IAccentColorService accentColorServer = (IAccentColorService)dependencyResolver.Resolve(typeof(IAccentColorService));

			// Configure the page bubbles on the left to be blue to look better with the dark theme
			accentColorServer.SetAccentColor((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("DodgerBlue"));

			// Show the fixture wizard
			bool? cancelled = await ShowWizardAsync();

			// Determine if the wizard was cancelled 
			Cancelled = (cancelled.HasValue && !cancelled.Value);

			// Create a collection for the fixture nodes we are creating
			List<ElementNode> nodes = new List<ElementNode>();

			// If the wizard was NOT cancelled then...
			if (!Cancelled)
			{
				if (_wizard == null) throw new InvalidOperationException("Wizard cannot be null");
				// Retrieve the pages from the wizard so that the selected state can be extracted
				SelectProfileWizardPage selectProfilePage = (SelectProfileWizardPage)_wizard.Pages.Single(page => page is SelectProfileWizardPage);
				GroupingWizardPage groupingPage = (GroupingWizardPage)_wizard.Pages.Single(page => page is GroupingWizardPage);
				AutomationWizardPage automationPage = (AutomationWizardPage)_wizard.Pages.Single(page => page is AutomationWizardPage);
				ColorSupportWizardPage colorSupportPage = (ColorSupportWizardPage)_wizard.Pages.Single(page => page is ColorSupportWizardPage);
				DimmingCurveWizardPage dimmingCurvePage = (DimmingCurveWizardPage)_wizard.Pages.Single(page => page is DimmingCurveWizardPage);

				// Save the fixture
				FixtureSpecificationManager.Instance().Save(selectProfilePage.Fixture);

				// Create the fixture element node configurator
				IntelligentFixtureNodeConfigurator fixtureNodeConfigurator = new IntelligentFixtureNodeConfigurator();

				// Loop over the number of fixture nodes to create
				for (int index = 0; index < groupingPage.NumberOfFixtures; index++)
				{
					// Base the fixture name on the index of the node
					string fixtureName = groupingPage.ElementPrefix + (index + 1).ToString();
					
					// Create the new display element node
					ElementNode node = ElementNodeService.Instance.CreateSingle(null, fixtureName, true, true);

					// Create the fixture node
					nodes.Add(fixtureNodeConfigurator.ConfigureFixtureNode(
						node,
						fixtureName,
						selectProfilePage.Fixture,
						colorSupportPage.ColorMixing,
						automationPage.AutomaticallyOpenAndCloseShutter,
						automationPage.AutomaticallyControlColorWheel,
						automationPage.AutomaticallyControlDimmer,
						automationPage.AutomaticallyOpenAndClosePrism,
						dimmingCurvePage.GetDimmingCurveSelection(),
						dimmingCurvePage.DimmingCurve));
				}

				// If the user selected to create a group of fixtures then...
				if (groupingPage.CreateGroup)
				{
					// Create the group with the specified name
					ElementNode groupNode = ElementNodeService.Instance.CreateSingle(null, groupingPage.GroupName, false);

					// Loop over the fixtures
					foreach (ElementNode node in nodes)
					{
						// Add the specified fixture to the group
						groupNode.AddChild(node);
					}

					// Save off the individual fixture nodes so they can be removed from the tree
					_nodesToDelete = nodes;

					// Store off the fixture nodes
					_leafNodes = nodes;

					// Return the group node
					nodes = new List<ElementNode>() { groupNode };
				}
				else
				{
					// Store off the fixture nodes
					_leafNodes = nodes;
				}
			}

			return nodes;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public bool SetupTemplate(IEnumerable<ElementNode>? selectedNodes = null)
		{
			// Nothing to setup
			return true;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool ConfigureColor
		{
			get
			{
				// A color property is configured as part of the wizard so additional dialogs are not necessary
				return false;
			}
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool ConfigureDimming
		{
			get
			{
				// Not supporting dimming curves for fixtures
				return false;
			}
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public bool Cancelled
		{
			get;
			private set;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public IEnumerable<ElementNode> GetElementsToDelete()
		{
			// Return the leaf nodes that need to be removed from the tree because they exist under the group
			return _nodesToDelete;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public IEnumerable<ElementNode> GetLeafNodes()
		{
			// Return the leaf nodes created by the template
			return _leafNodes ?? new List<ElementNode>();
		}

		#endregion
	}
}
