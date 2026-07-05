using VixenModules.Output.DDP;
using Xunit;

namespace Vixen.Tests.DDP;

public class DDPDataTests
{
	[Fact]
	public void Clone_CopiesHostNameAlongsideAddress()
	{
		var original = new DDPData
		{
			Address = "192.168.1.50",
			HostName = "wled-porch"
		};

		var clone = (DDPData)original.Clone();

		Assert.NotSame(original, clone);
		Assert.Equal(original.Address, clone.Address);
		Assert.Equal(original.HostName, clone.HostName);
	}

	[Fact]
	public void Clone_HostNameNull_ClonePreservesNull()
	{
		var original = new DDPData
		{
			Address = "192.168.1.50",
			HostName = null
		};

		var clone = (DDPData)original.Clone();

		Assert.Null(clone.HostName);
	}
}
