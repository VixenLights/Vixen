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
			Broadcast.Subscribe(this, LivePreviewChannels.TurnOnElement,
				(TurnOnElementMessage m) => _service?.TurnOnElement(m.State.Id, m.State.Duration, m.State.Intensity, m.State.Color, m.ContextName));

			Broadcast.Subscribe(this, LivePreviewChannels.TurnOnElements,
				(TurnOnElementsMessage m) => _service?.TurnOnElements(m.States, m.ContextName));

			Broadcast.Subscribe(this, LivePreviewChannels.TurnOffElement,
				(TurnOffElementMessage m) => _service?.TurnOffElement(m.ElementId, m.ContextName));

			Broadcast.Subscribe(this, LivePreviewChannels.TurnOffElements,
				(TurnOffElementsMessage m) => _service?.TurnOffElements(m.ElementIds, m.ContextName));

			Broadcast.Subscribe(this, LivePreviewChannels.ClearActiveEffects,
				(ClearActiveEffectsMessage m) => _service?.ClearActiveEffects(m.ContextName));

			Broadcast.Subscribe(this, LivePreviewChannels.ReleaseContext,
				(ReleaseContextMessage m) => _service?.ReleaseContext(m.ContextName));
		}

		private void _UnsubscribeFromBroadcasts()
		{
			Broadcast.Unsubscribe(this, LivePreviewChannels.TurnOnElement);
			Broadcast.Unsubscribe(this, LivePreviewChannels.TurnOnElements);
			Broadcast.Unsubscribe(this, LivePreviewChannels.TurnOffElement);
			Broadcast.Unsubscribe(this, LivePreviewChannels.TurnOffElements);
			Broadcast.Unsubscribe(this, LivePreviewChannels.ClearActiveEffects);
			Broadcast.Unsubscribe(this, LivePreviewChannels.ReleaseContext);
		}
	}
}
