namespace Vixen.Rule
{
	public interface INamingGenerator
	{
		// The name of the naming generator.
		// eg. "Numeric Counter"
		string Name { get; }

		// The number of iterations that would be in one full cycle of this generator.
		// eg. if a number counter is configured to go from 1-10, this would return 10.
		// if it is infinite, return -1.
		int IterationsInCycle { get; }

		// If this generator is configured to have an endless cycle.
		// eg. if it's a numeric counter that doesn't have an end specified.
		bool EndlessCycle { get; }

		// Generate and return names according to this generator.

		// generates the given number of names as an array. If it is longer than a single
		// cycle of the generator, it can be undefined: eg. loop again, or continue out of
		// bounds, etc.
		string[] GenerateNames(int count);

		// generate an array of names for one full cycle of the generator. If the cycle is
		// endless, an empty array is to be returned.
		string[] GenerateNames();

		// generates a single name for the given index in the cycle.
		string GenerateName(int cycleIndex);
	}
}