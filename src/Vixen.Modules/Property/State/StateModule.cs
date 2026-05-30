using System.Windows.Forms.Integration;
using Vixen.Module.Property;
using Vixen.Sys;
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
			if (_data == null)
			{
				_data = new StateData();
			}

			_data.Normalize();
		}

		/// <summary>
		/// Gets or sets the name that identifies the overall state definition.
		/// </summary>
		/// <value>The name that identifies the overall state definition.</value>
		public string Name
		{
			get => _data.Name;
			set => _data.Name = value;
		}

		/// <summary>
		/// Gets or sets the user-provided description of the state definition.
		/// </summary>
		/// <value>The user-provided description of the state definition.</value>
		public string Description
		{
			get => _data.Description;
			set => _data.Description = value;
		}

		/// <summary>
		/// Gets or sets the configured state items.
		/// </summary>
		/// <value>The configured state items.</value>
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

			_data = (StateData)source._data.Clone();
		}

		/// <inheritdoc />
		public override Vixen.Module.IModuleDataModel ModuleData {
			get => _data;
			set
			{
				_data = (StateData)value;
				_data.Normalize();
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
			StateMapperViewModel vm = new StateMapperViewModel(nodes);
			StateMapperView mapper = new StateMapperView(vm);
			ElementHost.EnableModelessKeyboardInterop(mapper);
			var response = mapper.ShowDialog();
			return response == true;
		}

		#endregion
	}
}
