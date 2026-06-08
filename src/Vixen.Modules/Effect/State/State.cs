using System.ComponentModel;
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
		internal const string NoStateDefinitionsLabel = "<No States Available>";
		internal const string NoStateItemsLabel = "<No State Items Available>";

		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private StateData _data;
		private EffectIntents _elementData = new();
		private IReadOnlyList<DiscoveredStateDefinition> _stateDefinitions = [];

		public State()
		{
			_data = new StateData();
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

		#region Properties

		/// <summary>
		/// Gets or sets the selected State definition display label.
		/// </summary>
		/// <value>The selected State definition display label.</value>
		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"State")]
		[ProviderDescription(@"Selects the State definition to render.")]
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

				var previousStateItemName = GetSelectedStateItemName();
				SelectStateDefinition(selectedDefinition, previousStateItemName);
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
		[ProviderDisplayName(@"State Item")]
		[ProviderDescription(@"Selects the State item name to render.")]
		[TypeConverter(typeof(StateItemNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(2)]
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

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"MarkCollection")]
		[ProviderDescription(@"MarkCollection")]
		[TypeConverter(typeof(IMarkCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(2)]
		public string MarkCollectionId
		{
			get
			{
				var c = MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
				return c?.Name ?? String.Empty;
			}
			set
			{
				var newMarkCollection = MarkCollections.FirstOrDefault(x => x.Name.Equals(value));
				var id = newMarkCollection?.Id ?? Guid.Empty;
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
		[ProviderDisplayName(@"Render Source")]
		[ProviderDescription(@"Selects how active State items are determined.")]
		[PropertyOrder(1)]
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
						if (MarkCollections.Any())
						{
							var mc = MarkCollections.FirstOrDefault(x => x.CollectionType == MarkCollectionType.Phoneme);
							if (mc != null)
							{
								MarkCollectionId = mc.Name;
							}
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
		[ProviderDisplayName(@"Playback Mode")]
		[ProviderDescription(@"Selects how multiple active State item names are scheduled.")]
		[PropertyOrder(3)]
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
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion
		
		private void CheckForInvalidColorData()
		{
			// initialize the color handling
			GetValidColors();
		}

		private void RenderNodes()
		{
			if (RenderSource != StateRenderSource.StateItem)
			{
				return;
			}

			var selectedDefinition = GetSelectedStateDefinition();
			var intervals = StateRenderPlanner.CreateStateItemIntervals(
				selectedDefinition?.Definition,
				_data.SelectedStateItemId,
				PlaybackMode,
				TimeSpan);
			var targetScopeNodes = GetTargetScopeNodes()
				.GroupBy(node => node.Id)
				.ToDictionary(group => group.Key, group => group.First());

			foreach (var interval in intervals)
			{
				RenderInterval(interval, targetScopeNodes);
			}
		}

		private void RenderInterval(
			StateRenderInterval interval,
			IReadOnlyDictionary<Guid, IElementNode> targetScopeNodes)
		{
			if (interval.Duration <= TimeSpan.Zero)
			{
				return;
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

					var resolvedColor = ResolveStateItemColor(leafNode, interval.Item.Color);
					if (resolvedColor == null)
					{
						continue;
					}

					var intent = CreateIntent(leafNode, resolvedColor.Value, 1.0, interval.Duration);
					_elementData.AddIntentForElement(element.Id, intent, interval.Start);
				}
			}
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
				var markCollection = MarkCollections.FirstOrDefault(x => x.Name.Equals(MarkCollectionId));
				InitializeMarkCollectionListeners(markCollection);
			}
		}

		/// <inheritdoc />
		protected override void MarkCollectionsRemoved(IList<IMarkCollection> addedCollections)
		{
			var mc = addedCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
			if(mc != null)
			{
				//Our collection is gone!!!!
				RemoveMarkCollectionListeners(mc);
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

		private static string CreateMissingStateDefinitionLabel(Guid stateDefinitionId) =>
			$"<Missing State: {StateDefinitionDiscovery.ToShortId(stateDefinitionId)}>";

		private static string CreateMissingStateItemLabel(Guid stateItemId) =>
			$"<Missing State Item: {StateDefinitionDiscovery.ToShortId(stateItemId)}>";

		#endregion

		#region Browsables

		private void SetRenderSourceBrowsables()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3)
			{
				{nameof(StateItem), RenderSource == StateRenderSource.StateItem},
				{nameof(MarkCollectionId), RenderSource == StateRenderSource.MarkCollection}
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
					SetRenderSourceBrowsables();
					IsDirty = true;
				}
				
			}
		}
	}
}
