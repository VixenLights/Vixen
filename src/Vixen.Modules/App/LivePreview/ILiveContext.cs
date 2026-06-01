using Vixen.Sys;

namespace VixenModules.App.LivePreview
{
	/// <summary>Abstracts the operations on a live execution context needed by <see cref="LivePreviewService"/>.</summary>
	/// <remarks>
	/// Introduced so unit tests can mock context behaviour without a running <c>VixenSystem</c>.
	/// The production implementation is <see cref="LiveContextAdapter"/>.
	/// </remarks>
	internal interface ILiveContext
	{
		/// <summary>The underlying system context name.</summary>
		/// <returns>Context name.</returns>
		string Name { get; }
		
		/// <summary>Queues a single effect for immediate execution.</summary>
		/// <param name="data">The effect node to execute.</param>
		void Execute(EffectNode data);

		/// <summary>Queues a sequence of effects for immediate execution.</summary>
		/// <param name="data">The effect nodes to execute.</param>
		void Execute(IEnumerable<EffectNode> data);

		/// <summary>Terminates all active effects targeting the specified element node.</summary>
		/// <param name="targetNode">The identifier of the element node whose effects to terminate.</param>
		void TerminateNode(Guid targetNode);

		/// <summary>Terminates all active effects targeting the specified element nodes.</summary>
		/// <param name="targetNodes">The identifiers of the element nodes whose effects to terminate.</param>
		void TerminateNodes(IEnumerable<Guid> targetNodes);

		/// <summary>Clears all active effects from the context.</summary>
		/// <param name="waitForReset"><see langword="true"/> to block until the reset completes; otherwise <see langword="false"/>.</param>
		void Clear(bool waitForReset = true);
	}
}
