using System.Collections.Generic;

namespace Vixen.Rule.Name {
	public class FlatNumericTemplate : INamingRule {
		public const string NumericValuePlaceholder = "{#}";

		public FlatNumericTemplate() {
		}

		public FlatNumericTemplate(int startNumber, int increment, string format) {
			StartNumber = startNumber;
			Increment = increment;
			Format = format;
		}

		public int StartNumber { get; set; }
		public int Increment { get; set; }
		public string Format { get; set; }
		public int Count { get; set; }

		public string Name {
			get { return "Numeric insertion"; }
		}

		public string[] GenerateNames(int channelCount) {
			List<string> names = new List<string>();

			int number = StartNumber;
			while(channelCount-- > 0) {
				string name = _GenerateName(Format, number);
				names.Add(name);
				number += Increment;
			}

			return names.ToArray();
		}

		private string _GenerateName(string format, int number) {
			return format.Replace(NumericValuePlaceholder, number.ToString());
			//int replaceAtIndex = format.IndexOf(NUMERIC_VALUE_PLACEHOLDER);
			//if(replaceAtIndex != -1) {
			//    return format.Replace(NUMERIC_VALUE_PLACEHOLDER, number.ToString());
			//}
			//return format;
		}
	}
}
