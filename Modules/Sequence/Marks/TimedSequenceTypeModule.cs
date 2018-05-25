using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Cache.Sequence;
using Vixen.Execution;
using Vixen.IO;
using Vixen.Module.SequenceType;
using Vixen.Sys;

namespace VixenModules.Sequence.Timed
{
	public class TimedSequenceTypeModule : SequenceTypeModuleInstanceBase
	{
		public override ISequence CreateSequence()
		{
			return new TimedSequence();
		}

		public override ISequenceCache CreateSequenceCache()
		{
			return new TimedSequenceCache();
		}

		public override IContentMigrator CreateMigrator()
		{
			return new TimedSequenceMigrator();
		}

		public override ISequenceExecutor CreateExecutor()
		{
			return new Executor();
		}
	}
}