using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Vixen.Commands;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Grayscale {
	class CommandHandler : CommandDispatch {
		public ICommand Result;

		public override void Handle(LightingValueCommand obj) {
			Result = new ByteValueCommand(_BasicGrayscaleLuma(obj.CommandValue.Color));
		}

		public override void Handle(ColorValueCommand obj) {
			Result = new ByteValueCommand(_BasicGrayscaleLuma(obj.CommandValue));
		}

		private byte _BasicGrayscaleLuma(Color color) {
			return (byte)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
		}
	}
}
