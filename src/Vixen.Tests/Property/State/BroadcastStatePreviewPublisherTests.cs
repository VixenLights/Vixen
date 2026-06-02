using Common.Broadcast;
using Common.Messages.LivePreview;
using VixenModules.Property.State.Setup.Preview;
using Xunit;

namespace Vixen.Tests.Property.State;

public sealed class BroadcastStatePreviewPublisherTests
{
	[Fact]
	public void Operations_PublishExpectedTypedMessages()
	{
		// Arrange
		var source = new object();
		var publisher = new BroadcastStatePreviewPublisher();
		var elementId = Guid.NewGuid();
		TurnOnElementsMessage? turnOnMessage = null;
		TurnOffElementsMessage? turnOffMessage = null;
		ClearActiveEffectsMessage? clearMessage = null;
		ReleaseContextMessage? releaseMessage = null;
		Broadcast.Subscribe(source, LivePreviewChannels.TurnOnElements, message => turnOnMessage = message);
		Broadcast.Subscribe(source, LivePreviewChannels.TurnOffElements, message => turnOffMessage = message);
		Broadcast.Subscribe(source, LivePreviewChannels.ClearActiveEffects, message => clearMessage = message);
		Broadcast.Subscribe(source, LivePreviewChannels.ReleaseContext, message => releaseMessage = message);

		try
		{
			// Act
			publisher.TurnOn([]);
			publisher.TurnOff([]);
			Assert.Null(turnOnMessage);
			Assert.Null(turnOffMessage);

			var pair = new StatePreviewPair(elementId, "#FF0000");
			publisher.TurnOn([pair, pair]);
			publisher.TurnOff([elementId, elementId]);
			publisher.Clear();
			publisher.Release();

			// Assert
			var state = Assert.Single(Assert.IsType<TurnOnElementsMessage>(turnOnMessage).States);
			Assert.Equal(elementId, state.Id);
			Assert.Equal("#FF0000", state.Color);
			Assert.Equal(int.MaxValue, state.Duration);
			Assert.Equal(100, state.Intensity);
			Assert.Equal(BroadcastStatePreviewPublisher.ContextName, turnOnMessage.ContextName);
			Assert.Equal([elementId], Assert.IsType<TurnOffElementsMessage>(turnOffMessage).ElementIds);
			Assert.Equal(BroadcastStatePreviewPublisher.ContextName, turnOffMessage.ContextName);
			Assert.Equal(BroadcastStatePreviewPublisher.ContextName, Assert.IsType<ClearActiveEffectsMessage>(clearMessage).ContextName);
			Assert.Equal(BroadcastStatePreviewPublisher.ContextName, Assert.IsType<ReleaseContextMessage>(releaseMessage).ContextName);
		}
		finally
		{
			Broadcast.Unsubscribe(source, LivePreviewChannels.TurnOnElements);
			Broadcast.Unsubscribe(source, LivePreviewChannels.TurnOffElements);
			Broadcast.Unsubscribe(source, LivePreviewChannels.ClearActiveEffects);
			Broadcast.Unsubscribe(source, LivePreviewChannels.ReleaseContext);
		}
	}
}
