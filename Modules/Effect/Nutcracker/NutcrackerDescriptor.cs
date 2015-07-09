using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Vixen.Sys;
using Vixen.Module.Effect;
using Vixen.Sys.Attribute;

namespace VixenModules.Effect.Nutcracker
{
	public class NutcrackerDescriptor : EffectModuleDescriptorBase
	{
		public NutcrackerDescriptor()
		{
			ModulePath = EffectName;
		}

		private static Guid _typeId = new Guid("{82334CB3-9472-42FE-A221-8482F5C731DB}");

		public override string EffectName
		{
			get { return "Nutcracker"; }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Pixel; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (Nutcracker); }
		}

		[ModuleDataPath]
		public static string ModulePath { get; set; }

		public override Type ModuleDataClass
		{
			get { return typeof (NutcrackerModuleData); }
		}

		public override string Author
		{
			get { return "Sean Meighan/Derek Backus"; }
		}

		public override string TypeName
		{
			get { return EffectName; }
		}

		public override string Description
		{
			get { return "Use Nutcracker Effects in Vixen."; }
		}

		public override string Version
		{
			get { return "0.0.1"; }
		}

		public override ParameterSignature Parameters
		{
			get
			{
				return new ParameterSignature(
					new ParameterSpecification("NutcrackerData", typeof (NutcrackerData), false)
					);
			}
		}
	}
}