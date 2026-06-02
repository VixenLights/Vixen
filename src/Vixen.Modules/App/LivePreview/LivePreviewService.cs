using System.Collections.Concurrent;
using System.Drawing;
using Common.Messages.LivePreview;
using NLog;
using Vixen.Sys;
using VixenModules.Effect.SetLevel;

namespace VixenModules.App.LivePreview
{
	/// <summary>Implements <see cref="ILivePreviewService"/> by managing a registry of named <see cref="ILiveContext"/> instances.</summary>
	/// <remarks>
	/// Each named context is created on first use. All contexts are released when <see cref="Dispose"/> is called.
	/// </remarks>
	internal sealed class LivePreviewService(IContextFactory contextFactory) : ILivePreviewService, IDisposable
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private const string DefaultContextName = "LivePreview";

		private readonly ConcurrentDictionary<string, ILiveContext> _contexts = new(StringComparer.OrdinalIgnoreCase);

		/// <inheritdoc/>
		public void TurnOnElement(Guid id, int seconds, double intensity, string color, string? contextName = null)
		{
			var context = GetOrCreateContext(contextName);
			var node = VixenSystem.Nodes.GetElementNode(id);
			if (node is null)
			{
				Log.Warn("TurnOnElement: element {ElementId} not found", id);
				return;
			}

			intensity = Math.Clamp(intensity, 0.0, 100.0);
			var effect = CreateEffect(node, seconds, intensity, color);
			context.Execute(new EffectNode(effect, TimeSpan.Zero));
			Log.Debug("TurnOnElement: {ElementName} on context {Context}", node.Name, contextName ?? DefaultContextName);
		}

		/// <inheritdoc/>
		public void TurnOnElements(IEnumerable<ElementState> states, string? contextName = null)
		{
			ArgumentNullException.ThrowIfNull(states);
			var context = GetOrCreateContext(contextName);
			var effectNodes = new List<EffectNode>();

			foreach (var state in states)
			{
				var node = VixenSystem.Nodes.GetElementNode(state.Id);
				if (node is null)
				{
					Log.Warn("TurnOnElements: element {ElementId} not found", state.Id);
					continue;
				}
				effectNodes.Add(new EffectNode(CreateEffect(node, state.Duration, state.Intensity, state.Color), TimeSpan.Zero));
			}

			context.Execute(effectNodes);
			Log.Debug("TurnOnElements: {Count} elements on context {Context}", effectNodes.Count, contextName ?? DefaultContextName);
		}

		/// <inheritdoc/>
		public void TurnOffElement(Guid id, string? contextName = null)
		{
			var context = GetOrCreateContext(contextName);
			context.TerminateNode(id);
			Log.Debug("TurnOffElement: {ElementId} on context {Context}", id, contextName ?? DefaultContextName);
		}

		/// <inheritdoc/>
		public void TurnOffElements(IEnumerable<Guid> ids, string? contextName = null)
		{
			ArgumentNullException.ThrowIfNull(ids);
			var context = GetOrCreateContext(contextName);
			context.TerminateNodes(ids);
			Log.Debug("TurnOffElements on context {Context}", contextName ?? DefaultContextName);
		}

		/// <inheritdoc/>
		public void ClearActiveEffects(string? contextName = null)
		{
			var context = GetOrCreateContext(contextName);
			context.Clear();
			Log.Debug("ClearActiveEffects on context {Context}", contextName ?? DefaultContextName);
		}

		/// <summary>Returns the named context, creating and starting it if it does not yet exist.</summary>
		/// <param name="contextName">The context name, or <see langword="null"/> to use the default context.</param>
		/// <returns>The existing or newly created <see cref="ILiveContext"/>.</returns>
		internal ILiveContext GetOrCreateContext(string? contextName)
		{
			var key = contextName ?? DefaultContextName;
			return _contexts.GetOrAdd(key, contextFactory.GetOrCreate);
		}

		/// <inheritdoc/>
		public void ReleaseContext(string contextName)
		{
			ArgumentNullException.ThrowIfNull(contextName);
			var key = string.IsNullOrEmpty(contextName) ? DefaultContextName : contextName;
			if (!_contexts.TryRemove(key, out var context))
			{
				Log.Warn("ReleaseContext: context {Context} not found", key);
				return;
			}
			try { contextFactory.Release(context); }
			catch (Exception ex) { Log.Error(ex, "Error releasing context {Context}", key); }
			Log.Debug("ReleaseContext: released {Context}", key);
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			foreach (var context in _contexts.Values)
			{
				try { contextFactory.Release(context); }
				catch (Exception ex) { Log.Error(ex, "Error releasing context {Context}", context.Name); }
			}
			_contexts.Clear();
		}

		private static SetLevel CreateEffect(ElementNode node, int seconds, double intensity, string color)
		{
			if (!string.IsNullOrEmpty(color) && (color.Length != 7 || !color.StartsWith('#')))
				throw new ArgumentException(@"Invalid color format; expected #RRGGBB.", nameof(color));

			var effect = new SetLevel
			{
				TimeSpan = seconds > 0 ? TimeSpan.FromSeconds(seconds) : TimeSpan.FromDays(30),
				IntensityLevel = intensity / 100,
				TargetNodes = [node]
			};

			if (!string.IsNullOrEmpty(color))
				effect.Color = ColorTranslator.FromHtml(color);

			return effect;
		}
	}
}
