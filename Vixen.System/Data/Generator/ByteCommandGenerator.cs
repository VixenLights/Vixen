using System;
using System.Drawing;
using Vixen.Commands;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Generator {
	class ByteCommandGenerator : IGenerator, IAnyCombinatorHandler {
		public void GenerateCommand(ICombinator combinator) {
			combinator.Dispatch(this);
		}

		public ICommand Value { get; private set; }

		public void Handle(ICombinator<float> obj) {
			Value = new ByteValue((byte)obj.Value);
		}

		public void Handle(ICombinator<DateTime> obj) {
			// Ignored
		}

		public void Handle(ICombinator<Color> obj) {
			Value = new ByteValue((byte)(obj.Value.ToArgb() & 0xff));
		}

		public void Handle(ICombinator<long> obj) {
			Value = new ByteValue((byte)obj.Value);
		}

		public void Handle(ICombinator<double> obj) {
			//double = %
			Value = new ByteValue((byte)(obj.Value * byte.MaxValue));
		}
	}
}
