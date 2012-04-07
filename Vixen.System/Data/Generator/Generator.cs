using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Vixen.Commands;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Generator {
	abstract public class Generator : IGenerator, IAnyCombinatorHandler {
		public void GenerateCommand(ICombinator combinator) {
			combinator.Dispatch(this);
		}

		virtual public void Handle(ICombinator<float> obj) { }

		virtual public void Handle(ICombinator<DateTime> obj) { }

		virtual public void Handle(ICombinator<Color> obj) { }

		virtual public void Handle(ICombinator<long> obj) { }

		virtual public void Handle(ICombinator<double> obj) { }

		public ICommand Value { get; protected set; }
	}
}
