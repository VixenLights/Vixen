using VixenModules.App.ExportWizard;
using Xunit;

namespace Vixen.Tests.ExportWizard;

public class FppHostValidatorTests
{
	[Fact]
	public void IsHostAddressValid_ValidIpv4_ReturnsTrue()
	{
		Assert.True(FppHostValidator.IsHostAddressValid("192.168.1.50"));
	}

	[Fact]
	public void IsHostAddressValid_ValidIpv6_ReturnsTrue()
	{
		Assert.True(FppHostValidator.IsHostAddressValid("::1"));
	}

	[Fact]
	public void IsHostAddressValid_ValidHostname_ReturnsTrue()
	{
		Assert.True(FppHostValidator.IsHostAddressValid("fpp"));
	}

	[Fact]
	public void IsHostAddressValid_ValidFqdn_ReturnsTrue()
	{
		Assert.True(FppHostValidator.IsHostAddressValid("fpp.local"));
	}

	[Fact]
	public void IsHostAddressValid_Null_ReturnsFalse()
	{
		Assert.False(FppHostValidator.IsHostAddressValid(null));
	}

	[Fact]
	public void IsHostAddressValid_EmptyString_ReturnsFalse()
	{
		Assert.False(FppHostValidator.IsHostAddressValid(""));
	}

	[Fact]
	public void IsHostAddressValid_ContainsScheme_ReturnsFalse()
	{
		Assert.False(FppHostValidator.IsHostAddressValid("http://192.168.1.50"));
	}

	[Fact]
	public void IsHostAddressValid_ContainsBackslash_ReturnsFalse()
	{
		Assert.False(FppHostValidator.IsHostAddressValid(@"\\fpp\share"));
	}
}
