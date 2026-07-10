namespace Vixen.Execution
{
	public class ContextFeatures
	{
		public ContextFeatures(ContextCaching caching)
		{
			Caching = caching;
		}

		public ContextCaching Caching { get; private set; }

		public override string ToString()
		{
			return "Caching = " + Caching;
		}
	}
}