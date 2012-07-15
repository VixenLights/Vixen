using Vixen.Execution;
using Vixen.IO;
using Vixen.Sys;

namespace Vixen.Module.SequenceType {
	public interface ISequenceType {
		/// <summary>
		/// Includes the leading period.
		/// </summary>
		string FileExtension { get; }
		ISequence CreateSequence();
		IMigrator CreateMigrator();
		ISequenceExecutor CreateExecutor();
		int ClassVersion { get; }
	}
}
