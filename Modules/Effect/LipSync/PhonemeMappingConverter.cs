using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Vixen.Module.App;
using Vixen.Services;
using VixenModules.App.LipSyncApp;

namespace VixenModules.Effect.LipSync
{
	public class PhonemeMappingConverter: TypeConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			var library = ApplicationServices.Get<IAppModuleInstance>(LipSyncMapDescriptor.ModuleID) as LipSyncMapLibrary;
			List<string> maps = library.Library.Values.Select(data => data.ToString()).ToList();
			StandardValuesCollection values = new StandardValuesCollection(maps);
			return values;
		}
	}
}
