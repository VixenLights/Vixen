using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Rule.Name
{
	internal class NumberedNamingTemplate : INamingTemplate
	{
		public IEnumerable<INamingGenerator> Generators
		{
			get
			{
				INamingGenerator[] result = new INamingGenerator[1];

				NumericCounter counter = new NumericCounter();
				counter.StartNumber = 1;
				counter.EndNumber = 5;
				counter.Endless = true;
				counter.Step = 1;
				result[0] = counter;

				return result;
			}
		}

		public string Format
		{
			get { return "-<1>"; }
		}

		public string Name
		{
			get { return "Numbered Items"; }
		}
	}
}