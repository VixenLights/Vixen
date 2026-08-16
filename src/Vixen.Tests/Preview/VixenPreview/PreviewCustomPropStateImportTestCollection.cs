using Xunit;

namespace Vixen.Tests.Preview.VixenPreview;

/// <summary>
/// Serializes tests that modify process-wide Vixen configuration state.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class PreviewCustomPropStateImportTestCollection
{
	/// <summary>
	/// Gets the name of the test collection.
	/// </summary>
	public const string Name = "Preview custom prop state import tests";
}
