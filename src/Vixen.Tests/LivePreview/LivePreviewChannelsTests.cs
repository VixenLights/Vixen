using Common.Messages.LivePreview;
using Xunit;

namespace Vixen.Tests.LivePreview
{
	public class LivePreviewChannelsTests
	{
		[Fact]
		public void ChannelNames_HaveExpectedValues()
		{
			Assert.Equal("LivePreview.TurnOnElement",      LivePreviewChannels.TurnOnElement.Name);
			Assert.Equal("LivePreview.TurnOnElements",     LivePreviewChannels.TurnOnElements.Name);
			Assert.Equal("LivePreview.TurnOffElement",     LivePreviewChannels.TurnOffElement.Name);
			Assert.Equal("LivePreview.TurnOffElements",    LivePreviewChannels.TurnOffElements.Name);
			Assert.Equal("LivePreview.ClearActiveEffects", LivePreviewChannels.ClearActiveEffects.Name);
			Assert.Equal("LivePreview.ReleaseContext",     LivePreviewChannels.ReleaseContext.Name);
		}
	}
}
