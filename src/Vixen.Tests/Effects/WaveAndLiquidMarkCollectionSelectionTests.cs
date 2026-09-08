using System.Collections.ObjectModel;
using Vixen.Marks;
using VixenModules.App.Marks;
using VixenModules.Effect.Liquid;
using VixenModules.Effect.Wave;
using Xunit;
using LiquidEffect = VixenModules.Effect.Liquid.Liquid;

namespace Vixen.Tests.Effects;

/// <summary>
/// Verifies Mark Collection selection behavior for Wave and Liquid child models.
/// </summary>
public sealed class WaveAndLiquidMarkCollectionSelectionTests
{
	/// <summary>
	/// Verifies that enabling marks on a Decaying Sine waveform selects the first available collection.
	/// </summary>
	[Fact]
	public void Waveform_UseMarks_SelectsTheFirstCollectionWhenNoneIsSelected()
	{
		// Arrange
		var first = CreateCollection("First");
		var effect = new Wave
		{
			MarkCollections = new ObservableCollection<IMarkCollection> { first }
		};
		var waveform = new Waveform { WaveType = WaveType.DecayingSine };
		effect.Waves.Add(waveform);

		// Act
		waveform.UseMarks = true;

		// Assert
		Assert.Equal(first.Id, waveform.MarkCollectionId);
	}

	/// <summary>
	/// Verifies that enabling mark-controlled flow selects the first available collection.
	/// </summary>
	[Fact]
	public void Emitter_UseMarks_SelectsTheFirstCollectionWhenNoneIsSelected()
	{
		// Arrange
		var first = CreateCollection("First");
		var effect = new LiquidEffect
		{
			MarkCollections = new ObservableCollection<IMarkCollection> { first }
		};
		var emitter = new Emitter();
		effect.EmitterList.Add(emitter);

		// Act
		emitter.FlowControl = FlowControl.UseMarks;

		// Assert
		Assert.Equal(first.Id, emitter.MarkCollectionId);
	}

	private static MarkCollection CreateCollection(string name)
	{
		return new MarkCollection { Id = Guid.NewGuid(), Name = name };
	}
}
