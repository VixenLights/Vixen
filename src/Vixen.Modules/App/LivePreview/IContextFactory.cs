using Vixen.Execution.Context;

namespace VixenModules.App.LivePreview
{
	/// <summary>Abstracts live context creation so <see cref="LivePreviewService"/> can be tested without VixenSystem.</summary>
	internal interface IContextFactory
	{
		/// <summary>Creates and starts a new live context registered with the given name.</summary>
		/// <param name="name">The display name for the new context.</param>
		/// <returns>A started <see cref="LiveContext"/> instance.</returns>
		LiveContext Create(string name);

		/// <summary>Releases a previously created context and removes it from VixenSystem.</summary>
		/// <param name="context">The context to release.</param>
		void Release(LiveContext context);
	}
}
