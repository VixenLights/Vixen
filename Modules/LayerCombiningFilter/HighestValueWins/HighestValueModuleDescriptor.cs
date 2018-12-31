using System;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.HighestValueWins
{
	public class HighestValueModuleDescriptor : LayerMixingFilterModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("{01D6D526-9586-44BB-82F4-10ABACF58B61}");

		public override string TypeName
		{
			get { return "Highest Value (Default)"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (HighestValueModule); }
		}

		public override string Author
		{
			get { return "Jon Chuchla"; }
		}

		public override string Description
		{
			get { return "Combines two layers by passing the higher value";}
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}
