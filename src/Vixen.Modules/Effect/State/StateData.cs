using System.Runtime.Serialization;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.State
{
	/// <summary>
	/// Stores persisted configuration for the State effect.
	/// </summary>
	[DataContract]
	public sealed class StateData: EffectTypeModuleData
	{
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
		/// Gets or sets whether the timeline renders the full visual or text
		/// </summary>
		[DataMember]
		public bool ShowEffectVisual { get; set; } = true;
		
		#region Overrides of EffectTypeModuleData

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			var result = new StateData
			{
				SelectedStateDefinitionId = SelectedStateDefinitionId,
				StatePropertyId = StatePropertyId,
				StateOwnerElementId = StateOwnerElementId,
				RenderSource = RenderSource,
				SelectedStateItemId = SelectedStateItemId,
				MarkCollectionId = MarkCollectionId,
				PlaybackMode = PlaybackMode
			};

			return result;
		}

		#endregion
	}
}
