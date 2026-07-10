using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.State
{
	public class StateDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid TypeIdentifier = new Guid("{F939FF91-5876-44E6-8A4B-8335D6930494}");

		public override string EffectName
		{
			get { return "State"; }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Basic; }
		}

		#region Overrides of EffectModuleDescriptorBase

		/// <inheritdoc />
		public override bool SupportsMarks => true;

		#endregion

		public override Guid TypeId
		{
			get { return TypeIdentifier; }
		}

		public override Type ModuleClass
		{
			get { return typeof(State); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(StateData); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string TypeName
		{
			get { return EffectName; }
		}

		public override string Description
		{
			get { return "Provides State effects for Props"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		#region Legacy

		public override ParameterSignature Parameters { get; } = null!;

		#endregion

	}
}

