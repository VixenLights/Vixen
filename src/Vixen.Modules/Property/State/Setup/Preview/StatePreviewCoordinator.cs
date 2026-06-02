namespace VixenModules.Property.State.Setup.Preview
{
	/// <summary>
	/// Coordinates incremental State preview updates while preserving multi-color element activations.
	/// </summary>
	internal sealed class StatePreviewCoordinator
	{
		private readonly IStatePreviewPublisher _publisher;
		private readonly HashSet<StatePreviewPair> _activePairs = [];

		/// <summary>
		/// Initializes a new instance of the <see cref="StatePreviewCoordinator"/> class.
		/// </summary>
		/// <param name="publisher">The publisher used to apply Live Preview operations.</param>
		/// <exception cref="ArgumentNullException"><paramref name="publisher"/> is <see langword="null" />.</exception>
		internal StatePreviewCoordinator(IStatePreviewPublisher publisher)
		{
			ArgumentNullException.ThrowIfNull(publisher);
			_publisher = publisher;
		}

		/// <summary>
		/// Applies the incremental update needed to reach the specified preview pairs.
		/// </summary>
		/// <param name="desiredPairs">The element and color pairs that should remain active.</param>
		/// <exception cref="ArgumentNullException"><paramref name="desiredPairs"/> is <see langword="null" />.</exception>
		internal void Refresh(IEnumerable<StatePreviewPair> desiredPairs)
		{
			ArgumentNullException.ThrowIfNull(desiredPairs);

			var desiredSet = desiredPairs.ToHashSet();
			var elementIdsToTurnOff = _activePairs
				.Except(desiredSet)
				.Select(pair => pair.ElementId)
				.ToHashSet();
			if (elementIdsToTurnOff.Count > 0)
			{
				_publisher.TurnOff(elementIdsToTurnOff);
				_activePairs.RemoveWhere(pair => elementIdsToTurnOff.Contains(pair.ElementId));
			}

			var pairsToTurnOn = desiredSet
				.Except(_activePairs)
				.ToHashSet();
			if (pairsToTurnOn.Count == 0)
			{
				return;
			}

			_publisher.TurnOn(pairsToTurnOn);
			_activePairs.UnionWith(pairsToTurnOn);
		}

		/// <summary>
		/// Clears active effects while retaining the State preview context.
		/// </summary>
		internal void Clear()
		{
			_publisher.Clear();
			_activePairs.Clear();
		}

		/// <summary>
		/// Releases the State preview context.
		/// </summary>
		internal void Release()
		{
			_publisher.Release();
			_activePairs.Clear();
		}
	}
}
