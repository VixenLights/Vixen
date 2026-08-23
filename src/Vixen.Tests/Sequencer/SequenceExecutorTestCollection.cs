using Xunit;

namespace Vixen.Tests.Sequencer;

/// <summary>
/// Defines the non-parallel collection for tests that replace the process-wide asynchronous-operation synchronization context.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class SequenceExecutorTestCollection
{
	/// <summary>
	/// Identifies the collection that serializes sequence-executor lifecycle tests.
	/// </summary>
	public const string Name = "Sequence executor lifecycle tests";
}
