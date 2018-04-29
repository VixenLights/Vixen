using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vixen.Utility
{
	public class NamingUtilities
	{
		public static string Uniquify(HashSet<string> names, string name)
		{
			if (names.Contains(name))
			{
				string originalName = name;
				bool unique;
				int counter = 2;
				do
				{
					name = string.Format("{0} - {1}", originalName, counter++);
					unique = names.Contains(name);
				} while (unique);
			}
			return name;
		}
	}
}
