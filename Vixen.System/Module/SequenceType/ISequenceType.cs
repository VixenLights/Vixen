using Vixen.Cache.Sequence;
using Vixen.Execution;
using Vixen.IO;
using Vixen.Sys;

namespace Vixen.Module.SequenceType
{
	public interface ISequenceType
	{
		/// <summary>
		/// Includes the leading period.
		/// </summary>
		string FileExtension { get; }

		ISequence CreateSequence();
		ISequenceCache CreateSequenceCache();
		IContentMigrator CreateMigrator();
		ISequenceExecutor CreateExecutor();
		int ObjectVersion { get; }

		// if this sequence type is a custom sequence loader; ie. generally implying that it's an
		// importing module type (to load other sequences from other software)
		bool IsCustomSequenceLoader { get; }

		// if this is a custom sequence loader, it should define this function, as it will be called
		// to load the sequence.
		ISequence LoadSequenceFromFile(string filePath);
	}
}