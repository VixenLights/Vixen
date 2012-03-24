using System;
using System.Drawing;

namespace Vixen.Sys.CombinationOperation {
	public interface ICombinationOperation {
		long Combine(long value, IIntentState intentState);
		float Combine(float value, IIntentState intentState);
		double Combine(double value, IIntentState intentState);
		DateTime Combine(DateTime value, IIntentState intentState);
		Color Combine(Color value, IIntentState intentState);
	}
}
