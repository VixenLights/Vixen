using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.CountDown
{
	public class CountDownDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("09770a4a-e69a-4d43-bcee-0f762f2d1948");

		public override ParameterSignature Parameters
		{
			get { return new ParameterSignature(); }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Pixel; }
		}

		#region Overrides of EffectModuleDescriptorBase

		/// <inheritdoc />
		public override bool SupportsMarks => true;

		#endregion

		public override string TypeName
		{
			get { return EffectName; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(CountDown); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(CountDownData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies a CountDown effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Count Down"; }
		}
	}
}
