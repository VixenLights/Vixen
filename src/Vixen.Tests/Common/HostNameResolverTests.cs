using System.Net.Sockets;
using Utilities;
using Xunit;

namespace Vixen.Tests.Common;

public class HostNameResolverTests
{
	[Fact]
	public void TryResolveToIPv4_LocalHost_ReturnsTrueAndAnIPv4Address()
	{
		bool result = HostNameResolver.TryResolveToIPv4("localhost", out var resolvedAddress);

		Assert.True(result);
		Assert.NotNull(resolvedAddress);
		Assert.Equal(AddressFamily.InterNetwork, resolvedAddress.AddressFamily);
	}

	[Fact]
	public void TryResolveToIPv4_ReservedInvalidTld_ReturnsFalse()
	{
		bool result = HostNameResolver.TryResolveToIPv4("this-host-does-not-exist.invalid", out var resolvedAddress);

		Assert.False(result);
		Assert.Null(resolvedAddress);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void TryResolveToIPv4_EmptyHostName_ReturnsFalse(string hostName)
	{
		bool result = HostNameResolver.TryResolveToIPv4(hostName, out var resolvedAddress);

		Assert.False(result);
		Assert.Null(resolvedAddress);
	}

	[Fact]
	public void TryResolveToIPv4_NullHostName_ReturnsFalse()
	{
		bool result = HostNameResolver.TryResolveToIPv4(null, out var resolvedAddress);

		Assert.False(result);
		Assert.Null(resolvedAddress);
	}
}
