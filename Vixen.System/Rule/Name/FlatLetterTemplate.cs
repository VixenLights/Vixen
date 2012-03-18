using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Rule.Name {
	public class FlatLetterTemplate : INamingRule {
		public const string LetterValuePlaceholder = "{A}";

		public FlatLetterTemplate() {
		}

		public FlatLetterTemplate(char startLetter, int increment, string format) {
			if(!char.IsLetter(startLetter)) throw new ArgumentException("startLetter must be a letter.");
			StartLetter = startLetter;
			Increment = increment;
			Format = format;
		}

		public char StartLetter { get; set; }
		public int Increment { get; set; }
		public string Format { get; set; }

		public string Name {
			get { return "Letter insertion"; }
		}

		public string[] GenerateNames(int channelCount) {
			List<string> names = new List<string>();

			string characters = StartLetter.ToString();
			while(channelCount-- > 0) {
				string name = _GenerateName(Format, characters);
				names.Add(name);
				characters = _IncrementValue(characters, Increment);
			}

			return names.ToArray();
		}

		private string _IncrementValue(string characters, int increment) {
			// Upper or lower case?
			bool isUpper = char.IsUpper(characters.First());
			char alphabetStart = isUpper ? 'A' : 'a';

			// Convert from string to list of ints
			List<int> characterNumberValues = _StringToInts(characters, alphabetStart);
			// Reverse the list
			characterNumberValues.Reverse();
			// Starting at index 0, recursively call an increment method until it runs out of index
			_RollValues(characterNumberValues, 0, increment);
			// Reverse the list
			characterNumberValues.Reverse();
			// Convert back to string
			return _IntsToString(characterNumberValues, alphabetStart);
		}

		private List<int> _StringToInts(string characters, char alphabetStart) {
			return characters.Select(x => x - alphabetStart).ToList();
		}

		private string _IntsToString(List<int> characterNumberValues, char alphabetStart) {
			return new string(characterNumberValues.Select(x => (char)(x + alphabetStart)).ToArray());
		}

		private void _RollValues(List<int> values, int atIndex, int incrementAmount) {
			if(atIndex >= values.Count) {
				values.Add(0);
				return;
			}

			int value = values[atIndex];
			value += incrementAmount;
			if(value >= 26) {
				value -= 26;
				_RollValues(values, atIndex + 1, 1);
			}
			values[atIndex] = value;
		}

		private string _GenerateName(string format, string stringValue) {
			return format.Replace(LetterValuePlaceholder, stringValue);
		}
	}
}
