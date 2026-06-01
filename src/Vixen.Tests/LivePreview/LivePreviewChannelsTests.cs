using Common.Messages.LivePreview;
using Xunit;

namespace Vixen.Tests.LivePreview
{
	public class LivePreviewChannelsTests
	{
		[Fact]
		public void ChannelConstants_HaveExpectedValues()
		{
			Assert.Equal("LivePreview.TurnOnElement",      LivePreviewChannels.TurnOnElement);
			Assert.Equal("LivePreview.TurnOnElements",     LivePreviewChannels.TurnOnElements);
			Assert.Equal("LivePreview.TurnOffElement",     LivePreviewChannels.TurnOffElement);
			Assert.Equal("LivePreview.TurnOffElements",    LivePreviewChannels.TurnOffElements);
			Assert.Equal("LivePreview.ClearActiveEffects", LivePreviewChannels.ClearActiveEffects);
		}
	}
}
