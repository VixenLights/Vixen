using System.Collections.Generic;
using Vixen.Data.Flow;

namespace Vixen.Rule
{
	public interface IPatchingRule
	{
		string Description { get; }
		IEnumerable<IDataFlowComponentReference> GenerateSourceReferences();
		IEnumerable<DataFlowPatch> GeneratePatches();
	}
}