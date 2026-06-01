using Common.Broadcast;
using Common.Messages.LivePreview;
using NLog;
using Vixen.Module;
using Vixen.Module.App;
using Vixen.Sys;

namespace VixenModules.App.LivePreview
{
	/// <summary>App module entry point for the Live Preview service.</summary>
	/// <remarks>
	/// This module is loaded automatically by Vixen at startup. It creates a registry of named
	/// <see cref="Vixen.Execution.Context.LiveContext"/> instances and subscribes to broadcast
	/// messages from <c>Common.Messages.LivePreview</c> so any module can trigger live element
	/// control without a direct dependency on this assembly.
	/// </remarks>
	public class Module : AppModuleInstanceBase
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private LivePreviewData _data = new();
		private LivePreviewService? _service;

		/// <summary>Gets the active <see cref="ILivePreviewService"/> instance, or <see langword="null"/> if the module is not loaded.</summary>
		public static ILivePreviewService? Instance { get; private set; }

		/// <inheritdoc/>
		public override IApplication Application { set { } }

		/// <inheritdoc/>
		public override IModuleDataModel StaticModuleData
		{
			get => _data;
			set => _data = (LivePreviewData)value;
		}

		/// <inheritdoc/>
		public override void Loading()
		{
			_service = new LivePreviewService(new VixenContextFactory());
			// Create the default context eagerly so it is visible in VixenSystem.Contexts immediately.
			_service.GetOrCreateContext(null);
			Instance = _service;
			_SubscribeToBroadcasts();
			Log.Info("LivePreview module loaded.");
		}

		/// <inheritdoc/>
		public override void Unloading()
		{
			_UnsubscribeFromBroadcasts();
			Instance = null;
			_service?.Dispose();
			_service = null;
			Log.Info("LivePreview module unloaded.");
		}

		private void _SubscribeToBroadcasts()
		{
			Broadcast.Subscribe<TurnOnElementMessage>(this, LivePreviewChannels.TurnOnElement,
				m => _service?.TurnOnElement(m.State.Id, m.State.Duration, m.State.Intensity, m.State.Color, m.ContextName));

			Broadcast.Subscribe<TurnOnElementsMessage>(this, LivePreviewChannels.TurnOnElements,
				m => _service?.TurnOnElements(m.States, m.ContextName));

			Broadcast.Subscribe<TurnOffElementMessage>(this, LivePreviewChannels.TurnOffElement,
				m => _service?.TurnOffElement(m.ElementId, m.ContextName));

			Broadcast.Subscribe<TurnOffElementsMessage>(this, LivePreviewChannels.TurnOffElements,
				m => _service?.TurnOffElements(m.States, m.ContextName));

			Broadcast.Subscribe<ClearActiveEffectsMessage>(this, LivePreviewChannels.ClearActiveEffects,
				m => _service?.ClearActiveEffects(m.ContextName));
		}

		private void _UnsubscribeFromBroadcasts()
		{
			Broadcast.Unsubscribe<TurnOnElementMessage>(this, LivePreviewChannels.TurnOnElement);
			Broadcast.Unsubscribe<TurnOnElementsMessage>(this, LivePreviewChannels.TurnOnElements);
			Broadcast.Unsubscribe<TurnOffElementMessage>(this, LivePreviewChannels.TurnOffElement);
			Broadcast.Unsubscribe<TurnOffElementsMessage>(this, LivePreviewChannels.TurnOffElements);
			Broadcast.Unsubscribe<ClearActiveEffectsMessage>(this, LivePreviewChannels.ClearActiveEffects);
		}
	}
}
