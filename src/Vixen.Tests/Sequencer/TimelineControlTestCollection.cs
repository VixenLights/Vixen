using Xunit;

namespace Vixen.Tests.Sequencer;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class TimelineControlTestCollection
{
	public const string Name = "Timeline control tests";
}
