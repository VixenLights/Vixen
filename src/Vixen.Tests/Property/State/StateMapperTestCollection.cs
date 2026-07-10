using Xunit;

namespace Vixen.Tests.Property.State;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class StateMapperTestCollection
{
	public const string Name = "State mapper tests";
}
