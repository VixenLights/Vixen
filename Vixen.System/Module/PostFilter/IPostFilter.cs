using System;
using System.Drawing;
using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Module.PostFilter {
	public interface IPostFilter : ISetup {
		void Affect(ICommand<float> value);
		void Affect(ICommand<Color> value);
		void Affect(ICommand<DateTime> value);
		void Affect(ICommand<long> value);
		void Affect(ICommand<double> value);
	}
}
