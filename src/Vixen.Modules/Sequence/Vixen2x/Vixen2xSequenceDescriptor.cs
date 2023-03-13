using System;
using Vixen.Module.SequenceType;
using VixenModules.Sequence.Timed;
using VixenModules.SequenceType.Vixen2x;

namespace VixenModules.Sequence.Vixen2x
{
	public class Vixen2xSequenceModuleDescriptor : SequenceTypeModuleDescriptorBase
	{
		private readonly Guid _typeId = new Guid("{92BBD2CB-B750-437F-8A88-49864D569AB4}");

		public override string FileExtension
		{
			get { return ".vix"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (Vixen2xSequenceTypeModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (TimedSequenceData); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof (Vixen2xSequenceStaticData); }
		}

		public override string Author
		{
			get { return "John McAdams"; }
		}

		public override string TypeName
		{
			get { return "Vixen 2.x Sequence"; }
		}

		public override string Description
		{
			get { return "For importing sequences from Vixen 2.x."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override int ObjectVersion
		{
			get { return 1; }
		}

		public override bool CanCreateNew
		{
			// Override to prevent creation of new sequence
			get { return false; }
		}
	}
}