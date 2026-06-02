using System.Reflection;
using Common.Broadcast;
using Common.Messages;
using Common.Messages.LivePreview;
using Xunit;

namespace Vixen.Tests.LivePreview
{
	public class BroadcastChannelTests
	{
		[Fact]
		public void Constructor_IsNotPublic()
		{
			// Act
			var publicConstructors = typeof(BroadcastChannel<TurnOnElementsMessage>).GetConstructors();

			// Assert
			Assert.Empty(publicConstructors);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		public void Constructor_RejectsMissingName(string? name)
		{
			// Arrange
			var constructor = typeof(BroadcastChannel<TurnOnElementsMessage>).GetConstructor(
				BindingFlags.Instance | BindingFlags.NonPublic,
				null,
				[typeof(string)],
				null);
			Assert.NotNull(constructor);

			// Act
			var exception = Assert.Throws<TargetInvocationException>(() => constructor.Invoke([name]));

			// Assert
			Assert.IsAssignableFrom<ArgumentException>(exception.InnerException);
		}

		[Fact]
		public void TypedOverloads_RejectNullChannel()
		{
			// Arrange
			var source = new object();
			var message = new TurnOnElementsMessage();

			// Act and assert
			Assert.Throws<ArgumentNullException>(() => Broadcast.Publish((BroadcastChannel<TurnOnElementsMessage>)null!, message));
			Assert.Throws<ArgumentNullException>(() => Broadcast.Subscribe(source, (BroadcastChannel<TurnOnElementsMessage>)null!, _ => { }));
			Assert.Throws<ArgumentNullException>(() => Broadcast.Unsubscribe(source, (BroadcastChannel<TurnOnElementsMessage>)null!));
		}

		[Fact]
		public void TypedChannel_PublishesWithoutWpfApplication()
		{
			// Arrange
			var source = new object();
			var expected = new TurnOnElementsMessage();
			TurnOnElementsMessage? actual = null;
			Broadcast.Subscribe(source, LivePreviewChannels.TurnOnElements, message => actual = message);

			try
			{
				// Act
				Broadcast.Publish(LivePreviewChannels.TurnOnElements, expected);

				// Assert
				Assert.Same(expected, actual);
			}
			finally
			{
				Broadcast.Unsubscribe(source, LivePreviewChannels.TurnOnElements);
			}
		}
	}
}
