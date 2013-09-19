using System;
using Vixen.Common.ValueTypes;
using Vixen.Data.Value;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.SetPosition
{
	public class SetPositionDescriptor : EffectModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{9B6D85EC-F16B-41f2-8584-8E85211E02B8}");

		public override string TypeName
		{
			get { return "Set position"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (SetPositionModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (SetPositionData); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Set the position of servo"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "SetPosition"; }
		}

		public override ParameterSignature Parameters
		{
			get
			{
				return new ParameterSignature(
					new ParameterSpecification("Start Position", typeof (Percentage)),
					new ParameterSpecification("End Position", typeof (Percentage))
					);
			}
		}
	}
}