using System.Net;
using Vixen.Tests.FPPClient.Helpers;
using VixenModules.App.FPPClient.Exceptions;
using Xunit;

namespace Vixen.Tests.FPPClient;

public class FppClientGetSequencesTests
{
	[Fact]
	public async Task GetSequencesAsync_Success_ReturnsNames()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("""["GreatestShow","Jingle_Bells"]""")
			});

		// Act
		var sequences = await client.GetSequencesAsync(TestContext.Current.CancellationToken);

		// Assert
		Assert.Equal(2, sequences.Count);
		Assert.Contains("GreatestShow", sequences);
		Assert.Contains("Jingle_Bells", sequences);
	}

	[Fact]
	public async Task GetSequencesAsync_EmptyArray_ReturnsEmpty()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("[]")
			});

		// Act
		var sequences = await client.GetSequencesAsync(TestContext.Current.CancellationToken);

		// Assert
		Assert.Empty(sequences);
	}

	[Fact]
	public async Task GetSequencesAsync_HttpError_ThrowsFppClientException()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.InternalServerError));

		// Act & Assert
		var ex = await Assert.ThrowsAsync<FppClientException>(
			() => client.GetSequencesAsync(TestContext.Current.CancellationToken));
		Assert.Equal(500, ex.HttpStatusCode);
	}
}
