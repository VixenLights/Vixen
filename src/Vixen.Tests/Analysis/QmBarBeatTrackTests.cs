using QMLibrary;
using Xunit;

namespace Vixen.Tests.Analysis;

public sealed class QmBarBeatTrackTests
{
	private const int SampleRate = 44_100;
	private const int BeatsPerBar = 4;
	private static readonly string[] BeatLabels = ["1", "2", "3", "4"];

	[Fact]
	public void BarBeatTracker_ExposesExpectedContract()
	{
		// Arrange
		var plugin = new QMBarBeatTrack(SampleRate);

		// Act
		var parameters = plugin.GetParameterDescriptors().ToList();
		var outputs = plugin.GetOutputDescriptors().ToList();

		// Assert
		Assert.Equal("qm-barbeattracker", plugin.GetIdentifier());
		Assert.Equal(ManagedPlugin.InputDomain.TimeDomain, plugin.GetInputDomain());

		Assert.Equal(["bpb", "alpha", "inputtempo", "constraintempo"],
			parameters.Select(parameter => parameter.identifier));

		var beatsPerBar = Assert.Single(parameters,
			parameter => parameter.identifier == "bpb");
		Assert.Equal("bpb", beatsPerBar.identifier);
		Assert.Equal(2, beatsPerBar.minValue);
		Assert.Equal(16, beatsPerBar.maxValue);
		Assert.Equal(BeatsPerBar, beatsPerBar.defaultValue);
		plugin.SetParameter("bpb", 3);
		Assert.Equal(3, plugin.GetParameter("bpb"));

		Assert.Equal(["beats", "bars", "beatcounts", "beatsd"],
			outputs.Select(output => output.identifier));
	}

	[Fact]
	public void BarBeatTracker_DisposeReleasesNativePlugin()
	{
		var plugin = new QMBarBeatTrack(SampleRate);

		plugin.Dispose();

		Assert.Throws<ObjectDisposedException>(() => plugin.GetIdentifier());
		Assert.Throws<ObjectDisposedException>(() =>
			plugin.Process([0f], ManagedRealtime.frame2RealTime(0, SampleRate)));
	}

	[Fact]
	public void BarBeatTracker_ProcessesDeterministicClickTrack()
	{
		// Arrange
		var samples = CreateClickTrack(SampleRate, BeatsPerBar, (32, 120d));
		var plugin = new QMBarBeatTrack(SampleRate);
		plugin.SetParameter("bpb", BeatsPerBar);

		var stepSize = plugin.GetPreferredStepSize();
		var blockSize = plugin.GetPreferredBlockSize();
		Assert.True(plugin.Initialise(1, (uint)stepSize, (uint)blockSize));

		// Act
		var features = ProcessSamples(plugin, samples, stepSize, blockSize);
		// Assert
		Assert.Equal([0, 1, 2, 3], features.Keys.Order());
		AssertFeatureBaseline(features[0], 127, "2", 476, 63_494);
		AssertFeatureBaseline(features[1], 31, "1", 1_973, 61_974);
		AssertFeatureBaseline(features[2], 127, "2", 476, 63_494);
		AssertFeatureBaseline(features[3], 125, "", 975, 62_972);
		Assert.All(features[0], feature => Assert.True(feature.hasTimestamp));
		Assert.All(features[1], feature => Assert.True(feature.hasTimestamp));
		Assert.All(features[2], feature => Assert.True(feature.hasTimestamp));
		Assert.All(features[3], feature => Assert.True(feature.hasTimestamp));
		Assert.All(features[0], feature => Assert.True(BeatLabels.Contains(feature.label)));
	}

	private static IDictionary<int, ICollection<ManagedFeature>> ProcessSamples(
		ManagedPlugin plugin,
		float[] samples,
		int stepSize,
		int blockSize)
	{
		for (var offset = 0; offset < samples.Length; offset += stepSize)
		{
			var block = new float[blockSize];
			Array.Copy(samples, offset, block, 0, Math.Min(blockSize, samples.Length - offset));
			plugin.Process(block, ManagedRealtime.frame2RealTime(offset, SampleRate));
		}

		return plugin.GetRemainingFeatures();
	}

	private static float[] CreateClickTrack(
		int sampleRate,
		int beatsPerBar,
		params (int Bars, double Bpm)[] sections)
	{
		var beatStarts = new List<int>();
		var cursor = 0;

		foreach (var (bars, bpm) in sections)
		{
			var samplesPerBeat = sampleRate * 60d / bpm;

			for (var beat = 0; beat < bars * beatsPerBar; beat++)
			{
				beatStarts.Add(cursor + (int)Math.Round(beat * samplesPerBeat));
			}

			cursor += (int)Math.Round(bars * beatsPerBar * samplesPerBeat);
		}

		var samples = new float[cursor + 257];

		for (var index = 0; index < beatStarts.Count; index++)
		{
			var isDownbeat = index % beatsPerBar == 0;
			AddClick(samples, beatStarts[index], sampleRate,
				isDownbeat ? 28_000f : 18_000f,
				isDownbeat ? 880d : 1_760d);
		}

		return samples;
	}

	private static void AddClick(
		float[] samples,
		int start,
		int sampleRate,
		float amplitude,
		double frequency)
	{
		var length = (int)(sampleRate * 0.025);

		for (var offset = 0; offset < length && start + offset < samples.Length; offset++)
		{
			var envelope = Math.Exp(-7d * offset / length);
			var phase = 2d * Math.PI * frequency * offset / sampleRate;
			samples[start + offset] += (float)(amplitude * envelope * Math.Sin(phase));
		}
	}

	private static void AssertFeatureBaseline(
		ICollection<ManagedFeature> features,
		int expectedCount,
		string expectedFirstLabel,
		double expectedFirstTimestampMilliseconds,
		double expectedLastTimestampMilliseconds)
	{
		Assert.Equal(expectedCount, features.Count);
		Assert.Equal(expectedFirstLabel, features.First().label);
		Assert.Equal(expectedFirstTimestampMilliseconds, features.First().timestamp.totalMilliseconds());
		Assert.Equal(expectedLastTimestampMilliseconds, features.Last().timestamp.totalMilliseconds());
	}
}
