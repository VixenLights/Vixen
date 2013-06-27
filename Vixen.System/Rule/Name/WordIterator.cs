using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Rule.Name
{
	public class WordIterator : INamingGenerator
	{
		public WordIterator()
		{
			Words = new List<string>();
		}

		public string Name
		{
			get { return "Words"; }
		}

		public int IterationsInCycle
		{
			get { return Words.Count; }
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
			cyclePosition %= Words.Count;
			return Words[cyclePosition];
		}

		public List<string> Words { get; set; }
	}
}