using System.Windows.Forms.Integration;
using Vixen.Module.Property;
using Vixen.Sys;
using VixenModules.Property.State.Setup.ViewModels;
using VixenModules.Property.State.Setup.Views;

namespace VixenModules.Property.State {
	public class StateModule : PropertyModuleInstanceBase {

		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		
		private StateData _data = new();

		public override void SetDefaultValues() {
			if (_data == null)
			{
				_data = new StateData();
			}
		}

		public string StateName
		{
			get => _data.StateName;
			set => _data.StateName = value;
		}

		public string ItemName
		{
			get => _data.ItemName;
			set => _data.ItemName = value;
		}

		public System.Drawing.Color ItemColor
		{
			get => _data.ItemColor;
			set => _data.ItemColor = value;
		}

		public override void CloneValues(IProperty sourceProperty)
		{
			var source = sourceProperty as StateModule;
			if (source == null)
			{
				Logging.Error(
					"StateModule: trying to CloneValues from another property, but it's not a StateModule!");
				return;
			}

			StateName = source.StateName;
			ItemName = source.ItemName;
			ItemColor = source.ItemColor;
		}

		public override Vixen.Module.IModuleDataModel ModuleData {
			get => _data;
			set => _data = (StateData)value;
		}

		public static StateModule GetStateModuleForElement(IElementNode element)
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
			return true;
		}

		#endregion
	}
}
