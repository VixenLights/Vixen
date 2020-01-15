using System;
using System.Collections.Generic;
using Vixen.Sys;

namespace VixenModules.Output.ElexolUSBIO
{
	class ElexolUSBDataPolicyFactory : IDataPolicyFactory
	{
		public IDataPolicy CreateDataPolicy()
		{
			return new ElexolUSBDataPolicy();
		}
	}
}