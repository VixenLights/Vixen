using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Vixen.Module.Property;
using Vixen.Sys;

namespace VixenModules.Property.Location {
	public class LocationModule : PropertyModuleInstanceBase {

		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private LocationData _data;

		public LocationModule() {
			_data = new LocationData();
		}

		public override void SetDefaultValues() {
			if (_data == null)
				_data = new LocationData();
			_data.X = _data.Y = _data.Z = 1;
		}

		public Point Position()
		{
			return new Point(_data.X, _data.Y);
		}

		public override Vixen.Module.IModuleDataModel ModuleData {
			get {
				return _data;
			}
			set {
				_data = (LocationData)value;
			}
		}

		#region Overrides of PropertyModuleInstanceBase

		/// <inheritdoc />
		public override void CloneValues(IProperty sourceProperty)
		{
			var source = sourceProperty as LocationModule;
			if (source == null)
			{
				Logging.Error(
					"LocationModule: trying to CloneValues from another property, but it's not a LocationModule!");
				return;
			}

			_data.X = source._data.X;
			_data.Y = source._data.Y;
			_data.Z = source._data.Z;
		}

		#endregion

		public static Point GetPositionForElement(IElementNode element)
		{
			Point p;
			LocationModule module = element.Properties.Get(LocationDescriptor.ModuleId) as LocationModule;
			if (module != null)
			{
				p = module.Position();
			}
			else
			{
				p = new Point();
			}
			
			return p;
		}

		#region Overrides of PropertyModuleInstanceBase

		/// <inheritdoc />
		public override bool HasSetup => true;

		#endregion

		public override bool Setup() {
			using (SetupForm setupForm = new SetupForm(_data)) {
				if (setupForm.ShowDialog() == DialogResult.OK) {
					_data.X = setupForm.X;
					_data.Y = setupForm.Y;
					_data.Z = setupForm.Z;
					return true;
				}
				return false;
			}
		}

	}
}
