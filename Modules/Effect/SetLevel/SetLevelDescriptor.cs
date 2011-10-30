using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Vixen.Sys;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using Vixen.Module.Effect;

namespace VixenModules.Effect.SetLevel
{
	public class SetLevelDescriptor : EffectModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{32cff8e0-5b10-4466-a093-0d232c55aac0}");
		internal static Guid _RGBPropertyId = new Guid("{5c31be79-a6a7-4864-a660-4e0215ad4778}");

		public override string EffectName { get { return "Set Level"; } }

		public override Guid TypeId { get { return _typeId; } }

		public override Type ModuleClass { get { return typeof(SetLevel); } }

		public override Type ModuleDataClass { get { return typeof(SetLevelData); } }

		public override string Author { get { return "Vixen Team"; } }

		public override string TypeName { get { return EffectName; } }

		public override string Description { get { return "Sets the target channels to a static output level and/or color."; } }

		public override string Version { get { return "0.1"; } }

		public override Guid[] Dependencies { get { return new Guid[] { _RGBPropertyId }; } }

		public override ParameterSignature Parameters
		{
			get
			{
				return new ParameterSignature(
					new ParameterSpecification("Level", typeof(Level)),
					new ParameterSpecification("Color", typeof(Color))
					);
			}
		}
	}
}
