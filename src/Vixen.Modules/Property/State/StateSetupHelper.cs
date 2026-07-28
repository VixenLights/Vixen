using Common.Controls;
using Vixen.Module.Property;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.Property.State.Setup.Services;

namespace VixenModules.Property.State
{
	/// <summary>
	/// Configures State properties for a selected element node.
	/// </summary>
	public class StateSetupHelper: IElementSetupHelper
	{
		private readonly Func<IPropertyModuleInstance?> _stateModuleFactory;
		private readonly IStateMapperDialogService _dialogService;

		/// <summary>
		/// Initializes a new instance of the <see cref="StateSetupHelper"/> class.
		/// </summary>
		public StateSetupHelper()
			: this(CreateStateModule, new StateMapperDialogService())
		{
		}

		internal StateSetupHelper(
			Func<IPropertyModuleInstance?> stateModuleFactory,
			IStateMapperDialogService dialogService)
		{
			ArgumentNullException.ThrowIfNull(stateModuleFactory);
			ArgumentNullException.ThrowIfNull(dialogService);

			_stateModuleFactory = stateModuleFactory;
			_dialogService = dialogService;
		}

		#region Implementation of IElementSetupHelper

		/// <inheritdoc />
		public string HelperName { get { return "State Mapping"; } }
		
		/// <inheritdoc />
		public bool Perform(IEnumerable<IElementNode> nodes)
		{
			var selectedNodes = nodes.Take(2).ToList();
			if (selectedNodes.Count != 1)
			{
				MessageBoxForm mb = new MessageBoxForm("State Mapping can only be applied to one Element Node",
					"Too many nodes selected", MessageBoxButtons.OK, SystemIcons.Information);
				mb.ShowDialog();
				return false;
			}

			var node = selectedNodes[0];
			if (node.Properties.Contains(StateDescriptor.ModuleId))
			{
				return node.Properties.Get(StateDescriptor.ModuleId)?.ModuleData is StateData existingData &&
				       ShowMapper(node, existingData);
			}

			IPropertyModuleInstance? stateModule = null;
			StateData? stateData = null;
			try
			{
				stateModule = _stateModuleFactory();
				if (stateModule?.ModuleData is StateData data)
				{
					stateData = data;
				}
			}
			catch
			{
				stateModule?.Dispose();
				return false;
			}

			if (stateModule is null || stateData is null)
			{
				stateModule?.Dispose();
				return false;
			}

			bool accepted;
			try
			{
				accepted = ShowMapper(node, stateData);
			}
			catch
			{
				stateModule.Dispose();
				return false;
			}

			if (!accepted)
			{
				stateModule.Dispose();
				return false;
			}

			node.Properties.AddWithoutDefaults(stateModule);
			if (ReferenceEquals(node.Properties.Get(StateDescriptor.ModuleId), stateModule))
			{
				return true;
			}

			stateModule.Dispose();
			return false;
		}

		private static IPropertyModuleInstance? CreateStateModule()
		{
			return ApplicationServices.Get<IPropertyModuleInstance>(StateDescriptor.ModuleId);
		}

		private bool ShowMapper(IElementNode node, StateData data)
		{
			return _dialogService.Show(node, data);
		}

		#endregion
	}
}
