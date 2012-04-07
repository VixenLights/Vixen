using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Commands;
using Vixen.Module.Preview;
using Vixen.Sys;

namespace TestPreview {
	public class TestPreviewModule : FormPreviewModuleInstanceBase {
		private IDataPolicy _dataPolicy;
		private TestPreviewForm _form;

		public TestPreviewModule() {
			_dataPolicy = new TestPreviewDataPolicy();
		}

		public override void UpdateState(ChannelCommands channelCommands) {
			if(_form != null) {
				_form.Update(channelCommands);
			}
		}

		public override IDataPolicy DataPolicy {
			get { return _dataPolicy; }
		}

		protected override Form Initialize() {
			_form = new TestPreviewForm();
			return _form;
		}
	}
}
