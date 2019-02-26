using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.App.WebServer.Model
{
	public class Status
	{
		public Status()
		{
			Details = new List<string>();
			IsSuccessful = true;
		}
		public string Message { get; set; }

		public List<string> Details { get; set; }

		public bool IsSuccessful { get; set; }
	}
}
