using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.App;
using System.Runtime.Serialization;

namespace VixenModules.App.VirtualEffect
{
	[DataContract]
	public class VirtualEffectLibraryData:ModuleDataModelBase
	{		
		public VirtualEffectLibraryData()
		{
			//VirtualEffect ve = new VirtualEffect("test", Guid.NewGuid(), null);
			////_library.Add(Guid.NewGuid(), ve);
			//Library.Add(Guid.NewGuid(), ve);		
		}

		[DataMember]
		private Dictionary<Guid, VirtualEffect> _library;
		public Dictionary<Guid, VirtualEffect> Library
		{
			get
			{
				if (_library == null)
					_library = new Dictionary<Guid, VirtualEffect>();

				return _library;
			}
			set
			{
				_library = value;
			}
		}

		public override IModuleDataModel Clone()
		{
			VirtualEffectLibraryData result = new VirtualEffectLibraryData();
			result.Library = new Dictionary<Guid, VirtualEffect>(Library);
			return result;
		}
	}
}
