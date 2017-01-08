using System.Collections.Generic;

namespace VixenApplication.Setup.ElementTemplates
{
	public class TemplateUtilities
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
