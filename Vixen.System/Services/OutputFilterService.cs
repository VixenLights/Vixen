namespace Vixen.Services {
	public class OutputFilterService {
		static private OutputFilterService _instance;

		private OutputFilterService() {
		}

		public static OutputFilterService Instance {
			get { return _instance ?? (_instance = new OutputFilterService()); }
		}
	}
}
