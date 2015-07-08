using System;
using System.Collections.Generic;
using System.Drawing;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace VixenModules.Effect.MultiAlternating
{
	public class MultiAlternatingDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("{C4A8BBB8-334B-4DEE-B5A3-A148BFFFF3F7}");
		private static readonly Guid _ColorGradientId = new Guid("{64f4ab26-3ed4-49a3-a004-23656ed0424a}");


		public override string EffectName
		{
			get { return "MultiAlternating"; }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Basic; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(MultiAlternating); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(MultiAlternatingData); }
		}

		public override string Author
		{
			get { return "Vixen Team / James Bolding"; }
		}

		public override string TypeName
		{
			get { return EffectName; }
		}

		public override string Description
		{
			get { return "Sets the target elements to an MultiAlternating output level and/or color."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Guid[] Dependencies
		{
			get { return new[] { _ColorGradientId }; }
		}

		public override ParameterSignature Parameters
		{
			get
			{
				return new ParameterSignature(
					new ParameterSpecification("Colors", typeof(List<ColorGradient>),false),
					new ParameterSpecification("Curves", typeof(List<Curve>), false),
					new ParameterSpecification("Interval", typeof(int)),
					new ParameterSpecification("GroupInterval", typeof(int)),
					new ParameterSpecification("Enabled",typeof(bool)),
					new ParameterSpecification("IntervalSkipCount", typeof(int))
					);
			}
		}
	}
}