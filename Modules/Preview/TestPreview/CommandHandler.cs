using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace TestPreview {
	class CommandHandler : CommandDispatch {
		public override void Handle(ColorValue obj) {
			Value = obj.CommandValue;
		}

		public override void Handle(LightingValueCommand obj) {
			Value = obj.CommandValue.Color;
		}

		public Color Value;
	}
}
