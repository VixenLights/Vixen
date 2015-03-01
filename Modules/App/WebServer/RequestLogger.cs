using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;

namespace VixenModules.App.WebServer
{
	//Writes request info to the console for debugging purposes
	internal class RequestLogger
	{

		private readonly Func<IDictionary<string, object>, Task> _next;

		public RequestLogger(Func<IDictionary<string, object>, Task> next)
		{
			if (next == null)
				throw new ArgumentNullException("next");
			_next = next;
		}

		public Task Invoke(IDictionary<string, object> environment)
		{
			string method = GetValueFromEnvironment(environment, "owin.RequestMethod");
			string path = GetValueFromEnvironment(environment, "owin.RequestPath");
			string protocol = GetValueFromEnvironment(environment, "owin.RequestProtocol");

			Console.WriteLine("Entry\t{0}\t{1}\t{2}", method, path, protocol);

			Stopwatch stopWatch = Stopwatch.StartNew();
			return _next(environment).ContinueWith(t =>
			{
				Console.WriteLine("Exit\t{0}\t{1}\t{2}\t{3}\t{4}", method, path, stopWatch.ElapsedMilliseconds,
				  GetValueFromEnvironment(environment, "owin.ResponseStatusCode"),
				  GetValueFromEnvironment(environment, "owin.ResponseReasonPhrase"));
				return t;
			});
		}

		private static string GetValueFromEnvironment(IDictionary<string, object> environment, string key)
		{
			object value;
			environment.TryGetValue(key, out value);
			return Convert.ToString(value, CultureInfo.InvariantCulture);
		}
	}
}
