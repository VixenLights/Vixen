using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.SequenceType;

namespace VixenModules.Sequence.Timed
{
	public class TimedSequenceModuleDescriptor : SequenceTypeModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{296bdba2-9bf3-4bff-a9f2-13efac5c8ecb}");

		public override string FileExtension
		{
			get { return TimedSequence.Extension; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (TimedSequenceTypeModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (TimedSequenceData); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string TypeName
		{
			get { return "Timed Sequence"; }
		}

		public override string Description
		{
			get { return "A basic timed sequence, which is a collection of effects configured to occur at predefined times."; }
		}

		public override string Version
		{
			get { return "7.0"; }
		}

		public override int ObjectVersion
		{
			get { return 7; }
		}
	}
}