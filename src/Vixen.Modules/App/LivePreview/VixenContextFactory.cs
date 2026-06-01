using Vixen.Execution.Context;
using Vixen.Sys;

namespace VixenModules.App.LivePreview
{
	/// <summary>Production <see cref="IContextFactory"/> implementation that delegates to <see cref="VixenSystem.Contexts"/>.</summary>
	internal sealed class VixenContextFactory : IContextFactory
	{
		/// <inheritdoc/>
		public LiveContext Create(string name)
		{
			var context = VixenSystem.Contexts.CreateLiveContext(name);
			context.Start();
			return context;
		}

		/// <inheritdoc/>
		public void Release(LiveContext context) => VixenSystem.Contexts.ReleaseContext(context);
	}
}
