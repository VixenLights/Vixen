using System.Collections.ObjectModel;
using Moq;
using Vixen.Intent;
using Vixen.Marks;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.App.Marks;
using VixenModules.Effect.Effect;
using Xunit;

namespace Vixen.Tests.Effects;

public sealed class BaseEffectMarkCollectionSelectionTests
{
	[Fact]
	public void MarkCollectionsChanged_NormalizesBeforeInvokingEffectHook()
	{
		// Arrange
		var first = new MarkCollection { Id = Guid.NewGuid(), Name = "First" };
		var effect = new TestEffect();
		effect.ActivateSelection();

		// Act
		effect.MarkCollections = new ObservableCollection<IMarkCollection> { first };

		// Assert
		Assert.Equal(first.Id, effect.SelectionIdObservedByChangeHook);
	}

	[Fact]
	public void ModeActivation_NormalizesAnEmptySelection()
	{
		// Arrange
		var first = new MarkCollection { Id = Guid.NewGuid(), Name = "First" };
		var effect = new TestEffect { MarkCollections = new ObservableCollection<IMarkCollection> { first } };

		// Act
		effect.ActivateSelection();

		// Assert
		Assert.Equal(first.Id, effect.SelectionIdObservedByChangeHook);
	}

	[Fact]
	public void MarkCollectionsAdded_RaisesMarkCollectionIdPropertyChanged()
	{
		// Arrange
		var effect = new TestEffect { MarkCollections = [] };
		var propertyNames = CapturePropertyChanges(effect);

		// Act
		effect.MarkCollections.Add(new MarkCollection { Id = Guid.NewGuid(), Name = "Added" });

		// Assert
		Assert.Contains("MarkCollectionId", propertyNames);
	}

	[Fact]
	public void MarkCollectionRenamed_RaisesMarkCollectionIdPropertyChangedWithoutChangingSelectionOrDirtyState()
	{
		// Arrange
		var markCollection = new MarkCollection { Id = Guid.NewGuid(), Name = "Original" };
		var effect = new TestEffect { MarkCollections = new ObservableCollection<IMarkCollection> { markCollection } };
		effect.ActivateSelection();
		effect.SetClean();
		var selectedId = effect.MarkCollectionId;
		var propertyNames = CapturePropertyChanges(effect);

		// Act
		markCollection.Name = "Renamed";

		// Assert
		Assert.Contains("MarkCollectionId", propertyNames);
		Assert.Equal(selectedId, effect.MarkCollectionId);
		Assert.False(effect.IsDirty);
	}

	[Fact]
	public void MarkCollectionPropertyChanged_NullOrEmptyNameRefreshesSelectorAndOtherPropertiesDoNot()
	{
		// Arrange
		var markCollection = new TestMarkCollection();
		var effect = new TestEffect { MarkCollections = new ObservableCollection<IMarkCollection> { markCollection } };
		var propertyNames = CapturePropertyChanges(effect);

		// Act
		markCollection.RaisePropertyChanged(null);
		markCollection.RaisePropertyChanged(string.Empty);
		markCollection.RaisePropertyChanged(nameof(IMarkCollection.Locked));

		// Assert
		Assert.Equal(["MarkCollectionId", "MarkCollectionId"], propertyNames);
	}

	[Fact]
	public void RemovedMarkCollection_DoesNotRaiseSelectorRefreshAfterRename()
	{
		// Arrange
		var markCollection = new MarkCollection { Id = Guid.NewGuid(), Name = "Removed" };
		var collections = new ObservableCollection<IMarkCollection> { markCollection };
		var effect = new TestEffect { MarkCollections = collections };
		var propertyNames = CapturePropertyChanges(effect);

		// Act
		collections.Remove(markCollection);
		propertyNames.Clear();
		markCollection.Name = "No longer subscribed";

		// Assert
		Assert.Empty(propertyNames);
	}

	[Fact]
	public void ReplacedMarkCollection_DoesNotRaiseSelectorRefreshWhileReplacementDoes()
	{
		// Arrange
		var oldMarkCollection = new MarkCollection { Id = Guid.NewGuid(), Name = "Old" };
		var replacementMarkCollection = new MarkCollection { Id = Guid.NewGuid(), Name = "Replacement" };
		var effect = new TestEffect { MarkCollections = new ObservableCollection<IMarkCollection> { oldMarkCollection } };
		effect.MarkCollections = new ObservableCollection<IMarkCollection> { replacementMarkCollection };
		var propertyNames = CapturePropertyChanges(effect);

		// Act
		oldMarkCollection.Name = "Old renamed";
		replacementMarkCollection.Name = "Replacement renamed";

		// Assert
		Assert.Equal(["MarkCollectionId"], propertyNames);
	}

	[Fact]
	public void DisposedEffect_DoesNotRaiseSelectorRefreshAfterCollectionRename()
	{
		// Arrange
		var markCollection = new MarkCollection { Id = Guid.NewGuid(), Name = "Disposed" };
		var effect = new TestEffect { MarkCollections = new ObservableCollection<IMarkCollection> { markCollection } };
		var propertyNames = CapturePropertyChanges(effect);

		try
		{
			// Act
			effect.Dispose();
			propertyNames.Clear();
			markCollection.Name = "No longer subscribed";

			// Assert
			Assert.Empty(propertyNames);
		}
		finally
		{
			effect.Dispose();
		}
	}

	private static List<string> CapturePropertyChanges(TestEffect effect)
	{
		var propertyNames = new List<string>();
		effect.PropertyChanged += (_, e) => propertyNames.Add(e.PropertyName);
		return propertyNames;
	}

	private sealed class TestEffect : BaseEffect
	{
		private readonly TestEffectData _data = new();
		private readonly TestSelection _selection = new();

		public TestEffect()
		{
			ModuleData = _data;
			Descriptor = Mock.Of<IEffectModuleDescriptor>(descriptor => !descriptor.SupportsMarks);
		}

		public Guid SelectionIdObservedByChangeHook { get; private set; }
		public Guid MarkCollectionId => _selection.MarkCollectionId;

		public void ActivateSelection()
		{
			_selection.IsActive = true;
			ActivateMarkCollectionSelections();
		}

		public void SetClean()
		{
			IsDirty = false;
		}

		protected override EffectTypeModuleData EffectModuleData => _data;

		protected override IEnumerable<IMarkCollectionSelection> GetMarkCollectionSelections()
		{
			return [_selection];
		}

		protected override void MarkCollectionsChangedCore()
		{
			SelectionIdObservedByChangeHook = _selection.MarkCollectionId;
		}

		protected override void TargetNodesChanged()
		{
		}

		protected override void _PreRender(CancellationTokenSource cancellationToken)
		{
		}

		protected override EffectIntents _Render()
		{
			return new EffectIntents();
		}
	}

	private sealed class TestMarkCollection : MarkCollection
	{
		public void RaisePropertyChanged(string propertyName)
		{
			OnPropertyChanged(propertyName);
		}
	}

	private sealed class TestSelection : IMarkCollectionSelection
	{
		public bool IsActive { get; set; }
		public Guid MarkCollectionId { get; set; }
		public MarkCollectionType? PreferredCollectionType => null;
		public bool AllowsFirstCollectionFallback => true;
	}

	private sealed class TestEffectData : EffectTypeModuleData
	{
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			return new TestEffectData { TargetPositioning = TargetPositioning };
		}
	}
}
