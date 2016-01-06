using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Rule.Name
{
	public class NumericCounter : INamingGenerator
	{
		public NumericCounter()
		{
			StartNumber = 1;
			EndNumber = 5;
			Step = 1;
			Endless = true;
		}

		public string Name
		{
			get { return "Sequential Numbers"; }
		}

		public int IterationsInCycle
		{
			get
			{
				if (EndlessCycle)
					return -1;

				return (int) Math.Floor((EndNumber - StartNumber)/(double) Step) + 1;
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
			if (EndlessCycle)
				return new string[0];

			return GenerateNames(IterationsInCycle);
		}

		public string GenerateName(int cyclePosition)
		{
			return (StartNumber + (Step*cyclePosition)).ToString();
		}

		public bool Endless { get; set; }
		public int StartNumber { get; set; }
		public int EndNumber { get; set; }

		private int _step;

		public int Step
		{
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