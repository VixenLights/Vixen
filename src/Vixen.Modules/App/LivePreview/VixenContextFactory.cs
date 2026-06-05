using Vixen.Sys;

namespace VixenModules.App.LivePreview
{
	/// <summary>Production <see cref="IContextFactory"/> implementation that delegates to <see cref="VixenSystem.Contexts"/>.</summary>
	internal sealed class VixenContextFactory : IContextFactory
	{
		/// <inheritdoc/>
		public ILiveContext GetOrCreate(string name)
		{
			var context = VixenSystem.Contexts.CreateLiveContext(name);
			context.Start();
			return new LiveContextAdapter(context);
		}

		/// <inheritdoc/>
		public void Release(ILiveContext context)
		{
			if (context is LiveContextAdapter adapter)
				VixenSystem.Contexts.ReleaseContext(adapter.Context);
		}
	}
}
