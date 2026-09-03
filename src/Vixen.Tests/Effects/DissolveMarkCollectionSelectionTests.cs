using System.Collections.ObjectModel;
using Vixen.Marks;
using VixenModules.App.Marks;
using VixenModules.Effect.Dissolve;
using Xunit;

namespace Vixen.Tests.Effects;

/// <summary>
/// Verifies Mark Collection selection behavior for the Dissolve effect.
/// </summary>
public sealed class DissolveMarkCollectionSelectionTests
{
	/// <summary>
	/// Verifies that activating Mark Collection mode selects the first available collection when no selection exists.
	/// </summary>
	[Fact]
	public void DissolveMode_MarkCollection_SelectsTheFirstCollectionWhenNoneIsSelected()
	{
		// Arrange
		var first = new MarkCollection { Id = Guid.NewGuid(), Name = "First" };
		var effect = new Dissolve
		{
			MarkCollections = new ObservableCollection<IMarkCollection> { first }
		};

		// Act
		effect.DissolveMode = DissolveMode.MarkCollection;

		// Assert
		var data = Assert.IsType<DissolveData>(effect.ModuleData);
		Assert.Equal(first.Id, data.MarkCollectionId);
	}
}
