using System;
using Vixen.Data.Flow;

namespace Vixen.Sys.Output
{
	public abstract class Output
	{
		protected internal Output(Guid id, string name, int index)
		{
			Id = id;
			Name = name;
			Index = index;
		}

		// Completely independent; nothing is current dependent upon this value.
		public string Name { get; set; }

		public Guid Id { get; private set; }

		public int Index { get; private set; }

		public IDataFlowComponentReference Source { get; set; }

		public IDataFlowData State
		{
			get { return Source != null ? Source.GetOutputState() : null; }
		}
	}
}