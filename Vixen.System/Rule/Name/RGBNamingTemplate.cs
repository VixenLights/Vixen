using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Rule.Name
{
	internal class RGBNamingTemplate : INamingTemplate
	{
		public IEnumerable<INamingGenerator> Generators
		{
			get
			{
				INamingGenerator[] result = new INamingGenerator[2];

				NumericCounter counter = new NumericCounter();
				counter.StartNumber = 1;
				counter.EndNumber = 10;
				counter.Endless = true;
				counter.Step = 1;
				result[0] = counter;

				LetterIterator iterator = new LetterIterator();
				iterator.Letters = "RGB";
				result[1] = iterator;

				return result;
			}
		}

		public string Format
		{
			get { return "NewName-{1}-{2}"; }
		}

		public string Name
		{
			get { return "RGB Items"; }
		}
	}
}