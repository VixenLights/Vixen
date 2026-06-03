using System.Windows.Forms.Integration;
using System.Windows.Interop;
using Vixen.Module.Property;
using Vixen.Sys;
using VixenModules.Property.State.Setup.Services;
using VixenModules.Property.State.Setup.ViewModels;
using VixenModules.Property.State.Setup.Views;

namespace VixenModules.Property.State {
	/// <summary>
	/// Provides named state definitions for display elements.
	/// </summary>
	public sealed class StateModule : PropertyModuleInstanceBase {

		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		
		private StateData _data = new();

		/// <inheritdoc />
		public override void SetDefaultValues() {
			_data.Normalize();
		}

		/// <summary>
		/// Gets the stable identifier for the attached State property.
		/// </summary>
		/// <value>The stable identifier for the attached State property.</value>
		public Guid Id => _data.Id;

		/// <summary>
		/// Gets or sets the configured State definitions.
		/// </summary>
		/// <value>The configured State definitions.</value>
		public List<StateDefinitionData> StateDefinitions
		{
			get => _data.StateDefinitions;
			set => _data.StateDefinitions = value;
		}

		/// <summary>
		/// Gets or sets the name of the first State definition.
		/// </summary>
		/// <value>The name of the first State definition.</value>
		public string Name
		{
			get => _data.Name;
			set => _data.Name = value;
		}

		/// <summary>
		/// Gets or sets the description of the first State definition.
		/// </summary>
		/// <value>The description of the first State definition.</value>
		public string Description
		{
			get => _data.Description;
			set => _data.Description = value;
		}

		/// <summary>
		/// Gets or sets the State items of the first State definition.
		/// </summary>
		/// <value>The State items of the first State definition.</value>
		public List<StateItemData> Items
		{
			get => _data.Items;
			set => _data.Items = value;
		}

		/// <inheritdoc />
		public override void CloneValues(IProperty sourceProperty)
		{
			var source = sourceProperty as StateModule;
			if (source == null)
			{
				Logging.Error(
					"StateModule: trying to CloneValues from another property, but it's not a StateModule!");
				return;
			}

			_data = source._data.CloneForNewProperty();
		}

		/// <inheritdoc />
		public override Vixen.Module.IModuleDataModel? ModuleData {
			get => _data;
			set
			{
				if( value != null)
				{
					_data = (StateData)value;
					_data.Normalize();
				}
			}
		}

		/// <summary>
		/// Gets the State property attached to an element node.
		/// </summary>
		/// <param name="element">The element node to inspect.</param>
		/// <returns>The attached State property, or <see langword="null" /> when the node does not have one.</returns>
		public static StateModule? GetStateModuleForElement(IElementNode element)
		{
			return element.Properties.Get(StateDescriptor.ModuleId) as StateModule;
		}

		#region Overrides of PropertyModuleInstanceBase

		/// <inheritdoc />
		public override bool HasElementSetupHelper => true;

		/// <inheritdoc />
		public override bool SetupElements(IEnumerable<IElementNode> nodes)
		{
			var selectedNodes = nodes.Take(2).ToList();
			if (selectedNodes.Count != 1)
			{
				return false;
			}

			var node = selectedNodes[0];
			var vm = new StateMapperViewModel(node, _data, new StateColorPickerService());
			var mapper = new StateMapperView(vm);
			if (Form.ActiveForm != null)
			{
				new WindowInteropHelper(mapper).Owner = Form.ActiveForm.Handle;
			}

			ElementHost.EnableModelessKeyboardInterop(mapper);
			var response = mapper.ShowDialog();
			return response == true;
		}

		#endregion
	}
}
