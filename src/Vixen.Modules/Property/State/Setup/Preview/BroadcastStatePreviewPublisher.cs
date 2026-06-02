using Common.Broadcast;
using Common.Messages.LivePreview;

namespace VixenModules.Property.State.Setup.Preview
{
	/// <summary>
	/// Publishes State preview operations through the Live Preview broadcast contract.
	/// </summary>
	internal sealed class BroadcastStatePreviewPublisher : IStatePreviewPublisher
	{
		internal const string ContextName = "State Preview";

		/// <inheritdoc />
		public void TurnOn(IReadOnlyCollection<StatePreviewPair> pairs)
		{
			ArgumentNullException.ThrowIfNull(pairs);

			var states = pairs
				.Distinct()
				.Select(pair => new ElementState
				{
					Id = pair.ElementId,
					Color = pair.Color,
					Duration = int.MaxValue,
					Intensity = 100
				})
				.ToArray();
			if (states.Length == 0)
			{
				return;
			}

			Broadcast.Publish(LivePreviewChannels.TurnOnElements, new TurnOnElementsMessage
			{
				States = states,
				ContextName = ContextName
			});
		}

		/// <inheritdoc />
		public void TurnOff(IReadOnlyCollection<Guid> elementIds)
		{
			ArgumentNullException.ThrowIfNull(elementIds);

			var distinctElementIds = elementIds
				.Distinct()
				.ToArray();
			if (distinctElementIds.Length == 0)
			{
				return;
			}

			Broadcast.Publish(LivePreviewChannels.TurnOffElements, new TurnOffElementsMessage
			{
				ElementIds = distinctElementIds,
				ContextName = ContextName
			});
		}

		/// <inheritdoc />
		public void Clear()
		{
			Broadcast.Publish(LivePreviewChannels.ClearActiveEffects, new ClearActiveEffectsMessage
			{
				ContextName = ContextName
			});
		}

		/// <inheritdoc />
		public void Release()
		{
			Broadcast.Publish(LivePreviewChannels.ReleaseContext, new ReleaseContextMessage
			{
				ContextName = ContextName
			});
		}
	}
}
