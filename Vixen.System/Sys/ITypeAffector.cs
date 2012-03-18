using System;
using System.Drawing;

namespace Vixen.Sys {
	public interface ITypeAffector {
		float Affect(float value);
		Color Affect(Color value);
		DateTime Affect(DateTime value);
		long Affect(long value);
		double Affect(double value);
	}
}
