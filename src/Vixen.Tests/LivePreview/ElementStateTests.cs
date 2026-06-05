using Common.Messages.LivePreview;
using Xunit;

namespace Vixen.Tests.LivePreview
{
	public class ElementStateTests
	{
		[Fact]
		public void ElementState_WithExpression_ProducesModifiedCopy()
		{
			var id = Guid.NewGuid();
			var original = new ElementState { Id = id, Color = "#FF0000", Intensity = 0.5, Duration = 5 };

			var copy = original with { Intensity = 1.0 };

			Assert.Equal(0.5,      original.Intensity);
			Assert.Equal(1.0,      copy.Intensity);
			Assert.Equal(id,       copy.Id);
			Assert.Equal("#FF0000", copy.Color);
			Assert.Equal(5,        copy.Duration);
		}
	}
}
