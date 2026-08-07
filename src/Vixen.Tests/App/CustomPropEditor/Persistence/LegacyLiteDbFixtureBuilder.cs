using LiteDB;

namespace Vixen.Tests.App.CustomPropEditor.Persistence;

internal static class LegacyLiteDbFixtureBuilder
{
	public static string Create(string imageEntryName, bool includeNestedTypeMetadata = false, int payloadSize = 0)
	{
		var path = Path.Combine(Path.GetTempPath(), $"VIX-3967-{Guid.NewGuid():N}.prp");
		using (var database = new LiteDatabase(path))
		{
			var prop = new BsonDocument
			{
				["_id"] = Guid.NewGuid(),
				["Name"] = "Legacy Custom Prop",
				["StateDefinition"] = new BsonDocument
				{
					["StateDefinitionName"] = "Legacy Face",
					["Name"] = "Smile"
				},
				["StateDefinitions"] = new BsonArray
				{
					new BsonDocument
					{
						["StateDefinitionName"] = "Legacy Face",
						["Name"] = "Frown"
					}
				}
			};

			if (includeNestedTypeMetadata)
			{
				prop["Unsafe"] = new BsonDocument
				{
					["Nested"] = new BsonDocument
					{
						["_type"] = "System.Diagnostics.Process, System"
					}
				};
			}

			if (payloadSize > 0)
			{
				var payload = new byte[payloadSize];
				Random.Shared.NextBytes(payload);
				prop["LargePayload"] = payload;
			}

			database.GetCollection("props").Insert(prop);
			using var image = new MemoryStream([0xFF, 0xD8, 0xFF, 0xD9]);
			database.FileStorage.Upload($"$/image/{imageEntryName}", imageEntryName, image);
		}

		return path;
	}
}
