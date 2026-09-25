using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VixenModules.App.FPPClient.Models;

internal sealed class FppUptimeConverter : JsonConverter<string>
{
	public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
		reader.TokenType switch
		{
			JsonTokenType.String => reader.GetString()!,
			JsonTokenType.Number => TimeSpan.FromMilliseconds(reader.GetDouble()).ToString("c", CultureInfo.InvariantCulture),
			_ => throw new JsonException("FPP uptime must be a string or a number of milliseconds.")
		};

	public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) =>
		writer.WriteStringValue(value);
}
