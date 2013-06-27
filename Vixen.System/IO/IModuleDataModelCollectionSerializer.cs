using System.Collections.Generic;
using Vixen.Module;

namespace Vixen.IO
{
	public interface IModuleDataModelCollectionSerializer
	{
		void Write(IEnumerable<IModuleDataModel> dataModels);
		IEnumerable<IModuleDataModel> Read();
	}
}