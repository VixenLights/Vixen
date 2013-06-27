using System;
using Common.ValueTypes;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Candle
{
	public class CandleDescriptor : EffectModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{504582D2-43AC-472E-8885-EC4BCBF2B1F7}");

		private ParameterSignature _parameterSignature = new ParameterSignature(
			new ParameterSpecification("Flicker frequency (changes per second)", typeof (int)),
			new ParameterSpecification("Change percentage (absolute)", typeof (Percentage)),
			new ParameterSpecification("Minimum level", typeof (Percentage)),
			new ParameterSpecification("Maximum level", typeof (Percentage)),
			new ParameterSpecification("Flicker frequency deviation cap", typeof (Percentage)),
			new ParameterSpecification("Change percentage deviation cap", typeof (Percentage)));

		public override string TypeName
		{
			get { return "Candle"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (CandleModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (CandleData); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Simulation of a candle flame flicker"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Candle Flicker"; }
		}

		public override ParameterSignature Parameters
		{
			get { return _parameterSignature; }
		}
	}
}