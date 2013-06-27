using Vixen.Execution;
using Vixen.IO;
using Vixen.Module.SequenceType;
using Vixen.Sys;
using VixenModules.Sequence.Timed;
using VixenModules.SequenceType.Vixen2x;

namespace VixenModules.Sequence.Vixen2x
{
	public class Vixen2xSequenceTypeModule : SequenceTypeModuleInstanceBase
	{
		public override ISequence CreateSequence()
		{
			return new TimedSequence();
		}

		public override IContentMigrator CreateMigrator()
		{
			return new SequenceMigrator();
		}

		public override ISequenceExecutor CreateExecutor()
		{
			return new Executor();
		}

		public override bool IsCustomSequenceLoader
		{
			get { return true; }
		}

		public override ISequence LoadSequenceFromFile(string Vixen2File)
		{
			Vixen2xSequenceImporterForm v2ImporterForm = new Vixen2xSequenceImporterForm();
			if (!v2ImporterForm.ProcessFile(Vixen2File)) {
				throw new System.FormatException("Not enough channel nodes to import file");
			}
			return v2ImporterForm.Sequence;
		}
	}
}