using System.Windows.Forms;
using Vixen.Module.Preview;

namespace VixenModules.Preview.TestPreview {
	public class TestPreviewModule : FormPreviewModuleInstanceBase {
		private TestPreviewForm _form;

		protected override void Update() {
			if(_form != null) {
				_form.Update(ChannelStates);
			}
		}

		protected override Form Initialize() {
			_form = new TestPreviewForm();
			return _form;
		}
	}
}
