using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.SequenceType;
using Vixen.Sys;

namespace VixenModules.Sequence.Vixen2x
{
    using Vixen.Execution;
    using Vixen.IO;

    public class Vixen2xSequence : SequenceTypeModuleInstanceBase
	{
		public Vixen2xSequence()
		{
		}

//		public Vixen2xSequence(Vixen2xSequence original)
//			: base(original)
//		{
//		}

	    public override ISequence CreateSequence()
	    {
	        throw new NotImplementedException();
	    }

        public override IMigrator CreateMigrator()
        {
            throw new NotImplementedException();
        }

        public override ISequenceExecutor CreateExecutor()
        {
            throw new NotImplementedException();
        }

        public override IModuleInstance Clone()
		{
			return new Vixen2xSequence();
		}
	}
}
