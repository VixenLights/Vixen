using Microsoft.AspNetCore.Hosting;
using NLog.Extensions.Logging;

namespace VixenModules.App.WebServer
{

	public class WebHost
	{ 
		private IWebHost? _host;
		private readonly Data _data;
		private bool _isRunning = false;
		private static CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

		public WebHost(Data data)
		{
			_data = data;
		}

		private IWebHost CreateHostBuilder()
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				.ConfigureLogging((hostingContext, logging) => {
					logging.AddNLog();
				})
				.CaptureStartupErrors(true)
				.UseUrls($"http://*:{_data.HttpPort}")
				.UseStartup<Startup>()
				.Build();
			
			return host;
		}

		public async void Start()
		{
			if (_isRunning)
			{
				await Stop();
			}
			_host = CreateHostBuilder();
			_cancelTokenSource = new CancellationTokenSource();
			await _host.StartAsync(_cancelTokenSource.Token);
		}

		public async Task Stop()
		{
			_cancelTokenSource.Cancel();
			if (_host != null)
			{
				await _host.StopAsync();
			}

			_isRunning = false;
		}

	}
}
