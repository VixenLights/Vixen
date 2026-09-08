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
		var selectedPropertyNames = new List<string>();
		selectedEffect.PropertyChanged += (_, e) => selectedPropertyNames.Add(e.PropertyName);

		var unrelatedEffect = new TestWave { MarkCollections = collections };
		unrelatedEffect.Waves.Add(new Waveform());
		unrelatedEffect.SetClean();

		// Act
		selectedCollection.Name = "Renamed";

		// Assert
		Assert.Equal("Renamed", selectedWaveform.MarkCollectionName);
		Assert.Contains(nameof(Wave.Waves), selectedPropertyNames);
		Assert.False(selectedEffect.IsDirty);
		Assert.False(unrelatedEffect.IsDirty);
	}

	/// <summary>
	/// Verifies that adding a Mark Collection refreshes a selected Wave effect without dirtying it.
	/// </summary>
	[Fact]
	public void MarkCollectionAdded_RefreshesWaveSelectorWithoutDirtyingEffect()
	{
		// Arrange
		var first = CreateCollection("First");
		var collections = new ObservableCollection<IMarkCollection> { first };
		var effect = new TestWave { MarkCollections = collections };
		var waveform = new Waveform { WaveType = WaveType.DecayingSine };
		effect.Waves.Add(waveform);
		waveform.UseMarks = true;
		effect.SetClean();
		var propertyNames = new List<string>();
		effect.PropertyChanged += (_, e) => propertyNames.Add(e.PropertyName);

		// Act
		collections.Add(CreateCollection("Second"));

		// Assert
		Assert.Contains(nameof(Wave.Waves), propertyNames);
		Assert.False(effect.IsDirty);
	}

	/// <summary>
	/// Verifies that removing a Mark Collection refreshes a Wave selector without dirtying an unrelated effect.
	/// </summary>
	[Fact]
	public void MarkCollectionRemoved_RefreshesWaveSelectorWithoutDirtyingUnrelatedEffect()
	{
		// Arrange
		var removable = CreateCollection("Remove");
		var retained = CreateCollection("Retain");
		var collections = new ObservableCollection<IMarkCollection> { removable, retained };
		var effect = new TestWave { MarkCollections = collections };
		var waveform = new Waveform { WaveType = WaveType.DecayingSine };
		effect.Waves.Add(waveform);
		waveform.UseMarks = true;
		effect.SetClean();
		var propertyNames = new List<string>();
		effect.PropertyChanged += (_, e) => propertyNames.Add(e.PropertyName);

		// Act
		collections.Remove(retained);

		// Assert
		Assert.Contains(nameof(Wave.Waves), propertyNames);
		Assert.False(effect.IsDirty);
	}

	/// <summary>
	/// Verifies that renaming a selected Mark Collection does not dirty or invalidate Liquid effects.
	/// </summary>
	[Fact]
	public void MarkCollectionRename_RefreshesSelectedEmitterNameWithoutDirtyingLiquidEffects()
	{
		// Arrange
		var selectedCollection = CreateCollection("Original");
		var collections = new ObservableCollection<IMarkCollection> { selectedCollection };
		var selectedEffect = new TestLiquid { MarkCollections = collections };
		var selectedEmitter = new Emitter();
		selectedEffect.EmitterList.Add(selectedEmitter);
		selectedEmitter.FlowControl = FlowControl.UseMarks;
		selectedEffect.SetClean();
		var selectedPropertyNames = new List<string>();
		selectedEffect.PropertyChanged += (_, e) => selectedPropertyNames.Add(e.PropertyName);

		var unrelatedEffect = new TestLiquid { MarkCollections = collections };
		unrelatedEffect.EmitterList.Add(new Emitter());
		unrelatedEffect.SetClean();

		// Act
		selectedCollection.Name = "Renamed";

		// Assert
		Assert.Equal("Renamed", selectedEmitter.MarkCollectionName);
		Assert.Contains(nameof(LiquidEffect.EmitterList), selectedPropertyNames);
		Assert.False(selectedEffect.IsDirty);
		Assert.False(unrelatedEffect.IsDirty);
	}

	/// <summary>
	/// Verifies that adding and removing Mark Collections refreshes the Liquid emitter list.
	/// </summary>
	[Fact]
	public void MarkCollectionChanges_RefreshLiquidEmitterList()
	{
		// Arrange
		var first = CreateCollection("First");
		var added = CreateCollection("Added");
		var collections = new ObservableCollection<IMarkCollection> { first };
		var effect = new TestLiquid { MarkCollections = collections };
		var emitter = new Emitter();
		effect.EmitterList.Add(emitter);
		var propertyNames = new List<string>();
		effect.PropertyChanged += (_, e) => propertyNames.Add(e.PropertyName);

		// Act
		collections.Add(added);
		collections.Remove(added);

		// Assert
		Assert.Equal(["First"], emitter.MarkNameCollection);
		Assert.Equal(2, propertyNames.Count(name => name == nameof(LiquidEffect.EmitterList)));
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

	private sealed class TestLiquid : LiquidEffect
	{
		public void SetClean()
		{
			IsDirty = false;
		}
	}
}
