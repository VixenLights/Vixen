using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace VixenModules.Output.DmxUsbPro {
	class DataPolicyFactory : IDataPolicyFactory {
		public IDataPolicy CreateDataPolicy() {
			return new DataPolicy();
		}
	}
}
