using System;
using Vixen.Module.MixingFilter;

namespace VixenModules.LayerMixingFilter.LumaKey
{
	public class LumaKeyModuleDescriptor : LayerMixingFilterModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("{7583F7D4-DF99-4492-8D10-257C2677FF7B}");

		public override string TypeName
		{
			get { return "Luma Key"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(LumaKeyModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(LumaKeyData); }
		}

		public override string Author
		{
			get { return "Jon Chuchla"; }
		}

		public override string Description
		{
			get
			{
				return
					"Creates a luma mask based on the selected brightness and fills with the higher layer.";
			}
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}
