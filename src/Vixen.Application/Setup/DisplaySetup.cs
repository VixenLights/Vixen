using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Rule;
using Vixen.Sys;

namespace VixenApplication.Setup
{
	public partial class DisplaySetup : BaseForm
	{
		private SetupElementsTree? _setupElementsTree;

		private SetupPatchingSimple? _setupPatchingSimple;
		private SetupPatchingGraphical? _setupPatchingGraphical;

		private SetupControllersSimple? _setupControllersSimple;

		private ISetupElementsControl? _currentElementControl;
		private ISetupPatchingControl? _currentPatchingControl;
		private ISetupControllersControl? _currentControllersControl;

		private readonly IElementTemplate[] _elementTemplates;
		private readonly IElementSetupHelper[] _elementSetupHelpers;

		public DisplaySetup()
		{
			InitializeComponent();
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			Icon = Resources.Icon_Vixen3;
			ThemeUpdateControls.UpdateControls(this);
			buttonHelp.Image = Common.Resources.Tools.GetIcon(Resources.help, iconSize);

			if (SystemFonts.MessageBoxFont != null)
			{
				elementLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 12F);
				patchingHeaderLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 12F);
				controllersHeaderLabel.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 12F);
			}

			_elementTemplates = Vixen.Services.ApplicationServices.GetAllElementTemplates();
			_elementSetupHelpers = Vixen.Services.ApplicationServices.GetAllElementSetupHelpers();
		}


		private void DisplaySetup_Load(object sender, EventArgs e)
		{
			_setupElementsTree = new SetupElementsTree(_elementTemplates, _elementSetupHelpers);
			_setupElementsTree.Dock = DockStyle.Fill;
			_setupElementsTree.MasterForm = this;

			_setupPatchingSimple = new SetupPatchingSimple();
			_setupPatchingSimple.Dock = DockStyle.Fill;
			_setupPatchingSimple.MasterForm = this;
			_setupPatchingGraphical = new SetupPatchingGraphical();
			_setupPatchingGraphical.Dock = DockStyle.Fill;
			_setupPatchingGraphical.MasterForm = this;

			_setupControllersSimple = new SetupControllersSimple(this);
			_setupControllersSimple.Dock = DockStyle.Fill;
			_setupControllersSimple.MasterForm = this;

			ActivateControllersControl(_setupControllersSimple);
			ActivateElementControl(_setupElementsTree);
			
			radioButtonPatchingSimple.Checked = true;
			//splitContainer1.SplitterDistance = tableLayoutPanelElementSetup.Width + 6;
			//splitContainer2.SplitterDistance = (int)(patchingPaneTableLayoutPanel.Width + (10 * ScalingTools.GetScaleFactor()));

		}


		private void ActivateElementControl(ISetupElementsControl control)
		{
			if (_currentElementControl != null)
			{
				_currentElementControl.ElementSelectionChanged -= control_ElementSelectionChanged;
				_currentElementControl.ElementsChanged -= control_ElementsChanged;
			}

			_currentElementControl = control;

			control.ElementSelectionChanged += control_ElementSelectionChanged;
			control.ElementsChanged += control_ElementsChanged;

			//tableLayoutPanelElementSetup.Controls.Clear();
			tableLayoutPanelElementSetup.Controls.Add(control.SetupElementsControl, 0, 2);


			//control.UpdatePatching(); //Occurs in load triggered by the table layout add above.
		}

		void control_ElementsChanged(object? sender, ElementsChangedEventArgs e)
		{
			if (_currentPatchingControl != null && _currentElementControl != null)
			{
				_currentPatchingControl.UpdateElementDetails(_currentElementControl.SelectedElements);
			}

			if (e.Action == ElementsChangedEventArgs.ElementsChangedAction.Remove ||
				e.Action == ElementsChangedEventArgs.ElementsChangedAction.Edit)
			{
				// TODO: this is iffy, should really redo the events for this system
				// TODO: The above should help a little with the iffiness of this, but I still question the brute force nature of this
				_currentControllersControl?.UpdatePatching();
			}

		}

		void control_ElementSelectionChanged(object? sender, ElementNodesEventArgs e)
		{
			if (_currentPatchingControl != null)
			{
				_currentPatchingControl.UpdateElementSelection(e.ElementNodes);
			}
		}



		private void ActivatePatchingControl(ISetupPatchingControl? control)
		{
			if (_currentPatchingControl != null)
			{
				_currentPatchingControl.FiltersAdded -= control_FiltersAdded;
				_currentPatchingControl.PatchingUpdated -= control_PatchingUpdated;
				patchingPaneTableLayoutPanel.Controls.Remove(_currentPatchingControl.SetupPatchingControl);
			}

			_currentPatchingControl = control;

			if (control == null)
			{
				return;
			}
			control.FiltersAdded += control_FiltersAdded;
			control.PatchingUpdated += control_PatchingUpdated;

			control.SetupPatchingControl.Dock = DockStyle.Fill;

			patchingPaneTableLayoutPanel.Controls.Add(control.SetupPatchingControl, 0, 3);
			patchingPaneTableLayoutPanel.SetColumnSpan(control.SetupPatchingControl, 2);




			if (_currentControllersControl == null)
			{
				control.UpdateControllerSelection(new ControllersAndOutputsSet());
			}
			else
			{
				control.UpdateControllerSelection(_currentControllersControl.SelectedControllersAndOutputs);
			}
			if (_currentElementControl == null)
			{
				control.UpdateElementSelection(Enumerable.Empty<ElementNode>());
			}
			else
			{
				control.UpdateElementSelection(_currentElementControl.SelectedElements);
			}
		}

		void control_PatchingUpdated(object? sender, EventArgs e)
		{
			if (_currentElementControl != null)
			{
				_currentElementControl.UpdatePatching();
			}

			if (_currentControllersControl != null)
			{
				_currentControllersControl.UpdatePatching();
			}
		}

		void control_FiltersAdded(object? sender, FiltersEventArgs e)
		{
		}



		private void ActivateControllersControl(ISetupControllersControl control)
		{
			if (_currentControllersControl != null)
			{
				_currentControllersControl.ControllerSelectionChanged -= control_ControllerSelectionChanged;
				_currentControllersControl.ControllersChanged -= control_ControllersChanged;
			}

			_currentControllersControl = control;

			control.ControllerSelectionChanged += control_ControllerSelectionChanged;
			control.ControllersChanged += control_ControllersChanged;

			//tableLayoutPanelControllerSetup.Controls.Clear();
			tableLayoutPanelControllerSetup.Controls.Add(control.SetupControllersControl, 0, 2);

			//control.UpdatePatching();  //On load does this 
		}

		void control_ControllersChanged(object? sender, EventArgs e)
		{
			if (_currentPatchingControl != null && _currentControllersControl != null)
			{
				_currentPatchingControl.UpdateControllerDetails(_currentControllersControl.SelectedControllersAndOutputs);
			}

			// TODO: this is iffy, should really redo the events for this system
			if (_currentElementControl != null)
			{
				_currentElementControl.UpdatePatching();
			}
		}

		/// <summary>
		/// Unpatch selected controllers
		/// </summary>
		/// <param name="sender">The source of the event</param>
		/// <param name="e">Contains the event data</param>
		public void control_UnpatchControllers(object? sender, EventArgs e)
		{
			_setupPatchingSimple.UnpatchControllers();
			if (_currentPatchingControl == _setupPatchingGraphical)
				_setupPatchingGraphical.UpdateConnectionsForControllers();
		}

		void control_ControllerSelectionChanged(object? sender, ControllerSelectionEventArgs e)
		{
			if (_currentPatchingControl != null)
			{
				_currentPatchingControl.UpdateControllerSelection(e.ControllersAndOutputs);
			}
		}

		private void radioButtonPatchingSimple_CheckedChanged(object? sender, EventArgs e)
		{
			if (sender is RadioButton button)
			{
				if (button.Checked)
				{
					ActivatePatchingControl(_setupPatchingSimple);
				}
			}

		}

		private void radioButtonPatchingGraphical_CheckedChanged(object? sender, EventArgs e)
		{
			if (sender is RadioButton button)
			{
				if (button.Checked)
				{
					ActivatePatchingControl(_setupPatchingGraphical);
				}
			}

		}

		private void buttonHelp_Click(object? sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Setup_Main);
		}

		public void SelectElements(IEnumerable<ElementNode> elements, Boolean updateScrollPosition = false)
		{
			if (_currentElementControl == null) return;
			_currentElementControl.SelectedElements = elements;
			if (updateScrollPosition)
				_currentElementControl.UpdateScrollPosition();
		}

		public void SelectControllersAndOutputs(ControllersAndOutputsSet controllersAndOutputs, Boolean updateScrollPosition = false)
		{
			if (_currentControllersControl == null) return;
			_currentControllersControl.SelectedControllersAndOutputs = controllersAndOutputs;
			if (updateScrollPosition)
				_currentControllersControl.UpdateScrollPosition();
		}

		private void buttonOk_Click(object sender, EventArgs e)
		{
			//This is an attempt to band aid up a issue that the graphical designer has with making filters and then abandoning them
			//Until we can fix up a better way to visualize unconnected filters, we will just clean them up from here.
			//Just doing it in Ok as if we cancel it reloads the system anyway.
			VixenSystem.Filters.RemoveOrphanedFilters();
			Vixen.Sys.PropertyManager.RemoveOrphanedProperties();
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
