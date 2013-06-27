using System.Collections.Generic;

namespace Vixen.Rule
{
	public interface INamingTemplate
	{
		// the generator(s) required for this template, in the order required.
		IEnumerable<INamingGenerator> Generators { get; }

		// the format string for the template.
		string Format { get; }

		// the user-readable name of this template.
		string Name { get; }
	}
}