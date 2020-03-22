using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vixen.Sys;

namespace VixenModules.Output.K8055_Controller
{
	internal class K8055DataPolicyFactory: IDataPolicyFactory
	{
		public IDataPolicy CreateDataPolicy()
		{
			return new K8055DataPolicy();
		}
	}
}
	