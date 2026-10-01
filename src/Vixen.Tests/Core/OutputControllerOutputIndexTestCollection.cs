using Xunit;

namespace Vixen.Tests.Core;

/// <summary>
/// Defines the non-parallel collection for tests that replace process-wide VixenSystem managers.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class OutputControllerOutputIndexTestCollection
{
	/// <summary>
	/// Identifies the collection that serializes output-controller index tests.
	/// </summary>
	public const string Name = "Output controller output index tests";
}
