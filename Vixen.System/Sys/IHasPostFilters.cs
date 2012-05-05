using System.Collections.Generic;
using Vixen.Module.PostFilter;

namespace Vixen.Sys {
	interface IHasPostFilters {
		void AddPostFilter(IPostFilterModuleInstance module);
		void InsertPostFilter(int index, IPostFilterModuleInstance module);
		void RemovePostFilter(IPostFilterModuleInstance module);
		void RemovePostFilterAt(int index);
		void ClearPostFilters();
		IEnumerable<IPostFilterModuleInstance> GetAllPostFilters();
	}
}
