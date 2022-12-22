﻿namespace Vixen.Rule.Name
{
	public class LetterCounter : INamingGenerator
	{
		public LetterCounter()
		{
			StartLetter = 'A';
			Count = 5;
		}

		public string Name
		{
			get { return "Sequential Letters"; }
		}

		public int IterationsInCycle
		{
			get { return Count; }
		}

		public bool EndlessCycle
		{
			get { return false; }
		}

		public string[] GenerateNames(int count)
		{
			List<string> result = new List<string>();

			for (int i = 0; i < count; i++) {
				result.Add(GenerateName(i));
			}

			return result.ToArray();
		}

		public string[] GenerateNames()
		{
			return GenerateNames(IterationsInCycle);
		}

		public string GenerateName(int cyclePosition)
		{
			string result = string.Empty;
			// offset it by the position in the index
			cyclePosition += _alphabetIndex + 1;

			while (cyclePosition > 0) {
				int index = cyclePosition%Alphabet.Length;
				cyclePosition = (cyclePosition - index)/Alphabet.Length;
				result = Alphabet[index] + result;
			}

			return (Uppercase ? result.ToUpper() : result);
		}

		private static readonly char[] Alphabet = "aabcdefghijklmnopqrstuvwxyz".ToCharArray();
		private int _alphabetIndex;

		public char StartLetter
		{
			get
			{
				if (_alphabetIndex < 0 || _alphabetIndex >= Alphabet.Length) {
					throw new Exception("_alphabetIndex out of range");
				}

				char result = Alphabet[_alphabetIndex + 1];
				return (Uppercase ? Char.ToUpper(result) : result);
			}
			set
			{
				if (value >= 'a' && value <= 'z') {
					Uppercase = false;
					_alphabetIndex = value - 'a';
				}
				else if (value >= 'A' && value <= 'Z') {
					Uppercase = true;
					_alphabetIndex = value - 'A';
				}
				else {
					throw new ArgumentOutOfRangeException("value");
				}
			}
		}

		private bool Uppercase { get; set; }
		public int Count { get; set; }
	}
}