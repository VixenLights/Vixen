namespace Vixen.Services
{
	public class OutputFilterService
	{
		private static OutputFilterService _instance;

		private OutputFilterService()
		{
		}

		public static OutputFilterService Instance
		{
			get { return _instance ?? (_instance = new OutputFilterService()); }
		}
	}
}