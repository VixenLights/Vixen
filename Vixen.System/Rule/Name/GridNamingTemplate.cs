using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Rule.Name
{
	class GridNamingTemplate : INamingTemplate
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

				counter = new NumericCounter();
				counter.StartNumber = 1;
				counter.EndNumber = 10;
				counter.Endless = false;
				counter.Step = 1;
				result[1] = counter;

				return result;
			}
		}

		public string Format
		{
			get { return "NewName-R{1}-C{2}"; }
		}

		public string Name
		{
			get { return "Grid Items"; }
		}
	}
}
