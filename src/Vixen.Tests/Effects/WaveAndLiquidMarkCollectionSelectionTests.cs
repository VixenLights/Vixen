using System.Collections.ObjectModel;
using Vixen.Marks;
using VixenModules.App.Marks;
using VixenModules.Effect.Liquid;
using VixenModules.Effect.Wave;
using Xunit;

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
		var waveform = new Waveform
		{
			MarkCollections = new ObservableCollection<IMarkCollection> { first },
			WaveType = WaveType.DecayingSine
		};

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
		var emitter = new Emitter
		{
			MarkCollections = new ObservableCollection<IMarkCollection> { first }
		};

		// Act
		emitter.FlowControl = FlowControl.UseMarks;

		// Assert
		Assert.Equal(first.Id, emitter.MarkCollectionId);
	}

	/// <summary>
	/// Verifies that choosing Decaying Sine after enabling marks selects the first available collection.
	/// </summary>
	[Fact]
	public void Waveform_DecayingSine_SelectsTheFirstCollectionWhenMarksAreAlreadyEnabled()
	{
		// Arrange
		var first = CreateCollection("First");
		var waveform = new Waveform
		{
			MarkCollections = new ObservableCollection<IMarkCollection> { first },
			UseMarks = true
		};

		// Act
		waveform.WaveType = WaveType.DecayingSine;

		// Assert
		Assert.Equal(first.Id, waveform.MarkCollectionId);
	}

	/// <summary>
	/// Verifies that renaming a selected Mark Collection does not dirty or invalidate Wave effects.
	/// </summary>
	[Fact]
	public void MarkCollectionRename_RefreshesSelectedWaveNameWithoutDirtyingWaveEffects()
	{
		// Arrange
		var selectedCollection = CreateCollection("Original");
		var collections = new ObservableCollection<IMarkCollection> { selectedCollection };
		var selectedEffect = new TestWave { MarkCollections = collections };
		var selectedWaveform = new Waveform { WaveType = WaveType.DecayingSine };
		selectedEffect.Waves.Add(selectedWaveform);
		selectedWaveform.UseMarks = true;
		selectedEffect.SetClean();

		var unrelatedEffect = new TestWave { MarkCollections = collections };
		unrelatedEffect.Waves.Add(new Waveform());
		unrelatedEffect.SetClean();

		// Act
		selectedCollection.Name = "Renamed";

		// Assert
		Assert.Equal("Renamed", selectedWaveform.MarkCollectionName);
		Assert.False(selectedEffect.IsDirty);
		Assert.False(unrelatedEffect.IsDirty);
	}

	private static MarkCollection CreateCollection(string name)
	{
		return new MarkCollection { Id = Guid.NewGuid(), Name = name };
	}

	private sealed class TestWave : Wave
	{
		public void SetClean()
		{
			IsDirty = false;
		}
	}
}
