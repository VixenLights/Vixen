using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Services
{
	public class ConfigurationService
	{
		private static ConfigurationService _instance;
		
		/// <inheritdoc />
		private ConfigurationService()
		{
			
		}

		public static ConfigurationService Instance()
		{
			if (_instance == null)
			{
				_instance = new ConfigurationService();
			}

			return _instance;
		}

		public Configuration Config { get; internal set; }
	}
}
