using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Services.EffectDefaults;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Pulse;
using Xunit;

namespace Vixen.Tests.EffectDefaults;

public sealed class EffectDefaultsStoreRoundTripTests
{
	[Fact]
	public void Store_RoundTripsThroughBinarySerialization()
	{
		var store = new EffectDefaultsStore();
		store.Entries.Add(new EffectDefaultEntry
		{
			TypeId = Guid.NewGuid(),
			EffectName = "Pulse",
			DataModelTypeName = typeof(PulseData).FullName,
			SavedUtc = DateTime.UtcNow,
			Payload = [1, 2, 3, 4]
		});

		var serializer = new DataContractSerializer(typeof(EffectDefaultsStore));
		byte[] bytes = EffectDefaultsService.WriteBinary(serializer, store);
		var roundTripped = (EffectDefaultsStore)EffectDefaultsService.ReadBinary(serializer, bytes);

		EffectDefaultEntry original = store.Entries[0];
		EffectDefaultEntry roundTrippedEntry = Assert.Single(roundTripped.Entries);
		Assert.Equal(original.TypeId, roundTrippedEntry.TypeId);
		Assert.Equal(original.EffectName, roundTrippedEntry.EffectName);
		Assert.Equal(original.DataModelTypeName, roundTrippedEntry.DataModelTypeName);
		Assert.Equal(original.SavedUtc, roundTrippedEntry.SavedUtc);
		Assert.Equal(original.Payload, roundTrippedEntry.Payload);
	}
}

public sealed class EffectDefaultsServiceCapturePipelineTests
{
	[Fact]
	public void CaptureScrubbedPayload_DoesNotMutateLiveModuleData()
	{
		// Regression test for the shared-reference hazard documented in the effect defaults ExecPlan's
		// Decision Log: PulseData.CreateInstanceForClone assigns LevelCurve by reference rather than deep
		// copying it, so a capture pipeline that cloned the live effect and scrubbed the clone could mutate
		// state still reachable from the live effect. The serialize/deserialize/scrub pipeline under test
		// must never touch the original PulseData or the Curve instance it references.
		var originalCurve = new Curve(CurveType.Flat100);
		var originalPoints = originalCurve.Points.ToArray();
		var data = new PulseData
		{
			LevelCurve = originalCurve,
			ColorGradient = new ColorGradient(Color.White)
		};

		var serializer = new DataContractSerializer(typeof(PulseData));
		byte[] payload = EffectDefaultsService.CaptureScrubbedPayload(serializer, data);

		Assert.NotEmpty(payload);
		Assert.Same(originalCurve, data.LevelCurve);
		Assert.Equal(originalPoints, data.LevelCurve.Points.ToArray());
	}
}
