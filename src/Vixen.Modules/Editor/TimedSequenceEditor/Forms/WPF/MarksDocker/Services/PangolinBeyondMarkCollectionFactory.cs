using System.Drawing;
using VixenModules.App.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Services
{
	internal static class PangolinBeyondMarkCollectionFactory
	{
		internal static IReadOnlyList<MarkCollection> CreateCollections(
			IReadOnlyList<PangolinBeyondMarkRecord> records,
			PangolinBeyondImportMode importMode,
			Color replacementColor)
		{
			ArgumentNullException.ThrowIfNull(records);

			return importMode switch
			{
				PangolinBeyondImportMode.GroupByColor => CreateColorCollections(records),
				PangolinBeyondImportMode.SingleCollection => [CreateCollection("Beyond Marks", replacementColor, records)],
				_ => throw new ArgumentOutOfRangeException(nameof(importMode), importMode, @"Unsupported Pangolin Beyond import mode.")
			};
		}

		private static IReadOnlyList<MarkCollection> CreateColorCollections(IReadOnlyList<PangolinBeyondMarkRecord> records)
		{
			var recordsByColor = new Dictionary<Color, List<PangolinBeyondMarkRecord>>();
			var colorOrder = new List<Color>();
			foreach (var record in records)
			{
				if (!recordsByColor.TryGetValue(record.Color, out var colorRecords))
				{
					colorRecords = [];
					recordsByColor.Add(record.Color, colorRecords);
					colorOrder.Add(record.Color);
				}

				colorRecords.Add(record);
			}

			return colorOrder
				.Select(color => CreateCollection($"Beyond Marks - #{color.R:X2}{color.G:X2}{color.B:X2}", color, recordsByColor[color]))
				.ToList();
		}

		private static MarkCollection CreateCollection(string name, Color color, IEnumerable<PangolinBeyondMarkRecord> records)
		{
			var marks = records.Select(record => new Mark(record.StartTime) { Text = record.Text }).ToList();
			var collection = new MarkCollection
			{
				Name = name,
				ShowMarkBar = true
			};
			collection.Decorator.Color = color;
			collection.AddMarks(marks);
			return collection;
		}
	}
}
