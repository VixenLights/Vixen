﻿using CSCore;

namespace VixenModules.Media.Audio
{
	public class CachedAudioData
	{
		public float[] AudioData { get; private set; }
		public WaveFormat WaveFormat { get; private set; }
		public CachedAudioData(string audioFileName)
		{
			using(var audioFileReader = CSCore.Codecs.CodecFactory.Instance.GetCodec(audioFileName).ToSampleSource())
			{
				WaveFormat = audioFileReader.WaveFormat;
				var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
				var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
				int samplesRead;
				while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
				{
					wholeFile.AddRange(readBuffer.Take(samplesRead));
				}
				AudioData = wholeFile.ToArray();
			}
			
		}
	}
}
