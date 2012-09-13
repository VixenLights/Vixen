using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Rule.Name
{
	class NumericCounter : INamingGenerator
	{
		public string Name
		{
			get { return "Numeric Counter"; }
		}

		public int IterationsInCycle
		{
			get
			{
				if (EndlessCycle)
					return -1;

				return (int)Math.Floor((EndNumber - StartNumber) / (double)Step) + 1;
			}
		}

		public bool EndlessCycle
		{
			get { return Endless || (Step < 0 && StartNumber < EndNumber) || (Step > 0 && StartNumber > EndNumber); }
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
			if (!EndlessCycle && cyclePosition >= IterationsInCycle) {
				throw new ArgumentOutOfRangeException("cyclePosition");
			}

			return (StartNumber + (Step * cyclePosition)).ToString();
		}

		public bool Endless { get; set; }
		public int StartNumber { get; set; }
		public int EndNumber { get; set; }

		private int _step;
		public int Step { 
			get { return _step; }
			set
			{
				if (value == 0) {
					throw new ArgumentException("Step value can't be 0");
				}

				_step = value;
			}
		}
	}
}
