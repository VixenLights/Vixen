using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Drawing;
using Vixen.Module;
using VixenModules.LayerMixingFilter.ChromaKey;
using VixenModules.LayerMixingFilter.LumaKey;
using VixenModules.LayerMixingFilter.MaskFill;
using Xunit;

namespace Vixen.Tests.Sequencer;

public sealed class LayerMixingFilterDataJsonSerializationTests
{
	[Fact]
	public void DataContractJsonSerializer_RoundTripsAllCurrentLayerMixingFilterDataModels()
	{
		var chromaKeyData = new ChromaKeyData
		{
			ModuleTypeId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
			ModuleInstanceId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
			LowerLimit = 0.15,
			UpperLimit = 0.85,
			KeyColor = Color.FromArgb(12, 34, 56),
			KeySaturation = 0.52,
			KeyHue = 214.5f,
			HueTolerance = 8.25f,
			SaturationTolerance = 0.0825f,
			TransparentOnZeroBrightness = true
		};

		var lumaKeyData = new LumaKeyData
		{
			ModuleTypeId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
			ModuleInstanceId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
			LowerLimit = 0.25,
			UpperLimit = 0.75
		};

		var maskAndFillData = new MaskAndFillData
		{
			ModuleTypeId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
			ModuleInstanceId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
			ExcludeZeroValues = false,
			RequiresMixingPartner = true
		};

		var chromaKeyRoundTrip = RoundTrip(chromaKeyData);
		var lumaKeyRoundTrip = RoundTrip(lumaKeyData);
		var maskAndFillRoundTrip = RoundTrip(maskAndFillData);

		Assert.Equal(chromaKeyData.ModuleTypeId, chromaKeyRoundTrip.ModuleTypeId);
		Assert.Equal(chromaKeyData.ModuleInstanceId, chromaKeyRoundTrip.ModuleInstanceId);
		Assert.Equal(chromaKeyData.LowerLimit, chromaKeyRoundTrip.LowerLimit);
		Assert.Equal(chromaKeyData.UpperLimit, chromaKeyRoundTrip.UpperLimit);
		Assert.Equal(chromaKeyData.KeyColor.ToArgb(), chromaKeyRoundTrip.KeyColor.ToArgb());
		Assert.Equal(chromaKeyData.KeySaturation, chromaKeyRoundTrip.KeySaturation);
		Assert.Equal(chromaKeyData.KeyHue, chromaKeyRoundTrip.KeyHue);
		Assert.Equal(chromaKeyData.HueTolerance, chromaKeyRoundTrip.HueTolerance);
		Assert.Equal(chromaKeyData.SaturationTolerance, chromaKeyRoundTrip.SaturationTolerance);
		Assert.Equal(chromaKeyData.TransparentOnZeroBrightness, chromaKeyRoundTrip.TransparentOnZeroBrightness);

		Assert.Equal(lumaKeyData.ModuleTypeId, lumaKeyRoundTrip.ModuleTypeId);
		Assert.Equal(lumaKeyData.ModuleInstanceId, lumaKeyRoundTrip.ModuleInstanceId);
		Assert.Equal(lumaKeyData.LowerLimit, lumaKeyRoundTrip.LowerLimit);
		Assert.Equal(lumaKeyData.UpperLimit, lumaKeyRoundTrip.UpperLimit);

		Assert.Equal(maskAndFillData.ModuleTypeId, maskAndFillRoundTrip.ModuleTypeId);
		Assert.Equal(maskAndFillData.ModuleInstanceId, maskAndFillRoundTrip.ModuleInstanceId);
		Assert.Equal(maskAndFillData.ExcludeZeroValues, maskAndFillRoundTrip.ExcludeZeroValues);
		Assert.Equal(maskAndFillData.RequiresMixingPartner, maskAndFillRoundTrip.RequiresMixingPartner);
	}

	[Fact]
	public void DataContractJsonSerializer_RestoresIndentedJson()
	{
		var data = new LumaKeyData
		{
			ModuleTypeId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
			ModuleInstanceId = Guid.Parse("88888888-8888-8888-8888-888888888888"),
			LowerLimit = 0.125,
			UpperLimit = 0.625
		};

		var json = SerializeIndented(data);
		var roundTrip = Deserialize<LumaKeyData>(json);

		Assert.Contains(Environment.NewLine, json);
		Assert.Contains("\"LowerLimit\"", json);
		Assert.Equal(data.ModuleTypeId, roundTrip.ModuleTypeId);
		Assert.Equal(data.ModuleInstanceId, roundTrip.ModuleInstanceId);
		Assert.Equal(data.LowerLimit, roundTrip.LowerLimit);
		Assert.Equal(data.UpperLimit, roundTrip.UpperLimit);
	}

	private static T RoundTrip<T>(T dataModel)
		where T : IModuleDataModel
	{
		var json = SerializeIndented(dataModel);
		return Deserialize<T>(json);
	}

	private static string SerializeIndented<T>(T dataModel)
		where T : IModuleDataModel
	{
		var serializer = new DataContractJsonSerializer(typeof(T));
		using var stream = new MemoryStream();
		serializer.WriteObject(stream, dataModel);
		var compactJson = Encoding.UTF8.GetString(stream.ToArray());
		using var document = JsonDocument.Parse(compactJson);

		return JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions
		{
			WriteIndented = true
		});
	}

	private static T Deserialize<T>(string json)
		where T : IModuleDataModel
	{
		var serializer = new DataContractJsonSerializer(typeof(T));
		using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

		return (T)serializer.ReadObject(stream)!;
	}
}
