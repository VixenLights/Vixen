using Vixen.Execution.Context;
using Vixen.Sys;

namespace VixenModules.App.LivePreview
{
	/// <summary>Adapts a <see cref="LiveContext"/> to the <see cref="ILiveContext"/> interface.</summary>
	internal sealed class LiveContextAdapter(LiveContext context) : ILiveContext
	{
		/// <summary>Gets the underlying <see cref="LiveContext"/> instance.</summary>
		internal LiveContext Context { get; } = context;

		public string Name => Context.Name;

		/// <inheritdoc/>
		public void Execute(EffectNode data) => Context.Execute(data);

		/// <inheritdoc/>
		public void Execute(IEnumerable<EffectNode> data) => Context.Execute(data);

		/// <inheritdoc/>
		public void TerminateNode(Guid targetNode) => Context.TerminateNode(targetNode);

		/// <inheritdoc/>
		public void TerminateNodes(IEnumerable<Guid> targetNodes) => Context.TerminateNodes(targetNodes);

		/// <inheritdoc/>
		public void Clear(bool waitForReset = true) => Context.Clear(waitForReset);
	}
}
