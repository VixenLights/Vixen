using System;
using System.Collections.Generic;
using System.Text;
using Vixen.Sys;

namespace VixenModules.Output.HelixController
{
	class HelixDataPolicyFactory : IDataPolicyFactory
	{
		public IDataPolicy CreateDataPolicy()
		{
			return new HelixDataPolicy();
		}
	}
}
