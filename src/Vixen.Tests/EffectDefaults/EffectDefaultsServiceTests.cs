using System.Drawing;
using System.Runtime.Serialization;
using System.Xml;
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

public sealed class EffectDefaultsServiceExportImportDiagnosticsTests
{
	private static EffectDefaultEntry MakeEntry(Guid typeId, string effectName, DateTime savedUtc) => new()
	{
		TypeId = typeId,
		EffectName = effectName,
		DataModelTypeName = typeof(PulseData).FullName,
		SavedUtc = savedUtc,
		Payload = [1, 2, 3]
	};

	[Fact]
	public void BuildExportStore_IncludesOnlyRequestedEntries()
	{
		Guid pulseId = Guid.NewGuid();
		Guid waveId = Guid.NewGuid();
		Guid textId = Guid.NewGuid();
		var entries = new Dictionary<Guid, EffectDefaultEntry>
		{
			[pulseId] = MakeEntry(pulseId, "Pulse", DateTime.UtcNow),
			[waveId] = MakeEntry(waveId, "Wave", DateTime.UtcNow),
			[textId] = MakeEntry(textId, "Text", DateTime.UtcNow)
		};

		EffectDefaultsStore store = EffectDefaultsService.BuildExportStore(entries, [pulseId, textId]);

		Assert.Equal(2, store.Entries.Count);
		Assert.Contains(store.Entries, entry => entry.TypeId == pulseId);
		Assert.Contains(store.Entries, entry => entry.TypeId == textId);
		Assert.DoesNotContain(store.Entries, entry => entry.TypeId == waveId);
	}

	[Fact]
	public void BuildExportStore_SkipsRequestedIdsWithNoEntry()
	{
		var entries = new Dictionary<Guid, EffectDefaultEntry>();

		EffectDefaultsStore store = EffectDefaultsService.BuildExportStore(entries, [Guid.NewGuid()]);

		Assert.Empty(store.Entries);
	}

	[Fact]
	public void MergeEntries_AddsNewAndOverwritesExisting_LeavingUntouchedEntriesAlone()
	{
		Guid existingId = Guid.NewGuid();
		Guid overwrittenId = Guid.NewGuid();
		Guid newId = Guid.NewGuid();

		var existingEntry = MakeEntry(existingId, "Existing", DateTime.UtcNow.AddDays(-1));
		var staleOverwrittenEntry = MakeEntry(overwrittenId, "StaleName", DateTime.UtcNow.AddDays(-1));
		var entries = new Dictionary<Guid, EffectDefaultEntry>
		{
			[existingId] = existingEntry,
			[overwrittenId] = staleOverwrittenEntry
		};

		var freshOverwrittenEntry = MakeEntry(overwrittenId, "FreshName", DateTime.UtcNow);
		var newEntry = MakeEntry(newId, "New", DateTime.UtcNow);

		EffectDefaultsImportResult result = EffectDefaultsService.MergeEntries(entries, [freshOverwrittenEntry, newEntry]);

		Assert.Equal(1, result.Imported);
		Assert.Equal(1, result.Overwritten);
		Assert.Equal(3, entries.Count);
		Assert.Same(existingEntry, entries[existingId]);
		Assert.Same(freshOverwrittenEntry, entries[overwrittenId]);
		Assert.Same(newEntry, entries[newId]);
	}

	[Fact]
	public void WriteDiagnosticXml_InlinesActualSettingsInsteadOfRawPayloadBytes()
	{
		Guid typeId = Guid.NewGuid();
		DateTime savedUtc = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
		var data = new PulseData
		{
			LevelCurve = new Curve(CurveType.Flat100),
			ColorGradient = new ColorGradient(Color.White)
		};
		var serializer = new DataContractSerializer(typeof(PulseData));
		byte[] payload = EffectDefaultsService.WriteBinary(serializer, data);
		var entry = new EffectDefaultEntry
		{
			TypeId = typeId,
			EffectName = "Pulse",
			DataModelTypeName = typeof(PulseData).FullName,
			SavedUtc = savedUtc,
			Payload = payload
		};

		string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xml");
		try
		{
			using (XmlWriter writer = XmlWriter.Create(path, new XmlWriterSettings { Indent = true }))
			{
				EffectDefaultsService.WriteDiagnosticXml(writer, [entry], _ => typeof(PulseData));
			}

			string xml = File.ReadAllText(path);
			Assert.Contains("\n  <", xml); // indented, not a single flat line
			Assert.Contains(typeId.ToString(), xml);
			Assert.Contains("Pulse", xml);
			Assert.Contains("2026-01-02T03:04:05", xml);

			// The actual settings must be inlined as readable elements, not left as an opaque
			// base64-encoded byte blob (the fix for the human-readability gap reported after use).
			Assert.Contains("LevelCurve", xml);
			Assert.Contains("ColorGradient", xml);
			Assert.DoesNotContain(Convert.ToBase64String(payload), xml);
		}
		finally
		{
			if (File.Exists(path)) File.Delete(path);
		}
	}

	[Fact]
	public void WriteDiagnosticXml_ExplainsUnavailableSettingsWhenEffectTypeIsNotInstalled()
	{
		var entry = MakeEntry(Guid.NewGuid(), "Obsolete", DateTime.UtcNow);

		string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xml");
		try
		{
			using (XmlWriter writer = XmlWriter.Create(path, new XmlWriterSettings { Indent = true }))
			{
				EffectDefaultsService.WriteDiagnosticXml(writer, [entry], _ => null);
			}

			string xml = File.ReadAllText(path);
			Assert.Contains("not currently installed", xml);
			Assert.DoesNotContain(Convert.ToBase64String(entry.Payload), xml);
		}
		finally
		{
			if (File.Exists(path)) File.Delete(path);
		}
	}
}
