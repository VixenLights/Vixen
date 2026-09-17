using System.Reflection;
using System.Runtime.CompilerServices;
using Common.AudioPlayer;
using NAudio.Wave;
using Xunit;

namespace Vixen.Tests.AudioPlayer;

/// <summary>
/// Verifies that delayed audio-output completion callbacks cannot clean a replacement output.
/// </summary>
public sealed class CoreAudioPlayerOwnershipTests
{
	/// <summary>
	/// Verifies that a completion callback from an old output does not dispose the current replacement output.
	/// </summary>
	[Fact]
	public void PlaybackStopped_WhenSenderIsStale_DoesNotDisposeCurrentOutput()
	{
		var player = CreateUninitializedPlayer();
		var oldOutput = new TestWavePlayer();
		var currentOutput = new TestWavePlayer();
		SetField(player, "_soundOut", currentOutput);

		InvokePlaybackStopped(player, oldOutput);

		Assert.Same(currentOutput, GetField(player, "_soundOut"));
		Assert.Equal(0, currentOutput.DisposeCount);
	}

	/// <summary>
	/// Verifies that a completion callback from the current output cleans that output.
	/// </summary>
	[Fact]
	public void PlaybackStopped_WhenSenderIsCurrent_CleansCurrentOutput()
	{
		var player = CreateUninitializedPlayer();
		var currentOutput = new TestWavePlayer();
		SetField(player, "_soundOut", currentOutput);

		InvokePlaybackStopped(player, currentOutput);

		Assert.Null(GetField(player, "_soundOut"));
		Assert.Equal(1, currentOutput.DisposeCount);
	}

	private static CoreAudioPlayer CreateUninitializedPlayer()
	{
		var player = (CoreAudioPlayer) RuntimeHelpers.GetUninitializedObject(typeof(CoreAudioPlayer));
		SetField(player, "_soundOutLock", new object());
		return player;
	}

	private static void InvokePlaybackStopped(CoreAudioPlayer player, IWavePlayer sender)
	{
		var method = typeof(CoreAudioPlayer).GetMethod("PlaybackDeviceOnPlaybackStopped", BindingFlags.Instance | BindingFlags.NonPublic)!;
		method.Invoke(player, [sender, new StoppedEventArgs()]);
	}

	private static object? GetField(object target, string fieldName)
	{
		var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!;
		return field.GetValue(target);
	}

	private static void SetField(object target, string fieldName, object? value)
	{
		var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!;
		field.SetValue(target, value);
	}

	private sealed class TestWavePlayer : IWavePlayer
	{
		public int DisposeCount { get; private set; }
		public float Volume { get; set; }
		public WaveFormat OutputWaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
		public PlaybackState PlaybackState { get; private set; } = PlaybackState.Stopped;
		public event EventHandler<StoppedEventArgs>? PlaybackStopped;

		public void Dispose()
		{
			DisposeCount++;
		}

		public void Init(IWaveProvider waveProvider)
		{
		}

		public void Pause()
		{
			PlaybackState = PlaybackState.Paused;
		}

		public void Play()
		{
			PlaybackState = PlaybackState.Playing;
		}

		public void Stop()
		{
			PlaybackState = PlaybackState.Stopped;
			PlaybackStopped?.Invoke(this, new StoppedEventArgs());
		}
	}

}
