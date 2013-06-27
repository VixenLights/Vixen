using System;

namespace Vixen.Sys.Output
{
	public class IntentOutput : Output
	{
		internal IntentOutput(Guid id, string name, int index)
			: base(id, name, index)
		{
		}

		public IntentChangeCollection IntentChangeCollection { get; set; }
	}
}