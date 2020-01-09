using System;

namespace Common.AudioPlayer
{
	public interface IPlayer:IDisposable
	{
		bool IsPaused { get; }

		bool IsStopped { get; }

		bool IsPlaying { get; }

		string Filename { get; }

		TimeSpan Duration { get; }

		void Stop();

		void Play();

		void Pause();

		bool Resume();

		TimeSpan Position { get; set; }

		int BytesPerSample { get; }

		long NumberSamples { get; }

		int Channels { get; }

		float Frequency { get; }

		float PlaybackRate { get; set; }

		bool UseTempo { get; set; }

		float Volume { get; set; }

		void SetPlaybackSettings(int latency, bool eventMode, bool exclusiveMode);

		void SwitchAudioDevice(string mediaDeviceId);

		string CurrentAudioDeviceId { get; }

		event Action PlaybackEnded;
	}
}
