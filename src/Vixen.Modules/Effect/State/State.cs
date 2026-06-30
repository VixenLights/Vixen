using System.ComponentModel;
using System.Collections.Specialized;
using System.Drawing;
using Vixen.Attributes;
using Vixen.Marks;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using Vixen.TypeConverters;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.Color;
using VixenModules.Property.State;

namespace VixenModules.Effect.State
{
	public sealed class State: BaseEffect
	{
		internal const string AllStateItemsLabel = "<All>";
		internal const string NoneStateItemLabel = "<None>";
		internal const string NoStateDefinitionsLabel = "<No States Available>";
		internal const string NoStateItemsLabel = "<No State Items Available>";

		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private StateData _data;
		private EffectIntents _elementData = new();
		private IReadOnlyList<DiscoveredStateDefinition> _stateDefinitions = [];
		private CustomStateItemCollection _customStateItems = null!;
		private const string BaseVisualRepresentationText = "State";

		public State()
		{
			_data = new StateData();
			UpdateCustomStateItemModel();
			SetRenderSourceBrowsables();
		}

		#region Overrides of EffectModuleInstanceBase

		protected override void TargetNodesChanged()
		{
			CheckForInvalidColorData();
			RefreshStateDefinitionSelection();
		}

		protected override void _PreRender(CancellationTokenSource? cancellationToken = null)
		{
			_elementData = new EffectIntents();
			RenderNodes();
		}

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		#endregion

		#region Overrides of BaseEffect

		protected override EffectTypeModuleData EffectModuleData => _data;

		#endregion

		#region Visual Representation

		/// <inheritdoc />
		public override bool ForceGenerateVisualRepresentation => _data.ShowEffectVisual;

		/// <inheritdoc />
		public override void GenerateVisualRepresentation(Graphics g, Rectangle clipRectangle)
		{
			try
			{
				var text = GetVisualRepresentationText();
				var adjustedFont = Vixen.Common.Graphics.GetAdjustedFont(
					g,
					text,
					clipRectangle,
					"Vixen.Fonts.DigitalDream.ttf",
					48);

				using (var stringBrush = new SolidBrush(Color.White))
				{
					using (var backgroundBrush = new SolidBrush(Color.DarkGray))
					{
						g.FillRectangle(backgroundBrush, clipRectangle);
					}
				
					g.DrawString(text, adjustedFont, stringBrush, clipRectangle.X + 2, 2);
				}
			}
			catch (Exception exception)
			{
				Logging.Error(exception, "Exception rendering the visualization for the State effect.");
			}
		}

		internal string GetVisualRepresentationText()
		{
			var text = $"{BaseVisualRepresentationText} - {StateDefinition}";
			switch (RenderSource)
			{
				case StateRenderSource.Custom:
					return $"{text} - Custom";
				case StateRenderSource.StateItem:
					return $"{text} - {StateItem}";
				case StateRenderSource.MarkCollection:
					return $"{text} - Marks";
				default:
					return text;
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the selected State definition display label.
		/// </summary>
		/// <value>The selected State definition display label.</value>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"State")]
		[ProviderDescription(@"StateDefinition")]
		[TypeConverter(typeof(StateDefinitionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(0)]
		[RefreshProperties(RefreshProperties.All)]
		public string StateDefinition
		{
			get
			{
				var selectedDefinition = GetSelectedStateDefinition();
				if (selectedDefinition != null)
				{
					return selectedDefinition.DisplayLabel;
				}

				return _data.SelectedStateDefinitionId == Guid.Empty
					? NoStateDefinitionsLabel
					: CreateMissingStateDefinitionLabel(_data.SelectedStateDefinitionId);
			}
			set
			{
				var selectedDefinition = GetDiscoveredStateDefinitions()
					.FirstOrDefault(definition => definition.DisplayLabel.Equals(value, StringComparison.Ordinal));
				if (selectedDefinition == null)
				{
					return;
				}

				var previousStateDefinitionId = _data.SelectedStateDefinitionId;
				var previousStateItemName = GetSelectedStateItemName();
				SelectStateDefinition(selectedDefinition, previousStateItemName);
				if (previousStateDefinitionId != selectedDefinition.StateDefinitionId)
				{
					ClearCustomStateItems();
				}

				IsDirty = true;
				OnPropertyChanged();
				OnPropertyChanged(nameof(StateItem));
				TypeDescriptor.Refresh(this);
			}
		}

		/// <summary>
		/// Gets or sets the selected State item display label.
		/// </summary>
		/// <value>The selected State item display label.</value>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"StateItem")]
		[ProviderDescription(@"StateItem")]
		[TypeConverter(typeof(StateItemNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(6)]
		public string StateItem
		{
			get
			{
				var selectedDefinition = GetSelectedStateDefinition();
				if (selectedDefinition == null || !GetStateItems(selectedDefinition).Any())
				{
					return NoStateItemsLabel;
				}

				if (_data.SelectedStateItemId == Guid.Empty)
				{
					return AllStateItemsLabel;
				}

				var selectedItem = GetSelectedStateItem(selectedDefinition);
				return selectedItem?.Name ?? CreateMissingStateItemLabel(_data.SelectedStateItemId);
			}
			set
			{
				if (value.Equals(AllStateItemsLabel, StringComparison.Ordinal))
				{
					_data.SelectedStateItemId = Guid.Empty;
					IsDirty = true;
					OnPropertyChanged();
					return;
				}

				var selectedDefinition = GetSelectedStateDefinition();
				if (selectedDefinition == null)
				{
					return;
				}

				var selectedItem = GetStateItems(selectedDefinition)
					.FirstOrDefault(item => item.Name.Equals(value, StringComparison.Ordinal));
				if (selectedItem == null)
				{
					return;
				}

				_data.SelectedStateItemId = selectedItem.Id;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the selected Mark Collection display name.
		/// </summary>
		/// <value>The selected Mark Collection display name.</value>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"MarkCollection")]
		[ProviderDescription(@"MarkCollection")]
		[TypeConverter(typeof(IMarkCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(6)]
		public string MarkCollectionId
		{
			get
			{
				var c = MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
				return c?.Name ?? String.Empty;
			}
			set
			{
				IMarkCollection? newMarkCollection = null;
				var id = Guid.Empty;
				if (!string.IsNullOrEmpty(value))
				{
					newMarkCollection = MarkCollections.FirstOrDefault(
						collection => collection.Name.Equals(value, StringComparison.Ordinal));
					if (newMarkCollection == null)
					{
						return;
					}

					id = newMarkCollection.Id;
				}

				if (!id.Equals(_data.MarkCollectionId))
				{
					var oldMarkCollection = MarkCollections.FirstOrDefault(x => x.Id.Equals(_data.MarkCollectionId));
					RemoveMarkCollectionListeners(oldMarkCollection);
					_data.MarkCollectionId = id;
					AddMarkCollectionListeners(newMarkCollection);
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the source used to determine active State items.
		/// </summary>
		/// <value>One of the enumeration values that specifies the State item render source.</value>
		[Value]
		[ProviderCategory("Config", 2)]
		[ProviderDisplayName(@"StateRenderSource")]
		[ProviderDescription(@"StateRenderSource")]
		[PropertyOrder(5)]
		[RefreshProperties(RefreshProperties.All)]
		public StateRenderSource RenderSource
		{
			get
			{
				return _data.RenderSource;

			}
			set
			{
				if (_data.RenderSource != value)
				{
					_data.RenderSource = value;
					SetRenderSourceBrowsables();
					IsDirty = true;
					OnPropertyChanged();
					if (_data.RenderSource == StateRenderSource.MarkCollection && _data.MarkCollectionId == Guid.Empty)
					{
						var stateMarkCollection = GetFirstStateMarkCollection();
						if (stateMarkCollection != null)
						{
							MarkCollectionId = stateMarkCollection.Name;
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the playback behavior used when multiple State item names are active.
		/// </summary>
		/// <value>One of the enumeration values that specifies the playback behavior.</value>
		[Value]
		[ProviderCategory("Config", 2)]
		[ProviderDisplayName(@"StatePlaybackMode")]
		[ProviderDescription(@"StatePlaybackMode")]
		[PropertyOrder(1)]
		[RefreshProperties(RefreshProperties.All)]
		public PlaybackMode PlaybackMode
		{
			get => _data.PlaybackMode;
			set
			{
				if (_data.PlaybackMode == value)
				{
					return;
				}

				_data.PlaybackMode = value;
				if (_data.PlaybackMode == PlaybackMode.Default)
				{
					NormalizeCustomStateItemsForDefault();
				}

				SetRenderSourceBrowsables();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the number of times the active State item sequence repeats in Iterate playback mode.
		/// </summary>
		/// <value>The number of Iterate playback repetitions. The default is 1.</value>
		[Value]
		[ProviderCategory("Config", 2)]
		[ProviderDisplayName(@"Iterations")]
		[ProviderDescription(@"Iterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(StateData.MinIterations, StateData.MaxIterations, 1)]
		[PropertyOrder(4)]
		public int Iterations
		{
			get => _data.Iterations;
			set
			{
				var normalizedValue = StateData.NormalizeIterations(value);
				if (_data.Iterations == normalizedValue)
				{
					return;
				}

				_data.Iterations = normalizedValue;
				IsDirty = true;
				OnPropertyChanged();
			}
		}
		
		/// <summary>
		/// Gets or sets the playback behavior used when multiple State item names are active.
		/// </summary>
		/// <value>One of the enumeration values that specifies the playback behavior.</value>
		[Value]
		[ProviderCategory("EffectVisual", 10)]
		[ProviderDisplayName(@"EffectVisual")]
		[ProviderDescription(@"EffectVisual.")]
		[PropertyOrder(3)]
		public bool ShowEffectVisual
		{
			get => _data.ShowEffectVisual;
			set
			{
				if (_data.ShowEffectVisual == value)
				{
					return;
				}

				_data.ShowEffectVisual = value;
				IsDirty = true;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ForceGenerateVisualRepresentation));
			}
		}

		/// <summary>
		/// Gets or sets the custom State item rows.
		/// </summary>
		/// <value>The custom State item rows.</value>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"CustomStateItems")]
		[ProviderDescription(@"CustomStateItems")]
		[PropertyOrder(6)]
		public CustomStateItemCollection CustomStateItems
		{
			get => _customStateItems;
			set => SetCustomStateItemModel(value, true);
		}

		#endregion
		
		private void CheckForInvalidColorData()
		{
			// initialize the color handling
			GetValidColors();
		}

		private void RenderNodes()
		{
			var selectedDefinition = GetSelectedStateDefinition();
			var intervals = CreateRenderIntervals(selectedDefinition?.Definition);
			var targetScopeNodes = GetTargetScopeNodes()
				.GroupBy(node => node.Id)
				.ToDictionary(group => group.Key, group => group.First());
			var segments = intervals.SelectMany(interval => CreateRenderSegments(interval, targetScopeNodes));

			foreach (var segment in StateRenderSegmentCoalescer.Coalesce(segments))
			{
				RenderSegment(segment);
			}
		}

		private IReadOnlyList<StateRenderInterval> CreateRenderIntervals(StateDefinitionData? selectedDefinition)
		{
			if (RenderSource == StateRenderSource.Custom)
			{
				return StateRenderPlanner.CreateCustomIntervals(
					selectedDefinition,
					_data.CustomStateItems ?? [],
					PlaybackMode,
					Iterations,
					TimeSpan);
			}

			if (RenderSource == StateRenderSource.MarkCollection)
			{
				var markCollection = GetSelectedMarkCollection();
				if (markCollection == null)
				{
					return [];
				}

				var marks = markCollection.MarksInclusiveOfTime(StartTime, StartTime + TimeSpan);
				return StateRenderPlanner.CreateMarkCollectionIntervals(
					selectedDefinition,
					marks,
					PlaybackMode,
					Iterations,
					StartTime,
					TimeSpan);
			}

			return StateRenderPlanner.CreateStateItemIntervals(
				selectedDefinition,
				_data.SelectedStateItemId,
				PlaybackMode,
				Iterations,
				TimeSpan);
		}

		private IEnumerable<StateRenderSegment> CreateRenderSegments(
			StateRenderInterval interval,
			IReadOnlyDictionary<Guid, IElementNode> targetScopeNodes)
		{
			if (interval.Duration <= TimeSpan.Zero)
			{
				yield break;
			}

			foreach (var elementNodeId in interval.Item.ElementNodeIds)
			{
				if (!targetScopeNodes.TryGetValue(elementNodeId, out var assignedNode))
				{
					continue;
				}

				foreach (var leafNode in assignedNode.GetLeafEnumerator())
				{
					var element = leafNode.Element;
					if (element == null)
					{
						continue;
					}

					var resolvedColor = ResolveStateItemColor(leafNode, interval.ColorOverride ?? interval.Item.Color);
					if (resolvedColor == null)
					{
						continue;
					}

					yield return new StateRenderSegment(
						interval.Item.Id,
						leafNode,
						element.Id,
						resolvedColor.Value,
						interval.Start,
						interval.Duration);
				}
			}
		}

		private void RenderSegment(StateRenderSegment segment)
		{
			var intent = CreateIntent(segment.LeafNode, segment.ResolvedColor, 1.0, segment.Duration);
			_elementData.AddIntentForElement(segment.ElementId, intent, segment.Start);
		}

		private IEnumerable<IElementNode> GetTargetScopeNodes()
		{
			foreach (var targetNode in TargetNodes.Where(node => node != null))
			{
				foreach (var scopedNode in EnumerateTargetScope(targetNode))
				{
					yield return scopedNode;
				}
			}
		}

		private static IEnumerable<IElementNode> EnumerateTargetScope(IElementNode node)
		{
			yield return node;

			foreach (var child in node.Children.Where(child => child != null))
			{
				foreach (var descendant in EnumerateTargetScope(child))
				{
					yield return descendant;
				}
			}
		}

		private static Color? ResolveStateItemColor(IElementNode leafNode, Color stateItemColor)
		{
			if (!ColorModule.isElementNodeDiscreteColored(leafNode))
			{
				return stateItemColor;
			}

			var validColors = ColorModule.getValidColorsForElementNode(leafNode, false).ToList();
			if (validColors.Count == 0)
			{
				return null;
			}

			return validColors.Contains(stateItemColor)
				? stateItemColor
				: validColors[0];
		}
		
		#region Overrides of EffectModuleInstanceBase

		/// <inheritdoc />
		protected override void MarkCollectionsChanged()
		{
			if (RenderSource == StateRenderSource.MarkCollection)
			{
				var markCollection = GetSelectedMarkCollection();
				InitializeMarkCollectionListeners(markCollection);
			}
		}

		/// <inheritdoc />
		protected override void MarkCollectionsRemoved(IList<IMarkCollection> removedCollections)
		{
			var markCollection = removedCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
			if (markCollection != null)
			{
				//Our collection is gone!!!!
				RemoveMarkCollectionListeners(markCollection);
				MarkCollectionId = String.Empty;
			}
		}

		#endregion

		#region Discovery

		/// <summary>
		/// Gets the available State definition option labels.
		/// </summary>
		/// <returns>The available State definition option labels.</returns>
		public List<string> GetStateDefinitionOptions()
		{
			var options = GetDiscoveredStateDefinitions()
				.Select(definition => definition.DisplayLabel)
				.ToList();

			if (_data.SelectedStateDefinitionId != Guid.Empty &&
				!_stateDefinitions.Any(definition => definition.StateDefinitionId == _data.SelectedStateDefinitionId))
			{
				options.Insert(0, CreateMissingStateDefinitionLabel(_data.SelectedStateDefinitionId));
			}

			return options.Count == 0 ? [NoStateDefinitionsLabel] : options;
		}

		/// <summary>
		/// Gets the available State item option labels.
		/// </summary>
		/// <returns>The available State item option labels.</returns>
		public List<string> GetStateItemOptions()
		{
			var selectedDefinition = GetSelectedStateDefinition();
			if (selectedDefinition == null)
			{
				return [NoStateItemsLabel];
			}

			var items = GetStateItems(selectedDefinition);
			if (items.Count == 0)
			{
				return [NoStateItemsLabel];
			}

			var options = new List<string> { AllStateItemsLabel };
			if (_data.SelectedStateItemId != Guid.Empty &&
				!items.Any(item => item.Id == _data.SelectedStateItemId))
			{
				options.Add(CreateMissingStateItemLabel(_data.SelectedStateItemId));
			}

			options.AddRange(GetUniqueStateItemNames(items));

			return options;
		}

		/// <summary>
		/// Gets the available custom State item option labels for the specified row.
		/// </summary>
		/// <param name="customStateItem">The custom State item row requesting options.</param>
		/// <returns>The available custom State item option labels.</returns>
		public List<string> GetCustomStateItemOptions(CustomStateItem customStateItem)
		{
			var selectedDefinition = GetSelectedStateDefinition();
			if (selectedDefinition == null)
			{
				return [NoStateItemsLabel];
			}

			var items = GetStateItems(selectedDefinition);
			if (items.Count == 0)
			{
				return [NoStateItemsLabel];
			}

			var labelsById = CreateCustomStateItemLabels(items);
			var options = PlaybackMode == PlaybackMode.Iterate
				? labelsById.Values.Prepend(NoneStateItemLabel).ToList()
				: labelsById
					.Where(pair => pair.Key == customStateItem.StateItemId || !IsCustomStateItemSelected(pair.Key, customStateItem))
					.Select(pair => pair.Value)
					.ToList();
			if (customStateItem.StateItemId != Guid.Empty && !labelsById.ContainsKey(customStateItem.StateItemId))
			{
				var index = PlaybackMode == PlaybackMode.Iterate ? 1 : 0;
				options.Insert(index, CreateMissingStateItemLabel(customStateItem.StateItemId));
			}

			return options;
		}

		/// <summary>
		/// Gets the display label for the specified custom State item row.
		/// </summary>
		/// <param name="customStateItem">The custom State item row.</param>
		/// <returns>The custom State item display label.</returns>
		public string GetCustomStateItemLabel(CustomStateItem customStateItem)
		{
			var selectedDefinition = GetSelectedStateDefinition();
			if (selectedDefinition == null || !GetStateItems(selectedDefinition).Any())
			{
				return NoStateItemsLabel;
			}

			if (customStateItem.StateItemId == Guid.Empty)
			{
				return PlaybackMode == PlaybackMode.Iterate ? NoneStateItemLabel : NoStateItemsLabel;
			}

			var labelsById = CreateCustomStateItemLabels(GetStateItems(selectedDefinition));
			return labelsById.TryGetValue(customStateItem.StateItemId, out var label)
				? label
				: CreateMissingStateItemLabel(customStateItem.StateItemId);
		}

		/// <summary>
		/// Selects a State item for the specified custom State item row.
		/// </summary>
		/// <param name="customStateItem">The custom State item row to update.</param>
		/// <param name="label">The selected State item display label.</param>
		public void SelectCustomStateItem(CustomStateItem customStateItem, string label)
		{
			if (PlaybackMode == PlaybackMode.Iterate && label.Equals(NoneStateItemLabel, StringComparison.Ordinal))
			{
				customStateItem.StateItemId = Guid.Empty;
				return;
			}

			var selectedDefinition = GetSelectedStateDefinition();
			if (selectedDefinition == null)
			{
				return;
			}

			var items = GetStateItems(selectedDefinition);
			var stateItem = CreateCustomStateItemLabels(items)
				.FirstOrDefault(pair => pair.Value.Equals(label, StringComparison.Ordinal));
			if (stateItem.Key == Guid.Empty)
			{
				return;
			}

			var selectedItem = items.FirstOrDefault(item => item.Id == stateItem.Key);
			if (selectedItem == null)
			{
				return;
			}

			customStateItem.StateItemId = selectedItem.Id;
			customStateItem.Color = selectedItem.Color;
		}

		/// <summary>
		/// Gets the discrete colors supported by the selected State item's assigned elements.
		/// </summary>
		/// <param name="customStateItem">The custom State item row.</param>
		/// <returns>The valid discrete colors, or an empty set when full color editing is allowed.</returns>
		public HashSet<Color> GetCustomStateItemValidColors(CustomStateItem customStateItem)
		{
			var selectedDefinition = GetSelectedStateDefinition();
			if (selectedDefinition == null || customStateItem.StateItemId == Guid.Empty)
			{
				return [];
			}

			var selectedItem = GetStateItems(selectedDefinition)
				.FirstOrDefault(item => item.Id == customStateItem.StateItemId);
			if (selectedItem == null)
			{
				return [];
			}

			return GetAssignedTargetNodes(selectedItem)
				.SelectMany(node => ColorModule.getValidColorsForElementNode(node, true))
				.ToHashSet();
		}

		private bool IsCustomStateItemSelected(Guid stateItemId, CustomStateItem currentItem)
		{
			return CustomStateItems.Any(item => !ReferenceEquals(item, currentItem) && item.StateItemId == stateItemId);
		}

		private IEnumerable<IElementNode> GetAssignedTargetNodes(StateItemData stateItem)
		{
			var targetScopeNodes = GetTargetScopeNodes()
				.GroupBy(node => node.Id)
				.ToDictionary(group => group.Key, group => group.First());

			foreach (var elementNodeId in stateItem.ElementNodeIds ?? [])
			{
				if (targetScopeNodes.TryGetValue(elementNodeId, out var assignedNode))
				{
					yield return assignedNode;
				}
			}
		}

		private IReadOnlyList<DiscoveredStateDefinition> GetDiscoveredStateDefinitions()
		{
			_stateDefinitions = StateDefinitionDiscovery.Discover(TargetNodes ?? []);
			return _stateDefinitions;
		}

		private DiscoveredStateDefinition? GetSelectedStateDefinition()
		{
			return GetDiscoveredStateDefinitions()
				.FirstOrDefault(definition => definition.StateDefinitionId == _data.SelectedStateDefinitionId);
		}

		private static IReadOnlyList<StateItemData> GetStateItems(DiscoveredStateDefinition definition)
		{
			return definition.Definition.Items ?? [];
		}

		private StateItemData? GetSelectedStateItem(DiscoveredStateDefinition definition)
		{
			return GetStateItems(definition)
				.FirstOrDefault(item => item.Id == _data.SelectedStateItemId);
		}

		private string? GetSelectedStateItemName()
		{
			var selectedDefinition = GetSelectedStateDefinition();
			if (selectedDefinition == null || _data.SelectedStateItemId == Guid.Empty)
			{
				return null;
			}

			return GetSelectedStateItem(selectedDefinition)?.Name;
		}

		private void RefreshStateDefinitionSelection()
		{
			var definitions = GetDiscoveredStateDefinitions();
			if (_data.SelectedStateDefinitionId == Guid.Empty)
			{
				var firstDefinition = definitions.FirstOrDefault();
				if (firstDefinition != null)
				{
					SelectStateDefinition(firstDefinition, null);
					IsDirty = true;
				}
			}
			else
			{
				var selectedDefinition = definitions.FirstOrDefault(
					definition => definition.StateDefinitionId == _data.SelectedStateDefinitionId);
				if (selectedDefinition != null)
				{
					_data.StatePropertyId = selectedDefinition.StatePropertyId;
					_data.StateOwnerElementId = selectedDefinition.OwnerElementId;
				}
			}

			OnPropertyChanged(nameof(StateDefinition));
			OnPropertyChanged(nameof(StateItem));
			TypeDescriptor.Refresh(this);
		}

		private void SelectStateDefinition(DiscoveredStateDefinition selectedDefinition, string? previousStateItemName)
		{
			_data.SelectedStateDefinitionId = selectedDefinition.StateDefinitionId;
			_data.StatePropertyId = selectedDefinition.StatePropertyId;
			_data.StateOwnerElementId = selectedDefinition.OwnerElementId;
			SelectStateItemForDefinition(selectedDefinition, previousStateItemName);
		}

		private void SelectStateItemForDefinition(
			DiscoveredStateDefinition selectedDefinition,
			string? previousStateItemName)
		{
			var items = GetStateItems(selectedDefinition);
			if (items.Count == 0)
			{
				return;
			}

			var matchingPreviousName = previousStateItemName == null
				? null
				: items.FirstOrDefault(item => item.Name.Equals(previousStateItemName, StringComparison.Ordinal));
			_data.SelectedStateItemId = matchingPreviousName?.Id ?? Guid.Empty;
		}

		private static IReadOnlyList<string> GetUniqueStateItemNames(IEnumerable<StateItemData> items)
		{
			var names = new List<string>();
			var knownNames = new HashSet<string>(StringComparer.Ordinal);

			foreach (var item in items)
			{
				if (knownNames.Add(item.Name))
				{
					names.Add(item.Name);
				}
			}

			return names;
		}

		private IReadOnlyDictionary<Guid, string> CreateCustomStateItemLabels(IReadOnlyList<StateItemData> items)
		{
			var elementNamesById = GetTargetNodeNamesById();
			var duplicateNames = items
				.GroupBy(item => item.Name, StringComparer.Ordinal)
				.Where(group => group.Count() > 1)
				.Select(group => group.Key)
				.ToHashSet(StringComparer.Ordinal);
			var proposedLabels = new Dictionary<Guid, string>();

			foreach (var item in items)
			{
				if (!duplicateNames.Contains(item.Name))
				{
					proposedLabels[item.Id] = item.Name;
					continue;
				}

				proposedLabels[item.Id] = $"{item.Name} ({CreateAssignmentSummary(item, elementNamesById)})";
			}

			proposedLabels = AddOrdinalToDuplicateLabels(proposedLabels);
			return AddShortIdToDuplicateLabels(proposedLabels);
		}

		private IReadOnlyDictionary<Guid, string> GetTargetNodeNamesById()
		{
			var namesById = new Dictionary<Guid, string>();
			foreach (var node in (TargetNodes ?? []).Where(node => node != null).SelectMany(TraverseTargetNode))
			{
				namesById.TryAdd(node.Id, node.Name);
			}

			return namesById;
		}

		private static IEnumerable<IElementNode> TraverseTargetNode(IElementNode node)
		{
			yield return node;

			foreach (var child in node.Children.Where(child => child != null))
			{
				foreach (var descendant in TraverseTargetNode(child))
				{
					yield return descendant;
				}
			}
		}

		private static string CreateAssignmentSummary(StateItemData item, IReadOnlyDictionary<Guid, string> elementNamesById)
		{
			var elementNodeIds = item.ElementNodeIds ?? [];
			if (elementNodeIds.Count == 0)
			{
				return "No assignments";
			}

			var firstElementId = elementNodeIds[0];
			var firstElementName = elementNamesById.TryGetValue(firstElementId, out var elementName)
				? elementName
				: StateDefinitionDiscovery.ToShortId(firstElementId);

			return elementNodeIds.Count == 1
				? firstElementName
				: $"{firstElementName} + {elementNodeIds.Count - 1} more";
		}

		private static Dictionary<Guid, string> AddOrdinalToDuplicateLabels(IReadOnlyDictionary<Guid, string> labels)
		{
			var duplicateLabels = labels
				.GroupBy(pair => pair.Value, StringComparer.Ordinal)
				.Where(group => group.Count() > 1)
				.Select(group => group.Key)
				.ToHashSet(StringComparer.Ordinal);
			var ordinals = new Dictionary<string, int>(StringComparer.Ordinal);
			var uniqueLabels = new Dictionary<Guid, string>();

			foreach (var pair in labels)
			{
				if (!duplicateLabels.Contains(pair.Value))
				{
					uniqueLabels[pair.Key] = pair.Value;
					continue;
				}

				ordinals.TryGetValue(pair.Value, out var ordinal);
				ordinal++;
				ordinals[pair.Value] = ordinal;
				uniqueLabels[pair.Key] = AddOrdinalToLabel(pair.Value, ordinal);
			}

			return uniqueLabels;
		}

		private static string AddOrdinalToLabel(string label, int ordinal)
		{
			return label.EndsWith(")", StringComparison.Ordinal)
				? $"{label[..^1]}, {ordinal})"
				: $"{label} ({ordinal})";
		}

		private static IReadOnlyDictionary<Guid, string> AddShortIdToDuplicateLabels(IReadOnlyDictionary<Guid, string> labels)
		{
			var duplicateLabels = labels
				.GroupBy(pair => pair.Value, StringComparer.Ordinal)
				.Where(group => group.Count() > 1)
				.Select(group => group.Key)
				.ToHashSet(StringComparer.Ordinal);

			if (duplicateLabels.Count == 0)
			{
				return labels;
			}

			return labels.ToDictionary(
				pair => pair.Key,
				pair => duplicateLabels.Contains(pair.Value)
					? $"{pair.Value} [{StateDefinitionDiscovery.ToShortId(pair.Key)}]"
					: pair.Value);
		}

		private static string CreateMissingStateDefinitionLabel(Guid stateDefinitionId) =>
			$"<Missing State: {StateDefinitionDiscovery.ToShortId(stateDefinitionId)}>";

		private static string CreateMissingStateItemLabel(Guid stateItemId) =>
			$"<Missing State Item: {StateDefinitionDiscovery.ToShortId(stateItemId)}>";

		#endregion

		#region Custom State Items

		private CustomStateItemCollection CreateCustomStateItemCollection()
		{
			var collection = new CustomStateItemCollection();
			collection.Parent = this;
			return collection;
		}

		private void InitializeCustomStateItemCollection(CustomStateItemCollection collection)
		{
			collection.Parent = this;
			foreach (var item in collection)
			{
				item.Parent = this;
			}

			collection.CollectionChanged += OnCustomStateItemsCollectionChanged;
			collection.ChildPropertyChanged += OnCustomStateItemsChildPropertyChanged;
		}

		private void UnsubscribeFromCustomStateItems(CustomStateItemCollection? collection)
		{
			if (collection == null)
			{
				return;
			}

			collection.CollectionChanged -= OnCustomStateItemsCollectionChanged;
			collection.ChildPropertyChanged -= OnCustomStateItemsChildPropertyChanged;
		}

		private void OnCustomStateItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (sender is CustomStateItemCollection collection)
			{
				collection.Parent = this;
				foreach (var item in collection)
				{
					item.Parent = this;
				}
			}

			InitializeNewCustomStateItems(e);
			UpdateCustomStateItemData();
			IsDirty = true;
			OnPropertyChanged(nameof(CustomStateItems));
		}

		private void InitializeNewCustomStateItems(NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems == null)
			{
				return;
			}

			foreach (CustomStateItem item in e.NewItems)
			{
				InitializeNewCustomStateItem(item);
			}
		}

		private void InitializeNewCustomStateItem(CustomStateItem customStateItem)
		{
			if (customStateItem.StateItemId != Guid.Empty)
			{
				return;
			}

			var stateItem = GetFirstAvailableCustomStateItem(customStateItem);
			if (stateItem == null)
			{
				return;
			}

			customStateItem.StateItemId = stateItem.Id;
			customStateItem.Color = stateItem.Color;
		}

		private StateItemData? GetFirstAvailableCustomStateItem(CustomStateItem customStateItem)
		{
			var selectedDefinition = GetSelectedStateDefinition();
			if (selectedDefinition == null)
			{
				return null;
			}

			return GetStateItems(selectedDefinition)
				.FirstOrDefault(item => !IsCustomStateItemSelected(item.Id, customStateItem));
		}

		private void OnCustomStateItemsChildPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			UpdateCustomStateItemData();
			IsDirty = true;
			OnPropertyChanged(nameof(CustomStateItems));
		}

		private void UpdateCustomStateItemData()
		{
			_data.CustomStateItems = CustomStateItems
				.Select(item => item.CreateData())
				.ToList();
		}

		private void SetCustomStateItemModel(CustomStateItemCollection? collection, bool markDirty)
		{
			if (ReferenceEquals(_customStateItems, collection))
			{
				return;
			}

			UnsubscribeFromCustomStateItems(_customStateItems);
			_customStateItems = collection ?? CreateCustomStateItemCollection();
			InitializeCustomStateItemCollection(_customStateItems);
			UpdateCustomStateItemData();

			if (markDirty)
			{
				IsDirty = true;
				OnPropertyChanged(nameof(CustomStateItems));
			}
		}

		private void UpdateCustomStateItemModel()
		{
			var collection = CreateCustomStateItemCollection();
			foreach (var itemData in _data.CustomStateItems ?? [])
			{
				var item = new CustomStateItem
				{
					Parent = this
				};
				item.UpdateFromData(itemData);
				collection.Add(item);
			}

			SetCustomStateItemModel(collection, false);
		}

		private void ClearCustomStateItems()
		{
			if (CustomStateItems.Count == 0)
			{
				return;
			}

			CustomStateItems.Clear();
		}

		private void NormalizeCustomStateItemsForDefault()
		{
			var selectedStateItemIds = new HashSet<Guid>();
			var normalizedItems = CustomStateItems
				.Where(item => item.StateItemId != Guid.Empty && selectedStateItemIds.Add(item.StateItemId))
				.ToList();

			if (normalizedItems.Count == CustomStateItems.Count)
			{
				return;
			}

			var collection = CreateCustomStateItemCollection();
			foreach (var item in normalizedItems)
			{
				collection.Add(item);
			}

			SetCustomStateItemModel(collection, true);
		}

		#endregion

		#region Mark Collections

		private IMarkCollection? GetSelectedMarkCollection()
		{
			return MarkCollections.FirstOrDefault(collection => collection.Id == _data.MarkCollectionId);
		}

		private IMarkCollection? GetFirstStateMarkCollection()
		{
			if (!Enum.TryParse<MarkCollectionType>("State", out var stateType))
			{
				return null;
			}

			return MarkCollections.FirstOrDefault(collection => collection.CollectionType == stateType);
		}

		#endregion

		#region Browsables

		private void SetRenderSourceBrowsables()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(4)
			{
				{nameof(StateItem), RenderSource == StateRenderSource.StateItem},
				{nameof(MarkCollectionId), RenderSource == StateRenderSource.MarkCollection},
				{nameof(CustomStateItems), RenderSource == StateRenderSource.Custom},
				{nameof(Iterations), PlaybackMode == PlaybackMode.Iterate}
			};

			SetBrowsable(propertyStates);
			TypeDescriptor.Refresh(this);
		}

		#endregion
		
		public override IModuleDataModel? ModuleData
		{
			get { return _data; }
			set
			{
				if (value != null)
				{
					_data = (StateData)value;
					CheckForInvalidColorData();
					UpdateCustomStateItemModel();
					SetRenderSourceBrowsables();
					IsDirty = true;
				}
				
			}
		}
	}
}
