using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.Sequence;

namespace VixenModules.Sequence.Vixen2x
{
	public class Vixen2xSequenceModuleDescriptor : SequenceModuleDescriptorBase
	{
		private readonly Guid _typeId = new Guid("92BBD2CB-B750-437F-8A88-49864D569AB4");

		override public string FileExtension
		{
			get { return ".vix"; }
		}

		override public Guid TypeId
		{
			get { return _typeId; }
		}

		override public Type ModuleClass
		{
			get { return typeof(Vixen2xSequence); }
		}

		override public Type ModuleDataClass
		{
			get { return typeof(Vixen2xSequenceData); }
		}

		override public string Author
		{
			get { return "Vixen Team"; }
		}

		override public string TypeName
		{
			get { return "Vixen 2.x Sequence"; }
		}

		override public string Description
		{
			get { return "A sequence from Vixen 2.x, used for import."; }
		}

		override public string Version
		{
			get { return "1.0"; }
		}

		public override bool CanCreateNew
		{
			// Override to prevent creation of new sequence
			get { return false; }
		}
	}
}
