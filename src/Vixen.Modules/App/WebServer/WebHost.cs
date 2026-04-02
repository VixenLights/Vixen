#nullable enable

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;

namespace VixenModules.App.WebServer
{

	public class WebHost
	{ 
		private IHost? _host;
		private readonly Data _data;
		private bool _isRunning;
		private static CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

		public WebHost(Data data)
		{
			_data = data;
		}

		private IHost CreateHostBuilder()
		{
			var host = new HostBuilder()
				.ConfigureWebHost(webBuilder =>
				{
					webBuilder.UseKestrel()
						.ConfigureLogging((_, logging) =>
						{
							logging.AddNLog();
						})
						.CaptureStartupErrors(true)
						.UseUrls($"http://*:{_data.HttpPort}")
						.UseStartup<Startup>();
				}).Build();
				
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
			await _cancelTokenSource.CancelAsync();
			if (_host != null)
			{
				await _host.StopAsync();
			}

			_isRunning = false;
		}

	}
}
