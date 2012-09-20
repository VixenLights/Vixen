using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Vixen.Rule.Name
{
	public class LetterIterator : INamingGenerator
	{
		public LetterIterator()
		{
			Letters = "ABCDE";
		}

		public string Name
		{
			get { return "Letter Set"; }
		}

		public int IterationsInCycle
		{
			get { return Letters.Length; }
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
			cyclePosition %= Letters.Length;
			return Char.ToString(Letters[cyclePosition]);
		}

		private string _letters;
		public string Letters
		{
			get { return _letters; }
			set { _letters = Regex.Replace(value, "[^A-Za-z0-9]", ""); }
		}
	}
}
