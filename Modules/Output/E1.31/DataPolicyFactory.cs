using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using VixenModules.Output.E131;

namespace VixenModules.Controller.E131 {
	class DataPolicyFactory : IDataPolicyFactory {
		public IDataPolicy CreateDataPolicy() {
			return new DataPolicy();
		}
	}
}
