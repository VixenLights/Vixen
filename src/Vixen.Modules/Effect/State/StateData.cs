using System.Runtime.Serialization;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.State
{
	/// <summary>
	/// Stores persisted configuration for the State effect.
	/// </summary>
	[DataContract]
	[KnownType(typeof(CustomStateItemData))]
	public sealed class StateData: EffectTypeModuleData
	{
		internal const int MinIterations = 1;
		internal const int MaxIterations = 20;

		private int _iterations = MinIterations;

		/// <summary>
		/// Gets or sets the stable identifier of the selected State definition.
		/// </summary>
		/// <value>The stable identifier of the selected State definition.</value>
		[DataMember]
		public Guid SelectedStateDefinitionId { get; set; } = Guid.Empty;

		/// <summary>
		/// Gets or sets the stable identifier of the State property that contains the selected definition.
		/// </summary>
		/// <value>The stable identifier of the containing State property.</value>
		[DataMember]
		public Guid StatePropertyId { get; set; } = Guid.Empty;

		/// <summary>
		/// Gets or sets the stable identifier of the element that owns the containing State property.
		/// </summary>
		/// <value>The stable identifier of the owner element.</value>
		[DataMember]
		public Guid StateOwnerElementId { get; set; } = Guid.Empty;

		/// <summary>
		/// Gets or sets the source used to determine active State items.
		/// </summary>
		/// <value>One of the enumeration values that specifies the State item render source. The default is <see cref="StateRenderSource.StateItem" />.</value>
		[DataMember]
		public StateRenderSource RenderSource { get; set; } = StateRenderSource.StateItem;

		/// <summary>
		/// Gets or sets the stable identifier of the selected State item anchor.
		/// </summary>
		/// <value>The selected State item anchor identifier, or <see cref="Guid.Empty" /> for all State items.</value>
		[DataMember]
		public Guid SelectedStateItemId { get; set; } = Guid.Empty;

		/// <summary>
		/// Gets or sets the stable identifier of the selected Mark Collection.
		/// </summary>
		/// <value>The selected Mark Collection identifier.</value>
		[DataMember]
		public Guid MarkCollectionId { get; set; } = Guid.Empty;

		/// <summary>
		/// Gets or sets the playback behavior used when multiple State item names are active.
		/// </summary>
		/// <value>One of the enumeration values that specifies the playback behavior. The default is <see cref="PlaybackMode.Default" />.</value>
		[DataMember]
		public PlaybackMode PlaybackMode { get; set; } = PlaybackMode.Default;

		/// <summary>
		/// Gets or sets the number of times the active State item sequence repeats in Iterate playback mode.
		/// </summary>
		/// <value>The number of Iterate playback repetitions. The default is 1.</value>
		[DataMember]
		public int Iterations
		{
			get => NormalizeIterations(_iterations);
			set => _iterations = NormalizeIterations(value);
		}

		/// <summary>
		/// Gets or sets a value that indicates whether custom State item rows cycle in individual timing slots.
		/// </summary>
		/// <value><see langword="true" /> if each custom row cycles independently; otherwise, <see langword="false" /> to group consecutive custom rows with the same State item name. The default is <see langword="true" />.</value>
		[DataMember]
		public bool CycleIndividually { get; set; } = true;
		
		/// <summary>
		/// Gets or sets whether the timeline renders the full visual or text
		/// </summary>
		[DataMember]
		public bool ShowEffectVisual { get; set; } = true;

		/// <summary>
		/// Gets or sets the ordered custom State item rows.
		/// </summary>
		/// <value>The ordered custom State item rows.</value>
		[DataMember]
		public List<CustomStateItemData> CustomStateItems { get; set; } = [];
		
		/// <summary>
		/// Gets or sets whether the timeline renders the full visual or text
		/// </summary>
		[DataMember]
		public bool ShowEffectVisual { get; set; } = true;
		
		#region Overrides of EffectTypeModuleData

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			EnsureCustomStateItems();

			var result = new StateData
			{
				SelectedStateDefinitionId = SelectedStateDefinitionId,
				StatePropertyId = StatePropertyId,
				StateOwnerElementId = StateOwnerElementId,
				RenderSource = RenderSource,
				SelectedStateItemId = SelectedStateItemId,
				MarkCollectionId = MarkCollectionId,
				PlaybackMode = PlaybackMode,
				Iterations = Iterations,
				CycleIndividually = CycleIndividually,
				ShowEffectVisual = ShowEffectVisual,
				CustomStateItems = CustomStateItems
					.Select(item => item.CreateInstanceForClone())
					.ToList()
			};

			return result;
		}

		#endregion

		/// <summary>
		/// Normalizes deserialized State effect data.
		/// </summary>
		/// <param name="context">The serialization context.</param>
		[OnDeserialized]
		public void OnDeserialized(StreamingContext context)
		{
			EnsureCustomStateItems();
		}

		internal static int NormalizeIterations(int iterations) =>
			Math.Clamp(iterations, MinIterations, MaxIterations);

		private void EnsureCustomStateItems()
		{
			CustomStateItems ??= [];
		}
	}
}
