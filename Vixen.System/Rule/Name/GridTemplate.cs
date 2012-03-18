using System.Collections.Generic;

namespace Vixen.Rule.Name {
	public class GridTemplate : INamingRule {
		public const string RowValuePlaceholder = "{R}";
		public const string ColumnValuePlaceholder = "{C}";

		public GridTemplate() {
		}

		public GridTemplate(int width, int height, string format, int startRowNumber = 1, int startColNumber = 1) {
			StartRowNumber = startRowNumber;
			StartColNumber = startColNumber;
			Width = width;
			Height = height;
			Format = format;
		}

		public int StartRowNumber { get; set; }
		
		public int StartColNumber { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public string Format { get; set; }

		public string Name {
			get { return "Grid"; }
		}

		public string[] GenerateNames(int channelCount) {
			List<string> names = new List<string>();

			for(int rowNumber = StartRowNumber, rowCount = 0; rowCount < Width; rowNumber++, rowCount++) {
				for(int colNumber = StartColNumber, colCount = 0; colCount < Height; colNumber++, colCount++) {
					string name = _GenerateName(Format, rowNumber, colNumber);
					names.Add(name);
				}
			}

			return names.ToArray();
		}

		private string _GenerateName(string format, int rowNumber, int colNumber) {
			string name = format.Replace(RowValuePlaceholder, rowNumber.ToString());
			name = name.Replace(ColumnValuePlaceholder, colNumber.ToString());
			return name;
		}
	}
}
