using System.Collections.ObjectModel;
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

	private sealed class TestEffect : BaseEffect
	{
		private readonly TestEffectData _data = new();
		private readonly TestSelection _selection = new();

		public Guid SelectionIdObservedByChangeHook { get; private set; }

		public void ActivateSelection()
		{
			_selection.IsActive = true;
			ActivateMarkCollectionSelections();
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
