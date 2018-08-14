using System.Windows.Forms;
using Vixen.Module.Property;
using Vixen.Sys;

namespace VixenModules.Property.Orientation {
	public class OrientationModule : PropertyModuleInstanceBase {
		private OrientationData _data;

		public OrientationModule() {
			_data = new OrientationData();
		}

		public override void SetDefaultValues() {
			if (_data == null)
				_data = new OrientationData();
			_data.Orientation = Property.Orientation.Orientation.Vertical;
		}

		public Orientation Orientation
		{
			get => _data.Orientation;
			set => _data.Orientation = value;
		}

		public override Vixen.Module.IModuleDataModel ModuleData {
			get => _data;
			set => _data = (OrientationData)value;
		}

		public static Orientation GetOrientationForElement(ElementNode element)
		{
			Orientation p = Property.Orientation.Orientation.Vertical;
			OrientationModule module = element?.Properties.Get(OrientationDescriptor.ModuleId) as OrientationModule;
			if (module != null)
			{
				p = module.Orientation;
			}
			
			return p;
		}

		#region Overrides of PropertyModuleInstanceBase

		/// <inheritdoc />
		public override bool HasSetup => true;

		#endregion

		public override bool Setup() {
			using (SetupForm setupForm = new SetupForm(_data.Orientation)) {
				if (setupForm.ShowDialog() == DialogResult.OK)
				{
					_data.Orientation = setupForm.Orientation;
					return true;
				}
				return false;
			}
		}

	}
}
