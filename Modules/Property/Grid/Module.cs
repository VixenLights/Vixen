using System.Linq;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.Property;

namespace VixenModules.Property.Grid {
	public class Module : PropertyModuleInstanceBase {
		private Data _data;

		public override void SetDefaultValues() {
			_data.Width = Owner.Count();
			_data.Height = 1;
		}

		public override bool HasSetup {
			get { return true; }
		}

		public override bool Setup() {
			using(SetupForm setupForm = new SetupForm(_data.Width, _data.Height, Owner.Count())) {
				if(setupForm.ShowDialog() == DialogResult.OK) {
					_data.Width = setupForm.SelectedWidth;
					_data.Height = setupForm.SelectedHeight;
					return true;
				}
				return false;
			}
		}

		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = (Data)value; }
		}
	}
}
