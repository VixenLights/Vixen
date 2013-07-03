using System;
using Vixen.Module.SequenceType;
using VixenModules.Sequence.Timed;
using VixenModules.SequenceType.Vixen2x;

namespace VixenModules.Sequence.Vixen2x {
	public class Vixen2xSequenceModuleDescriptor : SequenceTypeModuleDescriptorBase {
		private readonly Guid _typeId = new Guid("{92BBD2CB-B750-437F-8A88-49864D569AB4}");

		override public string FileExtension {
			get { return ".vix"; }
		}

	    override public Guid TypeId	{
			get { return _typeId; }
		}

		override public Type ModuleClass {
			get { return typeof(Vixen2xSequenceTypeModule); }
		}

		override public Type ModuleDataClass {
			get { return typeof(TimedSequenceData); }
		}

        public override Type ModuleStaticDataClass
        {
            get
            {
                return typeof(Vixen2xSequenceStaticData);
            }
        }

		override public string Author {
			get { return "John McAdams"; }
		}

		override public string TypeName	{
			get { return "Vixen 2.x Sequence"; }
		}

		override public string Description {
			get { return "For importing sequences from Vixen 2.x."; }
		}

		override public string Version {
			get { return "1.0"; }
		}

		override public int ClassVersion {
			get { return 3;	}
		}

		override public bool CanCreateNew {
			// Override to prevent creation of new sequence
			get { return false; }
		}
	}
}
