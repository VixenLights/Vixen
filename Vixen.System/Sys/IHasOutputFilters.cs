using System.Collections.Generic;
using Vixen.Module.OutputFilter;

namespace Vixen.Sys {
	interface IHasOutputFilters {
		void AddOutputFilter(IOutputFilterModuleInstance module);
		void InsertOutputFilter(int index, IOutputFilterModuleInstance module);
		void RemoveOutputFilter(IOutputFilterModuleInstance module);
		void RemoveOutputFilterAt(int index);
		void ClearOutputFilters();
		IEnumerable<IOutputFilterModuleInstance> GetAllOutputFilters();
	}
}
