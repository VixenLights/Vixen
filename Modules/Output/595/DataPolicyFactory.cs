using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace VixenModules.Output.Olsen595 {
	class DataPolicyFactory : IDataPolicyFactory {
		public IDataPolicy CreateDataPolicy() {
			return new DataPolicy();
		}
	}
}
