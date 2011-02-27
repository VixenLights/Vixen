using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Vixen.Sys {
	public class DataMover {
		static public IEnumerable<ITypedDataMover> GetAllMovers() {
			return
				(from type in Assembly.GetExecutingAssembly().GetTypes()
				 where type.GetInterface(typeof(ITypedDataMover).Name) != null
				 select type).Select(x => Activator.CreateInstance(x) as ITypedDataMover).ToArray();
		}
	}
}
