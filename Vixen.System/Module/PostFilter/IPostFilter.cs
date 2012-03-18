using System;
using System.Drawing;
using Vixen.Sys;

namespace Vixen.Module.PostFilter {
	public interface IPostFilter : ISetup {
		float Affect(float value);
		Color Affect(Color value);
		DateTime Affect(DateTime value);
		long Affect(long value);
		double Affect(double value);
		IFilterState CreateFilterState();
	}
}
